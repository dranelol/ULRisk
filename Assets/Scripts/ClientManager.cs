using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientManager : MonoBehaviour 
{
    public string ConnectIP;

    private static ClientManager _instance;

    public static ClientManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ClientManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }


    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ConnectToIP(InputField IPfield)
    {
        var list = IPfield.text.Split('.');
        if (IPfield.text == "" || list.Length <= 3)
        {
            //CoreGUIManager.Instance.Hide("Notification");
            //CoreGUIManager.Instance.SetNotificationSubmitText("Please enter valid IP Address");
            //CoreGUIManager.Instance.Show("NotificationSubmit");
            //StartMenuGUIManager.Instance.GetComponent<GUIAnimationEvents>().turnInteractableOff();
            return;
        }

        ConnectIP = IPfield.text;
        BoltLauncher.StartClient();

        //StartMenuGUIManager.Instance.FSM.Transition("Server_Lobby");

        ServerManager.Instance.InitGameSession();

        Messenger.Broadcast("UserAddedToLobby", Credentials.DisplayName);

        ServerManager.Instance.AddToSession(Credentials);
    }

    public void ConnectToIP(string IPfield)
    {
        var list = IPfield.Split('.');
        if (IPfield == "" || list.Length <= 3)
        {
            //CoreGUIManager.Instance.Hide("Notification");
            //CoreGUIManager.Instance.SetNotificationSubmitText("Please enter valid IP Address");
            //CoreGUIManager.Instance.Show("NotificationSubmit");
            //StartMenuGUIManager.Instance.GetComponent<GUIAnimationEvents>().turnInteractableOff();
            return;
        }
        ConnectIP = IPfield;
        BoltLauncher.StartClient();

        //StartMenuGUIManager.Instance.FSM.Transition("Server_Lobby");

        ServerManager.Instance.InitGameSession();

        Messenger.Broadcast("UserAddedToLobby", Credentials.DisplayName);
        ServerManager.Instance.AddToSession(Credentials);

        //BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse(ip));
    }
}
