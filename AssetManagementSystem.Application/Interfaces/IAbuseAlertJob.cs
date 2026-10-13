namespace AssetManagementSystem.Application.Interfaces;

/// <summary>
/// Njoftimi per sjellje te pazakonte. Ekzekutohet ne prapavije, pra kerkesa HTTP
/// kthehet menjehere pa pritur as leximin e userit, as dergimin e email-it.
///
/// Parametrat jane tipe te thjeshta me qellim: Hangfire e serializon thirrjen ne
/// databaze si tekst. Objekte nuk kalojne dot, dhe HttpContext nuk ekziston fare
/// kur job-i ekzekutohet — konteksti duhet KALUAR, nuk trashegohet.
/// </summary>
public interface IAbuseAlertJob
{
    Task ExecuteAsync(Guid userId, string endpoint, int requestCount, CancellationToken cancellationToken);
}
