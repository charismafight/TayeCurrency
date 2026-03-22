# dotnet-template

This is a personal template for backend project, to reduce duplicated startup thing.

## Startup steps

1. dotnet new webapi --use-controllers -o {ProjectName}.
2. cd {ProjectName}.
3. `dotnet add package Microsoft.EntityFrameworkCore.Sqlite` for develop.
4. refer to [MS_EF_packages](https://learn.microsoft.com/zh-cn/ef/core/providers/?tabs=dotnet-core-cli), use `dotnet add package Microsoft.EntityFrameworkCore.{db_type}`(Sqlite,InMemory) to add ef.
5. More setup steps for EF, use the [GUIDE](./Tutorial/EntityFramework.md)
6. dotnet dev-certs https --trust to use https.
7. dotnet run --launch-profile https to start on https.
8. execute vscode command .net: generate assets for build and debug to generate launch.json and task.json.
9. if the dotnet version is ***9*** or above, the framework use openapi(~/openapi/v1.json) instead of the default swagger.
10. Microsoft.AspNetCore.OpenApi will be install after project created by dotnet new webapi.
11. openapi/v1.json only offers a json format document, if we want a UI format document just like swagger, MS now suggest to use Scalar.
12. dotnet add package Scalar.AspNetCore to add Scalar to project.
13. add app.MapScalarApiReference(); to program.cs.
14. try access ~/scalar/v1 to get scalar for project documents.
15. dotnet add package StyleCop.Analyzers.
16. about Directory.Packages.props, some built-in packages will be added when executing dotnet new, we should add it to Directory.Packages.props first to keep the command going.
17. global.json specifies the dotnet version dotnet cli command used, it was set to '9.0.100' as the latest till this submit.
18. in the root folder, use dotnet new xunit -o {testProjectName} to create testproject.
19. dotnet sln add {path to test.csproj} to add project to sln file, if visual studio is needed.
20. cors problem: if API needs to be visit from an individually deployed website, add cors in Program.cs as follow or add specific headers, ips, or ports:

```c#
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
            b =>
            {
                b.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
});

...other codes

app.UseCors();

```
