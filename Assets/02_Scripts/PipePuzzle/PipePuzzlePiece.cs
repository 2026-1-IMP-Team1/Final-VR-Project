using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Linq;

public class PipePuzzlePiece : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    private int rotationStep = 0;
    private bool isRotating = false;
    private Quaternion targetRotation;
    [SerializeField] private float rotationSpeed = 10f;
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

    public void RotateLeft()
    {
        if (isRotating) return;
        rotationStep = (rotationStep + 1) % 4;

        if (IsSnapped && CurrentSlot != null)
        {
            RotateSlotAttachPoint(90f);
            Debug.Log("Rotate Left (Snapped)");
        }
        else
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, -90f, 0);
            isRotating = true;
            Debug.Log("Rotate Left (Free)");
        }
    }

    public void RotateRight()
    {
        if (isRotating) return;
        rotationStep = (rotationStep + 3) % 4;

        if (IsSnapped && CurrentSlot != null)
        {
            RotateSlotAttachPoint(-90f);
            Debug.Log("Rotate Right (Snapped)");
        }
        else
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, 90f, 0);
            isRotating = true;
            Debug.Log("Rotate Right (Free)");
        }
    }

    private void RotateSlotAttachPoint(float angle)
    {
        Transform attachPoint = CurrentSlot.socketInteractor.attachTransform;
        if (attachPoint != null)
            attachPoint.rotation *= Quaternion.Euler(angle, 0, 0);
        isRotating = false;
    }

    public int RotationStep => rotationStep;
}