using UnityEngine;
using System.Collections;

public class MapRegionInteraction : MonoBehaviour
{
    private MapRegionInfo regionInfo;

    void Awake()
    {
        regionInfo = GetComponent<MapRegionInfo>();
    }

    public void OnClick()
    {
        if((GameStates)GameManager.Instance.FSM.CurrentState == GameStates.SetupPickRegion)
        {
            pickRegion();
        }
    }

    private void pickRegion()
    {
        // build event; pick region, send to server
        Debug.Log("picked region");
        RegionPicked rp = RegionPicked.Create(BoltNetwork.server);

        rp.RegionID = regionInfo.ID;

        Debug.Log("sending credentials: " + ClientManager.Instance.Credentials.DisplayName);

        rp.NewOwner = ClientManager.Instance.Credentials;

        rp.Send();

        GameManager.Instance.FSM.DonePickRegion();
    }
}
