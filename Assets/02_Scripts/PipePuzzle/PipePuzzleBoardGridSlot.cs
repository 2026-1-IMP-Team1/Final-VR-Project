using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Linq;

public class PipePuzzleBoardGridSlot : MonoBehaviour
{
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