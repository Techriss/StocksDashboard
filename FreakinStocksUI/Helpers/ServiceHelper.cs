using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using FreakinStocksUI.Views;
using StocksData.Models;

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
                if (Properties.Settings.Default.LikedStocks?.Count > 0)
                {
                    File.WriteAllLines("Stocks.txt", Properties.Settings.Default.LikedStocks);
                }
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

        /// <summary>
        /// Reads MySQL Database Configuration from application settings, checks its integrity and writes it to the MySQL.txt file asynchronously.
        /// Used mainly when the application was reinstalled.
        /// </summary>
        /// <returns></returns>
        public static void UpdateMySQLConfigFile()
        {
            try
            {
                if
                (
                    Enum.Parse<DatabaseType>(Properties.Settings.Default.DatabaseType) is DatabaseType.MySQL &&
                    Properties.Settings.Default.DBServer is not null and not "" &&
                    Properties.Settings.Default.DBDatabase is not null and not "" &&
                    Properties.Settings.Default.DBUsername is not null and not "" &&
                    Properties.Settings.Default.DBPasswordEntropy is not null &&
                    Properties.Settings.Default.DBPasswordCipher is not null

                    &&

                    File.Exists("MySQL.txt")
                )
                {

                    WriteToMySQLConfig
                    (
                        true,
                        Properties.Settings.Default.DBServer,
                        Properties.Settings.Default.DBDatabase,
                        Properties.Settings.Default.DBUsername,
                        Properties.Settings.Default.DBPasswordEntropy,
                        Properties.Settings.Default.DBPasswordCipher
                    );
                }
            }
            catch (Exception ex)
            {
                _ = new Prompt(ex).ShowDialog();
            }
        }

        /// <summary>
        /// <para>
        /// Checks the provided MySQL Database Configuration and writes it to the MySQL.txt text file.
        /// Automatically converts bytes to <see cref="string"/> using <see cref="Convert.ToBase64String(byte[])"/> asynchronously
        /// </para>
        /// <para>
        /// If <paramref name="isMySQL"/> set to true, <paramref name="server"/>, <paramref name="database"/>, <paramref name="username"/>, <paramref name="entropy"/>, <paramref name="cipher"/> have to contain data; otherwise no data will be written.
        /// </para>
        /// </summary>
        /// <param name="isMySQL">The first parameter to the MySQL.txt file. Specifies whether MySQL is selected as the database instead of SQLite.</param>
        /// <param name="server">The MySQL database server. Should be 'localhost' by default.</param>
        /// <param name="database">The MySQL database name</param>
        /// <param name="username">The MySQL username. Should be 'root' by default.</param>
        /// <param name="entropy">The secure entropy generated in the encryption process of the MySQL password. Allows for a more secure encryption.</param>
        /// <param name="cipher">The secure cipher generated in the encryption process of the MySQL password.</param>
        /// <returns></returns>
        public static async Task WriteToMySQLConfigAsync(bool isMySQL, string server = null, string database = null, string username = null, byte[] entropy = null, byte[] cipher = null)
        {
            string[] lines;

            if (!isMySQL)
            {
                lines = new[]
                {
                    "false"
                };

                await File.WriteAllLinesAsync("MySQL.txt", lines);
            }
            else if
            (
                server is not null and not "" &&
                database is not null and not "" &&
                username is not null and not "" &&
                entropy is not null &&
                cipher is not null)
            {
                lines = new[]
                {
                    "true",
                    server,
                    database,
                    username,
                    Convert.ToBase64String(entropy),
                    Convert.ToBase64String(cipher)
                };

                await File.WriteAllLinesAsync("MySQL.txt", lines);
            }
        }

        /// <summary>
        /// <para>
        /// Checks the provided MySQL Database Configuration and writes it to the MySQL.txt text file.
        /// Automatically converts bytes to <see cref="string"/> using <see cref="Convert.ToBase64String(byte[])"/> synchronously
        /// </para>
        /// <para>
        /// If <paramref name="isMySQL"/> set to true, <paramref name="server"/>, <paramref name="database"/>, <paramref name="username"/>, <paramref name="entropy"/>, <paramref name="cipher"/> have to contain data; otherwise no data will be written.
        /// </para>
        /// </summary>
        /// <param name="isMySQL">The first parameter to the MySQL.txt file. Specifies whether MySQL is selected as the database instead of SQLite.</param>
        /// <param name="server">The MySQL database server. Should be 'localhost' by default.</param>
        /// <param name="database">The MySQL database name</param>
        /// <param name="username">The MySQL username. Should be 'root' by default.</param>
        /// <param name="entropy">The secure entropy generated in the encryption process of the MySQL password. Allows for a more secure encryption.</param>
        /// <param name="cipher">The secure cipher generated in the encryption process of the MySQL password.</param>
        public static void WriteToMySQLConfig(bool isMySQL, string server = null, string database = null, string username = null, byte[] entropy = null, byte[] cipher = null)
        {
            string[] lines;

            if (!isMySQL)
            {
                lines = new[]
                {
                    "false"
                };

                File.WriteAllLines("MySQL.txt", lines);
            }
            else if
            (
                server is not null and not "" &&
                database is not null and not "" &&
                username is not null and not "" &&
                entropy is not null &&
                cipher is not null)
            {
                lines = new[]
                {
                    "true",
                    server,
                    database,
                    username,
                    Convert.ToBase64String(entropy),
                    Convert.ToBase64String(cipher)
                };

                File.WriteAllLines("MySQL.txt", lines);
            }
        }
    }
}