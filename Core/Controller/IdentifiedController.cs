
namespace Core.Controller
{
    public class IdentifiedController<TController, T> : Controller<TController, T>
        where TController : Controller<TController, T> where T : class, IIdentified
    {
        
    }
}