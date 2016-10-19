using UnityEngine;
using System.Collections;

public class DDOL : MonoBehaviour 
{
    [SerializeField]
    private bool ddol;

    void Awake()
    {
        if(ddol == true)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
