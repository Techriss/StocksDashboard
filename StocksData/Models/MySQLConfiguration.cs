namespace StocksData.Models
{
    /// <summary>
    /// Secure read-only configuration for a MySQL Database 
    /// </summary>
    public record MySQLConfiguration(string Server, string Database, string Username, byte[] Entropy, byte[] Cipher);
}