# CollegeApi — урок 14

Веб-приложение ASP.NET Core с фронтендом `wwwroot/index.html`, endpoints `MapGet()` и доступом к SQL Server через Dapper.

## Запуск

1. Укажите рабочую строку подключения в `appsettings.json`.
2. Убедитесь, что таблицы `Students` и `Groups` имеют столбцы, используемые запросами в `Data`.
3. В каталоге проекта выполните:

```bash
dotnet restore
dotnet run
```

4. Откройте локальный адрес, показанный командой `dotnet run`. Главная страница загрузит `wwwroot/index.html`.
5. Нажмите «Студенты» или «Группы». Страница делает `fetch()` запросы к `/api/students` и `/api/groups` и показывает ответ.

## Ожидаемая схема колонок

`Students`: `StudentId`, `FirstName`, `LastName`, `GroupId`.

`Groups`: `GroupId`, `GroupName`.

Если фактическая схема отличается, скорректируйте модели и SQL-запросы.
