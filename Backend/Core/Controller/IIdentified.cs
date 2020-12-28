namespace Core.Controller
{
    public interface IIdentified : IMapped
    {
        public long Id { get; set; }
    }
}