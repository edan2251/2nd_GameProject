using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동설정")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float rotationSpeed = 10.0f;

    [Header("점프 설정")]
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;                              //중력 속도 추가
    public float landingDuration = 0.3f;                        //착지 후 착지 모션 지속 시간 ( 해당 시간 후에 캐릭터가 움직일 수 있게.


    [Header("공격 설정")]
    public float attackDuration = 0.8f;                         //공격 지속 시간
    public bool canMoveWhileAttacking = false;                  //공격중 이동 가능 여부 판단 bool

    [Header("컴포넌트")]
    public Animator animator;                                   //컴포넌트 하위에 animator 가 존재하기 때문에

    private CharacterController controller;
    private Camera playerCamera;

    //현재 상태 값들
    private float currentSpeed;
    private bool isAttacking = false;
    private bool isLanding = false;                             //착지 중인지 확인
    private float landingTimer;                                 //착지 타이머

    private Vector3 velocity;
    private bool isGrounded;                                    //땅에 있는지 판단
    private bool wasGrounded;                                   //직전 프레임에 땅에 있었는지 판단 (땅에 도착한건지, 점프를 시작한건지 확인하려고)
    private float attackTimer;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    void Update()
    {
        CheckGroudned();
        HandleMovement();
        Handlelanding();
        UpdateAnimator();
        HandleAttack();
        HandleJump();
    }

    void HandleMovement()
    {
        //공격 중이거나 착지 중일 때 움직임 제한
        if((isAttacking && !canMoveWhileAttacking) || isLanding)
        {
            currentSpeed = 0;
            return;
        }



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
        animator.SetBool("isGrounded", isGrounded);

        bool isFalling = !isGrounded && velocity.y < -0.1f; //캐릭터가 축 속도가 음수로 넘어가면 떨어지고 있다고 판단
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isLanding", isLanding);
    }

    void CheckGroudned()
    {
        wasGrounded = isGrounded;
        isGrounded = controller.isGrounded;                 //캐릭터 컨트롤러에서 상태값 받기

        if(!isGrounded && wasGrounded)
        {
            Debug.Log("떨어지기 시작");
        }
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
            
            if(!wasGrounded && animator != null)
            {
                isLanding = true;
                landingTimer = landingDuration;
            }
        }
    }

    void HandleJump()                                               //땅 위에 있지 않을 경우 중력 적용
    {
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if(animator != null)
            {
                animator.SetTrigger("jumpTrigger");
            }
        }

        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    void Handlelanding()
    {
        if(isLanding)
        {
            landingTimer -= Time.deltaTime;

            if(landingTimer <= 0)
            {
                isLanding = false;
            }
        }
    }

    void HandleAttack()
    {
        if(isAttacking)                                                         //공격 중일 때
        {
            attackTimer -= Time.deltaTime;                                      //공격 타이머를 감소

            if(attackTimer <= 0)
            {
                isAttacking = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking)                   //공격중이 아닐때 키를 누르면 공격
        {
            isAttacking = true;                                                 //공격중 표시
            attackTimer = attackDuration;                                       //타이머 리필

            if(animator != null)
            {
                animator.SetTrigger("attackTrigger");
            }
        }
    }

}
