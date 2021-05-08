using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace FreakinStocksUI.Helpers
{
    public static class ServiceHelper
    {
        public static void ConfigureLiveService()
        {
            Task.Run(() =>
            {
                try
                {
                    using (var sc = ServiceController.GetServices().FirstOrDefault(x => x.DisplayName == "FreakinStocksLiveData"))
                    {
                        if (sc is null)
                        {
                            InstallService();
                            RunService();
                        }
                        else if (sc.Status is not ServiceControllerStatus.Running)
                        {
                            RunService();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception has occurred: { ex.Message }");
                }
            });
        }

        private static void InstallService()
        {
            var path = Path.GetFullPath(@".\FreakinStocksLiveService.exe");

            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\sc.exe",
                Arguments = $"create FreakinStocksLiveData binPath={ path } start= auto",
                Verb = "runas",
                UseShellExecute = true,
            };
            Process.Start(psi);
        }

        private static void RunService()
        {
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\sc.exe",
                Arguments = $"start FreakinStocksLiveData",
                Verb = "runas",
                UseShellExecute = true,
            };

            Process.Start(psi); // execute service
        }

        private static void StopService()
        {
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\sc.exe",
                Arguments = $"stop FreakinStocksLiveData",
                Verb = "runas",
                UseShellExecute = true,
            };

            Process.Start(psi); // stops service
        }

        public static void RestartService()
        {
            using (var sc = ServiceController.GetServices().FirstOrDefault(x => x.DisplayName == "FreakinStocksLiveData"))
            {
                if (sc.Status is ServiceControllerStatus.Running)
                {
                    StopService();
                    RunService();
                }
            }
        }

        public static void SetServiceSymbols()
        {
            try
            {
                File.WriteAllLines("Stocks.txt", Properties.Settings.Default.LikedStocks);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERR] Could not write to file \"Stocks.txt\" - Reason: { ex.Message }");
            }
        }
    }
}