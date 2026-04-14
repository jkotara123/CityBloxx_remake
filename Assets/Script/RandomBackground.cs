using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackground : MonoBehaviour
{
    public Sprite[] Backgrounds;

    public SpriteRenderer Render;

    void Start()
    {
        Render = GetComponent<SpriteRenderer>();
        Render.sprite = Backgrounds[UnityEngine.Random.Range(0, Backgrounds.Length)];
    }

    void Update()
    {
    }
}