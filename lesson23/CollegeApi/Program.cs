using CollegeApi.Data;
using CollegeApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CollegeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CollegeDB")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/students/all", async (CollegeDbContext db) =>
    Results.Ok(await db.Students.AsNoTracking().OrderBy(x => x.Id).ToListAsync()));

app.MapGet("/groups/all", async (CollegeDbContext db) =>
    Results.Ok(await db.Groups.AsNoTracking().OrderBy(x => x.Id).ToListAsync()));

app.MapPost("/students/add", async (StudentCreate model, CollegeDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
        return Results.BadRequest(new { message = "Имя и фамилия обязательны." });

    if (model.GroupId.HasValue && !await db.Groups.AnyAsync(x => x.Id == model.GroupId.Value))
        return Results.BadRequest(new { message = "Указанная группа не найдена." });

    var student = new Student
    {
        FirstName = model.FirstName.Trim(),
        LastName = model.LastName.Trim(),
        GroupId = model.GroupId
    };

    db.Students.Add(student);
    await db.SaveChangesAsync();
    return Results.Created($"/students/{student.Id}", student);
});

app.MapPut("/students/{id:int}", async (int id, StudentCreate model, CollegeDbContext db) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null)
        return Results.NotFound(new { message = "Студент не найден." });

    if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
        return Results.BadRequest(new { message = "Имя и фамилия обязательны." });

    if (model.GroupId.HasValue && !await db.Groups.AnyAsync(x => x.Id == model.GroupId.Value))
        return Results.BadRequest(new { message = "Указанная группа не найдена." });

    student.FirstName = model.FirstName.Trim();
    student.LastName = model.LastName.Trim();
    student.GroupId = model.GroupId;

    await db.SaveChangesAsync();
    return Results.Ok(student);
});

app.MapDelete("/students/{id:int}", async (int id, CollegeDbContext db) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null)
        return Results.NotFound(new { message = "Студент не найден." });

    db.Students.Remove(student);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPost("/groups/add", async (GroupCreate model, CollegeDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(model.Name))
        return Results.BadRequest(new { message = "Название группы обязательно." });

    var group = new Group { Name = model.Name.Trim() };
    db.Groups.Add(group);
    await db.SaveChangesAsync();
    return Results.Created("/groups/all", group);
});

app.MapPut("/groups/{id:int}", async (int id, GroupCreate model, CollegeDbContext db) =>
{
    var group = await db.Groups.FindAsync(id);
    if (group is null)
        return Results.NotFound(new { message = "Группа не найдена." });

    if (string.IsNullOrWhiteSpace(model.Name))
        return Results.BadRequest(new { message = "Название группы обязательно." });

    group.Name = model.Name.Trim();
    await db.SaveChangesAsync();
    return Results.Ok(group);
});

app.MapDelete("/groups/{id:int}", async (int id, CollegeDbContext db) =>
{
    var group = await db.Groups.FindAsync(id);
    if (group is null)
        return Results.NotFound(new { message = "Группа не найдена." });

    var hasStudents = await db.Students.AnyAsync(student => student.GroupId == id);
    if (hasStudents)
        return Results.Conflict(new { message = "Нельзя удалить группу, в которой есть студенты." });

    db.Groups.Remove(group);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapFallbackToFile("index.html");
app.Run();
