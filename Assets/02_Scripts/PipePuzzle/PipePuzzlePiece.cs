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

    private int rotationStep = 0;
    private bool isRotating = false;
    private Quaternion targetRotation;
    [SerializeField] private float rotationSpeed = 10f;

    private Quaternion slotOriginalAttachRotation;
    private XRSocketInteractor modifiedSocket = null;

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

        // 이전에 수정한 소켓이 있으면 slotOriginalAttachRotation을 덮어쓰기 전에 먼저 복원
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
        // 호버 단계에서 이미 처리된 경우 중복 적용 방지
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
        // OnHoverEntered(X)가 OnSelectExited(X)보다 먼저 와서 early return된 경우를 위해
        // 다음 프레임에 아직 같은 소켓을 호버 중이면 회전을 재적용
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

    private void ApplyRotationStepToSlot(XRSocketInteractor socket)
    {
        // RotateLeft(CCW) 1회 = rotationStep+1, 슬롯 attachTransform Euler(90,0,0)
        // cwCount번 CW 회전 = Euler(-90*cwCount, 0, 0)
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