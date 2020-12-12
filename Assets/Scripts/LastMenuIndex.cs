using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VimSts.Common
{
    public class LastMenuIndex : MonoBehaviour
    {
        private static bool dontDestroyed = false;
        private static int lastMenuIndex  = 0;

        void Start()
        {
            if (dontDestroyed)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(this);
            dontDestroyed = true;
            lastMenuIndex = 0;
        }

        public static LastMenuIndex FindInstance()
        {
            return GameObject.Find("LastMenuIndex").GetComponent<LastMenuIndex>();
        }

        public int GetIndex()
        {
            return lastMenuIndex;
        }

        public void SetIndex(int i)
        {
            lastMenuIndex = i;
        }
    }
}
