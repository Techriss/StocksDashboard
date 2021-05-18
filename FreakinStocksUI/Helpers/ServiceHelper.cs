using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;

namespace FreakinStocksUI.Helpers
{
    public static class ServiceHelper
    {
        public const string ServiceName = "Freakin Stocks Live Data";
        private static readonly string servicePath = Path.GetFullPath(@".\Freakin Stocks Live Service.exe");


        public static void ConfigureLiveService()
        {
            Task.Run(() =>
            {
                try
                {
                    using (var sc = GetService())
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
                    MessageBox.Show($"Exception has occurred: { ex.Message }");
                }
            });
        }

        public static void RestartService()
        {
            try
            {
                using (var sc = GetService())
                {
                    if (sc is null)
                    {
                        InstallService();
                        RunService();
                    }
                    else if (sc.Status is ServiceControllerStatus.Running)
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace + ex.InnerException ?? " -- No Inner Exception");
            }
        }

        public static void ReinstallService()
        {
            try
            {
                using (var sc = GetService())
                {
                    if (sc is null)
                    {
                        InstallService();
                        RunService();
                    }
                    else if (sc.Status is ServiceControllerStatus.Running)
                    {
                        StopService();
                        UninstallService();
                        InstallService();
                        RunService();
                    }
                    else
                    {
                        UninstallService();
                        InstallService();
                        RunService();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace + ex.InnerException ?? " -- No Inner Exception");
            }
        }


        private static void InstallService()
        {
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\sc.exe",
                Arguments = $"create \"{ ServiceName }\" binPath= \"{ servicePath }\" start= auto",
                Verb = "runas",
                UseShellExecute = true,
            };

            try
            {
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[WAR] Installing was not approved or failed. Reason: { ex.Message + ex.StackTrace }");
            }
        }

        private static void RunService()
        {
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\sc.exe",
                Arguments = $"start \"{ ServiceName }\"",
                Verb = "runas",
                UseShellExecute = true,
            };

            try
            {
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WAR] Running was not approved or failed. Reason: { ex.Message }");
            }
        }

        public static void StopService()
        {
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\sc.exe",
                Arguments = $"stop \"{ ServiceName }\"",
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

        private static void UninstallService()
        {
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\sc.exe",
                Arguments = $"delete \"{ ServiceName }\"",
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

        public static ServiceController GetService()
        {
            return ServiceController.GetServices().FirstOrDefault(x => x.DisplayName == ServiceName);
        }
    }
}