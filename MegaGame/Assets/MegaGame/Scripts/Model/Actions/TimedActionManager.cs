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

        public class ActionItem
        {
            public object client;
            public TimedAction action;

            public ActionItem(object c, TimedAction a)
            {
                this.client = c;
                this.action = a;
            }
        }

        private Dictionary<object,List<ActionItem>> ActionItems = new Dictionary<object,List<ActionItem>>();

        public ActionItem RegisterAction(Action action, object client, float time)
        {
            if (!ActionItems.ContainsKey(client)) {
                ActionItems.Add(client, new List<ActionItem>());
            }

            ActionItem a = new ActionItem(client, new TimedAction(action, time));
            ActionItems[client].Add(a);

            return a;
        }

        public void UnregisterAction(ActionItem action, object client) {
            if ( client != null && ActionItems.ContainsKey(client) ) {
                ActionItems[client].Remove(action);

                // cleanup
                if (ActionItems[client].Count == 0) {
                    ActionItems.Remove(client);
                }
            }
        }

        public void DoActions()
        {
            foreach (object client in ActionItems.Keys)
            {
                foreach ( ActionItem item in ActionItems[client] ) {
                    item.action.DoTimedAction();
                }
            }
        }
    }
}