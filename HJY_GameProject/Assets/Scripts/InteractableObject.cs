using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("상호작용 정보")]
    public string objectName = "아이템 ";
    public string interactionText = "[E] 상호작용";
    public InteractionType interactionType = InteractionType.Item;

    [Header("하이라이트 설정")]
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 1.5f;

    public Renderer objectRenderer;
    private Color originalColor;
    private bool isHighlighted = false;

    public enum InteractionType
    {
        Item,           //아이템
        Machine,        //기계
        Building,       //건물
        NPC,            //NPC   
        Cellectible     //수집품
    }

    protected virtual void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if(objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        gameObject.layer = 8;                       //Layer 8 = Interactable 만들어주기
    }

    public virtual void OnPlayerEnter()
    {
        Debug.Log($"{objectName}) 감지됨");
        HighlightObject();
    }
    
    public virtual void OnPlayerExit()
    {
        Debug.Log($"{objectName}) 범위에서 벗어남");
        RemoveHighlight();
    }





    protected virtual void HighlightObject()        //가상함수로 하이라이트 구현
    {
        if (objectRenderer != null && !isHighlighted)
        {
            objectRenderer.material.color = highlightColor;
            objectRenderer.material.SetFloat("_Emission", highlightIntensity);
        }
    }

    protected virtual void RemoveHighlight()        //가상함수로 하이라이트 제거 구현
    {
        if(objectRenderer != null && !isHighlighted)
        {
            objectRenderer.material.color = originalColor;
            objectRenderer.material.SetFloat("_Emission", 0f);
            isHighlighted = false;
        }
    }

    protected virtual void CollectItem()                         //아이템 수집가상함수
    {
        Destroy(gameObject);                                     //아이템을 파괴한다.
    }

    protected virtual void OperateMachine()                     //기계 작동 함수
    {
        if(objectRenderer != null)
        {
            objectRenderer.material.color = Color.green;          //우선 색을 초록으로 만든다.
        }
    }

    protected virtual void AccessBuilding()                      //빌딩 접근
    {
        transform.Rotate(Vector3.up * 90f);                      //우선 회전을 시킨다.
    }

    protected virtual void TalkToNPC()                             //NPC 접근
    {
        Debug.Log($"{objectName}와 대화를 시작합니다.");           //우선 디버그 로그만 한다.
    }       

    public virtual void Interact()
    {
        //상호작용 타입에 따른 기본 동작
        switch(interactionType)
        {
            case InteractionType.Item:
                CollectItem();
                break;
            case InteractionType.Machine:
                OperateMachine();
                break;
            case InteractionType.Building:
                AccessBuilding();
                break;
            case InteractionType.Cellectible:
                CollectItem();
                    break;
            case InteractionType.NPC:
                TalkToNPC();
                break;
        }
    }

    public string GetInterationText()
    {
        return interactionText;
    }

}
