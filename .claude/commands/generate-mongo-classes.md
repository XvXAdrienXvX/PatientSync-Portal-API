---
description: Generates MongoDB POCO classes from a JSON schema file.
model: claude-haiku-4-5
context: [".claude/scripts/generate-mongo-classes.csx"]
---

<command-parameters>
  <required-argument name="SchemaFile">Path to the JSON schema file.</required-argument>
  <optional-argument name="OutputFolder">Relative folder for generated class files.</optional-argument>
  <optional-argument name="Namespace">C# namespace for generated classes.</optional-argument>
  <optional-argument name="RootName">Root class name when schema title is absent.</optional-argument>
  <examples>
    <example>SchemaFile=schema/mongo-schema.json OutputFolder=src/Models Namespace=MyApp.Mongo RootName=PatientRecord</example>
  </examples>
</command-parameters>

<execution-flow>
  <step number="1">
    <action>Run Generator Script</action>
    <instruction>Execute the script at "./.claude/scripts/generate-mongo-classes.csx" with the JSON schema arguments.</instruction>
    <command>
      dotnet script .claude/scripts/generate-mongo-classes.csx -- --schemaFile {{SchemaFile}} --outputFolder {{OutputFolder}} --namespace {{Namespace}} --rootName {{RootName}}
    </command>
  </step>
</execution-flow>
