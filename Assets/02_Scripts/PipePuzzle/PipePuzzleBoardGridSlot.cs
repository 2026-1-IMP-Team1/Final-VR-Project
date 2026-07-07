using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Linq;

public class PipePuzzleBoardGridSlot : MonoBehaviour
{
    [Tooltip("Grid coordinate (row, col), top-left is (0,0)")]
    public Vector2Int gridPosition;

    public XRSocketInteractor socketInteractor;

    // True when a pipe piece is currently snapped into this slot
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
