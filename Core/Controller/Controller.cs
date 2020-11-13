using API.Controllers;

namespace Core.Controller
{
    public abstract class Controller<TController, T> : Singleton<TController> 
        where TController : Controller<TController, T> where T : class
    {

        public virtual void OnStartup()
        {
        }

        public virtual void OnStop()
        {
        }
        
    }
}