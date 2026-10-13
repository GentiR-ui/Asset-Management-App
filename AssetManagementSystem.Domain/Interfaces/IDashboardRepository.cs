using AssetManagementSystem.Domain.ReadModels;
namespace AssetManagementSystem.Domain.Interfaces;
public interface IDashboardRepository
{
    // Pa parameter departamenti: kufizimi sipas departamentit zbatohet nga global
    // query filter-i mbi Asset (shih ApplicationDbContext). Nje rregull, nje vend.
    Task<AssetValueSummary> AssetValueSummaryAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// ⚠️ PERJASHTIM nga rregulli i ketij interface-i: kjo metode ekzekuton stored procedure
    /// permes Dapper-it, pra SQL i papERPUNUAR — dhe global query filter-i NUK zbatohet.
    ///
    /// Departamenti duhet kaluar GJITHMONE; null-i do thote qellimisht "te gjitha"
    /// (vetem Admin). Nje thirrje qe harron parametrin kthen te dhena te departamenteve
    /// te huaja, dhe asgje nuk ankohet — as kompilatori, as EF-i.
    ///
    /// Kjo vlen njesoj per Dapper, FromSql dhe Database.SqlQuery: filtri global eshte kod
    /// qe EF-i shkruan vetem per query-te qe i nderton VETE me LINQ.
    /// </summary>
    Task<IReadOnlyList<DepartmentAssetValue>> AssetValuesByDepartmentAsync(
        Guid? departmentId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StatusAssetValue>> AssetValuesByStatusAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryAssetValue>> AssetValuesByCategoryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgeBucketCount>> AssetValuesByAgeAsync(AssetAgeCutoffs cutoffs, CancellationToken cancellationToken = default);
}
