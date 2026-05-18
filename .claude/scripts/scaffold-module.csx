using System.Xml.Linq;

string moduleName = Args.FirstOrDefault();
if (string.IsNullOrWhiteSpace(moduleName))
{
    Console.Error.WriteLine("Usage: dotnet script scaffold-module.csx <ModuleName>");
    Environment.Exit(1);
}

var workspaceRoot = Environment.CurrentDirectory;
var moduleRoot = Path.Combine(workspaceRoot, "Modules", moduleName);
var moduleModulePath = Path.Combine(moduleRoot, $"{moduleName}.Module");
var moduleContractsPath = Path.Combine(moduleRoot, $"{moduleName}.Contracts");
var directories = new[]
{
    Path.Combine(moduleModulePath, "Features"),
    Path.Combine(moduleModulePath, "Domain"),
    Path.Combine(moduleModulePath, "Infrastructure"),
    Path.Combine(moduleContractsPath, "Interfaces")
};

Directory.CreateDirectory(moduleRoot);
foreach (var directory in directories)
{
    Directory.CreateDirectory(directory);
}

string contractsCsprojPath = Path.Combine(moduleContractsPath, $"{moduleName}.Contracts.csproj");
string moduleCsprojPath = Path.Combine(moduleModulePath, $"{moduleName}.Module.csproj");

const string contractsCsproj =
"<Project Sdk=\"Microsoft.NET.Sdk\">\n" +
"  <PropertyGroup>\n" +
"    <TargetFramework>net10.0</TargetFramework>\n" +
"    <ImplicitUsings>enable</ImplicitUsings>\n" +
"    <Nullable>enable</Nullable>\n" +
"  </PropertyGroup>\n" +
"  <ItemGroup>\n" +
"    <Folder Include=\"Interfaces\\\" />\n" +
"  </ItemGroup>\n" +
"</Project>\n";

string moduleCsproj =
"<Project Sdk=\"Microsoft.NET.Sdk\">\n" +
"  <PropertyGroup>\n" +
"    <TargetFramework>net10.0</TargetFramework>\n" +
"    <ImplicitUsings>enable</ImplicitUsings>\n" +
"    <Nullable>enable</Nullable>\n" +
"  </PropertyGroup>\n" +
"  <ItemGroup>\n" +
$"    <ProjectReference Include=\"../{moduleName}.Contracts/{moduleName}.Contracts.csproj\" />\n" +
"  </ItemGroup>\n" +
"  <ItemGroup>\n" +
"    <Folder Include=\"Features\\\" />\n" +
"    <Folder Include=\"Domain\\\" />\n" +
"    <Folder Include=\"Infrastructure\\\" />\n" +
"  </ItemGroup>\n" +
"</Project>\n";

if (!File.Exists(contractsCsprojPath))
{
    File.WriteAllText(contractsCsprojPath, contractsCsproj);
}

if (!File.Exists(moduleCsprojPath))
{
    File.WriteAllText(moduleCsprojPath, moduleCsproj);
}

var solutionPath = Path.Combine(workspaceRoot, "PatientSync.API", "PatientSync.API.slnx");
if (!File.Exists(solutionPath))
{
    Console.Error.WriteLine($"Solution file not found: {solutionPath}");
    Environment.Exit(1);
}

var document = XDocument.Load(solutionPath);
var root = document.Root ?? throw new InvalidOperationException("Solution XML root is missing.");
var folderName = $"/Modules/{moduleName}/";
var existingFolder = root.Elements("Folder").Any(e => string.Equals((string)e.Attribute("Name"), folderName, StringComparison.OrdinalIgnoreCase));
if (!existingFolder)
{
    var folderElement = new XElement("Folder",
        new XAttribute("Name", folderName),
        new XElement("Project", new XAttribute("Path", $"../Modules/{moduleName}/{moduleName}.Contracts/{moduleName}.Contracts.csproj")),
        new XElement("Project", new XAttribute("Path", $"../Modules/{moduleName}/{moduleName}.Module/{moduleName}.Module.csproj"))
    );

    root.Add(folderElement);
    document.Save(solutionPath);
}

var apiProjectPath = Path.Combine(workspaceRoot, "PatientSync.API", "PatientSync.API.csproj");
if (!File.Exists(apiProjectPath))
{
    Console.Error.WriteLine($"API project file not found: {apiProjectPath}");
    Environment.Exit(1);
}

var apiDocument = XDocument.Load(apiProjectPath);
var apiRoot = apiDocument.Root ?? throw new InvalidOperationException("API project XML root is missing.");
var moduleReference = $"../Modules/{moduleName}/{moduleName}.Module/{moduleName}.Module.csproj";
var existingReference = apiRoot
    .Descendants("ProjectReference")
    .Any(e => string.Equals((string)e.Attribute("Include"), moduleReference, StringComparison.OrdinalIgnoreCase));

if (!existingReference)
{
    var itemGroup = apiRoot.Elements("ItemGroup").FirstOrDefault();
    if (itemGroup == null)
    {
        itemGroup = new XElement("ItemGroup");
        apiRoot.Add(itemGroup);
    }

    itemGroup.Add(new XElement("ProjectReference", new XAttribute("Include", moduleReference)));
    apiDocument.Save(apiProjectPath);
}

Console.WriteLine($"Scaffolded module {moduleName}, ensured directories exist, updated PatientSync.API.slnx, and referenced module in PatientSync.API.csproj");
