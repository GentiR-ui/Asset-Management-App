using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AssetManagementSystem.Infrastructure.Identity;

/// <summary>
/// Lexon userin e kerkeses aktuale per nevojat e Infrastructure: ApplicationDbContext
/// (ApplyAuditFields) dhe AuditExecuteUpdateInterceptor.
///
/// Ekziston sepse Infrastructure referencon vetem Domain dhe nuk e sheh dot
/// ICurrentUserService, qe jeton ne Application. Me kete klase, Domain-i nuk ngarkohet
/// me nje kontrate qe nuk eshte rregull biznesi.
///
/// ⚠️ Pasoja qe duhet ditur: tani userin e lexojne DY klasa — kjo dhe CurrentUserService
/// ne Application. Logjika e claim-eve duhet te mbetet IDENTIKE ne te dyja. Nje here ka
/// divergjuar tashme: njera kontrollonte edhe "sub", tjetra jo, dhe UpdatedBy mbetej bosh
/// per disa token-e. Nese ndryshon njera, ndrysho edhe tjetren.
///
/// Brenda Infrastructure, kjo eshte vendi I VETEM qe di si lexohet useri.
/// </summary>
public sealed class CurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Guid.Empty kur nuk ka kerkese HTTP: migrim, seed, ose background job.
    /// Thirresit e kthejne ne NULL para se ta ruajne — zero-t do dukeshin si te dhene e vertete.
    /// </summary>
    public Guid UserId
    {
        get
        {
            // Veti e kalkuluar, jo fushe: lexohet NE MOMENTIN e thirrjes. Pikerisht kjo e
            // ben klasen te sigurt si Singleton, sepse IHttpContextAccessor e kthen
            // kerkesen e thread-it aktual permes AsyncLocal.
            var user = _httpContextAccessor.HttpContext?.User;

            var id = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? user?.FindFirstValue("sub");

            return Guid.TryParse(id, out var userId) ? userId : Guid.Empty;
        }
    }
}
