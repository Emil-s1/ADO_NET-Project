using CollegeApi.Data;
using CollegeApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddScoped<SqlConnectionFactory>();
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<GroupRepository>();

var app = builder.Build();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/students", async (StudentRepository repository) =>
    Results.Ok(await repository.GetAll()));

app.MapGet("/api/student/{id:int}", async (int id, StudentRepository repository) =>
{
    var student = await repository.GetById(id);
    return student is null ? Results.NotFound() : Results.Ok(student);
});

app.MapPost("/api/student", async (StudentCreate model, StudentRepository repository) =>
{
    if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
        return Results.BadRequest(new { message = "Имя и фамилия обязательны." });

    var id = await repository.Create(model);
    var created = await repository.GetById(id);
    return Results.Created($"/api/student/{id}", created);
});

app.MapPut("/api/student/{id:int}", async (int id, StudentCreate model, StudentRepository repository) =>
{
    var updated = await repository.Update(id, model);
    return updated ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/api/student/{id:int}", async (int id, StudentRepository repository) =>
{
    var deleted = await repository.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.MapGet("/api/groups", async (GroupRepository repository) =>
    Results.Ok(await repository.GetAll()));

app.MapFallbackToFile("index.html");
app.Run();
