namespace AssetManagementSystem.Domain.Common;
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    /// <summary>
    /// Nullable sepse jo cdo rresht krijohet nga nje user: login-i krijon RefreshToken para
    /// se useri te autentikohet, dhe seed-i e background job-et nuk kane user fare.
    /// NULL e thote te verteten; Guid.Empty do dukej si te dhene e vertete.
    /// </summary>
    public Guid? CreatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }



    
}