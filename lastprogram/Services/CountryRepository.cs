using lastprogram.Models;

namespace lastprogram.Services;

/// <summary>
/// Имитирует таблицу базы данных "Страны" и реализует LINQ-запросы.
/// </summary>
public class CountryRepository
{
    private readonly List<Country> _countries;

    public CountryRepository()
    {
        _countries = Seed();
    }

    public IEnumerable<Country> GetAll() => _countries;

    public IEnumerable<string> GetCountryNames() =>
        _countries.Select(c => c.Name);

    public IEnumerable<string> GetCapitalNames() =>
        _countries.Select(c => c.Capital);

    public IEnumerable<string> GetEuropeanCountries() =>
        _countries.Where(c => c.Continent.Equals("Европа", StringComparison.OrdinalIgnoreCase))
                  .Select(c => c.Name);

    public IEnumerable<string> GetCountriesWithAreaGreaterThan(double areaKm2) =>
        _countries.Where(c => c.AreaKm2 > areaKm2)
                  .Select(c => $"{c.Name} ({c.AreaKm2:n0} км²)");

    public IEnumerable<string> GetCountriesWithLetters(params char[] letters)
    {
        var lowerLetters = letters.Select(char.ToLowerInvariant).ToArray();
        return _countries
            .Where(c => lowerLetters.All(letter => c.Name.ToLowerInvariant().Contains(letter)))
            .Select(c => c.Name);
    }

    public IEnumerable<string> GetCountriesStartingWith(char letter) =>
        _countries.Where(c => c.Name.StartsWith(letter.ToString(), StringComparison.OrdinalIgnoreCase))
                  .Select(c => c.Name);

    public IEnumerable<string> GetCountriesByAreaRange(double minKm2, double maxKm2) =>
        _countries.Where(c => c.AreaKm2 >= minKm2 && c.AreaKm2 <= maxKm2)
                  .Select(c => $"{c.Name} ({c.AreaKm2:n0} км²)");

    public IEnumerable<string> GetCountriesWithPopulationGreaterThan(long population) =>
        _countries.Where(c => c.Population > population)
                  .Select(c => $"{c.Name} ({c.Population:n0} чел.)");

    public IEnumerable<string> GetTopCountriesByArea(int top = 5) =>
        _countries.OrderByDescending(c => c.AreaKm2)
                  .Take(top)
                  .Select(c => $"{c.Name} ({c.AreaKm2:n0} км²)");

    public IEnumerable<string> GetTopCountriesByPopulation(int top = 5) =>
        _countries.OrderByDescending(c => c.Population)
                  .Take(top)
                  .Select(c => $"{c.Name} ({c.Population:n0} чел.)");

    public string? GetLargestByArea() =>
        _countries.OrderByDescending(c => c.AreaKm2)
                  .Select(c => $"{c.Name} ({c.AreaKm2:n0} км²)")
                  .FirstOrDefault();

    public string? GetLargestByPopulation() =>
        _countries.OrderByDescending(c => c.Population)
                  .Select(c => $"{c.Name} ({c.Population:n0} чел.)")
                  .FirstOrDefault();

    public string? GetSmallestAreaInAfrica() =>
        _countries.Where(c => c.Continent.Equals("Африка", StringComparison.OrdinalIgnoreCase))
                  .OrderBy(c => c.AreaKm2)
                  .Select(c => $"{c.Name} ({c.AreaKm2:n0} км²)")
                  .FirstOrDefault();

    public double? GetAverageAreaInAsia() =>
        _countries.Where(c => c.Continent.Equals("Азия", StringComparison.OrdinalIgnoreCase))
                  .Select(c => c.AreaKm2)
                  .DefaultIfEmpty()
                  .Average();

    private static List<Country> Seed() =>
        new()
        {            
            new Country { Name = "Франция", Capital = "Париж", Population = 68000000, AreaKm2 = 643_801, Continent = "Европа" },
            new Country { Name = "Германия", Capital = "Берлин", Population = 84000000, AreaKm2 = 357_022, Continent = "Европа" },
            new Country { Name = "США", Capital = "Вашингтон", Population = 331_000_000, AreaKm2 = 9_833_520, Continent = "Северная Америка" },
            new Country { Name = "Канада", Capital = "Оттава", Population = 39_000_000, AreaKm2 = 9_984_670, Continent = "Северная Америка" },
            new Country { Name = "Китай", Capital = "Пекин", Population = 1_410_000_000, AreaKm2 = 9_596_961, Continent = "Азия" },
            new Country { Name = "Индия", Capital = "Нью-Дели", Population = 1_380_000_000, AreaKm2 = 3_287_263, Continent = "Азия" },
            new Country { Name = "Япония", Capital = "Токио", Population = 125_000_000, AreaKm2 = 377_975, Continent = "Азия" },
            new Country { Name = "Египет", Capital = "Каир", Population = 109_000_000, AreaKm2 = 1_010_408, Continent = "Африка" },
            new Country { Name = "Нигерия", Capital = "Абуджа", Population = 216_000_000, AreaKm2 = 923_768, Continent = "Африка" },
            new Country { Name = "ЮАР", Capital = "Претория", Population = 60_000_000, AreaKm2 = 1_219_090, Continent = "Африка" },
            new Country { Name = "Австралия", Capital = "Канберра", Population = 26_000_000, AreaKm2 = 7_692_024, Continent = "Океания" },
            new Country { Name = "Аргентина", Capital = "Буэнос-Айрес", Population = 46_000_000, AreaKm2 = 2_780_400, Continent = "Южная Америка" },
            new Country { Name = "Уругвай", Capital = "Монтевидео", Population = 3_500_000, AreaKm2 = 176_215, Continent = "Южная Америка" },
            new Country { Name = "Австрия", Capital = "Вена", Population = 9_100_000, AreaKm2 = 83_879, Continent = "Европа" },
        };
}

