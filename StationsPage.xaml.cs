using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Windows.Media.Protection.PlayReady;

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

                    // AppendTextToProgramInfo("Koordinaten zur Adresse gefunden: \nLatitude: " + Convert.ToString(lat) + " Longitude: " + Convert.ToString(lon));
                }
                else
                {
                    // AppendTextToProgramInfo("Es konnten keine Koordinaten zur eingegebenen Addresse bezgogen werden!\nBitte Eingabe prüfen!");
                }
            }
            else
            {
                // AppendTextToProgramInfo("Open Street Map reagiert nicht...");
            }

            string dataTK = " ";
            string apiUrlTK = $"https://creativecommons.tankerkoenig.de/json/list.php?lat={lat}&lng={lon}&rad={radius}&sort=dist&type=all&apikey={apiKey}";

            DateTime timestamp = DateTime.Now;

            HttpResponseMessage responseTK = client.GetAsync(apiUrlTK).Result;
            if (responseTK.IsSuccessStatusCode)
            {
                dataTK = await responseTK.Content.ReadAsStringAsync();
            }
            else
            {
                // AppendTextToProgramInfo("Tankerkönig reagiert nicht...");
            }

            RootObject root = JsonConvert.DeserializeObject<RootObject>(dataTK);
            if (root != null && root.Stations != null && root.Stations.Any())
            {
                // AppendTextToProgramInfo($"Anzahl gefundener Stationen im angegebenen Umkreis: {root.Stations.Count}");
                // AppendTextToInfopanel($"Spritpreise vom {timestamp} \n\nAngefragte Adresse:\n{street} {houseNumber}\n{postCode} {place}\n\n" +
                //                      $"Umkreis: {radius} km\nKraftstroffsuche: {fuelArt}\nGefundene Stationen: {root.Stations.Count}\n------------------------");

                for (int i = 0; i < root.Stations.Count; i++)
                {
                    string openTag;
                    if (root.Stations[i].IsOpen)
                    {
                        openTag = "- diese Tankstelle hat derzeit geöffnet -";
                    }
                    else
                    {
                        openTag = "- diese Tankstelle ist derzeit geschlossen -";
                    }
                    //AppendTextToInfopanel(root.Stations[i].Name + "\n"
                      //                    + root.Stations[i].Street + " " + root.Stations[i].HouseNumber + "\n"
                        //                  + root.Stations[i].PostCode + " " + root.Stations[i].Place);

                    switch (fuelArt)
                    {
                        case "Alle Wählbaren":
                            // AppendTextToInfopanel("Diesel    :   " + root.Stations[i].Diesel + "\n" + "Benzin E5 :   " + root.Stations[i].E5 + "\n" + "Benzin E10:   " + root.Stations[i].E10 + "\n" + openTag + "\n");
                            break;

                        case "Diesel":
                            // AppendTextToInfopanel("Diesel    :   " + root.Stations[i].Diesel + "\n" + openTag + "\n");
                            break;

                        case "Benzin E5":
                            // AppendTextToInfopanel("Benzin E5 :   " + root.Stations[i].E5 + "\n" + openTag + "\n");
                            break;

                        case "Benzin E10":
                            // AppendTextToInfopanel("Benzin E10:   " + root.Stations[i].E10 + "\n" + openTag + "\n");
                            break;
                    }
                }
            }
            else
            {
                // AppendTextToProgramInfo("Fehlende Koordinaten...,\noder es wurden keine Stationen im Umkreis der Koordinaten gefunden, \noder keine Daten erhalten!");
            }
        }
    }
}