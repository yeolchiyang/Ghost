using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    [SerializeField] UnityEngine.UI.Image StaminaUIWhite;//스테미나 출력 UI
    [SerializeField] Player player;//저장하고 있는 player클래스
    [SerializeField] Ghost ghost;//저장하고 있는 Ghost클래스
    [SerializeField] UnityEngine.UI.Text playerMentalityStatUI;//정신력 출력 UI
    [SerializeField] GameObject videoHandler;//videoHandlerUI에 접근하기 위한 변수
    [SerializeField] GameObject escUI;

    List<int> evidences = new List<int>();//CheckEvidenceToggle로 넣어주는 증거 고유값

    private void Start()
    {
        
    }

    public Player Player
    {
        get { return player; }
        set { player = value; }
    }
    public Ghost Ghost
    {
        get { return ghost; }
        set { ghost = value; }
    }
    public GameObject VideoHandler
    {
        get { return videoHandler; }
    }
    public GameObject EscUI
    {
        get { return escUI; }
    }


    public List<int> Evidences
    {
        get { return evidences; }
        set { evidences = value; }
    }

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            escUI.SetActive(!escUI.activeSelf);
        }

        setMentalityStatUI();
    }

    public void setStaminaFillAmount(float currentStamina, float maxStamina)
    {
        StaminaUIWhite.fillAmount = currentStamina / maxStamina;
    }

    public void setMentalityStatUI()
    {
        playerMentalityStatUI.text = "정신력 : "
           + player.CurrentMentalityStat.ToString("#.##");
    }

}
