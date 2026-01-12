using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using UnityEngine;

public class FirebaseService
{
    private FirebaseClient _client;

    // ?? key account hi?n t?i
    public string CurrentAccountKey { get; private set; }

    public FirebaseService(string url)
    {
        _client = new FirebaseClient(url);
    }
    public void SetCurrentAccount(string key)
    {
        CurrentAccountKey = key;
    }

    public async Task<List<AccountData>> GetAccounts()
    {
        return (await _client.Child("Accounts").OnceAsync<AccountData>())
            .Select(a => a.Object).ToList();
    }

    public async Task CreateAccount(AccountData account)
    {
        await _client.Child("Accounts")
            .Child($"Acc_{account.Timecreate}")
            .PutAsync(account);
    }

    public async Task SavePlayer(string accountKey, PlayerData player)
    {
        if (string.IsNullOrEmpty(accountKey))
        {
            Debug.LogError("SavePlayer: accountKey is null");
            return;
        }

        await _client
            .Child("Accounts")
            .Child(accountKey)
            .Child("Player")
            .PutAsync(player);
    }

}

