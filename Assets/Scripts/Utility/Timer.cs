using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer
{
    public int id;
    public string name;
    public float duration;
    public bool isPaused;
    private float timer;

    public UnityEvent<Timer> onTimerEnd;

    public Timer()
    {
        id = 0;
        name = "Standard TImer";
        duration = 10f;
        timer = duration;
    }
    public Timer(int id, string name, float duration)
    {
        this.id = id;
        this.name = name;
        this.duration = duration;
        timer = duration;
    }

    public void Tick(float modifier = 1f)
    {
        if (!isPaused)
        {
            timer -= Time.deltaTime * modifier;
            if (timer <= 0f)
            {
                onTimerEnd?.Invoke(this);
            }
        }
    }

    public void Stop()
    {
        isPaused = true;
        timer = duration;
    }

    public void Play()
    {
        isPaused = false;
    }
}
