//
//  ResettableObject.cs
//  
//  Created by Scott Mitchell on (DATE).
//  Copyright (c) 2015 Scott Mitchell. All rights reserved.
//

using UnityEngine;

public class ResettableObject : MonoBehaviour {
    public Transform initialPosition;

    public virtual void ResetMe() {
        transform.position = initialPosition.position;
    }
}