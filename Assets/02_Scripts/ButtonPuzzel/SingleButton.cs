using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SingleButton : MonoBehaviour
{
    public ButtonManager buttonManager;
    public int buttonId;

    [Header("누름 애니메이션")]
    public float pressDepth = 0.012f;
    public float pressSpeed = 14f;

    [Header("사운드")]
    public AudioClip pressSound;    // 버튼 누를 때
    private AudioSource audioSource;

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

        // AudioSource 자동 추가
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D 사운드

        interactable.selectEntered.AddListener(OnSelectEnter);
    }

    void Start()
    {
        restPos = transform.localPosition;
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
        }
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        isPressed = true;

        // 누를 때 사운드
        if (pressSound != null)
            audioSource.PlayOneShot(pressSound);

        if (buttonManager != null)
            buttonManager.OnButtonInteract(this);
    }

    public void SetSelected(bool selected, Color selectedColor)
    {
        rend.material.color = selected ? selectedColor : originalColor;
    }
}