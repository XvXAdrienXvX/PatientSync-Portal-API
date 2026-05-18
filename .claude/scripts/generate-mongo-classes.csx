using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

#nullable enable

var args = Args.ToArray();

string? schemaFile = GetArgValue("schemaFile") ?? GetArgValue("schema") ?? args.FirstOrDefault();
string outputFolder = GetArgValue("outputFolder") ?? GetArgValue("out") ?? "GeneratedMongoClasses";
string namespaceName = GetArgValue("namespace") ?? "MongoPocos";
string rootName = GetArgValue("rootName") ?? "RootDocument";

if (string.IsNullOrWhiteSpace(schemaFile))
{
    Console.Error.WriteLine("Usage: dotnet script .claude/scripts/generate-mongo-classes.csx -- --schemaFile <schema.json> [--outputFolder <folder>] [--namespace <Namespace>] [--rootName <RootName>]");
    Environment.Exit(1);
}

var schemaPath = Path.GetFullPath(schemaFile);
if (!File.Exists(schemaPath))
{
    Console.Error.WriteLine($"Schema file not found: {schemaPath}");
    Environment.Exit(1);
}

var outputPath = Path.GetFullPath(outputFolder);
Directory.CreateDirectory(outputPath);

var schemaJson = File.ReadAllText(schemaPath);

using (var document = JsonDocument.Parse(schemaJson))
{
    var rootElement = document.RootElement;

    var generator = new MongoPocoGenerator(outputPath, namespaceName);
    var className = GetRootClassName(rootElement, rootName);

    generator.Generate(rootElement, className);

    generator.WriteFiles();

    Console.WriteLine($"Generated {generator.ClassCount} class file(s) into {outputPath}");
}

string? GetArgValue(string argName)
{
    var prefix = $"--{argName}";
    for (var i = 0; i < args.Length; i++)
    {
        if (args[i].Equals(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return i + 1 < args.Length ? args[i + 1] : null;
        }

        if (args[i].StartsWith(prefix + "=", StringComparison.OrdinalIgnoreCase))
        {
            return args[i].Substring(prefix.Length + 1);
        }
    }

    return null;
}

string GetRootClassName(JsonElement rootElement, string fallback)
{
    if (rootElement.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
    {
        return MongoPocoGenerator.ToPascalCase(title.GetString()!);
    }

    return MongoPocoGenerator.ToPascalCase(fallback);
}

internal sealed class MongoPocoGenerator
{
    private readonly string _outputPath;
    private readonly string _namespaceName;
    private readonly Dictionary<string, ClassDefinition> _classes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _refMap = new(StringComparer.OrdinalIgnoreCase);

    public MongoPocoGenerator(string outputPath, string namespaceName)
    {
        _outputPath = outputPath;
        _namespaceName = namespaceName;
    }

    public int ClassCount => _classes.Count;

    internal static string ToPascalCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        var words = Regex.Split(text, "[^a-zA-Z0-9]+")
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .Select(w => char.ToUpperInvariant(w[0]) + w[1..].ToLowerInvariant());

        return string.Concat(words);
    }

    public void Generate(JsonElement rootSchema, string rootClassName)
    {
        ProcessDefinitions(rootSchema);
        ParseSchema(rootSchema, rootClassName);
    }

    public void WriteFiles()
    {
        foreach (var classDef in _classes.Values)
        {
            var fileName = Path.Combine(_outputPath, classDef.Name + ".cs");
            File.WriteAllText(fileName, BuildClassFile(classDef));
        }
    }

    private void ProcessDefinitions(JsonElement rootSchema)
    {
        if (rootSchema.TryGetProperty("$defs", out var defs) && defs.ValueKind == JsonValueKind.Object)
        {
            foreach (var item in defs.EnumerateObject())
            {
                _refMap[$"#/$defs/{item.Name}"] = ToPascalCase(item.Name);
                ParseSchema(item.Value, ToPascalCase(item.Name));
            }
        }

        if (rootSchema.TryGetProperty("definitions", out var definitions) && definitions.ValueKind == JsonValueKind.Object)
        {
            foreach (var item in definitions.EnumerateObject())
            {
                _refMap[$"#/definitions/{item.Name}"] = ToPascalCase(item.Name);
                ParseSchema(item.Value, ToPascalCase(item.Name));
            }
        }
    }

    private void ParseSchema(JsonElement schema, string className)
    {
        if (_classes.ContainsKey(className))
        {
            return;
        }

        var classDef = new ClassDefinition(className);
        _classes[className] = classDef;

        var properties = schema.TryGetProperty("properties", out var props) && props.ValueKind == JsonValueKind.Object
            ? props.EnumerateObject()
            : Enumerable.Empty<JsonProperty>();

        foreach (var property in properties)
        {
            var prop = CreateProperty(property.Name, property.Value, className);
            classDef.Properties.Add(prop);
        }
    }

    private PropertyDefinition CreateProperty(string propertyName, JsonElement schema, string parentClass)
    {
        var name = ToPascalCase(propertyName);
        var bsonName = propertyName;
        var isId = false;
        var representation = GetBsonRepresentation(schema);

        if (propertyName.Equals("_id", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("id", StringComparison.OrdinalIgnoreCase)
            || GetBool(schema, "x-bson-id"))
        {
            isId = true;
            bsonName = "_id";
        }

        var type = ResolveType(propertyName, schema, parentClass, out var nestedClass);
        if (nestedClass is not null)
        {
            ParseSchema(nestedClass.Schema, nestedClass.Name);
        }

        return new PropertyDefinition(name, type)
        {
            BsonElementName = bsonName,
            BsonRepresentation = representation,
            IsId = isId
        };
    }

    private string ResolveType(string propertyName, JsonElement schema, string parentClass, out NestedClass? nestedClass)
    {
        nestedClass = null;

        if (schema.TryGetProperty("$ref", out var refValue) && refValue.ValueKind == JsonValueKind.String)
        {
            var refText = refValue.GetString()!;
            if (_refMap.TryGetValue(refText, out var refType))
            {
                return refType;
            }

            var refName = refText.Split('/').Last();
            return ToPascalCase(refName);
        }

        var schemaType = GetString(schema, "type");
        if (string.IsNullOrEmpty(schemaType) && schema.TryGetProperty("properties", out _))
        {
            schemaType = "object";
        }

        if (schemaType == "array")
        {
            var items = schema.TryGetProperty("items", out var itemSchema)
                ? itemSchema
                : default;

            var itemType = ResolveType(propertyName + "Item", items, parentClass, out var nestedItemClass);
            if (nestedItemClass is not null)
            {
                nestedClass = nestedItemClass;
            }

            return $"List<{itemType}>";
        }

        if (schemaType == "object")
        {
            var nestedName = ToPascalCase(propertyName);
            nestedClass = new NestedClass(nestedName, schema);
            return nestedName;
        }

        return MapJsonType(schemaType, schema);
    }

    private static string MapJsonType(string? schemaType, JsonElement schema)
    {
        if (IsObjectId(schemaType, schema))
        {
            return "ObjectId";
        }

        return schemaType?.ToLowerInvariant() switch
        {
            "string" => "string",
            "integer" => "int",
            "number" => "double",
            "boolean" => "bool",
            "object" => "object",
            _ => "string"
        };
    }

    private static bool IsObjectId(string? schemaType, JsonElement schema)
    {
        var format = GetString(schema, "format");
        var bsonType = GetString(schema, "bsonType");
        var rawType = GetString(schema, "type");

        return string.Equals(format, "objectId", StringComparison.OrdinalIgnoreCase)
            || string.Equals(bsonType, "objectId", StringComparison.OrdinalIgnoreCase)
            || string.Equals(rawType, "ObjectId", StringComparison.OrdinalIgnoreCase)
            || string.Equals(rawType, "objectid", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetBsonRepresentation(JsonElement schema)
    {
        if (schema.TryGetProperty("x-bson-representation", out var repr) && repr.ValueKind == JsonValueKind.String)
        {
            return repr.GetString();
        }

        if (schema.TryGetProperty("format", out var format) && format.ValueKind == JsonValueKind.String
            && string.Equals(format.GetString(), "objectId", StringComparison.OrdinalIgnoreCase))
        {
            return "ObjectId";
        }

        return null;
    }

    private static bool GetBool(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var value)
            && value.ValueKind == JsonValueKind.True;
    }

    private static string? GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private string BuildClassFile(ClassDefinition classDef)
    {
        var builder = new StringBuilder();
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using MongoDB.Bson;");
        builder.AppendLine("using MongoDB.Bson.Serialization.Attributes;");
        builder.AppendLine();
        builder.AppendLine($"namespace {_namespaceName};");
        builder.AppendLine();
        builder.AppendLine($"public class {classDef.Name}");
        builder.AppendLine("{");

        foreach (var property in classDef.Properties)
        {
            if (property.IsId)
            {
                builder.AppendLine("    [BsonId]");
            }

            if (!string.Equals(property.Name, property.BsonElementName, StringComparison.OrdinalIgnoreCase))
            {
                builder.AppendLine($"    [BsonElement(\"{property.BsonElementName}\")]\n");
            }
            else if (property.IsId)
            {
                builder.AppendLine($"    [BsonElement(\"{property.BsonElementName}\")]\n");
            }
            else
            {
                // remove extra blank line if no attribute written
                if (property.IsId)
                {
                    builder.AppendLine();
                }
            }

            if (!string.IsNullOrWhiteSpace(property.BsonRepresentation))
            {
                builder.AppendLine($"    [BsonRepresentation(BsonType.{property.BsonRepresentation})]");
            }

            builder.AppendLine($"    public {property.Type} {SanitizeIdentifier(property.Name)} {{ get; set; }} = default!;");
            builder.AppendLine();
        }

        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string SanitizeIdentifier(string value)
    {
        if (Regex.IsMatch(value, "^[A-Za-z_][A-Za-z0-9_]*$"))
        {
            return value;
        }

        return "@" + value;
    }
}

internal sealed class ClassDefinition
{
    public ClassDefinition(string name)
    {
        Name = name;
        Properties = new List<PropertyDefinition>();
    }

    public string Name { get; }
    public List<PropertyDefinition> Properties { get; }
}

internal sealed class PropertyDefinition
{
    public PropertyDefinition(string name, string type)
    {
        Name = name;
        Type = type;
        BsonElementName = name;
    }

    public string Name { get; }
    public string Type { get; }
    public string BsonElementName { get; set; }
    public string? BsonRepresentation { get; set; }
    public bool IsId { get; set; }
}

internal sealed class NestedClass
{
    public NestedClass(string name, JsonElement schema)
    {
        Name = name;
        Schema = schema;
    }

    public string Name { get; }
    public JsonElement Schema { get; }
}
