using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoundedObject
{
    /// <summary>
    /// Repositions the object when going out of bounds
    /// </summary>
    void Reposition();
}
