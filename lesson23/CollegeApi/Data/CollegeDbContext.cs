using CollegeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeApi.Data;

public sealed class CollegeDbContext : DbContext
{
    public CollegeDbContext(DbContextOptions<CollegeDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Group> Groups => Set<Group>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        BuildStudent(modelBuilder);
        BuildGroup(modelBuilder);

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subjects");
            entity.HasKey(subject => subject.Id);
            entity.Property(subject => subject.Name)
                .HasMaxLength(150)
                .IsRequired();
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(teacher => teacher.Id);
            entity.Property(teacher => teacher.FirstName)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(teacher => teacher.LastName)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable("Grades");
            entity.HasKey(grade => grade.Id);
            entity.Property(grade => grade.Value).IsRequired();

            entity.HasOne<Student>()
                .WithMany()
                .HasForeignKey(grade => grade.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Subject>()
                .WithMany()
                .HasForeignKey(grade => grade.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void BuildStudent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(student => student.Id);

            entity.Property(student => student.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(student => student.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasOne<Group>()
                .WithMany()
                .HasForeignKey(student => student.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void BuildGroup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("Groups");
            entity.HasKey(group => group.Id);

            entity.Property(group => group.Name)
                .HasMaxLength(100)
                .IsRequired();
        });
    }
}
