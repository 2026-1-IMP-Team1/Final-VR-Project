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

    void Update()
    {
        if (rayInteractor.TryGetCurrentUIRaycastResult(out _)) return;

        if (rayInteractor.TryGetHitInfo(out Vector3 _, out Vector3 _, out int _, out bool _))
        {
            var hovered = rayInteractor.interactablesHovered;
            if (hovered.Count > 0)
            {
                hoveredPipe = hovered[0].transform.GetComponent<PipePuzzlePiece>();  // ← 타입 변경
            }
            else
            {
                hoveredPipe = null;
            }
        }
    }

    void OnEnable()
    {
        rotateRightAction.action.performed += OnRotateRight;
        rotateLeftAction.action.performed += OnRotateLeft;
    }

    void OnDisable()
    {
        rotateRightAction.action.performed -= OnRotateRight;
        rotateLeftAction.action.performed -= OnRotateLeft;
    }

    private void OnRotateRight(InputAction.CallbackContext ctx)
    {
        if (hoveredPipe != null && hoveredPipe.IsSnapped)
            hoveredPipe.RotateRight();
    }

    private void OnRotateLeft(InputAction.CallbackContext ctx)
    {
        if (hoveredPipe != null && hoveredPipe.IsSnapped)
            hoveredPipe.RotateLeft();
    }
}