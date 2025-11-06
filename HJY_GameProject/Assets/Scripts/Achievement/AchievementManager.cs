using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    [Header("Achievement Settings")]
    public List<AchievementData> allAchievements = new List<AchievementData>();

    [Header("UI References")]
    public GameObject achievementPopupPrefab;
    public Transform popupParent;
    public GameObject achievementPanel;
    public Transform achievementListContent;
    public GameObject achievementSlotPrefab;

    private Dictionary<AchievementType, int> progressData = new Dictionary<AchievementType, int>(); //통계 저장

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
            
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetAllAchievements();         //시작 시 리셋 강제로 (테스트용)
        foreach(AchievementType type in System.Enum.GetValues(typeof(AchievementType)))     //각 타입별 초기화
        {
            progressData[type] = 0;
        }
        LoadAchievements();
        UpdateAchievementUI();
    }


    public float GetProgress(AchievementData achievement)       //진행도 가져오기
    {
        if (achievement.isUnlocked) return 1f;
        int current = progressData.ContainsKey(achievement.achievementType) ? progressData[achievement.achievementType] : 0;
        return Mathf.Min((float)current / achievement.requiredAmount, 1f);
    }

    public void UpdateAchievementUI()   //업적 UI 업데이트 함수
    {
        if (achievementListContent == null || achievementSlotPrefab == null) return;    //두 내용중 하나라도 없으면 오류기 때문에 return
        
        foreach(Transform child in achievementListContent)  //기존 슬롯 제거
        {
            Destroy(child.gameObject);
        }

        foreach(AchievementData achievement in allAchievements)
        {
            GameObject slot = Instantiate(achievementSlotPrefab, achievementListContent);
            AchievementSlot slotScript = slot.GetComponent<AchievementSlot>();

            if(slotScript != null)
            {
                slotScript.SetAchievement(achievement, GetProgress(achievement));
            }
        }
    }

    void SaveAchievements()     //저장
    {
        foreach (var kvp in progressData)
        {
            PlayerPrefs.SetInt("Achievement_" + kvp.Key, kvp.Value);
        }

        foreach(AchievementData achievement in allAchievements)
        {
            PlayerPrefs.SetInt("Unlocked_" + achievement.name, achievement.isUnlocked ? 1 : 0);
        }

        PlayerPrefs.Save();
    }

    void LoadAchievements()     //불러오기
    {
        foreach(AchievementType type in System.Enum.GetValues(typeof(AchievementType)))
        {
            progressData[type] = PlayerPrefs.GetInt("Achievement_" + type, 0);
        }

        foreach (AchievementData achievement in allAchievements)
        {
            achievement.isUnlocked = PlayerPrefs.GetInt("Unlocked_" + achievement.name, 0) == 1;
        }    
    }

    public void ResetAllAchievements()       //초기화
    {
        foreach (AchievementType type in System.Enum.GetValues(typeof(AchievementType)))
        {
            progressData[type] = 0;
            PlayerPrefs.DeleteKey("Achievement_" + type);
        }

        foreach (AchievementData achievement in allAchievements)
        {
            achievement.isUnlocked = false;
            PlayerPrefs.DeleteKey("Unlocked_" + achievement.name);
        }

        PlayerPrefs.Save();
        UpdateAchievementUI();
    }   

    void ShowAchievementPopup(AchievementData achievement)
    {
        if(achievementPopupPrefab != null && popupParent != null)
        {
            GameObject popup = Instantiate(achievementPopupPrefab, popupParent);

            Text titleText = popup.transform.Find("Title")?.GetComponent<Text>();
            Text descText = popup.transform.Find("Description")?.GetComponent<Text>();

            if (titleText != null) titleText.text = "업적 달성";
            if (descText != null) descText.text = achievement.achievementName;

            Destroy(popup, 3.0f);
        }
    }

    void UnlockAchievement (AchievementData achievement)
    {
        achievement.isUnlocked = true;
        //보상이 있는 업적일 경우 여기에 추가
        ShowAchievementPopup(achievement);
        UpdateAchievementUI();
    }

    public void UpdateProgress(AchievementType type, int amount = 1)        //진행도 업데이트
    {
        progressData[type] += amount;

        foreach(AchievementData achievement in allAchievements)             //해당 타입의 모든 업적 체크
        {
            if(achievement.achievementType == type && !achievement.isUnlocked)
            {
                if (progressData[type] >= achievement.requiredAmount)
                {
                    UnlockAchievement(achievement);
                }
            }
        }
    }
}
