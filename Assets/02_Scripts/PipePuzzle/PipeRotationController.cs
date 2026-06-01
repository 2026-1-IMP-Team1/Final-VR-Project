using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PipeRotationController : MonoBehaviour
{
    [SerializeField] private InputActionReference rotateRightAction;
    [SerializeField] private InputActionReference rotateLeftAction;

    private PipePuzzlePiece hoveredPipe = null;
    private XRRayInteractor rayInteractor;

    void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    void OnEnable()
    {
        if (rotateRightAction != null)
            rotateRightAction.action.performed += OnRotateRight;
        if (rotateLeftAction != null)
            rotateLeftAction.action.performed += OnRotateLeft;
    }

    void OnDisable()
    {
        if (rotateRightAction != null)
            rotateRightAction.action.performed -= OnRotateRight;
        if (rotateLeftAction != null)
            rotateLeftAction.action.performed -= OnRotateLeft;
    }

    void Update()
    {
        var hovered = rayInteractor.interactablesHovered;
        hoveredPipe = hovered.Count > 0
            ? hovered[0].transform.GetComponent<PipePuzzlePiece>()
            : null;

        if (hoveredPipe == null) return;

        if (Input.GetKeyDown(KeyCode.X))
            hoveredPipe.RotateRight();
        if (Input.GetKeyDown(KeyCode.Z))
            hoveredPipe.RotateLeft();
    }

    private void OnRotateRight(InputAction.CallbackContext ctx)
    {
        hoveredPipe?.RotateRight();
    }

    private void OnRotateLeft(InputAction.CallbackContext ctx)
    {
        hoveredPipe?.RotateLeft();
    }
}