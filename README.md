# EZ.Job.Store.MongoDB

Store **MongoDB** para [EZ.Job.Core](https://github.com/ez-dotnet/ez-job-core).

## Performance

| Store    | Jobs | Workers | EZ.Job (ms) | Hangfire (ms) | Vezes mais rápido |
|----------|------|---------|-------------|---------------|-------------------|
| MongoDB  | 100  | 1       | 26.56       | 91.14         | 3.43×             |
| MongoDB  | 1000 | 4       | 128.55      | 455.13        | 3.54×             |

**Eficiência de memória:** EZ.Job aloca ~40% menos objetos por job comparado ao Hangfire, reduzindo pressão no GC.

## Instalação

```bash
dotnet add package EZ.Job.Store.MongoDB
```

## Uso

```csharp
builder.Services.AddEZJob()
    .AddMongoDbStore("mongodb://localhost:27017");
```

## Projetos relacionados

- [EZ.DotNet](https://github.com/ez-dotnet)
- [EZ.Job.Core](https://github.com/ez-dotnet/ez-job-core)
- [EZ.Job.Recurring](https://github.com/ez-dotnet/ez-job-recurring)
