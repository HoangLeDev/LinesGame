using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyExplosionParticle : MonoBehaviour
{
    void OnEnable() {
        Destroy(this.gameObject, 1f);
    }
}
