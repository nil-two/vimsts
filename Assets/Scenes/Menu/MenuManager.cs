using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VimSts.Common;

namespace VimSts.MenuScene
{
    [DefaultExecutionOrder(+1)]
    public class MenuManager : MonoBehaviour
    {
        private const int MOVE_TYPE_UP   = 1;
        private const int MOVE_TYPE_DOWN = 2;

        public float sceneTransitionSec;
        public AudioClip menuBGM;
        public AudioClip moveSE;
        public AudioClip selectSE;
        public Color activeMenuColor;
        public Color inactiveMenuColor;
        public Animator fade;
        public MenuBehaviour startMenu;
        public MenuBehaviour settingMenu;
        public MenuBehaviour quitMenu;

        private SoundManager sound;
        private LastMenuIndex lastMenuIndex;
        private MenuBehaviour[] menus;
        private int cursorMenuI;

        void Start()
        {
            sound = SoundManager.FindInstance();
            sound.PlayBGM(menuBGM);

            lastMenuIndex = LastMenuIndex.FindInstance();

            fade.SetBool("faded", false);

            menus = new MenuBehaviour[]{startMenu, settingMenu, quitMenu};
            cursorMenuI = lastMenuIndex.GetIndex();
            UpdateMenu();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.K))
            {
                MoveMenu(MOVE_TYPE_UP);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.J))
            {
                MoveMenu(MOVE_TYPE_DOWN);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space) || (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.M)))
            {
                SelectMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftBracket)))
            {
                QuitMenu();
            }
        }

        void UpdateMenu()
        {
            foreach (var menu in menus)
            {
                menu.SetColor(inactiveMenuColor);
            }
            menus[cursorMenuI].SetColor(activeMenuColor);
        }

        void MoveMenu(int moveType)
        {
            sound.PlaySE(moveSE);

            if (moveType == MOVE_TYPE_UP)
            {
                cursorMenuI--;
            }
            else if (moveType == MOVE_TYPE_DOWN)
            {
                cursorMenuI++;
            }

            if (cursorMenuI < 0)
            {
                cursorMenuI = menus.Length - 1;
            }
            else
            {
                cursorMenuI = cursorMenuI % menus.Length;
            }
            UpdateMenu();
        }

        void SelectMenu()
        {
            sound.PlaySE(selectSE);

            var menu = menus[cursorMenuI];
            if (menu.IsQuit())
            {
                Quit();
            }
            else
            {
                lastMenuIndex.SetIndex(cursorMenuI);
                LoadScene(menu.GetNextSceneName());
            }
        }

        void QuitMenu()
        {
            sound.PlaySE(moveSE);

            var quitMenuI = 0;
            for (var menuI = 0; menuI < menus.Length; menuI++)
            {
                if (menus[menuI].IsQuit())
                {
                    quitMenuI = menuI;
                    break;
                }
            }

            if (cursorMenuI == quitMenuI)
            {
                Quit();
            }
            else
            {
                cursorMenuI = quitMenuI;
                UpdateMenu();
            }
        }

        void LoadScene(string nextSceneName)
        {
            fade.SetBool("faded", true);
            StartCoroutine(WaitLoadScene(nextSceneName));
        }

        void Quit()
        {
            fade.SetBool("faded", true);
            StartCoroutine(WaitQuit());
        }

        IEnumerator WaitLoadScene(string nextSceneName)
        {
            yield return new WaitForSeconds(sceneTransitionSec);
            SceneManager.LoadScene(nextSceneName);
        }

        IEnumerator WaitQuit()
        {
            yield return new WaitForSeconds(sceneTransitionSec);
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
            #endif
        }
    }
}
