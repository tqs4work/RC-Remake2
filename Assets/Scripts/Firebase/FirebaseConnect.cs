using System;
using System.Collections.Generic;
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
using UnityEngine.Tilemaps;
using FirebaseException = Firebase.Database.FirebaseException;

public class FirebaseConnect : MonoBehaviour
{
    //UNITY
    async void Start()
    {
        //await AddData();
        //await ReadData();
        //await DeleteData();
        await Task.Delay(1000);
        //await UpdateData();
    }

    async void Update()
    {        
        await Task.Delay(1000);
    }

    public async void OnSignUpButtonClick()
    {
        await SignUp();
    }
    public async void OnSignInButtonClick()
    {
        await SignIn();
    }





    //******************************** FIREBASE REALM CRAFT TOPDOWN 2D ********************************//
    //FIREBASE URL
    public static string firebaseUrl = "https://realm-craft-topdown-2d-default-rtdb.asia-southeast1.firebasedatabase.app/";

    //LINK JSON
    private static readonly HttpClient client = new HttpClient();
    public string jsonUrl = "https://raw.githubusercontent.com/NTH-VTC/OnlineDemoC-/main/simple_players.json";

    static async Task Main(string[] args)
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

    //FIREBASE WORKING

    ////PutAsync: Ghi đè toàn bộ dữ liệu
    ////PatchAsync: Cập nhật một phần dữ liệu
    ///PostAsync: Thêm dữ liệu mới với key tự động tạo

    //Add
    public static async Task AddData()
    {
        var firebase = new FirebaseClient(firebaseUrl);
        var Data = new
        {
            Message = "Console.ReadLine()",
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        await firebase.Child("Tasks").PatchAsync(Data);
    }


    //Read
    public static async Task ReadData()
    {
        var firebase = new FirebaseClient(firebaseUrl);
        var Data = await firebase.Child("Tasks").Child("Message").OnceSingleAsync<dynamic>(); //lấy 1 đối tượng
        var allData = await firebase.Child("Tasks").Child("Player0").OnceAsync<dynamic>(); //lấy nhiều đối tượng

        //OnceAsync<dynamic>(), phương thức trả về một tập hợp (collection) chứa các đối tượng trong nút "Tasks" nên phải xử lý Key Value để xuất

        Console.WriteLine($"{Data}");

        foreach (var item in allData)
            Console.WriteLine($"{item.Key} : {item.Object}"); //Object = Value
        
    }

    //Update
    public static async Task UpdateData()
    {
        var firebase = new FirebaseClient(firebaseUrl);        
        var updateData = new
        {
            Message = Console.ReadLine(),
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        await firebase.Child("Tasks").PutAsync(updateData);
    }



    //Delete
    public static async Task DeleteData()
    {
        var firebase = new FirebaseClient(firebaseUrl);
        await firebase.Child("Tasks").Child("Message").DeleteAsync(); //xóa Child Message chứ k xóa Tasks
        Console.WriteLine("Delete Successful");
    }


    //Sign Up & Sign In (Authentication)
    public static async Task SignUp()
    {
        string username = GameObject.Find("Canvas").gameObject.transform.Find("InputUsername").gameObject.GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("Canvas").gameObject.transform.Find("InputPassword").gameObject.GetComponent<TMP_InputField>().text;
        var firebase = new FirebaseClient(firebaseUrl);
        var accounts = await firebase.Child("Accounts").OnceAsync<Account>();
        if (accounts != null)
        {
            foreach (var acc in accounts)
            {
                if (acc.Object.Username == username)
                {
                    ShowMess("Account Existed");
                    ClearInput();
                    return;
                }
            }
        }
                
        var account = CreateAccount(username, password);

        await firebase.Child("Accounts").Child("Acc Created " + account.Timecreate).PutAsync(account);       
        ShowMess("Sign Up Successful");
        ClearInput();
    }
    public static async Task SignIn()
    {
        string username = GameObject.Find("Canvas").gameObject.transform.Find("InputUsername").gameObject.GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("Canvas").gameObject.transform.Find("InputPassword").gameObject.GetComponent<TMP_InputField>().text;
        var firebase = new FirebaseClient(firebaseUrl);
        var accounts = await firebase.Child("Accounts").OnceAsync<Account>();
        foreach (var acc in accounts)
        {
            if (acc.Object.Username == username && acc.Object.Password == password)
            {
                Console.WriteLine("Sign In Successful");
                ShowMess("Sign In Successful");
                return;
            }
        }        
        ShowMess("Username or Password Incorrect");
    }

    public static Account CreateAccount(string username, string password)
    {
        var account = new Account
        {
            Username = username,
            Password = password,
            Timecreate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
            player = new Player() // Tạo mới đối tượng Player
        };

        // Gán thông tin cho Player
        account.player.Name = "NewPlayer";
        account.player.Hp = 100;
        account.player.Mp = 50;
        account.player.Exp = 0;
        account.player.Lv = 1;
        account.player.Gold = 0;
        account.player.IsOnline = false;
        account.player.LastLogin = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        account.player.inventory = new Inventory
        {
            Items = new List<Item>()
        };

        // Tạo một item mới từ Item ScriptableObject đã tạo trong Unity
        Item starterSword = ScriptableObject.CreateInstance<Item>();
        starterSword.itemID = "item001";
        starterSword.itemName = "Starter Sword";
        starterSword.description = "A basic sword for new adventurers.";
        starterSword.quantity = 1;
        starterSword.price = 100;
        starterSword.level = 1;
        starterSword.atk = 10;
        starterSword.def = 0;
        starterSword.hp = 0;
        starterSword.mp = 0;
        starterSword.bonus = 0;

        // Thêm item vào inventory
        account.player.inventory.Items.Add(starterSword);

        // Xét điều kiện cho Property ID đã được gán trong Player
        return account;
    }

    public static void AddItem(Item item, Player player)
    {
        player.inventory.Items.Add(item);
    }    

    static void ShowMess(string mess)
    {
        GameObject t = GameObject.Find("Canvas").gameObject.transform.Find("Text").gameObject;
        if (t != null)
        {
            t.GetComponent<TextMeshProUGUI>().text = mess;
        }
    }

    static void ClearInput()
    {
        GameObject.Find("Canvas").gameObject.transform.Find("InputUsername").gameObject.GetComponent<TMP_InputField>().text = "";
        GameObject.Find("Canvas").gameObject.transform.Find("InputPassword").gameObject.GetComponent<TMP_InputField>().text = "";
    }
    //CLASS B? SUNG THEO YÊU C?U (CÁC BI?N ?ÚNG CHÍNH T? VS KEY TREN JSON)

    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Timecreate { get; set; }
        public Player player { get; set; }
    }
    public class Player
    {
        private static int _nextId = 1; // Biến tĩnh để theo dõi ID tiếp theo
        public string ID { get; set; }
        public string Name { get; set; }
        public int Hp { get; set; }
        public int Mp { get; set; }
        public int Exp { get; set; }
        public int Lv { get; set; }
        public int Gold { get; set; }
        public bool IsOnline { get; set; }       
        public string LastLogin { get; set; }

        public Inventory inventory { get; set; }

        // Constructor
        public Player()
        {
            ID = _nextId.ToString("D4"); // Gán ID với định dạng 4 ký tự
            _nextId++; // Tăng ID tiếp theo
        }
    }

    public class Inventory
    {
        public List<Item> Items { get; set; }
    }
    

}
