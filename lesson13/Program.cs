using CollegeApi.Data;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(_ =>
    new SqlConnection(builder.Configuration.GetConnectionString("CollegeDB")));
builder.Services.AddScoped<GroupRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/groups", async (GroupRepository repository) =>
{
    var groups = await repository.GetAllAsync();

    if (!groups.Any())
        return Results.NotFound("Группы не найдены.");

    var result = string.Join(Environment.NewLine,
        groups.Select(group => $"{group.GroupId}. {group.GroupName}"));

    return Results.Text(result, "text/plain; charset=utf-8");
});

app.Run();
