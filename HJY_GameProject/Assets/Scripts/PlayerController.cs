using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동설정")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float rotationSpeed = 10.0f;

    [Header("공격 설정")]
    public float attackDuration = 0.8f;                         //공격 지속 시간
    public bool canMoveWhileAttacking = false;                  //공격중 이동 가능 여부 판단 bool

    [Header("컴포넌트")]
    public Animator animator;                                   //컴포넌트 하위에 animator 가 존재하기 때문에

    private CharacterController controller;
    private Camera playerCamera;

    //현재 상태 값들
    private float currentSpeed;
    private bool isAttacing = false;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(horizontal != 0 || vertical !=0)                         //둘중 하나라도 입력이 있을 때
        {
            //카메라가 보는 방향의 앞쪽으로 설정
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;            //이동 방향 설정

            if(Input.GetKey(KeyCode.LeftShift))                     //왼쪽 쉬프트를 눌러서 달리기로 변경
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }

            controller.Move(moveDirection * currentSpeed * Time.deltaTime);         //캐릭터 컨트롤러의 이동 입력

            //이동 진행 방향을 바라보면서 이동
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        }
        else
        {
            currentSpeed = 0;
        }
    }

    void UpdateAnimator()
    {
        //전체 최대속도 (runSpeed) 기준으로 0 ~ 1 계산
        float animatorSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
        animator.SetFloat("speed", animatorSpeed);
    }

}
