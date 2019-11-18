using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class Info
    {
        private List<object> toSerialize;

        protected char openingDelim;

        protected char closingDelim;

        protected char delim;

        protected void beginRegistration()
        {
            toSerialize = new List<object>();
        }

        protected void register(object obj)
        {
            toSerialize.Add(obj);
        }
        protected abstract void RegisterFieldsToSerialize();

        public override string ToString()
        {
            RegisterFieldsToSerialize();
            // This will join every object in toSerialize and print each object via the object.ToString() api
            // If the default ToString() behavior is insufficient then you must override it yourself
            return openingDelim +
                   String.Join("" + delim, toSerialize.ToArray()) +
                   closingDelim;
        }

        protected static string[] GetValues(string s, char delim)
        {
            return s.Substring(1, s.Length - 2)
                .Split(delim);
        }
    }
}