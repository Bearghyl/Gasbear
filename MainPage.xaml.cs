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
    }

}
