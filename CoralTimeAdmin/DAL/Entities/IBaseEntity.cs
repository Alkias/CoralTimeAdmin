namespace CoralTimeAdmin.DAL.Entities
{
    public interface IBaseEntity
    { }

    public class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }
    }
}