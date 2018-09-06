using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSpriteHandler : MonoBehaviour {

    public CharacterMask maskPrefab;
    public CharacterSpriteBase bodyType;

    public Transform maskParent;
    public CharacterSpriteSet head;
    public CharacterSpriteSet chest;
    public CharacterSpriteSet cape;
    public CharacterSpriteSet waist;
    public CharacterSpriteSet upperLegRight;
    public CharacterSpriteSet upperLegLeft;
    public CharacterSpriteSet lowerLegRight;
    public CharacterSpriteSet lowerLegLeft;
    public CharacterSpriteSet footRight;
    public CharacterSpriteSet footLeft;
    public CharacterSpriteSet upperArmRight;
    public CharacterSpriteSet upperArmLeft;
    public CharacterSpriteSet lowerArmRight;
    public CharacterSpriteSet lowerArmLeft;
    public CharacterSpriteSet handRight;
    public CharacterSpriteSet handLeft;
    public CharacterSpriteSet club;

    public Material spriteMaterial;
    public Material outlineMaterial;
    public float outlineWidth = 0.1f;

    public void ApplySkin(CharacterSpriteBase spriteSet) {
        bodyType = spriteSet;
        head.ApplySprite(bodyType.head, spriteMaterial, outlineMaterial, outlineWidth);
        chest.ApplySprite(bodyType.chest, spriteMaterial, outlineMaterial, outlineWidth);
        cape.ApplySprite(bodyType.cape, spriteMaterial, outlineMaterial, outlineWidth);
        waist.ApplySprite(bodyType.waist, spriteMaterial, outlineMaterial, outlineWidth);
        upperLegRight.ApplySprite(bodyType.upperLegRight, spriteMaterial, outlineMaterial, outlineWidth);
        upperLegLeft.ApplySprite(bodyType.upperLegLeft, spriteMaterial, outlineMaterial, outlineWidth);
        lowerLegRight.ApplySprite(bodyType.lowerLegRight, spriteMaterial, outlineMaterial, outlineWidth);
        lowerLegLeft.ApplySprite(bodyType.lowerLegLeft, spriteMaterial, outlineMaterial, outlineWidth);
        footRight.ApplySprite(bodyType.footRight, spriteMaterial, outlineMaterial, outlineWidth);
        footLeft.ApplySprite(bodyType.footLeft, spriteMaterial, outlineMaterial, outlineWidth);
        upperArmRight.ApplySprite(bodyType.upperArmRight, spriteMaterial, outlineMaterial, outlineWidth);
        upperArmLeft.ApplySprite(bodyType.upperArmLeft, spriteMaterial, outlineMaterial, outlineWidth);
        lowerArmRight.ApplySprite(bodyType.lowerArmRight, spriteMaterial, outlineMaterial, outlineWidth);
        lowerArmLeft.ApplySprite(bodyType.lowerArmLeft, spriteMaterial, outlineMaterial, outlineWidth);
        handRight.ApplySprite(bodyType.handRight, spriteMaterial, outlineMaterial, outlineWidth);
        handLeft.ApplySprite(bodyType.handLeft, spriteMaterial, outlineMaterial, outlineWidth);
        club.ApplySprite(bodyType.club, spriteMaterial, outlineMaterial, outlineWidth);
    }

    public void ApplyMask(CharacterMask mask) {
        foreach (CharacterMask oldMask in maskParent.GetComponentsInChildren<CharacterMask>()) {
            DestroyImmediate(oldMask.gameObject);
        }

        CharacterMask nMask = Instantiate(mask, maskParent, false);
        nMask.transform.localPosition = Vector3.zero;
    }

    [Serializable]
    public class CharacterSpriteSet {

        public SpriteRenderer coloredSprite;
        public SpriteRenderer outlineSprite;

        public void ApplySprite(Sprite sprite, Material spriteMaterial, Material outlineMaterial, float outlineWidth) {
            if (coloredSprite != null) {
                coloredSprite.sprite = sprite;
                coloredSprite.material = spriteMaterial;
            }

            if (outlineSprite != null) {
                outlineSprite.sprite = sprite;
                if (coloredSprite != null) {
                    outlineSprite.sortingOrder = coloredSprite.sortingOrder - 20;
                    outlineSprite.transform.localScale = coloredSprite.transform.localScale * (1 + outlineWidth) * UnityEngine.Random.Range(0.95f, 1.05f);
                    outlineSprite.material = outlineMaterial;
                }
            }
        }
    }
}
