using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Linq;

public class PipePuzzleBoardGridSlot : MonoBehaviour
{
    [Tooltip("그리드 좌표: (행, 열), 좌상단이 (0,0)")]
    public Vector2Int gridPosition;

    public XRSocketInteractor socketInteractor;
    public bool IsOccupied => socketInteractor.hasSelection;

    public PipePuzzlePiece OccupiedPipe =>
        socketInteractor.interactablesSelected
            .FirstOrDefault()
            ?.transform.GetComponent<PipePuzzlePiece>();

    void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
    }
}