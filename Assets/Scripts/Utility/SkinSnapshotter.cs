using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSnapshotter : MonoBehaviourSingleton<SkinSnapshotter> {

    public Camera cam;
    public CharacterSpriteHandler character;
    public CustomizationChoices skinChoices;

    private float scaler = 0.175f;
    private RenderTexture m_RenderTexture;

    public List<Texture2D> GenerateImages() {
        gameObject.SetActive(true);

        List<Texture2D> rawImages = new List<Texture2D>();
        foreach (GameObject o in skinChoices.options) {
            CustomizationSkinWrapper wr = o.GetComponent<CustomizationSkinWrapper>();
            character.ApplySkin(wr.spriteSet);
            rawImages.Add(CreateSnapshot());
        }
        gameObject.SetActive(false);
        return rawImages;
    }

    private Texture2D CreateSnapshot() {
        if(m_RenderTexture == null) {
            m_RenderTexture = new RenderTexture((int)(Screen.width * scaler), (int)(Screen.height * scaler), 0);
            cam.targetTexture = m_RenderTexture;
        }

        m_RenderTexture.Release();
        cam.Render();

        Texture2D tex = new Texture2D(m_RenderTexture.width, m_RenderTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = m_RenderTexture;
        tex.ReadPixels(new Rect(0, 0, m_RenderTexture.width, m_RenderTexture.height), 0, 0);
        tex.Apply();
        return tex;
    }

}
