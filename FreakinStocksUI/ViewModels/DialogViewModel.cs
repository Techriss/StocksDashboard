using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;
using FreakinStocksUI.Views;
using StocksData;
using StocksData.Models;

namespace FreakinStocksUI.ViewModels
{
    /// <summary>
    /// Logic implementation for a MySQL Database Configuration Dialog
    /// </summary>
    class DialogViewModel : ViewModelBase
    {
        public string Server { get; set; } = Properties.Settings.Default.DBServer ?? "Server";

        public string Database { get; set; } = Properties.Settings.Default.DBDatabase ?? "Database";

        public string Username { get; set; } = Properties.Settings.Default.DBUsername ?? "Username";

        internal SecureString Password { private get; set; }


        /// <summary>
        /// Closes the dialog and applies the changes made to the database selection and configuration
        /// </summary>
        public RelayCommand CloseCommand => new(() =>
        {
            if (Source is Window dialog)
            {
                MainViewModel.SettingsPage.RefreshDatabaseChoice();
                dialog.DialogResult = false;
                dialog.Close();
            }
        });

        /// <summary>
        /// Occurs when a change in the MySQL configuration was made throuh the dialog
        /// </summary>
        public RelayCommand ConfirmCommand => new(async () => await SetMySQL());

        /// <summary>
        /// Configures a MySQL database using the provided information in the dialog
        /// </summary>
        /// <returns></returns>
        private async Task SetMySQL()
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


                await ServiceHelper.WriteToMySQLConfigAsync(true, Server, Database, Username, entropy, cipher);

                (Source as Window).DialogResult = true;
                (Source as Window).Close();
            }
            catch (Exception)
            {
                (Source as Window).DialogResult = false;
                MainViewModel.SettingsPage.RefreshDatabaseChoice();
                _ = new Prompt("Invalid Database", "The provided MySQL database configuration is invalid.").ShowDialog();
            }
        }


        /// <summary>
        /// Creates an instance of the logic for a MySQL Database Configuration dialog
        /// </summary>
        /// <param name="source">The dialog view</param>
        public DialogViewModel(object source)
        {
            this.Source = source;
            ThemeAssist.SetThemeForPage(this.Source as DependencyObject);
        }
    }
}