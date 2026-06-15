using UnityEngine;
using UnityEngine.SceneManagement;

public class CakeRabbitTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string targetSceneName = "ButtonPuzzelRoom";

    [Header("Collision Settings")]
    public string playerTag = "Player";

    public bool checkCharacterController = true;

    private bool isTransitioning = false;

    private void Start()
    {
        Collider col = GetComponent<Collider>();

        Rigidbody rb = GetComponent<Rigidbody>();
        Rigidbody parentRb = GetComponentInParent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryTransition(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryTransition(collision.gameObject);
    }

    private void TryTransition(GameObject hitObject)
    {
        if (isTransitioning) return;

        bool hasRespawnComponent = false;

        if (hitObject.GetComponent<CustomFallRespawn>() != null)
        {
            hasRespawnComponent = true;
        }
        else if (hitObject.GetComponentInParent<CustomFallRespawn>() != null)
        {
            hasRespawnComponent = true;
        }
        else if (hitObject.transform.root != null && hitObject.transform.root.GetComponentInChildren<CustomFallRespawn>() != null)
        {
            hasRespawnComponent = true;
        }

        if (hasRespawnComponent)
        {
            isTransitioning = true;
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
