namespace AssetManagementSystem.Domain.Interfaces;

/// <summary>
/// Gjeneron tekstin e njoftimit. Kthen null nese AI-ja nuk arrihet — alerti eshte
/// e rendesishmja, teksti eshte zbukurim, dhe nje deshtim i AI-se nuk guxon ta ndale.
/// </summary>
public interface IFeedbackGenerator
{
    Task<string?> GenerateAsync(string endpoint, int requestCount, CancellationToken cancellationToken);
}
