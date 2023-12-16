using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscUI : MonoBehaviour
{

    [SerializeField] Button exitButton;
    [SerializeField] Button closeButton;


    private void Start()
    {
        exitButton.onClick.AddListener(exitButtonEvent);
        closeButton.onClick.AddListener(closeButtonEvent);
        
    }
    public void exitButtonEvent()
    {
        Application.Quit();
    }
    public void closeButtonEvent()
    {
        GameObject escUI = GameManager.gameManager.EscUI;
        GameManager.gameManager.EscUI.SetActive(!escUI.activeSelf);
    }
    

}
