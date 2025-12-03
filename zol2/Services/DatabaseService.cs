using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using zol2.Models;

namespace zol2.Services
{
    public class DatabaseService
    {
        private SqlConnection? _connection;

        public bool IsConnected => _connection != null && _connection.State == ConnectionState.Open;

        public string LastError { get; private set; } = string.Empty;

        private string GetMasterConnectionString() => $"Server={_serverName};Database=master;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        
        private string GetConnectionString() => $"Server={_serverName};Database=CountriesDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        private string _serverName = "(localdb)\\MSSQLLocalDB";

        public DatabaseService()
        {
        }

        public void SetServerName(string serverName)
        {
            _serverName = serverName;
        }

        public bool Connect()
        {
            try
            {
                EnsureDatabaseExists();

                _connection = new SqlConnection(GetConnectionString());
                _connection.Open();
                
                EnsureTableExists();
                
                SeedDataIfEmpty();

                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public void Disconnect()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        private void EnsureDatabaseExists()
        {
            using (var masterConn = new SqlConnection(GetMasterConnectionString()))
            {
                masterConn.Open();
                var cmd = masterConn.CreateCommand();
                cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CountriesDb') CREATE DATABASE CountriesDb";
                cmd.ExecuteNonQuery();
            }
        }

        private void EnsureTableExists()
        {
            if (_connection == null) return;

            var cmd = _connection.CreateCommand();
            cmd.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Countries' AND xtype='U')
                CREATE TABLE Countries (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Capital NVARCHAR(100) NOT NULL,
                    Population BIGINT NOT NULL,
                    Area FLOAT NOT NULL,
                    Region NVARCHAR(50) NOT NULL,
                    CapitalPopulation BIGINT NOT NULL DEFAULT 0
                )";
            cmd.ExecuteNonQuery();
        }

        private void SeedDataIfEmpty()
        {
            if (_connection == null) return;

            var countCmd = _connection.CreateCommand();
            countCmd.CommandText = "SELECT COUNT(*) FROM Countries";
            int count = (int)countCmd.ExecuteScalar();

            if (count == 0)
            {
                var insertCmd = _connection.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT INTO Countries (Name, Capital, Population, Area, Region, CapitalPopulation) VALUES 
                    (N'Россия', N'Москва', 144100000, 17098246, N'Европа/Азия', 13010112),
                    (N'Китай', N'Пекин', 1402000000, 9596961, N'Азия', 21542000),
                    (N'США', N'Вашингтон', 331000000, 9833520, N'Северная Америка', 689545),
                    (N'Германия', N'Берлин', 83240000, 357022, N'Европа', 3645000),
                    (N'Франция', N'Париж', 67390000, 551695, N'Европа', 2161000),
                    (N'Япония', N'Токио', 125800000, 377975, N'Азия', 13960000),
                    (N'Бразилия', N'Бразилиа', 212600000, 8515767, N'Южная Америка', 3055000),
                    (N'Нигерия', N'Абуджа', 206100000, 923768, N'Африка', 1235880),
                    (N'Египет', N'Каир', 102300000, 1010408, N'Африка', 9844000),
                    (N'Австралия', N'Канберра', 25690000, 7692024, N'Австралия и Океания', 426704)";
                insertCmd.ExecuteNonQuery();
            }
        }

        private List<Country> ExecuteReader(string query, Dictionary<string, object>? parameters = null)
        {
            var list = new List<Country>();
            if (_connection == null || _connection.State != ConnectionState.Open) return list;

            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = query;
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Country
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Capital = reader.GetString(reader.GetOrdinal("Capital")),
                            Population = reader.GetInt64(reader.GetOrdinal("Population")),
                            Area = reader.GetDouble(reader.GetOrdinal("Area")),
                            Region = reader.GetString(reader.GetOrdinal("Region")),
                            CapitalPopulation = reader.GetInt64(reader.GetOrdinal("CapitalPopulation"))
                        });
                    }
                }
            }
            return list;
        }

        public List<Country> GetAllCountries()
        {
            return ExecuteReader("SELECT * FROM Countries");
        }

        public List<string> GetPartialInfo()
        {
            var list = new List<string>();
            if (_connection == null) return list;
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT Name, Capital FROM Countries";
            using (var reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                    list.Add($"{reader.GetString(0)} - {reader.GetString(1)}");
                }
            }
            return list;
        }

        public Country? GetCountryByName(string name)
        {
            var res = ExecuteReader("SELECT * FROM Countries WHERE Name = @name", new Dictionary<string, object> { { "@name", name } });
            return res.Count > 0 ? res[0] : null;
        }

        public string GetCitiesOfCountry(string countryName)
        {
            var c = GetCountryByName(countryName);
            return c != null ? $"Столица (единственный город в БД): {c.Capital}, Население: {c.CapitalPopulation}" : "Страна не найдена";
        }

        public List<Country> GetCountriesStartingWith(string letter)
        {
             return ExecuteReader("SELECT * FROM Countries WHERE Name LIKE @l + '%'", new Dictionary<string, object> { { "@l", letter } });
        }

        public List<Country> GetCapitalsStartingWith(string letter)
        {
             return ExecuteReader("SELECT * FROM Countries WHERE Capital LIKE @l + '%'", new Dictionary<string, object> { { "@l", letter } });
        }

        public List<Country> GetTop3CapitalsMinPop()
        {
            return ExecuteReader("SELECT TOP 3 * FROM Countries ORDER BY CapitalPopulation ASC");
        }

        public List<Country> GetTop3CountriesMinPop()
        {
            return ExecuteReader("SELECT TOP 3 * FROM Countries ORDER BY Population ASC");
        }

        public List<string> GetAvgCapitalPopByRegion()
        {
             var list = new List<string>();
            if (_connection == null) return list;
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT Region, AVG(CapitalPopulation) FROM Countries GROUP BY Region";
            using (var reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                    list.Add($"{reader.GetString(0)}: {reader.GetInt64(1)}");
                }
            }
            return list;
        }

        public List<Country> GetTop3MinPopByRegion(string region)
        {
             return ExecuteReader("SELECT TOP 3 * FROM Countries WHERE Region = @r ORDER BY Population ASC", new Dictionary<string, object> { { "@r", region } });
        }

        public List<Country> GetTop3MaxPopByRegion(string region)
        {
             return ExecuteReader("SELECT TOP 3 * FROM Countries WHERE Region = @r ORDER BY Population DESC", new Dictionary<string, object> { { "@r", region } });
        }

        public double GetAvgPopInCountry(string countryName)
        {
            var c = GetCountryByName(countryName);
            return c != null ? c.CapitalPopulation : 0;
        }

        public string GetCityWithMinPopInCountry(string countryName)
        {
            var c = GetCountryByName(countryName);
            return c != null ? $"{c.Capital} ({c.CapitalPopulation})" : "Страна не найдена";
        }
        
        public List<string> GetAllRegions()
        {
             var list = new List<string>();
            if (_connection == null) return list;
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT Region FROM Countries";
            using (var reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
            }
            return list;
        }
    }
}

