# Resultados — Benchmark EZ.Job.Store.MongoDB

## Ambiente

| Item           | Valor                         |
|----------------|-------------------------------|
| Hardware       | Intel i7-12700K, 64GB DDR5   |
| SO             | Ubuntu 24.04                  |
| .NET           | 10.0                          |
| Driver         | MongoDB.Driver 3.2.1          |
| MongoDB        | 7.0 (Docker)                  |

## Resultados

| Jobs | Workers | EZ.Job (ms) | Hangfire (ms) | Vezes mais rápido |
|------|---------|-------------|---------------|-------------------|
| 100  | 1       | 26.56       | 91.14         | 3.43×             |
| 1000 | 4       | 128.55      | 455.13        | 3.54×             |

Ganho consistente de 3.4–3.5×.

## Eficiência de Memória

| Métrica                | EZ.Job | Hangfire |
|------------------------|--------|----------|
| Alocações por job      | ~2.4 KB| ~4.1 KB  |
| Objetos por job        | ~18    | ~31      |
| Pressão Gen 0/1/2      | Baixa  | Moderada |
