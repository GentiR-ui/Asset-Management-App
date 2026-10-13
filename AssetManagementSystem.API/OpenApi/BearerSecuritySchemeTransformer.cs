using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace AssetManagementSystem.API.OpenApi;

/// <summary>
/// Shton skemen e sigurise "Bearer" ne dokumentin OpenAPI.
///
/// Pa kete, Swagger UI nuk ka butonin Authorize dhe cdo endpoint i mbrojtur kthen 401 —
/// ndërfaqja do dukej e plote por do ishte e papërdorshme.
/// </summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Ngjit vetem token-in, pa fjalen 'Bearer'."
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

        return Task.CompletedTask;
    }
}
