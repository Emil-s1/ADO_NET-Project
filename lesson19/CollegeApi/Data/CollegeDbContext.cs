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
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            entity.HasOne<Group>()
                .WithMany()
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("Groups");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subjects");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable("Grades");
            entity.HasKey(x => x.Id);
            entity.HasOne<Student>()
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Subject>()
                .WithMany()
                .HasForeignKey(x => x.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
