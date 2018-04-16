using UnityEngine;
using System.Collections;

public class SoundMusicPlayer : MonoBehaviourSingleton<SoundMusicPlayer> {

    private AudioSource m_AudioSource;
    private Coroutine m_FadeRoutine = null;

    private void Awake() {
        m_AudioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip) {
        m_AudioSource.volume = SaveDataManager.Instance.data.volumeMusic;
        m_AudioSource.Play();

        if (m_FadeRoutine != null) StopCoroutine(m_FadeRoutine);
        m_FadeRoutine = StartCoroutine(CrossfadeMusic(clip));
    }

    private IEnumerator CrossfadeMusic(AudioClip nextClip, float fadeTime = 1) {
        if (m_AudioSource.isPlaying) {
            float startVolume = m_AudioSource.volume;
            float startTime = Time.time;
            while (m_AudioSource.volume != 0) {
                m_AudioSource.volume = (1 - MathUtilities.GetNormalizedTime(startTime, fadeTime, Time.time)) * startVolume;
                yield return null;
            }
        }

        if (nextClip != null) {
            float endVolume = SaveDataManager.Instance.data.volumeMusic;
            float startTime = Time.time;
            while (m_AudioSource.volume != endVolume) {
                m_AudioSource.volume = MathUtilities.GetNormalizedTime(startTime, fadeTime, Time.time) * endVolume;
                yield return null;
            }
        } else {
            m_AudioSource.Stop();
        }

        yield return null;
    }
}
