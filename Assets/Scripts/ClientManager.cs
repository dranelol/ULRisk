using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class ClientManager : MonoBehaviour 
{
    private string key = "ulVGD2015";

    private string createAccountURL = "http://donionrings.me/Games/NetworkingDemo/CreateAccount.php?";
    private string loginURL = "http://donionrings.me/Games/NetworkingDemo/Login.php?";
    private string getTokenInfoURL = "http://donionrings.me/Games/NetworkingDemo/GetTokenInfo.php?";

    private string loginName;
    private string loginPassword;

    private int authLevel;

    public UnityEvent OnLoginSuccessful;

    public UnityEvent OnLoginUnsuccessful;

    public UnityEvent OnAccountCreateSuccessful;

    public UnityEvent OnAccountCreateUnsuccessful;

    public UnityEvent OnPublicIPUnsuccessful;

    public string ConnectIP;

    public CredentialToken Credentials;

    public int AuthLevel
    {
        get
        {
            return authLevel;
        }
    }

    private string displayName;

    public string DisplayName
    {
        get
        {
            return displayName;
        }
    }

    private string loginIP;

    public string LoginIP
    {
        get
        {
            return loginIP;
        }
    }

    private bool gettingLoginInfo;

    public bool GettingLoginInfo
    {
        get
        {
            return gettingLoginInfo;
        }
    }

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

        ServerManager.Instance.AddToSession(Credentials);

        //BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse(ip));
    }
}
