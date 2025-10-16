using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Inventory Setting")]
    public int inventorySize = 20;      //인벤토리 슬롯 개수
    public GameObject inventoryUI;      //UI 패널
    public Transform itemSlotParanet;   //슬롯들이 들어갈 부모 오브젝트
    public GameObject itemSlotPrefab;   //슬롯 프리팹

    [Header("Input")]
    public KeyCode inventoryKey = KeyCode.I;                            //인벤토리 열기 키
    public List<InventorySlot> slots = new List<InventorySlot>();       //모든 슬롯 리스트
    private bool isInventoryOpen = false;                               //인벤토리가 열려있는지 확인

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        CreateInventorySlots();
        inventoryUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }
    }

    void CreateInventorySlots()         //인벤토리 슬롯들을 생성하는 함수
    {
        for(int i = 0; i < inventorySize; i++)
        {
            GameObject slotObject = Instantiate(itemSlotPrefab, itemSlotParanet);
            InventorySlot slot = slotObject.GetComponent<InventorySlot>();
            slots.Add(slot);            //리스트에 추가
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        if(isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    //아이템을 인벤토리에 추가 함수
    public bool AddItem(ItemData item, int amount = 1)
    {
        foreach (InventorySlot slot in slots)       //1단계 : 이미 있는 아이템에 추가 시도 (스택)
        {
            if(slot.item == item && slot.amount < item.maxStack)        //같은 아이템이고 최대 스택보다 작으면
            {
                int spaceLeft = item.maxStack - slot.amount;            //남은 공간 계산
                int amountToAdd = Mathf.Min(amount, spaceLeft);         //추가할 개수
                slot.AddAmount(amountToAdd);

                amount -= amountToAdd;                                  //기존 추가한 개수에 남은 개수를 구한다.

                if(amount <= 0)
                {
                    return true;
                }
            }
        }

        foreach (InventorySlot slot in slots)       //2단계 : 빈슬롯에 추가
        {
            if(slot.item == null)                   //빈슬롯 찾기
            {
                slot.SetItem(item, amount);
                return true;
            }
        }

        Debug.Log("인벤토리가 가득 참");
        return false;

    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        foreach(InventorySlot slot in slots)
        {
            if(slot.item == item)
            {
                slot.RemoveAmount(amount);
                return;
            }
        }
    }

    public int GetItemCount(ItemData item)
    {
        int count = 0;
        foreach (InventorySlot slot in slots)
        {
            if(slot.item == item)
            {
                count += slot.amount;
            }
        }
        return count;
    }
}
