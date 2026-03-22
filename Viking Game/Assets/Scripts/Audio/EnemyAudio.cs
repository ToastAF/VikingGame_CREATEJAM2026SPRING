using UnityEngine;

namespace KYSTER.Audio
{
    public class EnemyAudio : MonoBehaviour
    {
        [Header("Clips")]
        public AudioClip[] attackClips;
        public AudioClip[] hurtClips;
        public AudioClip[] idleClips;
        public AudioClip[] runClips;

        [Header("Playback")]
        [Range(0f, 1f)]
        public float volume = 1f;
        public bool playAtPosition = true;

        [Header("Sources (per-type)")]
        [Tooltip("Optional: assign per-type AudioSources. If empty they will be created at runtime.")]
        public AudioSource attackSource;
        public AudioSource hurtSource;
        public AudioSource idleSource;
        public AudioSource runSource;
        public AudioSource attackExtraSource;
        public AudioSource walkExtraSource;

        [Header("Cooldowns (seconds)")]
        public float attackCooldown = 0.1f;
        public float hurtCooldown = 0.1f;
        public float idleCooldown = 0f;
        public float runCooldown = 0.1f;
        public float attackExtraCooldown = 0.05f;
        public float walkExtraCooldown = 0.05f;

        double lastAttackTime = -9999;
        double lastHurtTime = -9999;
        double lastIdleTime = -9999;
        double lastRunTime = -9999;
        double lastAttackExtraTime = -9999;
        double lastWalkExtraTime = -9999;

        AudioClip GetRandom(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0) return null;
            return clips[Random.Range(0, clips.Length)];
        }

        public void PlayAttack()
        {
            if (Time.unscaledTime - lastAttackTime < attackCooldown) return;
            lastAttackTime = Time.unscaledTime;
            PlayClip(GetRandom(attackClips), ref attackSource, "Attack");
        }

        // Extra attack-related sounds (e.g., swing whoosh vs impact)
        public AudioClip[] attackExtraClips;
        public void PlayAttackExtra()
        {
            if (Time.unscaledTime - lastAttackExtraTime < attackExtraCooldown) return;
            lastAttackExtraTime = Time.unscaledTime;
            PlayClip(GetRandom(attackExtraClips), ref attackExtraSource, "AttackExtra");
        }

        public void PlayHurt()
        {
            if (Time.unscaledTime - lastHurtTime < hurtCooldown) return;
            lastHurtTime = Time.unscaledTime;
            PlayClip(GetRandom(hurtClips), ref hurtSource, "Hurt");
        }

        public void PlayIdle()
        {
            if (Time.unscaledTime - lastIdleTime < idleCooldown) return;
            lastIdleTime = Time.unscaledTime;
            PlayClip(GetRandom(idleClips), ref idleSource, "Idle");
        }

        public void PlayRun()
        {
            if (Time.unscaledTime - lastRunTime < runCooldown) return;
            lastRunTime = Time.unscaledTime;
            PlayClip(GetRandom(runClips), ref runSource, "Run");
        }

        // Extra walk-related sounds (e.g., footsteps that accompany run animation)
        public AudioClip[] walkExtraClips;
        public void PlayWalkExtra()
        {
            if (Time.unscaledTime - lastWalkExtraTime < walkExtraCooldown) return;
            lastWalkExtraTime = Time.unscaledTime;
            PlayClip(GetRandom(walkExtraClips), ref walkExtraSource, "WalkExtra");
        }
        void EnsureSourceExists(ref AudioSource src, string nameSuffix)
        {
            if (src != null) return;

            // Create a child GameObject to host the AudioSource so each type can play independently
            var go = new GameObject(gameObject.name + "_Audio_" + nameSuffix);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
        }

        void PlayClip(AudioClip clip, ref AudioSource src, string nameSuffix)
        {
            if (clip == null) return;
            EnsureSourceExists(ref src, nameSuffix);

            // configure spatialization per setting
            src.spatialBlend = playAtPosition ? 1f : 0f;

            // don't start a new clip while the previous one (of this type) is still playing
            if (src.isPlaying) return;

            float finalVolume = volume;
            if (AudioManager.Instance != null)
            {
                finalVolume *= AudioManager.Instance.masterVolume * AudioManager.Instance.sfxVolume;
            }

            src.clip = clip;
            src.volume = Mathf.Clamp01(finalVolume);
            src.Play();
        }
    }
}
