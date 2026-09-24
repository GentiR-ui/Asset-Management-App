using AssetManagementSystem.Application.DTOs.Common;
using AssetManagementSystem.Domain.ReadModels;

namespace AssetManagementSystem.Application.Common.Mappings;

public static class PagedMappings
{
    /// <summary>
    /// Perkthen nje faqe entitetesh ne pergjigje, duke perdorur mapimin e vet te secilit lloj.
    /// Ekziston qe te mos perseritet i njejti bllok ne kater sherbime.
    /// </summary>
    public static PagedResponse<TResponse> ToPagedResponse<TEntity, TResponse>(
        this PagedResult<TEntity> result,
        int page,
        int pageSize,
        Func<TEntity, TResponse> map)
    {
        return new PagedResponse<TResponse>
        {
            Items = result.Items.Select(map).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        };
    }
}
