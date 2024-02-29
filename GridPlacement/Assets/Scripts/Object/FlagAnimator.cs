using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagAnimator : MonoBehaviour
{
    [SerializeField]
    private Mesh[] frames;

    private int currentFrame;
    [SerializeField]
    private float timer;
    [SerializeField]
    private float frameDuration;
    [SerializeField]
    MeshFilter meshFilter1;
    [SerializeField]
    MeshFilter meshFilter2;


    private void Update()
    {
        // change frame every second (you might want to do this more or less frequently)
        if (timer <= 0)
        {
            meshFilter1.mesh = frames[currentFrame % frames.Length];
            meshFilter2.mesh = frames[currentFrame % frames.Length];
            currentFrame++;
            timer = frameDuration;

        }
        timer -= Time.deltaTime;
    }
}
