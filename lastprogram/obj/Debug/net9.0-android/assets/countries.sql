CREATE DATABASE CountriesDb;
GO

USE CountriesDb;
GO

IF OBJECT_ID('dbo.Countries', 'U') IS NOT NULL
    DROP TABLE dbo.Countries;
GO

CREATE TABLE dbo.Countries
(
    Name NVARCHAR(100) NOT NULL PRIMARY KEY,
    Capital NVARCHAR(100) NOT NULL,
    Population BIGINT NOT NULL,
    AreaKm2 FLOAT NOT NULL,
    Continent NVARCHAR(50) NOT NULL
);
GO

INSERT INTO dbo.Countries (Name, Capital, Population, AreaKm2, Continent) VALUES
    (N'Украина', N'Киев', 41000000, 603700, N'Европа'),
    (N'Франция', N'Париж', 68000000, 643801, N'Европа'),
    (N'Германия', N'Берлин', 84000000, 357022, N'Европа'),
    (N'США', N'Вашингтон', 331000000, 9833520, N'Северная Америка'),
    (N'Канада', N'Оттава', 39000000, 9984670, N'Северная Америка'),
    (N'Китай', N'Пекин', 1410000000, 9596961, N'Азия'),
    (N'Индия', N'Нью-Дели', 1380000000, 3287263, N'Азия'),
    (N'Япония', N'Токио', 125000000, 377975, N'Азия'),
    (N'Египет', N'Каир', 109000000, 1010408, N'Африка'),
    (N'Нигерия', N'Абуджа', 216000000, 923768, N'Африка'),
    (N'ЮАР', N'Претория', 60000000, 1219090, N'Африка'),
    (N'Австралия', N'Канберра', 26000000, 7692024, N'Океания'),
    (N'Аргентина', N'Буэнос-Айрес', 46000000, 2780400, N'Южная Америка'),
    (N'Уругвай', N'Монтевидео', 3500000, 176215, N'Южная Америка'),
    (N'Австрия', N'Вена', 9100000, 83879, N'Европа');
GO

