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

        public static void RestartService()
        {
            using (var sc = ServiceController.GetServices().FirstOrDefault(x => x.DisplayName == "FreakinStocksLiveData"))
            {
                if (sc is null)
                {
                    InstallService();
                    RunService();
                }
                if (sc.Status is ServiceControllerStatus.Running)
                {
                    StopService();
                    RunService();
                }
                else
                {
                    RunService();
                }
            }
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

            try
            {
                Process.Start(psi);
            }
            catch (Exception)
            {
                Debug.WriteLine("[WAR] Installing was not approved or failed");
            }
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

            try
            {
                Process.Start(psi);
            }
            catch (Exception)
            {
                Debug.WriteLine("[WAR] Running was not approved or failed");
            }
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

            try
            {
                Process.Start(psi);
            }
            catch (Exception)
            {
                Debug.WriteLine("[WAR] Stopping was not approved or failed");
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