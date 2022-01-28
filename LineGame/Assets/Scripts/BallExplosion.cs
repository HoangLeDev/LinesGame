using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallExplosion : MonoBehaviour
{
    public GameObject explosionParticle;

    public void ActivateBallExplosion() {
        explosionParticle.transform.parent = GameObject.Find("TilesContainer").transform;
        explosionParticle.SetActive(true);
        explosionParticle.GetComponent<DestroyExplosionParticle> ().enabled = true;
    }
}
