using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public static class SerializationConstants
    {
        public static char ENTITY_INFO_START = '~';
        public static char ENTITY_INFO_END = '~';
        public static char ENTITY_DELIM = '!';
        public static char GAME_INFO_START = '@';
        public static char GAME_INFO_END = '@';
        public static char GAME_INFO_DELIM = '#';
    }
}