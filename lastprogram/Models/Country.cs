namespace lastprogram.Models;

/// <summary>
/// Представление записи таблицы "Страны".
/// </summary>
public class Country
{
    public string Name { get; init; } = string.Empty;
    public string Capital { get; init; } = string.Empty;
    public long Population { get; init; }
    public double AreaKm2 { get; init; }
    public string Continent { get; init; } = string.Empty;
}

