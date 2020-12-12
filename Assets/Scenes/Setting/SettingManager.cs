using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VimSts.Common;

namespace VimSts.SettingScene
{
    [DefaultExecutionOrder(+1)]
    public class SettingManager : MonoBehaviour
    {
        private const int MOVE_TYPE_UP   = 1;
        private const int MOVE_TYPE_DOWN = 2;

        public float sceneTransitionSec;
        public AudioClip settingBGM;
        public AudioClip moveSE;
        public Color activeMenuColor;
        public Color inactiveMenuColor;
        public Text nTaskLinesText;
        public Animator fade;

        private SoundManager sound;
        private int nTaskLines;

        void Start()
        {
            sound = SoundManager.FindInstance();
            sound.PlayBGM(settingBGM);

            InitNTaskLinesWithUpdate();

            fade.SetBool("faded", false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.H))
            {
                ChangeNTaskLinesWithUpdate(-10);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.L))
            {
                ChangeNTaskLinesWithUpdate(+10);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftBracket)))
            {
                BackToMenu();
            }
        }

        void UpdateNTaskLines()
        {
            nTaskLinesText.text = $"{nTaskLines}";
        }

        void InitNTaskLinesWithUpdate()
        {
            nTaskLines = Setting.GetNTaskLines();
            UpdateNTaskLines();
        }

        void ChangeNTaskLinesWithUpdate(int diff)
        {
            sound.PlaySE(moveSE);

            nTaskLines += diff;
            if (nTaskLines < 10)
            {
                nTaskLines = 10;
            }
            else if (nTaskLines > 100)
            {
                nTaskLines = 100;
            }

            Setting.SetNTaskLines(nTaskLines);
            UpdateNTaskLines();
        }

        void BackToMenu()
        {
            fade.SetBool("faded", true);
            StartCoroutine(WaitLoadScene("MenuScene"));
        }

        IEnumerator WaitLoadScene(string nextSceneName)
        {
            yield return new WaitForSeconds(sceneTransitionSec);
            SceneManager.LoadScene(nextSceneName);
        }

    }
}
