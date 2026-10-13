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
        // ════════ INDEKSE MBULUESE PER RAPORTET E DASHBOARD-IT ════════
        //
        // Matur para shtimit: SELECT Status, COUNT(*), SUM(PurchasePrice) GROUP BY Status
        //   → 2,715 logical reads per 4 rreshta rezultat (skanim i tabeles se plote).
        //
        // Arsyeja: tabela ka 16 kolona (~437 bajt/rresht, 2,715 faqe), kurse query-ja
        // lexon TRI — Status, PurchasePrice dhe DepartmentId (ky i fundit nga filtri global).
        //
        // Indeksi i vjeter HasIndex(Status) ekzistonte dhe NUK u perdor kurre: pa
        // PurchasePrice brenda, do duhej nje Key Lookup per cdo rresht, pra skanimi i
        // tabeles ishte me i lire. Nje indeks mbulues duhet te mbuloje GJITHE query-n —
        // SELECT, WHERE, GROUP BY — ndryshe optimizuesi e injoron.
        //
        // Status eshte CELES (indeksi vjen i renditur sipas tij → Stream Aggregate, pa Sort);
        // PurchasePrice dhe DepartmentId jane INCLUDE (bartur ne gjethe, pa renditje).
        builder.HasIndex(asset => asset.Status)
            .IncludeProperties(asset => new { asset.PurchasePrice, asset.DepartmentId })
            .HasDatabaseName("IX_Assets_Status_Price");
        builder.HasIndex(asset => asset.Category)
            .IncludeProperties(asset => new { asset.PurchasePrice, asset.DepartmentId })
            .HasDatabaseName("IX_Assets_Category_Price");
        // Raporti i moshes: GROUP BY mbi shprehje te PurchaseDate + SUM(PurchasePrice).
        builder.HasIndex(asset => asset.PurchaseDate)
            .IncludeProperties(asset => new { asset.PurchasePrice, asset.DepartmentId })
            .HasDatabaseName("IX_Assets_PurchaseDate_Price");
        // Nje indeks, dy query:
        //   GET /api/assets  → WHERE DepartmentId = @d ORDER BY AssetTag
        //                      seek mbi DepartmentId dhe rend i gatshem → Sort-i bie nga plani
        //   value-by-department → GROUP BY DepartmentId, SUM(PurchasePrice)  (i mbuluar)
        //
        // DepartmentId i pari: zevendeson edhe indeksin automatik te celesit te huaj,
        // sepse EF-i nuk krijon te vecante kur ekziston nje me ate kolone te PARE.
        builder.HasIndex(asset => new { asset.DepartmentId, asset.AssetTag })
            .IncludeProperties(asset => asset.PurchasePrice)
            .HasDatabaseName("IX_Assets_Dept_Tag_Price");
        // Job-i i garancive: range scan mbi daten, me fushat qe email-i lexon.
        builder.HasIndex(asset => asset.WarrantyExpiryDate)
            .IncludeProperties(asset => new { asset.AssetTag, asset.Name, asset.DepartmentId })
            .HasDatabaseName("IX_Assets_Warranty");
        builder.HasOne(asset => asset.AssignedToEmployee)
            .WithMany()
            .HasForeignKey(asset => asset.AssignedToEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(asset => asset.DepartmentId)
            .IsRequired();
        builder.HasOne(asset => asset.Department)
            .WithMany(department => department.Assets)
            .HasForeignKey(asset => asset.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
