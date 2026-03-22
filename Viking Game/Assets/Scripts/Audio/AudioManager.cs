using UnityEngine;

namespace KYSTER.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources (optional)")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        [Header("Volume")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        [Range(0f, 1f)]
        public float sfxVolume = 1f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            float finalVolume = Mathf.Clamp01(volume * masterVolume * sfxVolume);
            if (sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, finalVolume);
            }
            else
            {
                var camPos = Camera.main ? Camera.main.transform.position : Vector3.zero;
                AudioSource.PlayClipAtPoint(clip, camPos, finalVolume);
            }
        }

        public void PlayOneShotAt(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;
            float finalVolume = Mathf.Clamp01(volume * masterVolume * sfxVolume);
            AudioSource.PlayClipAtPoint(clip, position, finalVolume);
        }

        public void SetMasterVolume(float v)
        {
            masterVolume = Mathf.Clamp01(v);
        }

        public void SetSFXVolume(float v)
        {
            sfxVolume = Mathf.Clamp01(v);
        }
    }
}
