using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;
using System.Linq;

public class PipePuzzlePiece : MonoBehaviour
{
    [Header("Pipe Directions")]
    [SerializeField] private bool openUp;
    [SerializeField] private bool openDown;
    [SerializeField] private bool openLeft;
    [SerializeField] private bool openRight;

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

    /// <summary>
    /// 현재 회전 상태에서 실제로 열려 있는 방향 집합을 반환합니다.
    /// rotationStep 1 = RotateLeft(CCW) 1회, rotationStep 3 = RotateRight(CW) 1회
    /// </summary>
    public HashSet<PipeDirection> GetOpenDirections()
    {
        var result = new HashSet<PipeDirection>();

        // RotateRight(CW +90°Y)가 rotationStep을 -1(+3 mod4) 하므로
        // CW 회전 횟수 = (4 - rotationStep) % 4
        int cwCount = (4 - rotationStep) % 4;

        if (openUp)    result.Add(RotateCW(PipeDirection.Up,    cwCount));
        if (openRight) result.Add(RotateCW(PipeDirection.Right, cwCount));
        if (openDown)  result.Add(RotateCW(PipeDirection.Down,  cwCount));
        if (openLeft)  result.Add(RotateCW(PipeDirection.Left,  cwCount));

        return result;
    }

    private static PipeDirection RotateCW(PipeDirection dir, int count)
    {
        return (PipeDirection)(((int)dir + count) % 4);
    }
}