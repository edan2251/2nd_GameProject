using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PickUpItem : InteractableObject
{
    public ItemData itemData;   //아이템 데이터
    public int amount = 1;      //아이템 개수

    public override void Interact()
    {
        base.Interact();

        if(InventoryManager.Instance != null)                                       //인벤토리 매니저가 있으면
        {
            bool added = InventoryManager.Instance.AddItem(itemData, amount);       //인벤토리에 아이템 추가 시도

            if(added)
            {
                Destroy(gameObject);            //성공하면 오브젝트 제거
            }
        }
    }
}
