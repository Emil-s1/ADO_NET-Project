# CollegeApi — урок 15

ASP.NET Core Web API с обработкой маршрутов, Dapper и HTML/JavaScript-фронтендом.

## Запуск

1. Укажите строку подключения к SQL Server в `appsettings.json`.
2. Проверьте схему таблиц:
   - `Students`: `StudentId`, `FirstName`, `LastName`, `GroupId`
   - `Groups`: `GroupId`, `GroupName`
3. Выполните в каталоге проекта:

```bash
dotnet restore
dotnet run
```

4. Откройте адрес, выведенный `dotnet run`.
5. Страница автоматически запрашивает `GET /api/students/all`. Кнопка «Все группы» вызывает `GET /api/groups/all`.

## Маршруты

- `GET /api/students/all` — все студенты.
- `GET /api/students/{id}` — студент по ID.
- `GET /api/groups/all` — все группы.
- `GET /api/groups/{id}` — группа по ID.

Если в вашей базе имена колонок отличаются, измените SQL в репозиториях и свойства моделей.
