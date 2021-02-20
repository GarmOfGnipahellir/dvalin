# NOTE! VERY SUPER ULTRA WIP

## Build

### User Settings

You need a user project file to define reference paths and Valheim dir. E.g:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <ReferencePath>\some\path\Valheim\valheim_Data\Managed\;\some\path\Valheim\BepInEx\core\</ReferencePath>
    <PluginsDir>\some\path\BepInEx\plugins\</PluginsDir>
  </PropertyGroup>
</Project>
```

Should be here: `Dvalin/Dvalin/Dvalin.csproj.user`

### VSCode

Also if you wan't to build with VSCode you can base your task on this:

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build debug",
            "type": "shell",
            "command": "/path/to/msbuild/MSBuild.exe",
            "args": [
                "${workspaceFolder}/Dvalin.sln",
                "/p:Configuration=Debug"
            ],
            "group": {
                "kind": "build",
                "isDefault": true
            },
            "presentation": {
                "reveal": "silent"
            },
            "problemMatcher": "$msCompile"
        }
    ]
}
```

### Unity

Unity needs all the Valheim and Dvalin assemblies imported to build.  
Put these in each respective package.