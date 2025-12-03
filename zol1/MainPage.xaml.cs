using Microsoft.Maui.Controls;
using zol1.Services;
using System;

namespace zol1
{
    public partial class MainPage : ContentPage
    {
        private DatabaseService _dbService;

        public MainPage()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        private void OnConnectClicked(object sender, EventArgs e)
        {
            string result = _dbService.Connect();
            OutputLabel.Text = result;
            // Можно добавить визуальную индикацию, если нужно
        }

        private void OnDisconnectClicked(object sender, EventArgs e)
        {
            string result = _dbService.Disconnect();
            OutputLabel.Text = result;
        }

        private void OnExecuteQueryClicked(object sender, EventArgs e)
        {
            if (!_dbService.IsConnected)
            {
                OutputLabel.Text = "Ошибка: Сначала подключитесь к базе данных!";
                return;
            }

            string selectedQuery = QueryPicker.SelectedItem as string;
            string param = ParamEntry.Text;
            string result = "";

            if (string.IsNullOrEmpty(selectedQuery))
            {
                OutputLabel.Text = "Пожалуйста, выберите запрос из списка.";
                return;
            }

            try 
            {
                switch (selectedQuery)
                {
                    case "Все товары":
                        result = _dbService.GetAllProducts();
                        break;
                    case "Все типы":
                        result = _dbService.GetAllTypes();
                        break;
                    case "Все менеджеры":
                        result = _dbService.GetAllManagers();
                        break;
                    case "Товары с макс. кол-вом":
                        result = _dbService.GetProductsMaxQuantity();
                        break;
                    case "Товары с мин. кол-вом":
                        result = _dbService.GetProductsMinQuantity();
                        break;
                    case "Товары с мин. себестоимостью":
                        result = _dbService.GetProductsMinCost();
                        break;
                    case "Товары с макс. себестоимостью":
                        result = _dbService.GetProductsMaxCost();
                        break;
                    case "Товары заданного типа (ввод)":
                        result = _dbService.GetProductsByType(param);
                        break;
                    case "Продажи менеджера (ввод)":
                        result = _dbService.GetSalesByManager(param);
                        break;
                    case "Продажи фирме (ввод)":
                        result = _dbService.GetSalesByCustomer(param);
                        break;
                    case "Последняя продажа":
                        result = _dbService.GetLatestSale();
                        break;
                    case "Среднее кол-во по типам":
                        result = _dbService.GetAvgQuantityByType();
                        break;
                    default:
                        result = "Неизвестный запрос.";
                        break;
                }
            }
            catch (Exception ex)
            {
                result = $"Ошибка при выполнении: {ex.Message}";
            }

            OutputLabel.Text = result;
        }
    }
}
