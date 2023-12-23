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
            await Navigation.PushModalAsync(new StationsPage());

            using (HttpClient client = new HttpClient()) // HIER HABE ICH AUFGEHÖRT ZU PROGRAMMIEREN
            {
                // Eingabe
                string address = ent_streetHousenumber.Text + "," + ent_postcodePlace.Text;
                string fuelArt = GetSelectedFuelType();
                int radius = Convert.ToInt32(sli_radius.Value);

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
                // Bis jetzt habe ich die Eingabevariablen soweit fertig.
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
    }

}
