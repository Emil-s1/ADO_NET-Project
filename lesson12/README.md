# CollegeApi

ASP.NET Core Web API на C# с Dapper, Microsoft.Data.SqlClient и Swagger.

## Запуск

1. Проверьте строку подключения в `appsettings.json`.
2. Убедитесь, что в `CollegeDB` есть таблицы `Students` и `Groups` с колонками, используемыми запросами.
3. В каталоге проекта выполните:

```bash
dotnet restore
dotnet run
```

4. Откройте адрес приложения и перейдите на `/swagger`.

## Эндпоинты

- `GET /api/students`
- `GET /api/students/{id}`
- `GET /api/groups`
- `GET /api/groups/{id}`

Если в вашей базе используются другие названия колонок, измените SQL-запросы и свойства моделей под фактическую схему.
