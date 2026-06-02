using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PhotoHint : MonoBehaviour
{
    public string photoName = "kitten";
    public TextMeshProUGUI labelText;
    public GameObject labelCanvas;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    void Start()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        labelCanvas.SetActive(false);
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        labelText.text = photoName;
        labelCanvas.SetActive(true);
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        labelCanvas.SetActive(false);
    }
}