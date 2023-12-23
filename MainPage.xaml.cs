using Newtonsoft.Json;

namespace Gasbear
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        async void btn_sniff_Clicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                // Eingabe
                string address = ent_streetHousenumber.Text + "," + ent_postcodePlace.Text;
                string fuelArt = GetSelectedFuelType();
                int radius = Convert.ToInt32(sli_radius.Value);
                string apiKey;
                string infoText = string.Empty;
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string filePath = Path.Combine(documentsPath, "API-Key.txt");
                double lat = 0;
                double lon = 0;

                if (File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath);
                    apiKey = sr.ReadToEnd();
                    // TODO Info, dass API-Schlüsseldatei gefunden wurde.
                }
                else
                {
                    apiKey = string.Empty;
                    // TODO Info, dass keine API-Schlüsseldatei existiert
                }

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

                        AppendText(infoText, "Koordinaten zur Adresse gefunden: \nLatitude: " + Convert.ToString(lat) + " Longitude: " + Convert.ToString(lon));
                    }
                    else
                    {
                        AppendText(infoText, "Es konnten keine Koordinaten zur eingegebenen Addresse bezgogen werden!\nBitte Eingabe prüfen!");
                    }
                }
                else
                {
                    AppendText(infoText, "Open Street Map reagiert nicht...");
                }

                await Navigation.PushModalAsync(new StationsPage()); // Dies muss am Ende der Methode stehen (Parameter noch einfügen)
            }
        }
        async void btn_apiKey_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ApiKeyPage());
        }
        private void sli_radius_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            double stepValue = 5;
            double newValue = Math.Round(e.NewValue / stepValue) * stepValue;
            sli_radius.Value = newValue;
            lbl_radius.Text = $"{newValue} km";
        }

        private string GetSelectedFuelType()
        {
            if (rbtn_fuelsDiesel.IsChecked)
            {
                return "Diesel";
            }
            else if (rbtn_fuelsE5.IsChecked)
            {
                return "Benzin E5";
            }
            else if (rbtn_fuelsE10.IsChecked)
            {
                return "Benzin E10";
            }
            else
            {
                return "Alle Wählbaren";
            }
        }
        private void AppendText(string var, string text)
        {
            var += text + "\n";
        }
    }

}
