using Android.Net.Wifi;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;

namespace Gasbear;

public partial class StationsPage : ContentPage
{
	public StationsPage(string address, string fuelArt, int radius)
	{
		InitializeComponent();
		GetResult(address, fuelArt, radius);

        btn_navigate.BackgroundColor = Colors.Grey;
        btn_navigate.IsEnabled = false;

        btn_saveAsTxt.BackgroundColor = Colors.Grey;
        btn_saveAsTxt.IsEnabled = false;

        lbl_actionsInfo.TextColor = Colors.Grey;
        lbl_actionsInfo.Text = "Bedenke, dass sich die Spritpreise jederzeit ändern können!";
    }

    public string stationLat = "0";
    public string stationLon = "0";

	public async void GetResult(string address, string fuelArt, int radius)
	{
        using (HttpClient client = new HttpClient())
        {
            string apiKey;
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filePath = Path.Combine(documentsPath, "API-Key.txt");
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                apiKey = sr.ReadToEnd();
            }
            else
            {
                apiKey = string.Empty;
            }
            double lat = 0;
            double lon = 0;
            DateTime timestamp = DateTime.Now;

            string encodedAddress = Uri.EscapeDataString(address);
            //Open Street Map benötigt folgende Zeile, sonst kommt keine Antwort zurück.
            client.DefaultRequestHeaders.Add("User-Agent", "YourApp");

            string apiUrlOSM = ("https://nominatim.openstreetmap.org/search?format=json&q=" + encodedAddress);
            HttpResponseMessage responseOSM = await client.GetAsync(apiUrlOSM);
            if (responseOSM.IsSuccessStatusCode)
            {
                string dataOSM = await responseOSM.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(dataOSM);
                if (result.Count > 0)
                {
                    lat = result[0].lat;
                    lon = result[0].lon;
                }
                else
                {
                    AppendStationsInfo("Es konnten keine Koordinaten zur eingegebenen Addresse bezgogen werden!\n\nBitte Eingabe prüfen!\n\n");
                }
            }
            else
            {
                AppendStationsInfo("Open Street Map reagiert nicht...\n");
            }

            string dataTK = " ";
            string apiUrlTK = $"https://creativecommons.tankerkoenig.de/json/list.php?lat={lat}&lng={lon}&rad={radius}&sort=dist&type=all&apikey={apiKey}";

            HttpResponseMessage responseTK = client.GetAsync(apiUrlTK).Result;
            if (responseTK.IsSuccessStatusCode)
            {
                dataTK = await responseTK.Content.ReadAsStringAsync();
            }
            else
            {
                AppendStationsInfo("Tankerkönig reagiert nicht...\n");
            }

            RootObject root = JsonConvert.DeserializeObject<RootObject>(dataTK);
            if (root != null && root.Stations != null && root.Stations.Any())
            {
                AppendStationsInfo($"Anzahl gefundener Stationen: {root.Stations.Count}\n");
                AppendStationsInfo($"Spritpreise vom {timestamp} \n\nAngefragte Adresse:\n{address}\n\n" +
                                      $"Umkreis: {radius} km\nKraftstroffsuche: {fuelArt}\nGefundene Stationen: {root.Stations.Count}");

                btn_saveAsTxt.BackgroundColor = Colors.LightBlue;
                btn_saveAsTxt.IsEnabled = true;

                foreach (var station in root.Stations)
                {
                    string stationText = "Leer";

                    switch (fuelArt)
                    {
                        case "Alle Wählbaren":
                            stationText = $"{station.Brand}\n" +
                                $"{station.Street} {station.HouseNumber}\n" +
                                $"{station.PostCode} {station.Place}\n" +
                                $"Entfernung: {station.Dist} km\n" +
                                $"Diesel: \t\t\t\t\t\t{station.Diesel}\n" +
                                $"Benzin E5: \t\t\t{station.E5}\n" +
                                $"Benzin E10: \t\t{station.E10}";
                            break;

                        case "Diesel":
                            stationText = $"{station.Brand}\n" +
                                $"{station.Street} {station.HouseNumber}\n" +
                                $"{station.PostCode} {station.Place}\n" +
                                $"Entfernung: {station.Dist} km\n" +
                                $"Diesel: \t\t\t\t\t\t{station.Diesel}";
                            break;

                        case "Benzin E5":
                            stationText = $"{station.Brand}\n" +
                                $"{station.Street} {station.HouseNumber}\n" +
                                $"{station.PostCode} {station.Place}\n" +
                                $"Entfernung: {station.Dist} km\n" +
                                $"Benzin E5: \t\t\t{station.E5}";
                            break;

                        case "Benzin E10":
                            stationText = $"{station.Brand}\n" +
                                $"{station.Street} {station.HouseNumber}\n" +
                                $"{station.PostCode} {station.Place}\n" +
                                $"Entfernung: {station.Dist} km\n" +
                                $"Benzin E10: \t\t{station.E10}";
                            break;
                    }

                    var radioButton = new RadioButton()
                    {
                        Content = stationText,
                        Margin = 16,
                        HorizontalOptions = LayoutOptions.Start,
                        BackgroundColor = Colors.Transparent,
                        BorderWidth = 3,
                        BorderColor = Colors.LightBlue,
                        CornerRadius = 16,
                        WidthRequest = 300,
                        HeightRequest = 160,
                    };

                    radioButton.CheckedChanged += (sender, e) =>
                    {
                        if (e.Value) // Überprüft, ob der Radiobutton ausgewählt wurde
                        {
                            stationLat = $"{station.Lat}";
                            stationLon = $"{station.Lng}";
                            btn_navigate.BackgroundColor = Colors.LightGreen;
                            btn_navigate.IsEnabled = true;
                        }
                    };

                    slo_stationButtons.Children.Add(radioButton);
                }
            }
            else
            {
                AppendStationsInfo("Fehlende Koordinaten...,\noder es wurden keine Stationen im Umkreis der Koordinaten gefunden, \noder keine Daten erhalten!");
            }
            
        }
    }

    public void AppendStationsInfo(string text)
    {
        lbl_stationsInfo.Text += text;
    }

    private async void btn_navigate_Clicked(object sender, EventArgs e)
    {
        await NavigateToStation();
    }

    private async Task NavigateToStation()
    {
        double sLat = Convert.ToDouble(stationLat);
        double sLon = Convert.ToDouble(stationLon);
        var location = new Location(sLat, sLon);

        if (await Map.Default.TryOpenAsync(location) == false)
        {
            lbl_actionsInfo.TextColor = Colors.IndianRed;
            lbl_actionsInfo.Text = "Navigationsapp konnte nicht geöffnet werden";
            lbl_actionsInfo.TextColor = Colors.Grey;
        }
    }
}