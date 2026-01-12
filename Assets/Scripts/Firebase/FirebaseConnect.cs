using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseConnect : MonoBehaviour
{

    private const string firebaseUrl =
        "https://realm-craft-topdown-2d-default-rtdb.asia-southeast1.firebasedatabase.app/";

    private FirebaseService _firebase;

    private void Awake()
    {
        _firebase = new FirebaseService(firebaseUrl);
    }

    // ================= UI BUTTON =================

    public async void OnSignUpButtonClick()
    {
        await SignUp();
    }

    public async void OnSignInButtonClick()
    {
        await SignIn();
    }

    // ================= CORE LOGIC =================

    private async Task SignUp()
    {
        string username = GetUsername();
        string password = GetPassword();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Username or password is empty");
            return;
        }

        var accounts = await _firebase.GetAccounts();

        if (accounts.Any(a => a.Username == username))
        {
            ShowMessage("Account already exists");
            ClearInput();
            return;
        }

        AccountData account = AccountFactory.Create(username, password);
        await _firebase.CreateAccount(account);

        ShowMessage("Sign up success");
        ClearInput();
    }

    private async Task SignIn()
    {
        string username = GetUsername();
        string password = GetPassword();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Username or password is empty");
            return;
        }

        var accounts = await _firebase.GetAccounts();

        var account = accounts
            .FirstOrDefault(a => a.Username == username && a.Password == password);

        if (account == null)
        {
            ShowMessage("Username or password incorrect");
            return;
        }



        // ✅ LOAD PLAYER VÀO RUNTIME
        PlayerRuntime.Instance.Player.LoadFromData(account.Player);

        ShowMessage("Sign in successful");

        SceneManager.LoadScene("GameScene");
    }

    // ================= HELPERS =================

    private string GetUsername()
    {
        return GameObject.Find("Canvas")
            .transform.Find("InputUsername")
            .GetComponent<TMP_InputField>().text.Trim();
    }

    private string GetPassword()
    {
        return GameObject.Find("Canvas")
            .transform.Find("InputPassword")
            .GetComponent<TMP_InputField>().text.Trim();
    }

    private void ClearInput()
    {
        GetInputField("InputUsername").text = "";
        GetInputField("InputPassword").text = "";
    }

    private TMP_InputField GetInputField(string name)
    {
        return GameObject.Find("Canvas")
            .transform.Find(name)
            .GetComponent<TMP_InputField>();
    }

    private void ShowMessage(string message)
    {
        GameObject.Find("Canvas")
            .transform.Find("Text")
            .GetComponent<TextMeshProUGUI>().text = message;
    }


    //*******************************************************************************
    /*
    // Firebase URL
    private const string firebaseUrl = "https://realm-craft-topdown-2d-default-rtdb.asia-southeast1.firebasedatabase.app/";
    private static readonly HttpClient client = new HttpClient();

    static async void Start()
    {
        //VIETNAMESE
        Console.OutputEncoding = Encoding.UTF8;


        //CONNECT FIREBASE
        FirebaseApp.Create(new AppOptions()
        {
            //KEY GENERATE (PROJECT SETTING -> SERVICE ACCOUNTS -> GENERATE NEW KEY)
            Credential = GoogleCredential.FromFile("realm-craft-topdown-2d-firebase-adminsdk-fbsvc-1e1dc59d80")
        });

        var firebase = new FirebaseClient(firebaseUrl);

        //CONNECT AND LOAD JSON
        string json = await client.GetStringAsync("https://raw.githubusercontent.com/NTH-VTC/OnlineDemoC-/main/simple_players.json");

        //var allPlayers = JsonConvert.DeserializeObject<List<Player>>(json);

    }

    public async void OnSignUpButtonClick()
    {
        await SignUp();
    }

    public async void OnSignInButtonClick()
    {
        await SignIn();
    }

    private async Task SignUp()
    {
        string username = GameObject.Find("Canvas").gameObject.transform.Find("InputUsername").gameObject.GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("Canvas").gameObject.transform.Find("InputPassword").gameObject.GetComponent<TMP_InputField>().text;
        var firebase = new FirebaseClient(firebaseUrl);
        var accounts = await firebase.Child("Accounts").OnceAsync<Account>();

        foreach (var acc in accounts)
        {
            if (acc.Object.Username == username)
            {
                ShowMessage("Account Existed");
                ClearInput();
                return;
            }
        }
        var account = CreateAccount(username, password);
        await firebase.Child("Accounts").Child("Acc Created " + account.Timecreate).PutAsync(account);
        ShowMessage("Sign Up Successful");
        ClearInput();
        
    }



    private async Task SignIn()
    {
        string username = GameObject.Find("Canvas").gameObject.transform.Find("InputUsername").gameObject.GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("Canvas").gameObject.transform.Find("InputPassword").gameObject.GetComponent<TMP_InputField>().text;
        var firebase = new FirebaseClient(firebaseUrl);
        var accounts = await firebase.Child("Accounts").OnceAsync<Account>();
        foreach (var acc in accounts)
        {
            if (acc.Object.Username == username && acc.Object.Password == password)
            {
                PlayerData playerData = acc.Object.Player;
                PlayerRuntime.Instance.Player.LoadFromData(playerData);

                ShowMessage("Sign In Successful");

                SceneManager.LoadScene("GameScene");
                return;
            }

        }

        ShowMessage("Username or Password Incorrect");
    }

    private void ClearInput()
    {
        foreach (var fieldName in new[] { "InputUsername", "InputPassword" })
        {
            GameObject.Find("Canvas").transform.Find(fieldName).GetComponent<TMP_InputField>().text = "";
        }
    }

    private void ShowMessage(string message)
    {
        GameObject messageText = GameObject.Find("Canvas").transform.Find("Text").gameObject;
        if (messageText != null)
        {
            messageText.GetComponent<TextMeshProUGUI>().text = message;
        }
    }

    private Account CreateAccount(string username, string password)
    {
        var account = new Account
        {
            Username = username,
            Password = password,
            Timecreate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
            Player = new Player()
        };

        // Initialize player stats
        account.Player.CreatePlayer();

        // Khởi tạo item trong luồng chính (Unity main thread)
        Item item = ScriptableObject.CreateInstance<Item>();
        InitializeStarterItem(item);

        // Convert Item → Item2String (DTO cho Firebase)
        Item2String starterSword = Item2String.FromItem(item);

        // Add vào inventory
        account.Player.Inventory.Add(starterSword);

        return account;

    }


    private void InitializeStarterItem(Item item)
    {
        item.itemID = "item001";
        item.itemName = "Starter Sword";
        item.description = "A basic sword for new adventurers.";
        item.icon = null; // Gán biểu tượng nếu có
        item.quantity = 1;
        item.price = 100;
        item.level = 1;
        item.atk = 10;
        item.def = 0;
        item.hp = 0;
        item.mp = 0;
        item.bonus = 0;
    }

    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Timecreate { get; set; }
        public Player Player { get; set; }
    }

    public class Player
    {        
        public string ID { get; private set; } // Chỉ cho phép đọc
        public string Name { get; set; }
        public int Hp { get; set; }
        public int Mp { get; set; }
        public int Exp { get; set; }
        public int Lv { get; set; }
        public int Gold { get; set; }
        public bool IsOnline { get; set; }
        public string LastLogin { get; set; }
        public List<Item2String> Inventory { get; set; }
        
        public void CreatePlayer()
        {
            Name = "NewPlayer";
            Hp = 100;
            Mp = 50;
            Exp = 0;
            Lv = 1;
            Gold = 0;
            IsOnline = false;
            LastLogin = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            Inventory = new List<Item2String>();
        }
    }
    */
    //*******************************************************************************

}