using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame {
    public class TimedActionManager
    {
        private static TimedActionManager manager = new TimedActionManager();

        public static TimedActionManager GetInstance()
        {
            return manager;
        }

        private class ActionItem
        {
            public object client;
            public TimedAction action;

            public ActionItem(object c, TimedAction a)
            {
                this.client = c;
                this.action = a;
            }
        }

        private List<ActionItem> ActionItems = new List<ActionItem>();

        public void RegisterAction(Action action, object client, float time)
        {
            ActionItems.Add(new ActionItem(client, new TimedAction(action, time)));
        }

        public void RegisterAction(TimedAction action, object client)
        {
            ActionItems.Add(new ActionItem(client, action));
        }

        public void DoActions()
        {
            foreach (ActionItem item in ActionItems)
            {
                item.action.DoTimedAction();
            }
        }
    }
}