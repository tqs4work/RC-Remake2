using UnityEngine;
using System.Threading.Tasks;

public class PlayerRuntime : MonoBehaviour
{
    public static PlayerRuntime Instance;

    public Player Player;

    // ?? KEY ACCOUNT HI?N T?I
    public string AccountKey;

    private FirebaseService _firebase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Player = new Player();

            _firebase = new FirebaseService(
                "https://realm-craft-topdown-2d-default-rtdb.asia-southeast1.firebasedatabase.app/");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void OnApplicationQuit()
    {
        await SavePlayer();
    }

    private async void OnDestroy()
    {
        if (Instance == this)
            await SavePlayer();
    }

    public async Task SavePlayer()
    {
        if (Player == null) return;
        if (string.IsNullOrEmpty(AccountKey)) return;

        PlayerData data = PlayerDataConverter.ToData(Player);

        // ? TRUY?N ?ÚNG ACCOUNT KEY
        await _firebase.SavePlayer(AccountKey, data);
    }
}
