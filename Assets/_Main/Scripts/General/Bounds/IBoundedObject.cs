using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoundedObject
{
    int BoundedInstanceID { get; }
    /// <summary>
    /// Repositions the object when going out of bounds
    /// </summary>
    void Reposition();

    /// <summary>
    /// When the object should not reposition.
    /// </summary>
    /// <returns></returns>
    bool IgnoreReposition();
}
