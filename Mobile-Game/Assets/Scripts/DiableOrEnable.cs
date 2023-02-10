using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiableOrEnable : MonoBehaviour
{
    // Start is called before the first frame update
    
    public void EnableDisable()
    {
        Button button = GetComponent<Button>();
        if(button.interactable == true)
        {
            button.interactable = false;
        }
    }
}
