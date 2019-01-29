using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSpear : MonoBehaviour
{
    public BlackFade fader;
    public GameObject spearPrefab;
    public bool canThrow;
    public int hurtTracker;
    float rotClamp;
    GameObject spear;
    public GameObject[] tenties;
    Animator anim;
    Rigidbody2D rb;
    Vector3 hand;
    Vector2 hurl;
    
    void Start()
    {
        canThrow = true;
        hurtTracker = 0;
        tenties = GameObject.FindGameObjectsWithTag("Tentacle");
        anim = GetComponent<Animator>();
        spear = transform.Find("spear").gameObject;
        fader = GameObject.FindGameObjectWithTag("Fader").GetComponent<BlackFade>();
        hand = spear.transform.position;
        rb = spear.GetComponent<Rigidbody2D>();
        hurl = new Vector2(-420, 75);
    }

    public IEnumerator ImLazy()
    {
        hurtTracker = 0;
        anim.Play("ArmSwing");
        canThrow = false;
        yield return new WaitForSecondsRealtime(0.067f);

        //Let Go of Spear and arm its rigidbody here
        transform.DetachChildren();
        rb.gravityScale = 1;
        rb.AddForce(hurl);

        yield return new WaitForSecondsRealtime(0.75f);
        anim.Play("Idle");
        spear = Instantiate(spearPrefab, hand, Quaternion.Euler(0, 0, -90 - 74), transform);
        rb = spear.GetComponent<Rigidbody2D>();

        foreach (GameObject go in tenties)
        {
            if(go.GetComponent<Tentacle>().isHurt)
            {
                hurtTracker++;
            }

            if(hurtTracker >= tenties.Length)
            {
                fader.kragnDead = true;
            }
        }

        canThrow = true;
    }

    void Update()
    {
        if (fader.ready)
        {
            if (Input.GetKeyDown(KeyCode.Space) && canThrow)
            {
                StartCoroutine(ImLazy());
            }
        }
    }
}
