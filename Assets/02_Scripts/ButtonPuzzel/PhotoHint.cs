using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

// Displays a hint label when the player hovers their XR ray over a photo card on the wall
public class PhotoHint : MonoBehaviour
{
    // The word shown as a hint (e.g. "kitten" → player deduces "ten" → 10)
    public string photoName = "kitten";
    // TextMeshPro component that renders the hint text
    public TextMeshProUGUI labelText;
    // World-space canvas containing the label; toggled on/off on hover
    public GameObject labelCanvas;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    void Start()
    {
        // Get the XRSimpleInteractable on this GameObject and register hover callbacks
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);

        // Hide the label canvas by default until the player hovers over the photo
        labelCanvas.SetActive(false);
    }

    // Triggered when the player's XR ray enters this object — show the hint label
    void OnHoverEnter(HoverEnterEventArgs args)
    {
        labelText.text = photoName;
        labelCanvas.SetActive(true);
    }

    // Triggered when the player's XR ray leaves this object — hide the hint label
    void OnHoverExit(HoverExitEventArgs args)
    {
        labelCanvas.SetActive(false);
    }
}