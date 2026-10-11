using Microsoft.Data.SqlClient;
using CollegeApp.Data;
using CollegeApp.Models;

const string connectionString =
    @"Server=(localdb)\MSSQLLocalDB;Database=CollegeDB;Integrated Security=True;TrustServerCertificate=True;";

try
{
    EnsureSchema(connectionString);

    var studentRepository = new StudentRepository(connectionString);
    var groupRepository = new GroupRepository(connectionString);

    Console.WriteLine("Группы:");
    foreach (Group group in groupRepository.GetAll())
        Console.WriteLine(group);

    Console.WriteLine("\nСтуденты:");
    foreach (Student student in studentRepository.GetAll())
        Console.WriteLine(student);

  
}
catch (SqlException ex)
{
    Console.Error.WriteLine($"Ошибка SQL Server: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    Console.Error.WriteLine($"Ошибка приложения: {ex.Message}");
}

static void EnsureSchema(string connectionString)
{
    const string sql = """
        IF OBJECT_ID(N'dbo.Groups', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Groups
            (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL UNIQUE
            );
        END;

        IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Students
            (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                FirstName NVARCHAR(100) NOT NULL,
                LastName NVARCHAR(100) NOT NULL,
                GroupId INT NULL,
                CONSTRAINT FK_Students_Groups
                    FOREIGN KEY (GroupId) REFERENCES dbo.Groups(Id)
                    ON DELETE SET NULL
            );
        END;

        IF OBJECT_ID(N'dbo.Teachers', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Teachers
            (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                FirstName NVARCHAR(100) NOT NULL,
                LastName NVARCHAR(100) NOT NULL
            );
        END;

        IF OBJECT_ID(N'dbo.Subjects', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Subjects
            (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(150) NOT NULL UNIQUE,
                TeacherId INT NULL,
                CONSTRAINT FK_Subjects_Teachers
                    FOREIGN KEY (TeacherId) REFERENCES dbo.Teachers(Id)
                    ON DELETE SET NULL
            );
        END;

        IF OBJECT_ID(N'dbo.Grades', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Grades
            (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                StudentId INT NOT NULL,
                SubjectId INT NOT NULL,
                Grade TINYINT NOT NULL CHECK (Grade BETWEEN 1 AND 5),
                GradeDate DATE NOT NULL DEFAULT (CONVERT(date, GETDATE())),
                CONSTRAINT FK_Grades_Students
                    FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id)
                    ON DELETE CASCADE,
                CONSTRAINT FK_Grades_Subjects
                    FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(Id)
                    ON DELETE CASCADE
            );
        END;
        """;

    using var connection = new SqlConnection(connectionString);
    using var command = new SqlCommand(sql, connection);
    connection.Open();
    command.ExecuteNonQuery();
}
