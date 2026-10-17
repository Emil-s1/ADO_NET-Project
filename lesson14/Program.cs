using CollegeApi.Data;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(_ =>
    new SqlConnection(builder.Configuration.GetConnectionString("CollegeDB")));
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<GroupRepository>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/students", async (StudentRepository repository) =>
    Results.Ok(await repository.GetAllAsync()));

app.MapGet("/api/groups", async (GroupRepository repository) =>
    Results.Ok(await repository.GetAllAsync()));

app.MapGet("/api/students/{id:int}", async (int id, StudentRepository repository) =>
{
    var student = await repository.GetByIdAsync(id);
    return student is null ? Results.NotFound() : Results.Ok(student);
});

app.MapGet("/api/groups/{id:int}", async (int id, GroupRepository repository) =>
{
    var group = await repository.GetByIdAsync(id);
    return group is null ? Results.NotFound() : Results.Ok(group);
});

app.Run();
