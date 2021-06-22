using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin
{
    // Looks for GameObjects with a specific name
    // GameObject.Find does not work when an object is inactive, so we use this instead
    // Based on GameObjectSearcher from RuntimeUnityEditor
    // see https://raw.githubusercontent.com/ManlyMarco/RuntimeUnityEditor/029227feb35f2543944630dd51d662e9b40f6051/RuntimeUnityEditor/ObjectTree/GameObjectSearcher.cs
    public class GameObjectSearcher
    {
        private List<GameObject> _cachedRootGameObjects;

        public static IEnumerable<GameObject> FindAllRootGameObjects()
        {
            foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>())
            {
                if (obj.parent == null)
                {
                    yield return obj.gameObject;
                }
            }
        }

        public IEnumerable<GameObject> GetRootObjects()
        {
            if (_cachedRootGameObjects == null)
            {
                this.Refresh();
            } else
            {
                _cachedRootGameObjects.RemoveAll(o => o == null);
            }

            return _cachedRootGameObjects;
        }

        public void Refresh()
        {
            _cachedRootGameObjects = FindAllRootGameObjects().ToList();
        }

        public GameObject Find(string name)
        {
            foreach (var obj in GetRootObjects())
            {
                foreach (var trans in obj.GetComponentsInChildren<Transform>(true))
                {
                    if (trans.name == name)
                    {
                        return trans.gameObject;
                    }
                }
            }
            return null;
        }
    }
}