using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PipeRotationController : MonoBehaviour
{
    [SerializeField] private InputActionReference rotateRightAction;
    [SerializeField] private InputActionReference rotateLeftAction;
    [SerializeField] public UnityEvent onPipeRotated;

    private PipePuzzlePiece hoveredPipe = null;
    private XRRayInteractor rayInteractor;

    void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    void OnEnable()
    {
        if (rotateRightAction != null)
        {
            rotateRightAction.action.Enable();
            rotateRightAction.action.performed += OnRotateRight;
        }
        if (rotateLeftAction != null)
        {
            rotateLeftAction.action.Enable();
            rotateLeftAction.action.performed += OnRotateLeft;
        }
    }

    void OnDisable()
    {
        if (rotateRightAction != null)
        {
            rotateRightAction.action.performed -= OnRotateRight;
            rotateRightAction.action.Disable();
        }
        if (rotateLeftAction != null)
        {
            rotateLeftAction.action.performed -= OnRotateLeft;
            rotateLeftAction.action.Disable();
        }
    }

    void Update()
    {
        // A trigger press moves an interactable from hovered to selected, so check both lists
        PipePuzzlePiece found = null;

        foreach (var interactable in rayInteractor.interactablesHovered)
        {
            found = interactable.transform.GetComponent<PipePuzzlePiece>();
            if (found != null) break;
        }

        if (found == null)
        {
            foreach (var interactable in rayInteractor.interactablesSelected)
            {
                found = interactable.transform.GetComponent<PipePuzzlePiece>();
                if (found != null) break;
            }
        }

        hoveredPipe = found;

        // Keyboard fallback — only for XR Device Simulator, not real VR hardware
        if (Input.GetKeyDown(KeyCode.X))
        {
            hoveredPipe?.RotateRight();
            onPipeRotated?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            hoveredPipe?.RotateLeft();
            onPipeRotated?.Invoke();
        }
    }

    private void OnRotateRight(InputAction.CallbackContext ctx)
    {
        hoveredPipe?.RotateRight();
        onPipeRotated?.Invoke();
    }

    private void OnRotateLeft(InputAction.CallbackContext ctx)
    {
        hoveredPipe?.RotateLeft();
        onPipeRotated?.Invoke();
    }
}
