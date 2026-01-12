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

public class FirebaseConnect : MonoBehaviour
{
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
                ShowMessage("Sign In Successful");
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
        private static int _nextId = 1;
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

        public Player()
        {
            ID = _nextId.ToString("D4");
            _nextId++;
        }

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

}