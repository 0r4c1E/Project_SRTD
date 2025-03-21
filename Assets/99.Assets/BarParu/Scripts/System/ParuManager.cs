using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BarParu
{
    public class ParuManager : MonoBehaviour
    {
        public static ParuManager inst;

        private void Awake()
        {
            if (inst == null) inst = this;
        }
    }

    public class Palog
    {
        public static void Log(string s)
        {
            Debug.Log("[Palog] : " + s);
        }
    }
}
