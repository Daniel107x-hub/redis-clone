# Commands to create the whole solution
```csharp
dotnet new sln
dotnet new console -o [PROJECT_NAME]
dotnet sln add ./[PROJECT_NAME]/[PROJECT_NAME].csproj
dotnet new xunit -o [PROJECT_NAME].Tests
dotnet add ./[PROJECT_NAME].Tests/[PROJECT_NAME].Tests.csproj reference ./[PROJECT_NAME]/[PROJECT_NAME].csproj
dotnet sln add ./[PROJECT_NAME].Tests/[PROJECT_NAME].Tests.csproj
```