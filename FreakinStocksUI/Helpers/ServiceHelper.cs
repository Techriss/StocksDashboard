using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace FreakinStocksUI.Helpers
{
    public static class ServiceHelper
    {
        public static void ConfigureLiveService()
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
                throw;
            }
        }

        private static void InstallService()
        {
            var path = Path.GetFullPath(@".\FreakinStocksLiveService.exe");

            var psi1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\sc.exe",
                Arguments = $"create FreakinStocksLiveData binPath={ path } start= auto",
                Verb = "runas",
                UseShellExecute = true,
            };
            Process.Start(psi1);
        }

        private static void RunService()
        {
            var psi2 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\sc.exe",
                Arguments = $"start FreakinStocksLiveData",
                Verb = "runas",
                UseShellExecute = true,
            };

            Process.Start(psi2); // execute service
        }


        public static void SetServiceSymbols()
        {
            FreakinStocksLiveService.Worker.Symbols = Properties.Settings.Default.LikedStocks.ToArray();
        }
    }
}