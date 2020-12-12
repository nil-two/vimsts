using UnityEngine;

namespace VimSts.Common
{
    public class SoundManager : MonoBehaviour
    {
        private static bool dontDestroyed = false;

        private AudioSource audioSource;

        void Start()
        {
            if (dontDestroyed)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(this);
            dontDestroyed = true;

            audioSource = GetComponent<AudioSource>();
        }

        public static SoundManager FindInstance()
        {
            return GameObject.Find("AudioSource").GetComponent<SoundManager>();
        }

        public void PlayBGM(AudioClip bgm)
        {
            if (audioSource.clip == bgm)
            {
                return;
            }

            audioSource.clip = bgm;
            audioSource.Play();
        }

        public void PlaySE(AudioClip se, float volume = 1f)
        {
            audioSource.PlayOneShot(se, volume);
        }
    }
}
