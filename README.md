# DX Localization NuGet Generator
This project aims to create localization NuGet packages from provided DevExpress NuGet packages and localization libraries provided by DevExpress Localization service.

# Usage

The DXLocalizationNugetGenerator executable has two commands. The CreateNuspec must precede the CreateNuget command.

### Create nuspec files

Create nuspec files before the NuGet packages creation. You will need:
* --inputDXNuGetPath - path to DevExpress Nuget packages
* --inputLocalizationPath - path to DevExpress localization libraries from DevExpress Localization Service
* --outputLanguageCode - target two letter language code
* --outputNuspecPath - target nuspec path.

```
.\DXLocalizationNugetGenerator.exe CreateNuspec --inputDXNuGetPath="C:\Program Files (x86)\DevExpress 20.2\Components\System\Components\packages" --inputLocalizationPath=D:\Playground\devexpress-nuget-localization\source\localization --outputLanguageCode=cs --outputNuspecPath=D:\Playground\devexpress-nuget-localization\target\nuspec
```

### Create NuGet packages

Create NuGet packages. This command takes all nuspec files from the given input path and creates NuGet packages. If the NuGet executable does not exist in PATH, the latest stable NuGet executable is downloaded and used for the creation of the NuGet package. To use this command, you will need:
* --i - path to the directory that consists of nuspec files
* --o - path to output directory that will contain the generated NuGet files.

```
.\DXLocalizationNugetGenerator.exe CreateNuget -i D:\Playground\devexpress-nuget-localization\target\nuspec -o D:\Playground\devexpress-nuget-localization\target\nuget
```

# Contributing

Contributions are welcome.  Feel free to file issues and pull requests on the repo.