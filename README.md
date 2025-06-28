## Stgeps to re-create

```bash
dotnet new webapp -n RazorAdmin -o . --no-https
dotnet run
dotnet new sln -n RazorAdmin
dotnet sln add src/RazorAdmin.csproj
```