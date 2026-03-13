# How to push new version to Nuget Gallery

## 1. Update version in PlcKafkaLibrary.csproj

```xml
<PropertyGroup>
    <Version>2.0.0</Version>
</PropertyGroup>
```

## 2. Build the library

```bash
dotnet build PlcRabbitLibrary/PlcRabbitLibrary.csproj -c Release
```

## 2. Pack the library

```bash
dotnet pack PlcRabbitLibrary/PlcRabbitLibrary.csproj -c Release -o ./out/nuget
```

## 3. Push to Nuget Gallery

```bash
dotnet nuget push out/nuget/PlcRabbitLibrary.2.0.0.nupkg --api-key <NUGET_API_KEY> --source https://api.nuget.org/v3/index.json
```
