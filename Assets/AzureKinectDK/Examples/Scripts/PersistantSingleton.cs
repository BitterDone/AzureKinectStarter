namespace Stahle.Utility
{
    //A Singleton that doesn't get destroyed when switching scenes
    public class PersistantSingleton<T> : Singleton<T> where T : Singleton<T>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

    }
}
