using Newtonsoft.Json;
using System.IO;

namespace Gasbear;

public partial class StationsPage : ContentPage
{
	public StationsPage(string address, string fuelArt, int radius)
	{
		InitializeComponent();
		GetResult(address, fuelArt, radius);
    }

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

                    AppendStationsInfo("Koordinaten zur Adresse gefunden: \nLatitude: " + Convert.ToString(lat) + " Longitude: " + Convert.ToString(lon) + "\n");
                }
                else
                {
                    AppendStationsInfo("Es konnten keine Koordinaten zur eingegebenen Addresse bezgogen werden!\nBitte Eingabe prüfen!");
                }
            }
            else
            {
                AppendStationsInfo("Open Street Map reagiert nicht...");
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
                AppendStationsInfo("Tankerkönig reagiert nicht...");
            }

            RootObject root = JsonConvert.DeserializeObject<RootObject>(dataTK);
            if (root != null && root.Stations != null && root.Stations.Any())
            {
                AppendStationsInfo($"Anzahl gefundener Stationen im angegebenen Umkreis: {root.Stations.Count}\n");
                AppendStationsInfo($"Spritpreise vom {timestamp} \n\nAngefragte Adresse:\n{address}\n\n" +
                                      $"Umkreis: {radius} km\nKraftstroffsuche: {fuelArt}\nGefundene Stationen: {root.Stations.Count}");
            }
        }
    }

    public void AppendStationsInfo(string text)
    {
        lbl_stationsInfo.Text += text;
    }
}