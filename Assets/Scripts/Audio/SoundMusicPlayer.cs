using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundMusicPlayer : MonoBehaviourSingleton<SoundMusicPlayer> {

    private AudioSource m_AudioSource;
    private Coroutine m_FadeRoutine = null;

    private void Awake() {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.loop = true;
    }

    public void SetVolume(float f) {
        m_AudioSource.volume = f;
    }

    public void PlayMusic(AudioClip clip) {
        if (m_AudioSource.isPlaying && m_AudioSource.clip == clip) return;

        if (m_FadeRoutine != null) StopCoroutine(m_FadeRoutine);
        m_FadeRoutine = StartCoroutine(CrossfadeMusic(clip));
    }

    private IEnumerator CrossfadeMusic(AudioClip nextClip, float fadeTime = 2) {
        if (m_AudioSource.isPlaying) {
            float startVolume = m_AudioSource.volume;
            float startTime = Time.time;
            while (m_AudioSource.volume != 0) {
                m_AudioSource.volume = (1 - MathUtilities.GetNormalizedTime(startTime, fadeTime, Time.time)) * startVolume;
                yield return null;
            }

            m_AudioSource.Stop();
        }

        if (nextClip != null) {
            m_AudioSource.clip = nextClip;
            m_AudioSource.Play();

            float startTime = Time.time;
            while (m_AudioSource.volume != SoundManager.BackgroundMusicVolume) {
                m_AudioSource.volume = MathUtilities.GetNormalizedTime(startTime, fadeTime, Time.time) * SoundManager.BackgroundMusicVolume;
                yield return null;
            }
        } else {
            m_AudioSource.Stop();
        }

        yield return null;
    }
}
