
using System.Net;

namespace Gasbear
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void btn_sniff_Clicked(object sender, EventArgs e)
        {
            string address = ent_streetHousenumber.Text + ", " + ent_postcodePlace.Text;
            string fuelArt = GetSelectedFuelType();
            int radius = Convert.ToInt32(sli_radius.Value);

            await Navigation.PushModalAsync(new StationsPage(address, fuelArt, radius));
        }
        private async void btn_apiKey_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ApiKeyPage());
        }
        private async void btn_myPosition_Clicked(object sender, EventArgs e)
        {
            lbl_mainActionsInfo.Text = "Standort wird ermittelt...";
            CancellationTokenSource _cancelTokenSource;
            bool _isCheckingLocation;

            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);

                    Placemark placemark = placemarks?.FirstOrDefault();

                    if (placemark != null)
                    {
                        lbl_mainActionsInfo.TextColor = Colors.Green;
                        lbl_mainActionsInfo.Text = "Standortabfrage erfolgreich!";
                        ent_streetHousenumber.Text = $"{placemark.Thoroughfare} {placemark.SubThoroughfare}";
                        ent_postcodePlace.Text = $"{placemark.PostalCode} {placemark.Locality}";
                    }
                    else
                    {
                        lbl_mainActionsInfo.TextColor = Colors.IndianRed;
                        lbl_mainActionsInfo.Text = "Keine Adresse zu den Koordinaten gefunden!";

                    }

                }

            }

            catch (Exception ex)
            {
                lbl_mainActionsInfo.TextColor = Colors.IndianRed;
                lbl_mainActionsInfo.Text = ex.Message;
            }
            finally
            {
                _isCheckingLocation = false;
            }
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
