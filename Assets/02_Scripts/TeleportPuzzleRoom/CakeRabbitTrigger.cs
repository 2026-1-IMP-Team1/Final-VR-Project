using UnityEngine;
using UnityEngine.SceneManagement;

public class CakeRabbitTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("플레이어가 닿았을 때 이동할 씬의 이름입니다.")]
    public string targetSceneName = "ButtonPuzzelRoom";

    [Header("Collision Settings")]
    [Tooltip("플레이어 오브젝트에 설정된 태그입니다.")]
    public string playerTag = "Player";

    [Tooltip("태그 검사 외에 CharacterController 컴포넌트 유무로도 플레이어를 판별할지 여부입니다.")]
    public bool checkCharacterController = true;

    private bool isTransitioning = false;

    private void Start()
    {
        // 1. Collider 확인
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"[CakeRabbitTrigger] {gameObject.name} 오브젝트에 Collider(충돌체)가 없습니다! Box Collider나 Sphere Collider 등을 추가해 주세요.");
        }

        // 2. Rigidbody 확인 (물리 충돌 감지를 위해 둘 중 하나에는 Rigidbody가 필요합니다)
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
        // 충돌이 일어나면 무조건 어떤 오브젝트와 부딪혔는지 로그를 남겨 디버깅을 돕습니다.
        Debug.Log($"[CakeRabbitTrigger] Trigger 진입 감지! 부딪힌 오브젝트: {other.gameObject.name} | 태그: {other.gameObject.tag}");
        TryTransition(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌이 일어나면 무조건 어떤 오브젝트와 부딪혔는지 로그를 남겨 디버깅을 돕습니다.
        Debug.Log($"[CakeRabbitTrigger] Collision 충돌 감지! 부딪힌 오브젝트: {collision.gameObject.name} | 태그: {collision.gameObject.tag}");
        TryTransition(collision.gameObject);
    }

    private void TryTransition(GameObject hitObject)
    {
        if (isTransitioning) return;

        bool isPlayer = false;

        // 플레이어 판정 로직 강화:
        // 1. 직접 부딪힌 오브젝트가 "Player" 태그인 경우
        // 2. 최상위(Root) 오브젝트가 "Player" 태그인 경우 (VR 카메라 리그 등)
        // 3. 부모 오브젝트 중 하나가 "Player" 태그인 경우
        if (hitObject.CompareTag(playerTag) || 
            (hitObject.transform.root != null && hitObject.transform.root.CompareTag(playerTag)) ||
            (hitObject.GetComponentInParent<Transform>() != null && hitObject.GetComponentInParent<Transform>().CompareTag(playerTag)))
        {
            isPlayer = true;
        }

        // 4. CharacterController가 있는 경우 플레이어로 판정
        if (!isPlayer && checkCharacterController)
        {
            if (hitObject.GetComponent<CharacterController>() != null || 
                hitObject.GetComponentInParent<CharacterController>() != null || 
                hitObject.transform.root.GetComponentInChildren<CharacterController>() != null)
            {
                isPlayer = true;
            }
        }

        if (isPlayer)
        {
            isTransitioning = true;
            Debug.Log($"[CakeRabbitTrigger] ★플레이어 감지 성공★ 씬 전환을 시도합니다: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.Log($"[CakeRabbitTrigger] 부딪힌 오브젝트({hitObject.name})는 플레이어가 아닌 것으로 판정되었습니다. (Player 태그 또는 CharacterController 없음)");
        }
    }
}
