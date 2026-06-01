using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CustomFallRespawn : MonoBehaviour
{
    [Header("Gravity Settings")]
    [Tooltip("적용할 중력의 크기입니다.")]
    public float gravity = -9.81f;

    [Header("Respawn Settings")]
    [Tooltip("플레이어가 이 Y좌표 아래로 떨어지면 처음 위치로 되돌아갑니다.")]
    public float fallThreshold = -10f;

    private Vector3 startPosition;
    private Vector3 velocity;
    
    private CharacterController characterController;

    void Start()
    {
        // 게임을 처음 시작했을 때의 위치를 저장해둡니다.
        startPosition = transform.position;
        
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 땅에 닿아있을 경우 수직 속도(중력)를 초기화하여 바닥에 밀착되도록 합니다.
        if (characterController.isGrounded)
        {
            if (velocity.y < 0)
            {
                // 땅에 닿아있어도 미세한 음수 값을 주어 CharacterController가 바닥을 계속 인식하게 함
                velocity.y = -2f;
            }
        }
        else
        {
            // 공중에 있을 때 중력 가속도 적용
            velocity.y += gravity * Time.deltaTime;
        }

        // 중력에 의한 이동 적용
        characterController.Move(velocity * Time.deltaTime);

        // 맵 밖으로 계속 추락하는 것을 방지 (일정 높이 이하로 떨어지면 리스폰)
        if (transform.position.y < fallThreshold)
        {
            // 위치를 강제로 이동시키기 위해 CharacterController를 잠시 비활성화
            characterController.enabled = false;
            transform.position = startPosition;
            velocity = Vector3.zero; // 떨어지던 속도 초기화
            characterController.enabled = true;
            Physics.SyncTransforms(); // 물리 엔진과 위치 동기화
        }
    }
}
