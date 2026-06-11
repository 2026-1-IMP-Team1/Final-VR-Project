using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PipePuzzleLogic : MonoBehaviour
{
    [Header("Grid Slots")]
    [SerializeField] private PipePuzzleBoardGridSlot[] allSlots;

    [Header("입구 설정")]
    [Tooltip("입구가 위치한 그리드 좌표")]
    [SerializeField] private Vector2Int entryGridPos;
    [Tooltip("흐름이 들어오는 방향")]
    [SerializeField] private PipeDirection entryFromDirection;

    [Header("출구 설정")]
    [Tooltip("출구가 위치한 그리드 좌표")]
    [SerializeField] private Vector2Int exitGridPos;
    [Tooltip("흐름이 빠져나가는 방향")]
    [SerializeField] private PipeDirection exitToDirection;

    [Header("이벤트")]
    public UnityEvent onPuzzleSolved;
    public UnityEvent onPuzzleFailed;

    private Dictionary<Vector2Int, PipePuzzleBoardGridSlot> grid;

    void Start()
    {
        grid = new Dictionary<Vector2Int, PipePuzzleBoardGridSlot>();
        foreach (var slot in allSlots)
        {
            grid[slot.gridPosition] = slot;
            slot.socketInteractor.selectEntered.AddListener(_ => OnPiecePlaced());
            slot.socketInteractor.selectExited.AddListener(_ => OnPieceRemoved());
        }
    }

    private void OnPiecePlaced()
    {
        CheckSolution();
    }

    private void OnPieceRemoved()
    {
        // ...
    }

    private void CheckSolution()
    {
        // 입구 슬롯 확인
        if (!grid.TryGetValue(entryGridPos, out var entrySlot) || entrySlot.OccupiedPipe == null)
            return;

        // 입구 방향이 열려 있는지 확인
        if (!entrySlot.OccupiedPipe.GetOpenDirections().Contains(entryFromDirection))
            return;

        // BFS로 입구에서 연결된 칸 탐색
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

                if (!neighborSlot.OccupiedPipe.GetOpenDirections().Contains(Opposite(dir))) continue;

                visited.Add(neighborPos);
                queue.Enqueue(neighborPos);
            }
        }

        // 출구가 BFS로 도달 가능하고, 출구 방향을 열고 있는지 확인
        if (!visited.Contains(exitGridPos))
            return;

        if (!grid.TryGetValue(exitGridPos, out var exitSlot) || exitSlot.OccupiedPipe == null)
            return;

        if (!exitSlot.OccupiedPipe.GetOpenDirections().Contains(exitToDirection))
            return;

        Debug.Log("[PipePuzzleLogic] 성공!");
        onPuzzleSolved?.Invoke();
        SceneManager.LoadScene("TeleportPuzzleRoom");
    }

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

    private static PipeDirection Opposite(PipeDirection dir)
    {
        return (PipeDirection)(((int)dir + 2) % 4);
    }
}
