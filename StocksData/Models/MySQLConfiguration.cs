namespace StocksData.Models
{
    public record MySQLConfiguration(string Server, string Database, string Username, byte[] Entropy, byte[] Cipher);
}