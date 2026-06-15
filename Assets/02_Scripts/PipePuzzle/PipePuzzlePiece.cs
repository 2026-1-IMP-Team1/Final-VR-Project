using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PipePuzzlePiece : MonoBehaviour
{
    [Header("Pipe Directions")]
    [SerializeField] private bool openUp;
    [SerializeField] private bool openDown;
    [SerializeField] private bool openLeft;
    [SerializeField] private bool openRight;

    private XRGrabInteractable grabInteractable;

    // Each step represents one 90-degree CCW rotation (RotateLeft increments by 1)
    private int rotationStep = 0;
    private bool isRotating = false;
    private Quaternion targetRotation;
    [SerializeField] private float rotationSpeed = 10f;

    private Quaternion slotOriginalAttachRotation;
    private XRSocketInteractor modifiedSocket = null;

    // True when this piece is held by a socket (snapped into a slot)
    public bool IsSnapped =>
        grabInteractable.interactorsSelecting
            .Any(i => i is XRSocketInteractor);

    public PipePuzzleBoardGridSlot CurrentSlot =>
        grabInteractable.interactorsSelecting
            .OfType<XRSocketInteractor>()
            .FirstOrDefault()
            ?.GetComponent<PipePuzzleBoardGridSlot>();

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.hoverEntered.AddListener(OnHoverEntered);
        grabInteractable.hoverExited.AddListener(OnHoverExited);
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (args.interactorObject is not XRSocketInteractor socket) return;
        if (socket.attachTransform == null || modifiedSocket == socket) return;

        // Restore the previously modified socket before saving a new baseline
        if (modifiedSocket != null && !IsSnapped)
            ResetModifiedSocket();

        slotOriginalAttachRotation = socket.attachTransform.rotation;
        modifiedSocket = socket;
        ApplyRotationStepToSlot(socket);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (args.interactorObject is not XRSocketInteractor socket) return;
        if (socket != modifiedSocket || IsSnapped) return;

        ResetModifiedSocket();
    }

    private void ResetModifiedSocket()
    {
        if (modifiedSocket != null && modifiedSocket.attachTransform != null)
            modifiedSocket.attachTransform.rotation = slotOriginalAttachRotation;
        modifiedSocket = null;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is not XRSocketInteractor socket) return;
        if (socket.attachTransform == null) return;

        isRotating = false;

        // Skip if hover already set up this socket to avoid double-applying
        if (modifiedSocket != socket)
        {
            slotOriginalAttachRotation = socket.attachTransform.rotation;
            modifiedSocket = socket;
            ApplyRotationStepToSlot(socket);
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject is not XRSocketInteractor socket) return;
        ResetModifiedSocket();

        // If hover re-entered before select exited, the rotation was skipped — reapply next frame
        StartCoroutine(RecheckHoverNextFrame(socket));
    }

    private IEnumerator RecheckHoverNextFrame(XRSocketInteractor socket)
    {
        yield return null;
        if (modifiedSocket != null || IsSnapped) yield break;

        bool stillHovering = false;
        foreach (var interactable in socket.interactablesHovered)
        {
            if (interactable.transform == transform) { stillHovering = true; break; }
        }

        if (stillHovering)
        {
            slotOriginalAttachRotation = socket.attachTransform.rotation;
            modifiedSocket = socket;
            ApplyRotationStepToSlot(socket);
        }
    }

    // Rotates the socket's attach transform to match this piece's current rotationStep.
    // rotationStep counts CCW steps, so CW count = (4 - rotationStep) % 4.
    private void ApplyRotationStepToSlot(XRSocketInteractor socket)
    {
        int cwCount = (4 - rotationStep) % 4;
        socket.attachTransform.rotation = slotOriginalAttachRotation * Quaternion.Euler(-90f * cwCount, 0, 0);
    }

    void Update()
    {
        if (isRotating)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }

    // CCW 90-degree rotation; step increments by 1
    public void RotateLeft()
    {
        if (isRotating) return;
        rotationStep = (rotationStep + 1) % 4;

        if (IsSnapped && CurrentSlot != null)
        {
            RotateSlotAttachPoint();
            Debug.Log("Rotate Left (Snapped)");
        }
        else
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, -90f, 0);
            isRotating = true;
            Debug.Log("Rotate Left (Free)");
        }
    }

    // CW 90-degree rotation; step decrements by 1 (+3 mod 4)
    public void RotateRight()
    {
        if (isRotating) return;
        rotationStep = (rotationStep + 3) % 4;

        if (IsSnapped && CurrentSlot != null)
        {
            RotateSlotAttachPoint();
            Debug.Log("Rotate Right (Snapped)");
        }
        else
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, 90f, 0);
            isRotating = true;
            Debug.Log("Rotate Right (Free)");
        }
    }

    private void RotateSlotAttachPoint()
    {
        if (CurrentSlot?.socketInteractor != null)
            ApplyRotationStepToSlot(CurrentSlot.socketInteractor);
        isRotating = false;
    }

    public int RotationStep => rotationStep;

    /// <summary>
    /// Returns the set of directions that are currently open after applying rotation.
    /// RotateLeft (CCW) increments rotationStep; RotateRight (CW) decrements it (+3 mod 4).
    /// </summary>
    public HashSet<PipeDirection> GetOpenDirections()
    {
        var result = new HashSet<PipeDirection>();

        // CW rotation count derived from CCW-based rotationStep
        int cwCount = (4 - rotationStep) % 4;

        if (openUp)    result.Add(RotateCW(PipeDirection.Up,    cwCount));
        if (openRight) result.Add(RotateCW(PipeDirection.Right, cwCount));
        if (openDown)  result.Add(RotateCW(PipeDirection.Down,  cwCount));
        if (openLeft)  result.Add(RotateCW(PipeDirection.Left,  cwCount));

        return result;
    }

    // Rotates a direction clockwise by `count` steps (each step is 90 degrees)
    private static PipeDirection RotateCW(PipeDirection dir, int count)
    {
        return (PipeDirection)(((int)dir + count) % 4);
    }
}
