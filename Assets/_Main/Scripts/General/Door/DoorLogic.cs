using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRGame.DesignPatterns.Observers;
using VRGame.Puzzles;

public class DoorLogic : MonoBehaviour
{
    public bool Opened { get; private set; }
    
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private GameObject openedDoor;
    [SerializeField] private List<Puzzle> puzzles = new List<Puzzle>();

    [Header("Events")]
    [SerializeField] private UnityEvent onOpened;
    [SerializeField] private UnityEvent onClosed;

    private DoorPuzzleObserver _observer;

    private void Awake()
    {
        _observer = new DoorPuzzleObserver(puzzles.Count, this);

        foreach (var puzzle in puzzles)
        {
            puzzle.Attach(_observer);
        }
    }

    public void Open()
    {
        if (Opened) return;
        Opened = true;
        
        closedDoor.SetActive(false);
        openedDoor.SetActive(true);
        
        onOpened?.Invoke();
    }

    public void Close()
    {
        if (!Opened) return;
        Opened = false;
        
        openedDoor.SetActive(false);
        closedDoor.SetActive(true);
        
        onClosed?.Invoke();
    }

    private void OnDestroy()
    {
        foreach (var puzzle in puzzles)
        {
            puzzle.Detach(_observer);
        }
        
        _observer.Dispose();
        _observer = null;
    }
}

/// <summary>
/// Has a register of all the puzzles needed to be resolved to open
/// When they all are resolved, opens the dor
/// </summary>
public class DoorPuzzleObserver : IObserver<int, bool>
{
    /// <summary>
    /// A dictionary containing the cached solved state of each solved, using their id
    /// </summary>
    private Dictionary<int, bool> _solved = new();
    private int _resolvedPuzzles;
    private readonly int _maxPuzzles;
    private DoorLogic _door;

    public DoorPuzzleObserver(int maxPuzzles, DoorLogic door)
    {
        _maxPuzzles = maxPuzzles;
        _door = door;
    }
    
    public void OnNotify(int id, bool state)
    {
        var wasSolved = _solved.TryGetValue(id, out var value) && value;
        if (wasSolved == state) return;

        _solved[id] = state;
        _resolvedPuzzles += state ? 1 : -1;

        // When all the puzzles are solved, it opens the door
        if (_resolvedPuzzles >= _maxPuzzles)
        {
            _door.Open();
        }
        else
        {
            _door.Close();
        }
    }

    public void Dispose()
    {
        _solved.Clear();
        _solved = null;
        _door = null;
    }
}
