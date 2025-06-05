using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

public static class UpdateChecker
{
    private static readonly string versionUrl = "https://raw.githubusercontent.com/Kemyt68/Bazunia/refs/heads/master/version.txt";
    private static readonly string updateZipUrl = "https://github.com/Kemyt68/Bazunia/releases/latest/download/Bazunia.zip";
    private static readonly string localVersion = "v1.5"; // Aktualna wersja aplikacji
    
    public static async Task CheckForUpdateAsync()
    {
        try
        {
            using var http = new HttpClient();
            var latestVersion = (await http.GetStringAsync(versionUrl)).Trim();

            if (latestVersion != localVersion)
            {
                var tempZip = Path.Combine(Path.GetTempPath(), "update.zip");
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                
                MessageBox.Show("Nowa wersja dostępna: " + latestVersion, "Aktualizacja", MessageBoxButton.OK, MessageBoxImage.Information);
                //Console.WriteLine("Pobieranie aktualizacji...");

                var data = await http.GetByteArrayAsync(updateZipUrl);
                await File.WriteAllBytesAsync(tempZip, data);

                //var updaterPath = Path.Combine(appPath, "Updater.exe");
                //var updaterPath = "C:\\Users\\Acer\\Desktop\\Anki\\Bazunia_exe\\Updater\\bin\\Debug\\net7.0\\Updater.exe";
                var updaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater.exe");

                var currentPid = Process.GetCurrentProcess().Id;

                // Uruchom Updater.exe
                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterPath,
                    Arguments = $"{currentPid} \"{tempZip}\" \"{appPath}",
                    UseShellExecute = true
                });

                // Zamknij główną aplikację
                Environment.Exit(0);
            }
            else
            {
                //MessageBox.Show("Aplikacja jest aktualna.", "Aktualizacja", MessageBoxButton.OK, MessageBoxImage.Information);
                //Console.WriteLine("Aplikacja jest aktualna.");
            }
        }
        catch (Exception)
        {
            MessageBox.Show("Brak dostępu do repozytorium.", "Aktualizacja", MessageBoxButton.OK, MessageBoxImage.Error);
            //Console.WriteLine($"Błąd aktualizacji: {ex.Message}");
        }
    }
}
