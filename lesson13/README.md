# CollegeApi — урок 13

ASP.NET Core Web API с `MapGet()`, Dapper, SQL Server и Swagger.

## Запуск

1. В `appsettings.json` укажите строку подключения к вашей базе `CollegeDB`.
2. Убедитесь, что таблица `Groups` содержит столбцы `GroupId` и `GroupName`. Если имена отличаются, измените SQL-запрос в `Data/GroupRepository.cs`.
3. Из каталога проекта выполните:

```bash
dotnet restore
dotnet run
```

4. Откройте `/swagger` на локальном адресе, который выведет приложение.
5. Выполните `GET /groups`. Ответом будет текст со списком всех групп.

## Структура

- `Models/Group.cs` — модель группы.
- `Data/GroupRepository.cs` — получение групп из SQL Server через Dapper.
- `Program.cs` — endpoint `MapGet("/groups", ...)`.
