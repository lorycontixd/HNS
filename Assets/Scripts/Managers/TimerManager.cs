using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimerManager : Singleton<TimerManager>
{
    public List<Timer> timers = new List<Timer>();

    public bool Add(Timer t, bool playOnAdd = true)
    {
        if (timers.Contains(t))
        {
            return false;
        }
        timers.Add(t);
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
}
