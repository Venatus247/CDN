using System;
using System.Reflection;

namespace API.Controllers
{
    //Singleton -> Entwurfsmuster
    //Es stellt sicher, dass von einer Klasse genau ein Objekt existiert
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var constructor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                        new Type[0], null);
                    _instance = (T) constructor?.Invoke(null);
                }

                return _instance;
            }
        }
    }
}