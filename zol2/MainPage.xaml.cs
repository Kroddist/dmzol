using System;
using System.Text;
using zol2.Services;
using zol2.Models;

namespace zol2
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _dbService;

        public MainPage(DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
        }

        private void UpdateStatus()
        {
            if (_dbService.IsConnected)
            {
                StatusLabel.Text = "Подключено к БД 'CountriesDb'";
                StatusLabel.TextColor = Colors.Green;
                ConnectBtn.IsEnabled = false;
                DisconnectBtn.IsEnabled = true;
                OperationsPanel.IsVisible = true;
                ErrorLabel.IsVisible = false;
                
                LoadRegions();
            }
            else
            {
                StatusLabel.Text = "Отключено";
                StatusLabel.TextColor = Colors.Red;
                ConnectBtn.IsEnabled = true;
                DisconnectBtn.IsEnabled = false;
                OperationsPanel.IsVisible = false;
            }
        }

        private void LoadRegions()
        {
            var regions = _dbService.GetAllRegions();
            RegionPicker.ItemsSource = regions;
            if (regions.Count > 0) RegionPicker.SelectedIndex = 0;
        }

        private void OnConnectClicked(object sender, EventArgs e)
        {
            string server = ServerNameEntry.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(server))
            {
                ErrorLabel.Text = "Введите имя сервера.";
                ErrorLabel.IsVisible = true;
                return;
            }

            _dbService.SetServerName(server);

            if (_dbService.Connect())
            {
                UpdateStatus();
                OutputEditor.Text = "Успешное подключение. База данных и таблица проверены/созданы.";
            }
            else
            {
                ErrorLabel.Text = $"Ошибка: {_dbService.LastError}";
                ErrorLabel.IsVisible = true;
            }
        }

        private void OnDisconnectClicked(object sender, EventArgs e)
        {
            _dbService.Disconnect();
            UpdateStatus();
            OutputEditor.Text = "Отключено от базы данных.";
        }

        private void OnGetAllCountriesClicked(object sender, EventArgs e)
        {
            var list = _dbService.GetAllCountries();
            ShowResults(list);
        }

        private void OnGetPartialInfoClicked(object sender, EventArgs e)
        {
            var list = _dbService.GetPartialInfo();
            OutputEditor.Text = string.Join(Environment.NewLine, list);
        }

        private void OnSearchCountryClicked(object sender, EventArgs e)
        {
            string name = CountryNameEntry.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name)) return;

            var country = _dbService.GetCountryByName(name);
            OutputEditor.Text = country != null ? country.ToString() : "Страна не найдена.";
        }

        private void OnGetCityInfoClicked(object sender, EventArgs e)
        {
            string name = CountryNameEntry.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                 OutputEditor.Text = "Введите название страны выше.";
                 return;
            }
            
            var info = _dbService.GetCitiesOfCountry(name);
            OutputEditor.Text = info;
        }

        private void OnCountriesByLetterClicked(object sender, EventArgs e)
        {
            string l = LetterEntry.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(l)) return;
            
            var list = _dbService.GetCountriesStartingWith(l);
            ShowResults(list);
        }

        private void OnCapitalsByLetterClicked(object sender, EventArgs e)
        {
             string l = LetterEntry.Text?.Trim() ?? string.Empty;
             if (string.IsNullOrEmpty(l)) return;
            
            var list = _dbService.GetCapitalsStartingWith(l);
            ShowResults(list);
        }

        private void OnTop3CapitalsMinPopClicked(object sender, EventArgs e)
        {
             var list = _dbService.GetTop3CapitalsMinPop();
             ShowResults(list);
        }

        private void OnTop3CountriesMinPopClicked(object sender, EventArgs e)
        {
             var list = _dbService.GetTop3CountriesMinPop();
             ShowResults(list);
        }

        private void OnAvgCapitalPopByRegionClicked(object sender, EventArgs e)
        {
            var list = _dbService.GetAvgCapitalPopByRegion();
            OutputEditor.Text = string.Join(Environment.NewLine, list);
        }

        private void OnTop3MinByRegionClicked(object sender, EventArgs e)
        {
             if (RegionPicker.SelectedItem == null) return;
             string region = RegionPicker.SelectedItem.ToString() ?? string.Empty;
             if (string.IsNullOrEmpty(region)) return;

             var list = _dbService.GetTop3MinPopByRegion(region);
             ShowResults(list);
        }

        private void OnTop3MaxByRegionClicked(object sender, EventArgs e)
        {
             if (RegionPicker.SelectedItem == null) return;
             string region = RegionPicker.SelectedItem.ToString() ?? string.Empty;
             if (string.IsNullOrEmpty(region)) return;
             
             var list = _dbService.GetTop3MaxPopByRegion(region);
             ShowResults(list);
        }

        private void OnAvgPopInCountryClicked(object sender, EventArgs e)
        {
             string name = CountryNameEntry.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                 OutputEditor.Text = "Введите название страны в поле выше (Задание 3).";
                 return;
            }
            double avg = _dbService.GetAvgPopInCountry(name);
            OutputEditor.Text = $"Среднее население (или население столицы): {avg:N0}";
        }

        private void OnMinPopCityInCountryClicked(object sender, EventArgs e)
        {
             string name = CountryNameEntry.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                 OutputEditor.Text = "Введите название страны в поле выше (Задание 3).";
                 return;
            }
            string info = _dbService.GetCityWithMinPopInCountry(name);
            OutputEditor.Text = info;
        }

        private void ShowResults(System.Collections.IEnumerable list)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var item in list)
            {
                sb.AppendLine(item.ToString());
            }
            if (sb.Length == 0) sb.Append("Нет данных.");
            OutputEditor.Text = sb.ToString();
        }
    }
}
