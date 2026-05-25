using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PipeRotationController : MonoBehaviour
{
    private PipePuzzlePiece hoveredPipe = null;
    private XRRayInteractor rayInteractor;

    void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    void Update()
    {
        var hovered = rayInteractor.interactablesHovered;

        if (hovered.Count > 0)
            hoveredPipe = hovered[0].transform.GetComponent<PipePuzzlePiece>();
        else
            hoveredPipe = null;

        if (hoveredPipe == null) return;

        if (Input.GetKeyDown(KeyCode.L))
        {
            hoveredPipe.RotateRight();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            hoveredPipe.RotateLeft();
        }
    }
}