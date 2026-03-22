# EntityFramework

## Prerequisite installation

```powershell
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## Add DBContext class

Add class like below to project.

```c#
public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"Data Source=DBName.db");
    }

    // list Dbset Below to use
}
```

## Add EF service to App

Add ef service to Program.cs:

```c#
builder.Services.AddDbContext<DataContext>();
```

```c#
//(Option way)
builder.Services.AddSqlite<DataContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
```

## Execute Ef command to create migrations

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Seeding data

Seeding can be set in OnConfiguring method of the DbContext subclass:

```c#
optionsBuilder.UseSeeding((context, _) =>
    // use context to prepare data
);
```

After executing `dotnet ef database update`, UseSeeding method will be triggered.
