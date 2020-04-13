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
        public void DoTimedAction()
        {
            if (Time.time >=  LastTime + Timeout)
            {
                Action.DoAction();
                LastTime = Time.time;
            }
        }
    }
}
