using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PipePuzzlePiece : MonoBehaviour
{
    [Header("컴포넌트")]
    private XRGrabInteractable grabInteractable;

    [Header("상태")]
    private bool isSnapped = false;
    private PipePuzzleBoardGridSlot currentSlot = null;

    private int rotationStep = 0;

    private bool isRotating = false;
    private Quaternion targetRotation;
    [SerializeField] private float rotationSpeed = 10f;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
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

    public void RotateRight()
    {
        if (!isSnapped || isRotating) return;
        rotationStep = (rotationStep + 1) % 4;
        ApplyRotation();
    }

    public void RotateLeft()
    {
        if (!isSnapped || isRotating) return;
        rotationStep = (rotationStep + 3) % 4;
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        float yAngle = rotationStep * 90f;
        targetRotation = currentSlot.transform.rotation * Quaternion.Euler(0, yAngle, 0);
        isRotating = true;
    }

    public void OnSnappedToGrid(PipePuzzleBoardGridSlot slot)
    {
        isSnapped = true;
        currentSlot = slot;
        rotationStep = 0;
        targetRotation = slot.transform.rotation;
        isRotating = true;
    }

    public void OnRemovedFromGrid()
    {
        isSnapped = false;
        currentSlot = null;
        isRotating = false;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isRotating = false;
        isSnapped = false;
        currentSlot = null;
    }

    public bool IsSnapped => isSnapped;
    public int RotationStep => rotationStep;
}