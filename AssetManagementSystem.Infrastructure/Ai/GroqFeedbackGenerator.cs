using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AssetManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AssetManagementSystem.Infrastructure.Ai;

/// <summary>
/// Implementimi i IFeedbackGenerator me Groq (API i pajtueshem me formen e OpenAI-t).
///
/// Ofruesi i pare i provuar ishte Gemini; u zevendesua sepse free tier-i i tij nuk
/// ofrohet ne Kosove (400 FAILED_PRECONDITION, i testuar me dy celesa). Ndryshimi
/// preku vetem kete klase, nje rresht ne DI dhe konfigurimin — asgje ne Application.
/// </summary>
public sealed class GroqFeedbackGenerator : IFeedbackGenerator
{
    private const string Endpoint = "https://api.groq.com/openai/v1/chat/completions";

    private readonly HttpClient _httpClient;
    private readonly ILogger<GroqFeedbackGenerator> _logger;
    private readonly bool _enabled;
    private readonly string _model;
    private readonly string _tone;

    public GroqFeedbackGenerator(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GroqFeedbackGenerator> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Modeli ne konfigurim, jo ne kod: emrat e modeleve dalin nga perdorimi shpesh.
        // Gemini e deshmoi kete — gemini-2.0-flash ishte hequr dhe kthente 404.
        _model = configuration["Groq:Model"] ?? "openai/gpt-oss-20b";
        _tone = configuration["Feedback:Tone"] ?? "formal";

        var apiKey = configuration["Groq:ApiKey"];
        _enabled = !string.IsNullOrWhiteSpace(apiKey);

        if (_enabled)
        {
            // Celesi shkon ne HEADER, jo ne query string si te Gemini — pra nuk
            // perfundon ne logje serveri, ne proxy, as ne historik URL-sh.
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
        }
    }

    public async Task<string?> GenerateAsync(
        string endpoint,
        int requestCount,
        CancellationToken cancellationToken)
    {
        // Pa celes, gjeneratori rri i fikur dhe njoftimi dergohet me tekst te parazgjedhur.
        // Logohet me qellim: ndryshe "AI: jo" nuk dallohet nga nje deshtim i vertete.
        if (!_enabled)
        {
            _logger.LogInformation("Gjeneratori i AI-se eshte i fikur: mungon Groq:ApiKey.");
            return null;
        }

        var payload = new
        {
            model = _model,
            messages = new[] { new { role = "user", content = BuildPrompt(endpoint, requestCount) } },

            // 1000 e jo 300: gpt-oss eshte model ARSYETUES. Token-et e arsyetimit nuk
            // shfaqen te "content", por hyjne ne max_tokens. Me 300, arsyetimi i harxhonte
            // te gjithe dhe content-i kthehej bosh — pa gabim, vetem "AI: jo".
            max_tokens = 1000,

            // Arsyetim i ulet: me vleren e parazgjedhur, modeli shpenzonte 110+ token per
            // te menduar dhe nje nga tri pergjigjet kthehej BOSH. Me "low": 5-27 token,
            // dhe 4/4 pergjigje te plota. I matur, jo i hamendesuar.
            reasoning_effort = "low",

            // Per shaka, perseritja e vret: 1.2 jep tekst te ndryshem cdo here.
            // Per nje njoftim operacional, larmia nuk sjell gje: 0.4 e mban te qendrueshem.
            temperature = _tone == "playful" ? 1.2 : 0.4
        };

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(Endpoint, payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogWarning(
                    "Groq u pergjigj me {Status}: {Error}", (int)response.StatusCode, error);

                return null;
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            // Pergjigje pa "choices" eshte rast normal, jo gabim: ndodh kur filtrat
            // e ofruesit e bllokojne gjenerimin.
            if (!document.RootElement.TryGetProperty("choices", out var choices)
                || choices.GetArrayLength() == 0)
            {
                _logger.LogWarning("Groq nuk ktheu asnje pergjigje. Perdoret teksti i parazgjedhur.");
                return null;
            }

            var text = choices[0].GetProperty("message").GetProperty("content").GetString();

            return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        }
        catch (Exception exception)
        {
            // Graceful degradation, si te CacheService: logo dhe vazhdo.
            // Alerti eshte e rendesishmja; teksti eshte zbukurim.
            _logger.LogWarning(exception, "Thirrja e Groq deshtoi. Perdoret teksti i parazgjedhur.");

            return null;
        }
    }

    /// <summary>
    /// Ndertimi i promptit sipas tonit te konfiguruar.
    ///
    /// Ne prompt shkojne VETEM dy fakte: endpoint-i dhe numri i kerkesave. Asnje email,
    /// asnje emer, asnje Id — te dhenat e kompanise nuk dalin jashte rrjetit te saj.
    ///
    /// Toni eshte konfigurim, jo kod: i njejti mekanizem jep nje demo argetuese ose nje
    /// njoftim operacional, pa prekur asgje tjeter.
    ///
    /// Prompti eshte ne ANGLISHT me qellim. Modeli i vetem i disponueshem per kete celes
    /// eshte openai/gpt-oss-20b, dhe shqipja e tij doli e paperdorshme — fjali pa kuptim
    /// dhe numra te shpikur. Ne anglisht: 5/5 pergjigje te sakta, me numrin e duhur.
    /// Modeli nuk ndryshohej dot (403 per te tjeret), prandaj u ndryshua gjuha.
    /// </summary>
    private string BuildPrompt(string endpoint, int requestCount) => _tone == "playful"
        ? $"""
           You are the API of an asset-management system. The same user has just hit
           GET {endpoint} {requestCount} times in 60 seconds, and you are tired.

           Write ONE dry, witty line, max 20 words. Use the number {requestCount}.
           Mock the BEHAVIOUR, never the person. No insults, no profanity.
           Return ONLY the line, no quotes, no preamble.

           EXAMPLES:
           - Five refreshes in a minute. I promise, no laptop has escaped the office since you started.
           - That is five identical questions. The answer was not shy the first four times.
           - Five hits in sixty seconds. The database says hello, and asks for a short break.
           """
        : $"""
           Write a short, professional notice, maximum two sentences.
           A user made {requestCount} GET requests to {endpoint} within 60 seconds.
           Do not accuse anyone of abuse — this is an observation notice.
           Return ONLY the text, no preamble.
           """;
}
