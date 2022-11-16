using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TimerManager : Singleton<TimerManager>
{
    public List<Timer> timers = new List<Timer>();

    public UnityEvent<Timer> onTimerEnd;

    private void Start()
    {
    }
    private void Update()
    {
        TickAllTimers();
    }

    private void TickAllTimers()
    {
        foreach(Timer t in timers)
        {
            t.Tick();
        }
    }

    public bool Add(Timer t, bool playOnAdd = true)
    {
        if (timers.Contains(t))
        {
            return false;
        }
        timers.Add(t);
        t.onTimerEnd += OnTimerFinished;
        if (playOnAdd)
            t.Play();
        return true;
    }

    public bool Remove(Timer t)
    {
        if (timers.Contains(t))
        {
            timers.Remove(t);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool Remove(string name)
    {
        List<Timer> timerswithname = timers.Where(t => t.name == name).ToList();
        if (timerswithname.Count <= 0)
        {
            return false;
        }else if(timerswithname.Count == 1)
        {
            Timer t = timerswithname[0];
            timers.Remove(t);
            return true;
        }
        else
        {
            throw new UnityException($"More than one timer exists with name {name}. This should not happen");
        }
    }
    public Timer GetTimer(string name)
    {
        return timers.FirstOrDefault(t => t.name == name);
    }
    public Timer GetTimer(Timer timer)
    {
        return timers.FirstOrDefault(t => t == timer);
    }

    #region Listeners
    private void OnTimerFinished(Timer timer)
    {
        Debug.Log($"Timer {timer.name} finisehd!!!");
        onTimerEnd?.Invoke(timer);
    }
    #endregion
}
