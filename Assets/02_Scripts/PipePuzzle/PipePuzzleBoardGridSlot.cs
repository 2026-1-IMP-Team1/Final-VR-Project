using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PipePuzzleBoardGridSlot : MonoBehaviour
{
    [Header("Socket 설정")]
    public XRSocketInteractor socketInteractor;

    [Header("상태")]
    public bool isOccupied = false;
    public PipePuzzlePiece occupiedPipe = null;

    void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();

        socketInteractor.selectEntered.AddListener(OnPipeSnapped);
        socketInteractor.selectExited.AddListener(OnPipeRemoved);
    }

    private void OnPipeSnapped(SelectEnterEventArgs args)
    {
        isOccupied = true;
        occupiedPipe = args.interactableObject.transform.GetComponent<PipePuzzlePiece>();

        if (occupiedPipe != null)
            occupiedPipe.OnSnappedToGrid(this);
    }

    private void OnPipeRemoved(SelectExitEventArgs args)
    {
        isOccupied = false;
        if (occupiedPipe != null)
            occupiedPipe.OnRemovedFromGrid();
        occupiedPipe = null;
    }
}