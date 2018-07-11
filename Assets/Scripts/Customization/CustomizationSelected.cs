﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomizationSelected {
    public static SelectionWrapper<CharacterMask> woodenMask;
    public static SelectionWrapper<CharacterMask> hawkMask;
    public static SelectionWrapper<CharacterMask> royalMask;
    public static SelectionWrapper<CharacterMask> skullMask;

    public static SelectionWrapper<GameBall> stoneBall;
    public static SelectionWrapper<GameBall> mudBall;
    public static SelectionWrapper<GameBall> beachBall;
    public static SelectionWrapper<GameBall> sunBall;

    public static SelectionWrapper<GameObject> particle;

    public static SelectionWrapper<CharacterMask> GetMaskType(int i) {
        if (i == 0) return woodenMask;
        if (i == 1) return hawkMask;
        if (i == 2) return royalMask;
        if (i == 3) return skullMask;
        return null;
    }

    public static SelectionWrapper<GameBall> GetBallType(int i) {
        if (i == 0) return stoneBall;
        if (i == 1) return mudBall;
        if (i == 2) return beachBall;
        if (i == 3) return sunBall;
        return null;
    }

    public class SelectionWrapper<T> {

        public T Obj { get; private set; }
        public int ID { get; private set; }

        public SelectionWrapper(T obj, int number) {
            Obj = obj;
            ID = number;
        }
    }
}
