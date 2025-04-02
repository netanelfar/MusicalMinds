using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class KeyControl : MonoBehaviour
{



    [SerializeField] GameObject lowC;
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AudioManager.Instance.PlayNote("Low C");
        }
        
    }

    public void Press(string Note)
    {
        AudioManager.Instance.PlayNote(Note);
    }
 
}
