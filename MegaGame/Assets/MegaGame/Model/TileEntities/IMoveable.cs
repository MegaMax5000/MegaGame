﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public interface IMoveable
    {
        void DoMove(TileEntityConstants.Direction direction);
    }
}