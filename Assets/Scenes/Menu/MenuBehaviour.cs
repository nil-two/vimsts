using UnityEngine;
using UnityEngine.UI;

namespace VimSts.MenuScene
{
    public class MenuBehaviour : MonoBehaviour
    {
        public string nextSceneName = "";
        public bool isQuit          = false;

        private Text text;

        void Start()
        {
            text = GetComponent<Text>();
        }

        public void SetColor(Color color)
        {
            text.color = color;
        }

        public string GetNextSceneName()
        {
            return nextSceneName;
        }

        public bool IsQuit()
        {
            return isQuit;
        }
    }
}
