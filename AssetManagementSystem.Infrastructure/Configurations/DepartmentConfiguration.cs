namespace AssetManagementSystem.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AssetManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;    

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(department => department.Id);

        builder.Property(department => department.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(department => department.Description)
            .HasMaxLength(500);

        builder.Property(department => department.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(department => department.Code).IsUnique();
        builder.HasIndex(department => department.Name).IsUnique();



    }
}