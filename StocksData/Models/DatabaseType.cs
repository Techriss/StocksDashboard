namespace StocksData.Models
{
    /// <summary>
    /// The allowed database types. Implement the <see cref="IDataAccess"/> interface to create a data access layer for a new one.
    /// </summary>
    public enum DatabaseType
    {
        MySQL,
        SQLite
    }
}
