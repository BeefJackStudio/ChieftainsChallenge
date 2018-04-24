using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SoundEffectCollection : ScriptableObject {

    [SerializeField]
    public List<AudioClipLayer> layers = new List<AudioClipLayer>();

    public void Play(AudioSource source) {
        foreach(AudioClipLayer layer in layers) {
            List<AudioClip> layerList = layer.clips;
            if (layerList.Count == 0) continue;

            AudioClip clip;
            if (layerList.Count == 1) clip = layerList[0];
            else clip = layerList[UnityEngine.Random.Range(0, layerList.Count - 1)];

            source.PlayOneShot(clip, layer.volumeScale);
        }
    }

    [Serializable]
    public class AudioClipLayer {
        public string layerName;
        public float volumeScale = 1;
        public List<AudioClip> clips;
    }
}
