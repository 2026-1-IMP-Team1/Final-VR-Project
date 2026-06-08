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
        if (col == null)
        {
            Debug.LogError($"[CakeRabbitTrigger] {gameObject.name} 오브젝트에 Collider(충돌체)가 없습니다! Box Collider나 Sphere Collider 등을 추가해 주세요.");
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        Rigidbody parentRb = GetComponentInParent<Rigidbody>();
        if (rb == null && parentRb == null)
        {
            Debug.LogWarning($"[CakeRabbitTrigger] {gameObject.name} 또는 그 부모 오브젝트에 Rigidbody 컴포넌트가 없습니다. " +
                             $"Unity 물리 엔진 특성상 양쪽 오브젝트(플레이어와 CakeRabbit) 모두 Rigidbody가 없으면 충돌이 감지되지 않을 수 있습니다. " +
                             $"CakeRabbit 오브젝트에 [Rigidbody]를 추가하고, [Is Kinematic]을 체크(True)해 주는 것을 추천합니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[CakeRabbitTrigger] Trigger 진입 감지! 부딪힌 오브젝트: {other.gameObject.name} | 태그: {other.gameObject.tag}");
        TryTransition(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[CakeRabbitTrigger] Collision 충돌 감지! 부딪힌 오브젝트: {collision.gameObject.name} | 태그: {collision.gameObject.tag}");
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
            Debug.Log($"[CakeRabbitTrigger] ★CustomFallRespawn 감지 성공!★ 씬 전환을 시도합니다: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.Log($"[CakeRabbitTrigger] 부딪힌 오브젝트({hitObject.name})는 CustomFallRespawn 컴포넌트가 없어 씬 전환이 무시되었습니다.");
        }
    }
}
