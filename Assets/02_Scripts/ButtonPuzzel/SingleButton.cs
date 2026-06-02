using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SingleButton : MonoBehaviour
{
    public ButtonManager buttonManager;
    public int buttonId;

    [Header("누름 애니메이션")]
    public float pressDepth = 0.012f;   // 눌렸을 때 들어가는 깊이(m). 반대로 들어가면 음수로
    public float pressSpeed = 14f;

    private XRSimpleInteractable interactable;
    private Renderer rend;
    private Color originalColor;

    private Vector3 restPos;
    private Vector3 pressedPos;
    private bool isPressed;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;

        interactable.selectEntered.AddListener(OnSelectEnter);
        interactable.selectExited.AddListener(OnSelectExit);
    }

    void Start()
    {
        restPos = transform.localPosition;
        // 캡이 향한 방향(-위쪽)으로 들어가도록 계산
        Vector3 dir = transform.parent != null
            ? transform.parent.InverseTransformDirection(-transform.up)
            : -transform.up;
        pressedPos = restPos + dir.normalized * pressDepth;
    }

    void Update()
    {
        Vector3 target = isPressed ? pressedPos : restPos;
        transform.localPosition = Vector3.Lerp(transform.localPosition, target,
                                               Time.deltaTime * pressSpeed);
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEnter);
            interactable.selectExited.RemoveListener(OnSelectExit);
        }
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        isPressed = true;
        if (buttonManager != null)
            buttonManager.OnButtonInteract(this);
    }

    void OnSelectExit(SelectExitEventArgs args)
    {
        isPressed = false;
    }

    public void SetSelected(bool selected, Color selectedColor)
    {
        rend.material.color = selected ? selectedColor : originalColor;
    }
}