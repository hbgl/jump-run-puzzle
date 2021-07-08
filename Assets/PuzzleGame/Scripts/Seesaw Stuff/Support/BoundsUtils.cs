using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpRunPuzzle.Support
{
    public static class BoundsUtils
    {
        public static bool IsInPadBounds(Bounds padBounds, Bounds otherBounds)
        {
            var isInBoundsY = padBounds.center.y <= otherBounds.min.y;
            var isInBoundsXZ = otherBounds.min.x < padBounds.max.x
                && otherBounds.max.x > padBounds.min.x
                && otherBounds.min.z < padBounds.max.z
                && otherBounds.max.z > padBounds.min.z;
            return isInBoundsY && isInBoundsXZ;
        }
    }
}
