using AssetManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetManagementSystem.Infrastructure.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(employee => employee.Id);

        builder.Property(employee => employee.EmployeeCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(employee => employee.EmployeeCode).IsUnique();

        builder.HasOne(employee => employee.User)
               .WithOne()
               .HasForeignKey<Employee>(employee => employee.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(employee => employee.Department)
               .WithMany(department => department.Employees)
               .HasForeignKey(employee => employee.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
