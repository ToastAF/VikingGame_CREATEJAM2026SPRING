using UnityEngine;

namespace KYSTER.Audio
{
    public class AnimationEventAudio : MonoBehaviour
    {
        [Tooltip("Clip to play when the animation event invokes PlaySound")]
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;

        [Tooltip("If true, plays the clip at this GameObject's position; otherwise plays globally")]
        public bool playAtPosition = true;

        // Called from AnimationEvent (no parameters)
        public void PlaySound()
        {
            if (clip == null) return;

            if (AudioManager.Instance != null)
            {
                if (playAtPosition)
                    AudioManager.Instance.PlayOneShotAt(clip, transform.position, volume);
                else
                    AudioManager.Instance.PlayOneShot(clip, volume);
            }
            else
            {
                if (playAtPosition)
                    AudioSource.PlayClipAtPoint(clip, transform.position, volume);
                else
                {
                    var camPos = Camera.main ? Camera.main.transform.position : Vector3.zero;
                    AudioSource.PlayClipAtPoint(clip, camPos, volume);
                }
            }
        }

        // Called from AnimationEvent(float) if you want to pass a custom volume
        public void PlaySoundWithVolume(float v)
        {
            float vol = Mathf.Clamp01(v);
            if (clip == null) return;

            if (AudioManager.Instance != null)
            {
                if (playAtPosition)
                    AudioManager.Instance.PlayOneShotAt(clip, transform.position, vol);
                else
                    AudioManager.Instance.PlayOneShot(clip, vol);
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, transform.position, vol);
            }
        }
    }
}
