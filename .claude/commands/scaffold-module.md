---
description: Orchestrates a new .NET module scaffold via a .csx script and appends its projects into the existing PatientSync.API.slnx solution file.
model: claude-haiku-4-5
context: ["PatientSync.API.slnx"]
---

<command-parameters>
  <required-argument name="ModuleName">Must be PascalCase</required-argument>
  <examples>
    <example>IdentityModule</example>
  </examples>
</command-parameters>

<execution-flow>
  <step number="1">
    <action>Run Scaffolding Script</action>
    <instruction>Execute the orchestrator script at "./.claude/scripts/scaffold-module.csx" with the required ModuleName argument.</instruction>
    <command>
      dotnet script .claude/scripts/scaffold-module.csx -- --moduleName {{ModuleName}}
    </command>
  </step>
</execution-flow>

<token-saving-constraints>
  <constraint>No Code: Do not generate any C# classes, interfaces, or template code files.</constraint>
  <constraint>Target Restriction: Only modify the existing file at "./PatientSync.API.slnx". Creating a new .slnx file is strictly forbidden.</constraint>
  <constraint>Silent Mode: Output only a single-line summary of actions taken. Do not explain architecture or XML manipulation steps.</constraint>
</token-saving-constraints>
