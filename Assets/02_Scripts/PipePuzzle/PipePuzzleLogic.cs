using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PipePuzzleLogic : MonoBehaviour
{
    [Header("Grid Slots")]
    [SerializeField] private PipePuzzleBoardGridSlot[] allSlots;

    [Header("Entry Settings")]
    [Tooltip("Grid coordinate of the entry slot")]
    [SerializeField] private Vector2Int entryGridPos;
    [Tooltip("Direction from which flow enters the puzzle")]
    [SerializeField] private PipeDirection entryFromDirection;

    [Header("Exit Settings")]
    [Tooltip("Grid coordinate of the exit slot")]
    [SerializeField] private Vector2Int exitGridPos;
    [Tooltip("Direction through which flow leaves the puzzle")]
    [SerializeField] private PipeDirection exitToDirection;

    [Header("Rotation Controllers")]
    [SerializeField] private PipeRotationController[] rotationControllers;

    [Header("Events")]
    public UnityEvent onPuzzleSolved;
    public UnityEvent onPuzzleFailed;

    private Dictionary<Vector2Int, PipePuzzleBoardGridSlot> grid;

    void Start()
    {
        // Build a position-keyed lookup and subscribe to slot events
        grid = new Dictionary<Vector2Int, PipePuzzleBoardGridSlot>();
        foreach (var slot in allSlots)
        {
            grid[slot.gridPosition] = slot;
            slot.socketInteractor.selectEntered.AddListener(_ => CheckSolution());
            slot.socketInteractor.selectExited.AddListener(_ => OnPieceRemoved());
        }

        foreach (var rc in rotationControllers)
            if (rc != null)
                rc.onPipeRotated.AddListener(CheckSolution);
    }

    private void OnPieceRemoved()
    {
        // Reserved for future feedback when a piece is pulled out of a slot
    }

    public void CheckSolution()
    {
        // Verify the entry slot has a pipe open toward the entry direction
        if (!grid.TryGetValue(entryGridPos, out var entrySlot) || entrySlot.OccupiedPipe == null)
            return;

        if (!entrySlot.OccupiedPipe.GetOpenDirections().Contains(entryFromDirection))
            return;

        // BFS from entry to find all reachable connected pipes
        var visited = new HashSet<Vector2Int>();
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(entryGridPos);
        visited.Add(entryGridPos);

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            var openDirs = grid[pos].OccupiedPipe.GetOpenDirections();

            foreach (var dir in openDirs)
            {
                var neighborPos = pos + DirectionToOffset(dir);
                if (visited.Contains(neighborPos)) continue;
                if (!grid.TryGetValue(neighborPos, out var neighborSlot)) continue;
                if (neighborSlot.OccupiedPipe == null) continue;

                // Neighbor must have an opening facing back toward the current cell
                if (!neighborSlot.OccupiedPipe.GetOpenDirections().Contains(Opposite(dir))) continue;

                visited.Add(neighborPos);
                queue.Enqueue(neighborPos);
            }
        }

        // Puzzle is solved only if the exit is reachable and its pipe opens toward the exit direction
        if (!visited.Contains(exitGridPos))
            return;

        if (!grid.TryGetValue(exitGridPos, out var exitSlot) || exitSlot.OccupiedPipe == null)
            return;

        if (!exitSlot.OccupiedPipe.GetOpenDirections().Contains(exitToDirection))
            return;

        Debug.Log("[PipePuzzleLogic] Puzzle solved!");
        onPuzzleSolved?.Invoke();
        SceneManager.LoadScene("TeleportPuzzleRoom");
    }

    // Maps a PipeDirection to a grid offset (row increases downward)
    private static Vector2Int DirectionToOffset(PipeDirection dir)
    {
        return dir switch
        {
            PipeDirection.Up    => new Vector2Int(-1,  0),
            PipeDirection.Right => new Vector2Int( 0,  1),
            PipeDirection.Down  => new Vector2Int( 1,  0),
            PipeDirection.Left  => new Vector2Int( 0, -1),
            _                   => Vector2Int.zero
        };
    }

    // Returns the direction directly opposite to the given one
    private static PipeDirection Opposite(PipeDirection dir)
    {
        return (PipeDirection)(((int)dir + 2) % 4);
    }
}
