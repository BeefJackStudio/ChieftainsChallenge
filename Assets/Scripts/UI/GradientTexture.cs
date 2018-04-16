using UnityEngine;
using System.Collections;

public class GradientTexture {
    public Texture2D gradient;
    public Sprite sgrad;

    public void Create(AnimationCurve curve, int width, Color low, Color mid, Color hi) {
        Gradient colGrad = new Gradient();
        GradientColorKey[] colKeys = new GradientColorKey[3];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        colKeys[0].color = low;
        colKeys[0].time = 0;
        colKeys[1].color = mid;
        colKeys[2].color = hi;
        colKeys[2].time = 1;

        alphaKeys[0].alpha = .5f;
        alphaKeys[0].time = 0;
        alphaKeys[1].alpha = .5f;
        alphaKeys[1].time = 1;

        GameObject.Destroy(gradient);
        gradient = new Texture2D(width, 1);

        float step = 1.0f / (float)width;

        for (int i = 0; i < width; i++) {
            float val = curve.Evaluate(1.0f - (i * step));
            if (val >= 0.5f) {
                colKeys[1].time = i * step;
                break;
            }
        }

        colGrad.SetKeys(colKeys, alphaKeys);

        for (int i = 0; i < width; i++) {
            Color col = colGrad.Evaluate(1.0f - (i * step));
            gradient.SetPixel(i, 0, col);
        }
        gradient.Apply();
        sgrad = Sprite.Create(gradient, new Rect(0, 0, width, 1), new Vector2(width / 2, 0));
    }

    public Sprite Get() {

        return sgrad;
    }
}
