using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    private float length, startPos;
    public GameObject cam;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x / 3;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-Time.deltaTime * parallaxEffect + ((transform.position.x < startPos - length) ? length  * 3 /* Full Length */: 0), 0, 0);
    }
}
