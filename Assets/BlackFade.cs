using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackFade : MonoBehaviour
{
    public bool ready;
    public bool psuedoBuffer;
    public bool kragnDead;
    bool sceneFin;
    Image img;
    Color alphaDown;

    public void SceneOut()
    {
        // Transition out of the scene in whatever way using this method

        Debug.Log("Mini Game Scene Complete");
    }

    IEnumerator BufferTime()
    {
        yield return new WaitForSeconds(1.5f);
        psuedoBuffer = true;
    }
    
    void Start()
    {
        ready = false;
        psuedoBuffer = false;
        sceneFin = false;
        img = GetComponent<Image>();
        alphaDown = new Color(0, 0, 0, -0.005f);
        StartCoroutine(BufferTime());
    }
    
    void Update()
    {
        if (!ready && psuedoBuffer)
        {
            img.color = img.color + alphaDown;

            if (img.color.a <= 0)
            {
                ready = true;
            }
        }

        if(kragnDead && !sceneFin)
        {
            img.color = img.color - alphaDown;

            if(img.color.a >= 1f)
            {
                SceneOut();
                sceneFin = true;
            }
        }
    }
}
