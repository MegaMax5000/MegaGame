using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class TimedAction
    {
        private float Timeout;
        private float LastTime = Time.time;
        private Action Action;
        
        public TimedAction(Action action, float timeout)
        {
            this.Action = action;
            this.Timeout = timeout;
        }

        public void SetTimeout(float newTimeout)
        {
            Timeout = newTimeout;
            LastTime = Time.time;
        }

        public void DoTimedAction()
        {
            if (Time.time >=  LastTime + Timeout + TimedActionManager.GetLag())
            {
                Action();
                LastTime = Time.time;
            }
        }
    }
}
