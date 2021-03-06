using UnityEngine;

namespace Dvalin
{
    /// <summary>
    /// A wrapper for Valheim's <see cref="ItemDrop" />.
    /// </summary>
    public class ItemDropWrapper : ItemDrop
    {
        public bool getFromRuntime = false;
        public bool includeInRealease = false;

        public ItemDrop runtimeItemDrop
        {
            get
            {
                if (!getFromRuntime) return this;

                var prefab = ObjectDB.instance.GetItemPrefab(gameObject.name);
                if (prefab == null) return this;

                return prefab.GetComponent<ItemDrop>();
            }
        }
    }

    // TODO: eventually we'll want someting like this to wrap valheim stuff
    // with custom interfaces and runtime creation of required components
    public class DvalinWrapper<T> : MonoBehaviour where T : MonoBehaviour
    {
        public bool getFromRuntime = false;

        public static implicit operator T(DvalinWrapper<T> wrapper)
        {
            if (!wrapper.getFromRuntime)
            {
                // if we're not getting a valheim prefab
                // create the components for this object
                return wrapper.BuildComponents();
            }

            return wrapper;
        }

        public virtual T BuildComponents()
        {
            return gameObject.AddComponent<T>();
        }
    }
}