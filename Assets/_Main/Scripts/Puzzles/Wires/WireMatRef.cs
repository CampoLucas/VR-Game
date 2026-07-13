using UnityEngine;

namespace VRGame.Puzzles.Wires
{
    [System.Serializable]
    public class WireMatRef
    {
        [field: SerializeField] public Renderer Renderer { get; private set; }
        [field: SerializeField] public int MaterialIndex { get; private set; }
    }
}