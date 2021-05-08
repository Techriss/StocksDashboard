using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security;
using System.Windows;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;
using StocksData;
using StocksData.Models;

namespace FreakinStocksUI.ViewModels
{
    class DialogViewModel : ViewModelBase
    {
        public string Server { get; set; } = Properties.Settings.Default.DBServer ?? "Server";

        public string Database { get; set; } = Properties.Settings.Default.DBDatabase ?? "Database";

        public string Username { get; set; } = Properties.Settings.Default.DBUsername ?? "Username";

        internal SecureString Password { private get; set; }



        public RelayCommand CloseCommand => new(() =>
        {
            (Source as Window).DialogResult = false;
            (Source as Window).Close();
        });

        public RelayCommand ConfirmCommand => new(async () =>
        {
            var x = new NetworkCredential("", Password).Password;
            Encryption.Encrypt(ref x, out var cipher, out var entropy);
            x = null;

            var mysql = new MySQLDataAccess(new MySQLConfiguration(Server, Database, Username, entropy, cipher));

            try
            {
                await mysql.RepairDatabaseAsync();

                Properties.Settings.Default.DBServer = Server;
                Properties.Settings.Default.DBDatabase = Database;
                Properties.Settings.Default.DBUsername = Username;
                Properties.Settings.Default.DBPasswordEntropy = entropy;
                Properties.Settings.Default.DBPasswordCipher = cipher;
                Properties.Settings.Default.Save();


                var lines = new string[] { "true", Server, Database, Username, Convert.ToBase64String(entropy), Convert.ToBase64String(cipher) };
                await File.WriteAllLinesAsync("MySQL.txt", lines);

                (Source as Window).DialogResult = true;
                (Source as Window).Close();
            }
            catch (Exception ex)
            {
                (Source as Window).DialogResult = false;
                Debug.WriteLine($"[ERR] Invalid Database. Result: { ex.Message }");
            }
        });
    }
}