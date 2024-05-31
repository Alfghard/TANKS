using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeControls : MonoBehaviour
{
    public void SwitchControl()
    {   
        bool ControlState = PlayerTank.ControlManette;
        PlayerTank.ControlManette = !ControlState;
    }

    public void SwitchManette()
    {   
        PlayerTank.ControlManette = true;
        Debug.Log("Switched Controls to Manette");
    }

    public void SwitchSouris()
    {   
        PlayerTank.ControlManette = false;
        Debug.Log("Switched Controls to Souris");
    }
    

}
