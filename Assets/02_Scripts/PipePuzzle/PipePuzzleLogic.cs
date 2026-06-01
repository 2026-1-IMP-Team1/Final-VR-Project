using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        if (AreAllSlotsFilled())
            CheckSolution();
    }

    private void OnPieceRemoved()
    {
        // ...
    }

    private bool AreAllSlotsFilled()
    {
        foreach (var slot in allSlots)
            if (!slot.IsOccupied) return false;
        return true;
    }

    private void CheckSolution()
    {
        // 입구 슬롯 확인
        if (!grid.TryGetValue(entryGridPos, out var entrySlot) || entrySlot.OccupiedPipe == null)
        {
            Debug.LogWarning("[PipePuzzleLogic] 입구 슬롯에 파이프가 없습니다.");
            return;
        }

        // 입구 방향이 열려 있는지 확인
        if (!entrySlot.OccupiedPipe.GetOpenDirections().Contains(entryFromDirection))
        {
            Debug.Log("[PipePuzzleLogic] 실패: 입구 파이프가 입구 방향을 향하지 않음");
            onPuzzleFailed?.Invoke();
            return;
        }

        // BFS로 연결된 모든 칸 탐색
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

                // 이웃 칸이 반대 방향을 열고 있어야 연결됨
                if (!neighborSlot.OccupiedPipe.GetOpenDirections().Contains(Opposite(dir))) continue;

                visited.Add(neighborPos);
                queue.Enqueue(neighborPos);
            }
        }

        // 모든 칸이 연결되어 있는지 확인
        if (visited.Count != allSlots.Length)
        {
            Debug.Log($"[PipePuzzleLogic] 실패: 연결된 칸 {visited.Count}/{allSlots.Length}");
            onPuzzleFailed?.Invoke();
            return;
        }

        // 출구 슬롯이 출구 방향을 열고 있는지 확인
        if (!grid.TryGetValue(exitGridPos, out var exitSlot) || exitSlot.OccupiedPipe == null)
        {
            Debug.LogWarning("[PipePuzzleLogic] 출구 슬롯에 파이프가 없습니다.");
            return;
        }

        if (!exitSlot.OccupiedPipe.GetOpenDirections().Contains(exitToDirection))
        {
            Debug.Log("[PipePuzzleLogic] 실패: 출구 파이프가 출구 방향을 향하지 않음");
            onPuzzleFailed?.Invoke();
            return;
        }

        Debug.Log("[PipePuzzleLogic] 성공!");
        onPuzzleSolved?.Invoke();
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
