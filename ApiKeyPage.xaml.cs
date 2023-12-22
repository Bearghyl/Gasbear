namespace Gasbear;

public partial class ApiKeyPage : ContentPage
{
    public ApiKeyPage()
    {
        InitializeComponent();
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string filePath = Path.Combine(documentsPath, "API-Key.txt");

        if (File.Exists(filePath))
        {
            lbl_apiDataInfo.TextColor = Colors.Green;
            lbl_apiDataInfo.Text = "Datei mit einem Schlüssel vorhanden.";
            btn_delete.BackgroundColor = Colors.Red;
            btn_delete.IsEnabled = true;
        }
        else
        {
            lbl_apiDataInfo.TextColor = Colors.DarkGrey;
            lbl_apiDataInfo.Text = "Keine Schlüsseldatei vorhanden...";
            btn_delete.BackgroundColor = Colors.Grey;
            btn_delete.IsEnabled = false;
        }        
    }
    private async void OpenWebsite(object sender, EventArgs e)
    {
        string url = "https://creativecommons.tankerkoenig.de/";
        await Launcher.OpenAsync(new Uri(url));
    }
    private async void btn_save_Clicked(object sender, EventArgs e)
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string filePath = Path.Combine(documentsPath, "API-Key.txt");

        PermissionStatus writePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
        if (writePermission == PermissionStatus.Granted && !File.Exists(filePath))
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                await sw.WriteLineAsync(ent_apiKey.Text);
            }
            cbx_apiKey.IsChecked = true;
        }
        else
        {
            lbl_permissionInfo.Text = "Fehlende Berechtigung für die App. " +
                "Bitte in den Einstellungen die benötigten Berechtigungen prüfen.";
        }
    }
    private void btn_delete_Clicked(object sender, EventArgs e)
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string filePath = Path.Combine(documentsPath, "API-Key.txt");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            lbl_apiDataInfo.TextColor = Colors.Red;
            lbl_apiDataInfo.Text = "Schlüsseldatei gelöscht.";
        }
    }
}