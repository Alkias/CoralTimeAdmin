namespace CoralTimeAdmin.Helpers
{
    /// <summary>
    /// All classes that represent a DB table
    /// and have a unique identifier must implement this interface.
    /// </summary>
    public interface IUID
    {

        /// <summary>
        /// The PRIMARY KEY constraint uniquely identifies each record in a database table.
        /// </summary>
        int Id { get; set; }
    }
}