using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedBallAnimation : MonoBehaviour
{
    private bool isSmall;
    private float scale;
    private float scaleSpeed;
    // Start is called before the first frame update
    void OnEnable()
    {
        isSmall = false;
        scaleSpeed = 0.35f;
        scale = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isSmall)
        {
            scale += Time.deltaTime*scaleSpeed;
            if(scale>=0.28f) isSmall = false;
        }
        else
        {
            scale -= Time.deltaTime*scaleSpeed;
            if(scale<= 0.15f) isSmall = true;
        }
        transform.localScale = new Vector2(scale,scale);
    }
}
