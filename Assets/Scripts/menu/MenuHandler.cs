using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MenuHandler : MonoBehaviour
{
    public static MenuHandler Instance;

    private GameObject home;
    private Button homeSingleplayerButton;
    private Button homeMultiplayerButton;
    private Button homeHelpButton;

    private GameObject multiplayer;
    private Button multiplayerBackButton;
    private Button multiplayerServerButton;
    private Button multiplayerEnterButton;

    private string serverAddr;

    public string GetServerAddr()
    {
        return serverAddr;
    }

    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        home = transform.Find("Home").gameObject;
        multiplayer = transform.Find("Multiplayer").gameObject;

        homeSingleplayerButton = home.transform.Find("SinglePlayer").gameObject.GetComponent<Button>();
        homeMultiplayerButton = home.transform.Find("Multiplayer").gameObject.GetComponent<Button>();
        homeHelpButton = home.transform.Find("Help").gameObject.GetComponent<Button>();

        multiplayerBackButton = multiplayer.transform.Find("Back").gameObject.GetComponent<Button>();
        multiplayerServerButton = multiplayer.transform.Find("Server").gameObject.GetComponent<Button>();
        multiplayerEnterButton = multiplayer.transform.Find("Enter").gameObject.GetComponent<Button>();

        InitOnclickEvents();
    }

    private void InitOnclickEvents()
    {
        // Home Screen
        homeSingleplayerButton.onClick.AddListener(() => {
            Debug.Log("Singleplayer clicked");
            SinglePlayerScreen();
            SceneManager.LoadScene(2);
        });

        homeMultiplayerButton.onClick.AddListener(() => {
            Debug.Log("Multiplayer clicked");
            MultiplayerScreen();
        });

        homeHelpButton.onClick.AddListener(() => {
             Debug.Log("HELP clicked");
         });

        // Multiplayer Screen
        multiplayerBackButton.onClick.AddListener(() => {
            Debug.Log("BACK clicked");
            HomeScreen();
        });

        multiplayerServerButton.onClick.AddListener(() => {
            Debug.Log("SERVER clicked");
            MultiplayerServerButtonHandler();
        });

        multiplayerEnterButton.onClick.AddListener(() => {
            Debug.Log("ENTER clicked");
            MultiplayerEnterButtonHandler();
        });
    }

    private void HomeScreen()
    {
        home.SetActive(true);
        multiplayerEnterButton.gameObject.transform.parent.gameObject.SetActive(true);
        multiplayer.SetActive(false);
    }

    private void SinglePlayerScreen()
    {
        home.SetActive(false);
        multiplayer.SetActive(false);
    }

    private void MultiplayerScreen()
    {
        home.SetActive(false);
        multiplayer.SetActive(true);
    }

    private void GameScreen()
    {
        home.SetActive(false);
        multiplayer.SetActive(false);
    }

    private void MultiplayerServerButtonHandler()
    {
        try
        {
            //multiplayerEnterButton.gameObject.transform.parent.gameObject.SetActive(false);

            UDPSocketHandler.Instance.ServerHandler();

            StartCoroutine(CheckConnection());

            //127.0.0.1:4000

        }
        catch (Exception e)
        {
            Debug.Log("Error trying to create server: Details: `" + e.Message + "`");
        }
    }

    private IEnumerator CheckConnection()
    {
        int limitTime = 10;

        while (!UDPSocketHandler.Instance.ConnectionEstablished() && limitTime > 0)
        {
            limitTime--;

            yield return new WaitForSeconds(1);
        }

        Debug.Log("Connection Establisehd: " + UDPSocketHandler.Instance.ConnectionEstablished());

        if (UDPSocketHandler.Instance.ConnectionEstablished())
        {
            //multiplayerEnterButton.gameObject.transform.parent.gameObject.SetActive(true);
            GameScreen();
            SceneManager.LoadScene(1);
        }
    }

    private void MultiplayerEnterButtonHandler()
    {
        serverAddr = multiplayer.GetComponentInChildren<InputField>()?.text;
        string[] addrParams = serverAddr.Split(':');
        string ip = "";
        int port = -1;

        try
        {
            if (addrParams.Length == 2)
            {
                ip = addrParams[0];
                port = int.Parse(addrParams[1]);
            }

            UDPSocketHandler.Instance.ClientHandler(ip, port);

            StartCoroutine(CheckConnection());

        } catch (Exception e)
        {
            Debug.Log("Error trying to connect to the server: Details `" + e.Message + "`");
        }

    }
}
