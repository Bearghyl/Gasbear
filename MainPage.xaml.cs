using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using Windows.Devices.Radios;

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
            // Eingabe
            string address = ent_streetHousenumber.Text + "," + ent_postcodePlace.Text;
            string fuelArt = GetSelectedFuelType();
            int radius = Convert.ToInt32(sli_radius.Value);

            await Navigation.PushModalAsync(new StationsPage(address, fuelArt, radius));
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
