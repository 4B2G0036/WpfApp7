using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace WpfApp7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string defaultURL = "https://data.moenv.gov.tw/api/v2/aqx_p_432?api_key=e8dd42e6-9b8b-43f8-991e-b3dee723a52d&limit=1000&sort=ImportDate%20desc&format=JSON";
        AQIData aqiData = new AQIData();
        List<Field> fields = new List<Field>();
        List<Record> records = new List<Record>();
        public MainWindow()
        {
            InitializeComponent();
            URLTextBox.Text = defaultURL;
        }

        private async void GetAQIButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTextBox.Text = "抓取資料中...";
            string data = await FetchContentAsync(defaultURL);
            ContentTextBox.Text = data;
            aqiData = JsonSerializer.Deserialize<AQIData>(data);
            fields = aqiData.fields.ToList();
            records = aqiData.records.ToList();

            StatusTextBlock.Text = $"共有{records.Count}筆資料";
            DisplayAQIData();
        }

        private void DisplayAQIData()
        {
            RecordDataGrid.ItemsSource = records;
        }

        private async Task<string> FetchContentAsync(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(100);
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException e)
                {
                    return null;
                }
            }
        }

        private void RecordDataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}