using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetResultInfo : MonoBehaviour 
{
    [SerializeField]
    private Text displayName;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetInfo(UserStatsToken stats)
    {
        displayName.text = stats.DisplayName;
    }
}
