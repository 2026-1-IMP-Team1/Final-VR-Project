using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Handles a single numbered button: XR interaction, press animation, sound, and color feedback
public class SingleButton : MonoBehaviour
{
    // Reference to the central manager that validates the player's button selections
    public ButtonManager buttonManager;
    // Unique ID for this button (1–10); matched against ButtonManager.correctAnswer
    public int buttonId;

    // How far (metres) the button travels inward when pressed
    public float pressDepth = 0.012f;
    // Lerp speed for the press/release animation
    public float pressSpeed = 14f;

    public AudioClip pressSound;
    private AudioSource audioSource;

    private XRSimpleInteractable interactable;
    private Renderer rend;
    // Stores the button's original color so it can be restored after deselection
    private Color originalColor;

    // Local positions used as animation targets
    private Vector3 restPos;    // Default (unpressed) position
    private Vector3 pressedPos; // Depressed position when selected
    private bool isPressed;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;

        // Add AudioSource at runtime; spatialBlend = 1 makes it a 3D positional sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound — volume falls off with distance

        interactable.selectEntered.AddListener(OnSelectEnter);
    }

    void Start()
    {
        restPos = transform.localPosition;

        // Calculate the press direction in local space relative to the parent panel.
        // -transform.up points "into" the button surface, simulating a physical push.
        Vector3 dir = transform.parent != null
            ? transform.parent.InverseTransformDirection(-transform.up)
            : -transform.up;
        pressedPos = restPos + dir.normalized * pressDepth;
    }

    void Update()
    {
        // Smoothly animate between rest and pressed positions every frame
        Vector3 target = isPressed ? pressedPos : restPos;
        transform.localPosition = Vector3.Lerp(transform.localPosition, target,
                                               Time.deltaTime * pressSpeed);
    }

    void OnDestroy()
    {
        // Clean up the listener to prevent memory leaks or stale callbacks
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEnter);
        }
    }

    // Triggered when the player selects this button with the XR ray
    void OnSelectEnter(SelectEnterEventArgs args)
    {
        isPressed = true;

        if (pressSound != null)
            audioSource.PlayOneShot(pressSound);

        // Notify ButtonManager so it can track selection and check the answer
        if (buttonManager != null)
            buttonManager.OnButtonInteract(this);
    }

    // Called by ButtonManager to update the button's color based on selection state
    // selected = true  → apply selectedColor (yellow, green, or red)
    // selected = false → restore the original color
    public void SetSelected(bool selected, Color selectedColor)
    {
        rend.material.color = selected ? selectedColor : originalColor;
    }
}