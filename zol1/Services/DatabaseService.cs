using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace zol1.Services
{
    public class DatabaseService
    {
        private string _dbPath;
        private SqliteConnection? _connection;
        public bool IsConnected => _connection != null && _connection.State == System.Data.ConnectionState.Open;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "stationery.db");
        }

        public string Connect()
        {
            try
            {
                if (IsConnected) return "Уже подключено.";

                var connectionString = $"Data Source={_dbPath}";
                _connection = new SqliteConnection(connectionString);
                _connection.Open();

                InitializeDatabase();

                return $"Успешное подключение к БД: {_dbPath}";
            }
            catch (Exception ex)
            {
                return $"Ошибка подключения: {ex.Message}";
            }
        }

        public string Disconnect()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
                return "Успешное отключение от БД.";
            }
            return "База данных не была подключена.";
        }

        private void InitializeDatabase()
        {
            var commands = new List<string>
            {
                @"CREATE TABLE IF NOT EXISTS ProductTypes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT NOT NULL
                );",
                @"CREATE TABLE IF NOT EXISTS Managers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT NOT NULL
                );",
                @"CREATE TABLE IF NOT EXISTS Customers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT NOT NULL
                );",
                @"CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT NOT NULL, 
                    TypeId INTEGER, 
                    Quantity INTEGER, 
                    Cost REAL,
                    FOREIGN KEY(TypeId) REFERENCES ProductTypes(Id)
                );",
                @"CREATE TABLE IF NOT EXISTS Sales (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    ProductId INTEGER, 
                    ManagerId INTEGER, 
                    CustomerId INTEGER, 
                    Quantity INTEGER, 
                    UnitPrice REAL, 
                    SaleDate TEXT,
                    FOREIGN KEY(ProductId) REFERENCES Products(Id),
                    FOREIGN KEY(ManagerId) REFERENCES Managers(Id),
                    FOREIGN KEY(CustomerId) REFERENCES Customers(Id)
                );"
            };

            foreach (var cmd in commands)
            {
                using var command = _connection.CreateCommand();
                command.CommandText = cmd;
                command.ExecuteNonQuery();
            }

            SeedData();
        }

        private void SeedData()
        {
            using var checkCmd = _connection.CreateCommand();
            checkCmd.CommandText = "SELECT COUNT(*) FROM ProductTypes";
            var result = checkCmd.ExecuteScalar();
            var count = result != null ? Convert.ToInt64(result) : 0;

            if (count == 0)
            {
                ExecuteSql("INSERT INTO ProductTypes (Name) VALUES ('Ручки'), ('Карандаши'), ('Бумага');");
                ExecuteSql("INSERT INTO Managers (Name) VALUES ('Иванов И.И.'), ('Петров П.П.'), ('Сидоров С.С.');");
                ExecuteSql("INSERT INTO Customers (Name) VALUES ('ООО Вектор'), ('ЗАО СтройМаш'), ('ИП Смирнов');");
                
                // Товары
                ExecuteSql("INSERT INTO Products (Name, TypeId, Quantity, Cost) VALUES ('Ручка синяя', 1, 1000, 10.5);");
                ExecuteSql("INSERT INTO Products (Name, TypeId, Quantity, Cost) VALUES ('Ручка красная', 1, 500, 11.0);");
                ExecuteSql("INSERT INTO Products (Name, TypeId, Quantity, Cost) VALUES ('Карандаш HB', 2, 2000, 5.0);");
                ExecuteSql("INSERT INTO Products (Name, TypeId, Quantity, Cost) VALUES ('Бумага А4', 3, 50, 250.0);");

                // Продажи
                ExecuteSql("INSERT INTO Sales (ProductId, ManagerId, CustomerId, Quantity, UnitPrice, SaleDate) VALUES (1, 1, 1, 100, 15.0, '2023-10-01');");
                ExecuteSql("INSERT INTO Sales (ProductId, ManagerId, CustomerId, Quantity, UnitPrice, SaleDate) VALUES (3, 2, 2, 200, 8.0, '2023-10-02');");
                ExecuteSql("INSERT INTO Sales (ProductId, ManagerId, CustomerId, Quantity, UnitPrice, SaleDate) VALUES (4, 3, 3, 10, 400.0, '2023-10-05');");
            }
        }

        private void ExecuteSql(string sql)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        // Общий метод для выполнения запросов SELECT и возврата строки
        public string ExecuteQuery(string sql)
        {
            if (!IsConnected) return "Ошибка: Нет подключения к БД.";

            var sb = new StringBuilder();
            try
            {
                using var command = _connection.CreateCommand();
                command.CommandText = sql;
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        sb.Append($"{reader.GetName(i)}: {reader.GetValue(i)} | ");
                    }
                    sb.AppendLine();
                }

                if (sb.Length == 0) return "Нет данных.";
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Ошибка выполнения запроса: {ex.Message}";
            }
        }

        // Задание 3
        public string GetAllProducts() => ExecuteQuery("SELECT p.Name AS Товар, t.Name AS Тип, p.Quantity AS Колво, p.Cost AS Себестоимость FROM Products p JOIN ProductTypes t ON p.TypeId = t.Id");
        public string GetAllTypes() => ExecuteQuery("SELECT Name AS Тип FROM ProductTypes");
        public string GetAllManagers() => ExecuteQuery("SELECT Name AS Менеджер FROM Managers");
        public string GetProductsMaxQuantity() => ExecuteQuery("SELECT * FROM Products WHERE Quantity = (SELECT MAX(Quantity) FROM Products)");
        public string GetProductsMinQuantity() => ExecuteQuery("SELECT * FROM Products WHERE Quantity = (SELECT MIN(Quantity) FROM Products)");
        public string GetProductsMinCost() => ExecuteQuery("SELECT * FROM Products WHERE Cost = (SELECT MIN(Cost) FROM Products)");
        public string GetProductsMaxCost() => ExecuteQuery("SELECT * FROM Products WHERE Cost = (SELECT MAX(Cost) FROM Products)");

        // Задание 4
        public string GetProductsByType(string typeName) => ExecuteQuery($"SELECT p.Name, t.Name FROM Products p JOIN ProductTypes t ON p.TypeId = t.Id WHERE t.Name LIKE '%{typeName}%'");
        public string GetSalesByManager(string managerName) => ExecuteQuery($"SELECT s.Id, m.Name as Manager, p.Name as Product, s.Quantity FROM Sales s JOIN Managers m ON s.ManagerId = m.Id JOIN Products p ON s.ProductId = p.Id WHERE m.Name LIKE '%{managerName}%'");
        public string GetSalesByCustomer(string customerName) => ExecuteQuery($"SELECT s.Id, c.Name as Customer, p.Name as Product FROM Sales s JOIN Customers c ON s.CustomerId = c.Id JOIN Products p ON s.ProductId = p.Id WHERE c.Name LIKE '%{customerName}%'");
        public string GetLatestSale() => ExecuteQuery("SELECT * FROM Sales ORDER BY SaleDate DESC LIMIT 1");
        public string GetAvgQuantityByType() => ExecuteQuery("SELECT t.Name, AVG(p.Quantity) as AvgQty FROM Products p JOIN ProductTypes t ON p.TypeId = t.Id GROUP BY t.Name");
    }
}

