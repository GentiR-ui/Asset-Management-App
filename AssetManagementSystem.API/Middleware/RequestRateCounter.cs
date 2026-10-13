using System.Collections.Concurrent;

namespace AssetManagementSystem.API.Middleware;

/// <summary>
/// Numeron kerkesat per celes brenda nje dritareje kohore. Regjistrohet si Singleton,
/// pra numeruesit jetojne sa jeton procesi.
///
/// KUFIZIM I DITUR: numeruesit rrine ne memorie te procesit.
///   - humbin ne rinisje te aplikacionit
///   - nuk ndahen mes disa instancave te API-se
/// Ne prodhim kjo do te shkonte ne Redis me INCR + EXPIRE: atomik dhe i perbashket.
/// Per demonstrimin e sjelljes, memoria mjafton dhe nuk shton varesi.
///
/// KUFIZIM I DYTE: celesat nuk pastrohen kurre. Fjalori rritet derisa procesi te rniset.
/// Me nje numer te kufizuar userash dhe endpoint-esh kjo nuk ka pasoje praktike.
/// </summary>
public sealed class RequestRateCounter
{
    private readonly ConcurrentDictionary<string, Counter> _counters = new();

    /// <summary>
    /// Rrit numeruesin e ketij celesi dhe kthen vleren e re.
    /// Nese dritarja e meparshme ka skaduar, numerimi rifillon nga 1.
    /// </summary>
    public int Increment(string key, TimeSpan window)
    {
        var counter = _counters.GetOrAdd(key, _ => new Counter(DateTime.UtcNow));

        // ConcurrentDictionary e mbron FJALORIN, jo objektin brenda tij. Pa kete lock,
        // dy kerkesa paralele mund ta lexojne te njejtin Count dhe te kthejne te njejtin
        // numer — dhe atehere pragu ose do ndizej dy here, ose nuk do ndizej fare.
        lock (counter)
        {
            var now = DateTime.UtcNow;

            if (now - counter.WindowStart >= window)
            {
                counter.WindowStart = now;
                counter.Count = 0;
            }

            return ++counter.Count;
        }
    }

    private sealed class Counter
    {
        public Counter(DateTime windowStart) => WindowStart = windowStart;

        public DateTime WindowStart { get; set; }

        public int Count { get; set; }
    }
}
