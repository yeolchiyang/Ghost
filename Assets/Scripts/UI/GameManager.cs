using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    [SerializeField] UnityEngine.UI.Image StaminaUIWhite;//���׹̳� ��� UI
    [SerializeField] Player player;//�����ϰ� �ִ� playerŬ����
    [SerializeField] Ghost ghost;//�����ϰ� �ִ� GhostŬ����
    [SerializeField] UnityEngine.UI.Text playerMentalityStatUI;//���ŷ� ��� UI
    [SerializeField] GameObject videoHandler;//videoHandlerUI�� �����ϱ� ���� ����
    [SerializeField] GameObject escUI;

    List<int> evidences = new List<int>();//CheckEvidenceToggle�� �־��ִ� ���� ������

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
        playerMentalityStatUI.text = "���ŷ� : "
           + player.CurrentMentalityStat.ToString("#.##");
    }

}
