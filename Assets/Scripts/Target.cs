using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Target
{
    Vector3 Velocity { get; }
    Vector3 Position { get; }
    float Orientation { get; }
}
