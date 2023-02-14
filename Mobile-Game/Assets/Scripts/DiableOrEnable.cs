using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiableOrEnable : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject textBox;
    public void EnableDisable()
    {
        Button button = GetComponent<Button>();
        if(button?.interactable == true)
        {
            button.interactable = false;
        }
    }
    public void ShowCredits()
    {
        TextMeshProUGUI text = textBox.GetComponent<TextMeshProUGUI>();
        
        if(text.enabled == false)
        {
            text.enabled = true;
        }
        else
        {
            text.enabled = false;
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
