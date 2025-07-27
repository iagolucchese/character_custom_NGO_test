using UnityEngine;

namespace ImportedScripts
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Header("Singleton")]
        [SerializeField] protected bool dontDestroyOnLoad = true;

        #region Properties
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
                
                instance = (T)FindObjectOfType(typeof(T));
                if (instance != null) return instance;

                GameObject newGo = new GameObject
                {
                    name = typeof(T).Name + "'s Singleton"
                };
                instance = newGo.AddComponent<T>();
                return instance;
            }
        }
        public static bool InstanceIsValid => Instance != null;
        public static bool InstanceIsInvalid => !InstanceIsValid;
        #endregion

        #region Unity Messages
        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}