using UnityEngine;

using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseTest : MonoBehaviour
{
    private const string GAMELOBBY = "GameLobby";
    private const string GAMESESSION = "Gamesession";
    FirebaseDatabase db;
    FirebaseAuth auth;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseDatabase.DefaultInstance;
            ListenForPlayerThrow();
        });
        
    }
    private void OnEnable()
    {
        ThrowProjectile.SendToFireBase += SaveToFirebase;
    }
    private void OnDisable()
    {
        ThrowProjectile.SendToFireBase -= SaveToFirebase;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            AnonymousSignIn();

        if (Input.GetKeyDown(KeyCode.D))
            DataTest(auth.CurrentUser.UserId, Random.Range(0, 100).ToString());

        if (Input.GetKeyDown(KeyCode.S))
        {
            TryCreateGame();
            
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            GetInfo();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateGame();
        }
    }

    private void AnonymousSignIn()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogWarning(task.Exception);
            }
            else
            {
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            }
        });
    }

    private void DataTest(string userID, string data)
    {
        Debug.Log("Trying to write data...");
        var db = FirebaseDatabase.DefaultInstance;
        db.RootReference.Child("users").Child(userID).SetValueAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);
            else
                Debug.Log("DataTestWrite: Complete");
        });
    }
    private void SaveToFirebase(ProjectileInfo sendToFireBase) // sends projectileInfo on firebase which will be used to send the projectile on other players screen
    {
        var db = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        string jsonString = JsonUtility.ToJson(sendToFireBase);
        //puts the json data in the "users/userId" part of the database.
        db.Child("users").Child(userId).Child("Projectile").Child("Info").SetRawJsonValueAsync(jsonString).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.Log(task.Exception);
        });
    }
    private void GetInfo() // this should read the data of the other player throw and put it into a method //this writes to the database
    {
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        db?.RootReference.Child("users").Child(userId).Child("Projectile").Child("Info").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogWarning(task.Exception);
            }
            else
            {
                DataSnapshot snap = task.Result;
                MySnap(snap.GetRawJsonValue());
                Debug.Log(snap);
            }
        });
    }
    private void TryCreateGame() // This should be done when player presses start game
    {

        db.GetReference(GAMELOBBY).LimitToFirst(1).GetValueAsync().ContinueWithOnMainThread(task => //here we have a way of going thorugh each game session just replace the 1 with an int
        {
            if(task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }
            else 
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot.GetRawJsonValue());
                string game = snapshot.GetRawJsonValue();
                string bas = game.Remove(0,24);
                //bas = bas.Remove(bas.Length, 1);
                string res = bas.Remove(bas.Length-1, 1);
                Debug.Log(res);
                AimMessage ai = JsonUtility.FromJson<AimMessage>(res);
                Debug.Log(ai.Gamesession);
                
                //string strValue = (string)snapshot.Value; // here is the value of Gamesession
                //bool f = System.Convert.ToBoolean(strValue);
                
                //if(f == false) // ae there is no value at GAMESESSION or the value of gamesession is false
                //{
                //    //create a game
                //}
                //else
                //{
                //    //join that game and close the opening
                //}
            }
        });
    }
    private void CreateGame()
    {
        db?.RootReference.Child(GAMELOBBY).Push().Child(GAMESESSION).SetValueAsync("true").ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogWarning(task.Exception);
            }
        });
    }
    private void MySnap(string jsonString)
    {
        ProjectileInfo obj = JsonUtility.FromJson<ProjectileInfo>(jsonString);
        Debug.Log(obj.angle + " " + obj.speed);
    }
    private void ListenForPlayerThrow()
    {
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        db.GetReference("users").Child(userId).Child("Projectile").Child("Info").ValueChanged += UpdateGameState;
    }
    private void UpdateGameState(object sender, ValueChangedEventArgs args)
    {
        if(args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Debug.Log("Value has changed: " + args.Snapshot.GetRawJsonValue());
    }
}

