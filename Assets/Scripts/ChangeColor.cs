using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{    
    public Renderer render;
    public ParticleSystem particles;
    public Color color;
    public bool chooseRandom;
    
    void Start()
    {
        if(!chooseRandom)
            render.material.color = color;
        else
            render.material.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
        color = render.material.color;
        particles.startColor = color;
    }
}
