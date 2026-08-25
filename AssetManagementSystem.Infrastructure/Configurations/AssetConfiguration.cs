using AssetManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetManagementSystem.Infrastructure.Configurations;

public sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(asset => asset.Id);

        builder.Property(asset => asset.AssetTag)
            .IsRequired()
            .HasMaxLength(50);

       
        builder.HasIndex(asset => asset.AssetTag).IsUnique();

        builder.Property(asset => asset.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(asset => asset.SerialNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(asset => asset.SerialNumber).IsUnique();

 
        builder.Property(asset => asset.Category).IsRequired();
        builder.Property(asset => asset.Status).IsRequired();


        builder.Property(asset => asset.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(asset => asset.Notes)
            .HasMaxLength(1000);

        
        builder.HasIndex(asset => asset.Status);
        builder.HasIndex(asset => asset.Category);
    }
}
