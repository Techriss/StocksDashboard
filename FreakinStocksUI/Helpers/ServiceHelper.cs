using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using FreakinStocksUI.Views;

namespace FreakinStocksUI.Helpers
{
    /// <summary>
    /// Public helper class for interacting with the Live Data Worker Service
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// The constant public name of the Live Data Worker Service
        /// </summary>
        public const string ServiceName = "Freakin Stocks Live Data";
        /// <summary>
        /// The constant full path for the Freakin Stocks Live Data Worker Service executable
        /// </summary>
        private static readonly string servicePath = Path.GetFullPath(@".\Freakin Stocks Live Service.exe");


        /// <summary>
        /// Function for configuring the Live Data Service on the application startup to be running
        /// </summary>
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
                    _ = new Prompt(ex).ShowDialog();
                }
            });
        }

        /// <summary>
        /// Restarts the Live Data Service in case it is not installed, stopped or already running
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges
        /// </remarks>
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
                _ = new Prompt(ex).ShowDialog();
            }
        }

        /// <summary>
        /// Fully reinstalls the Live Data Service in case it is not installed, stopped or already running. Re
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges
        /// </remarks>
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
                _ = new Prompt(ex).ShowDialog();
            }
        }


        /// <summary>
        /// Installs the Live Data Service from the executable in the current running directory
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges
        /// </remarks>
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
                _ = new Prompt(ex).ShowDialog();
            }
        }

        /// <summary>
        /// Starts the installed Live Data Service
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges
        /// </remarks>
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
            catch
            {
                _ = new Prompt("Startup Cancelled", "The Live Data Service startup was not approved. The application's Live Data functionality will be limited.").ShowDialog();
            }
        }

        /// <summary>
        /// Stops the running Live Data Service
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges
        /// </remarks>
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
                _ = new Prompt("Startup Cancelled", "The Live Data Service startup was not approved. The application's Live Data functionality will be limited.").ShowDialog();
            }
        }

        /// <summary>
        /// Uninstalls the stopped Live Data Service. Waits for stopping if running. The service should be stopped for the uninstall to execute instantly
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges
        /// </remarks>
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
                _ = new Prompt("Startup Cancelled", "The Live Data Service startup was not approved. The application's Live Data functionality will be limited.").ShowDialog();
            }
        }

        /// <summary>
        /// Copies the liked stock symbols from the application settings to the Stocks.txt text file
        /// </summary>
        public static void SetServiceSymbols()
        {
            try
            {
                File.WriteAllLines("Stocks.txt", Properties.Settings.Default.LikedStocks);
            }
            catch (Exception ex)
            {
                _ = new Prompt("File not Accessible", $"[ERR] Could not write to file \"Stocks.txt\" - Reason: { ex.Message }").ShowDialog();
            }
        }

        /// <summary>
        /// Gets the Live Data Service from the list of all services installed on the machine
        /// </summary>
        /// <returns>The <see cref="ServiceController"/> for the Live Data Worker Service</returns>
        public static ServiceController GetService()
        {
            return ServiceController.GetServices().FirstOrDefault(x => x.DisplayName == ServiceName);
        }
    }
}