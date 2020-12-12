using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VimSts.Common
{
    public class Setting : MonoBehaviour
    {
        public static int GetNTaskLines()
        {
            return PlayerPrefs.GetInt("nTaskLines", 20);
        }

        public static void SetNTaskLines(int nTaskLines)
        {
            PlayerPrefs.SetInt("nTaskLines", nTaskLines);
        }
    }
}
