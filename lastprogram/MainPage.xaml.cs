using lastprogram.Services;

namespace lastprogram;

public partial class MainPage : ContentPage
{
    private readonly CountryRepository _repository = new();

    public MainPage()
    {
        InitializeComponent();
        BuildSections();
    }

    private void BuildSections()
    {
        ResultsLayout.Children.Clear();

        // Задание 1: таблица "Страны" описана в CountryRepository (Seed) + Country.cs.
        AddSection("Задание 1: Структура таблицы",
            new[]
            {
                "CREATE TABLE Countries (",
                "    Name NVARCHAR(100) NOT NULL,",
                "    Capital NVARCHAR(100) NOT NULL,",
                "    Population BIGINT NOT NULL,",
                "    AreaKm2 FLOAT NOT NULL,",
                "    Continent NVARCHAR(50) NOT NULL",
                ");"
            });

        // Задание 2
        AddSection("Задание 2.1: Вся информация",
            _repository.GetAll().Select(c => $"{c.Name} — столица {c.Capital}, {c.Population:n0} чел., {c.AreaKm2:n0} км², {c.Continent}"));

        AddSection("Задание 2.2: Названия стран", _repository.GetCountryNames());
        AddSection("Задание 2.3: Названия столиц", _repository.GetCapitalNames());
        AddSection("Задание 2.4: Европейские страны", _repository.GetEuropeanCountries());
        AddSection("Задание 2.5: Площадь > 500 000 км²", _repository.GetCountriesWithAreaGreaterThan(500_000));

        // Задание 3
        AddSection("Задание 3.1: Страны с буквами a и u", _repository.GetCountriesWithLetters('a', 'u'));
        AddSection("Задание 3.2: Страны начинаются с 'a'", _repository.GetCountriesStartingWith('А'));
        AddSection("Задание 3.3: Площадь в диапазоне 100 000–1 000 000 км²", _repository.GetCountriesByAreaRange(100_000, 1_000_000));
        AddSection("Задание 3.4: Население > 10 000 000", _repository.GetCountriesWithPopulationGreaterThan(10_000_000));

        // Задание 4
        AddSection("Задание 4.1: Топ-5 по площади", _repository.GetTopCountriesByArea());
        AddSection("Задание 4.2: Топ-5 по населению", _repository.GetTopCountriesByPopulation());

        AddSection("Задание 4.3: Самая большая по площади",
            new[] { _repository.GetLargestByArea() ?? "Нет данных" });

        AddSection("Задание 4.4: Самая большая по населению",
            new[] { _repository.GetLargestByPopulation() ?? "Нет данных" });

        AddSection("Задание 4.5: Самая маленькая по площади в Африке",
            new[] { _repository.GetSmallestAreaInAfrica() ?? "Нет данных" });

        var avgAsia = _repository.GetAverageAreaInAsia();
        AddSection("Задание 4.6: Средняя площадь стран Азии",
            new[] { avgAsia.HasValue ? $"{avgAsia.Value:n0} км²" : "Нет данных" });
    }

    private void AddSection(string title, IEnumerable<string> lines)
    {
        ResultsLayout.Children.Add(new Label
        {
            Text = title,
            Style = (Style)Application.Current!.Resources["SubHeadline"]
        });

        ResultsLayout.Children.Add(new Label
        {
            Text = string.Join(Environment.NewLine, lines),
            Style = (Style)Application.Current!.Resources["Body"]
        });
    }
}
