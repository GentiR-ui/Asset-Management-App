using AssetManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace AssetManagementSystem.Infrastructure.Configurations;
public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(token => token.Id);
        // 64 karaktere hex = SHA-256. Unique: i njejti token nuk mund te ekzistoje dy here,
        // dhe index-i e ben kerkimin ne /refresh seek, jo scan.
        builder.Property(token => token.TokenHash)
            .IsRequired()
            .HasMaxLength(64);
        builder.HasIndex(token => token.TokenHash).IsUnique();
        // Kerkimi i dyte i shpeshte: te gjithe token-et e nje useri, per logout.
        builder.HasIndex(token => token.UserId);
        // Job-i i pastrimit: WHERE ExpiresAtUtc < @cutoff — pa kete, skanim i plote
        // cdo nate. Tani tabela ka pak rreshta, por rritet me cdo login dhe refresh.
        builder.HasIndex(token => token.ExpiresAtUtc);
        // Cascade, ndryshe nga entitetet e tjera: nese useri fshihet, token-et e tij
        // nuk kane me kuptim. Nuk eshte te dhene biznesi qe duhet mbrojtur.
        builder.HasOne(token => token.User)
               .WithMany()
               .HasForeignKey(token => token.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
