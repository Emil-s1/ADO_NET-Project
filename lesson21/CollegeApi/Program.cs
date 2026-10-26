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
    Results.Ok(await db.Students.AsNoTracking()
        .OrderBy(student => student.Id)
        .ToListAsync()));

app.MapGet("/groups/all", async (CollegeDbContext db) =>
    Results.Ok(await db.Groups.AsNoTracking()
        .OrderBy(group => group.Id)
        .ToListAsync()));

app.MapPost("/students/add", async (StudentCreate model, CollegeDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(model.FirstName) ||
        string.IsNullOrWhiteSpace(model.LastName))
        return Results.BadRequest(new { message = "Имя и фамилия обязательны." });

    if (model.GroupId.HasValue &&
        !await db.Groups.AnyAsync(group => group.Id == model.GroupId.Value))
        return Results.BadRequest(new { message = "Указанная группа не найдена." });

    var student = new Student
    {
        FirstName = model.FirstName.Trim(),
        LastName = model.LastName.Trim(),
        GroupId = model.GroupId
    };

    db.Students.Add(student);
    await db.SaveChangesAsync();
    return Results.Created($"/students/all", student);
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

app.MapFallbackToFile("index.html");
app.Run();
