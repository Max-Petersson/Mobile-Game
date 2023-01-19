using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;

public class GameSessions
{
    public string path = "";
    public string spot1 = "";
    public string spot2 = "";
    public GameSessions(string path, string spot1, string spot2)
    {
        this.path = path;
        this.spot1 = spot1;
        this.spot2 = spot2;
    }
    
}
public class PlayerProp
{
    string id;
    string name;

    public PlayerProp(string id, string name)
    {
        this.id = id;
        this.name = name;
    }
}
public class Game
{
    public string Player1 = "none";
    public string Player2 = "none";
    
}
public class FirebaseTest : MonoBehaviour
{
    private const string GAMELOBBY = "GameLobby";
    private const string GAMESESSION = "Gamesession";
    FirebaseDatabase db;
    FirebaseAuth auth;
    FirebaseApp app;
    DatabaseReference dbRef;
    private string myPath = "";
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseDatabase.DefaultInstance;
            app = FirebaseApp.Create();
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
        ListenForPlayerThrow();
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
        db?.GetReference(GAMELOBBY).Child(GAMESESSION).GetValueAsync().ContinueWithOnMainThread(task => //here we have a way of going thorugh each game session just replace the 1 with an int
        {
            if(task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }
            else 
            {
                DataSnapshot snapshot = task.Result;

                string blo = snapshot.GetRawJsonValue();

                string theObject = blo.Replace("Player1", "").Replace("Player2", "");
               
                char[] delimitercharacters = {':', ','};
                string[] keysAndValues = theObject.Split(delimitercharacters);

                List<string> keys = new List<string>();

                foreach (string key in keysAndValues)
                {
                    string blue = key.Trim(new char[] { '{', '}', '"' });
                    if(blue.Length > 3)
                    {
                        keys.Add(blue);
                    }
                }

                string spot1 = "";
                string spot2 = "";
                string path = "";

                Debug.Log(keys.Count);

                List <GameSessions> gameSessions = new List <GameSessions>();
                for(int i = 0; i < keys.Count; i++)
                {
                    if(i == 0 || i % 3 == 0)
                    {
                        path = keys[i];
                        //its a key
                    }
                    else if(i == 1 || i % 3 == 1)
                    {
                        spot1 = keys[i];
                        //spot 2
                    }
                    else if(i == 2 || i % 3 == 2)
                    {
                        spot2 = keys[i];
                        GameSessions gamesesh = new(path, spot1, spot2);
                        gameSessions.Add(gamesesh);
                        
                        //spot 3 try to join
                    }
                }
                
                for (int i = 0; i < gameSessions.Count; i++)
                {
                    
                    if (gameSessions[i].spot1 == "none")
                    {
                        Debug.Log("Joining " + gameSessions[i].path + " as player 1");
                        JoinGame(gameSessions[i].path,"Player1");//put function for joinin a game here
                        myPath = path;
                        ListenForPlayerJoin(gameSessions[i].path);
                        return;
                    }
                    else if(gameSessions[i].spot2 == "none")
                    {
                        Debug.Log("Joining " + gameSessions[i].path + " as player 2");
                        JoinGame(gameSessions[i].path, "Player2"); 
                        return;
                    }
                }
            }
        });
    }
    private void JoinGame(string path, string spot)
    {
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        ProjectileInfo pi = new(0, 0);
        string js = JsonUtility.ToJson(pi);
       
        db.GetReference(GAMELOBBY).Child(GAMESESSION).Child(path).Child(spot).SetValueAsync("Taken").ContinueWithOnMainThread(task => 
        { 
            if(task.Exception != null)
            {
                Debug.LogWarning(task.Exception);
            }
            else
            {
                
            }
        });
       
    }
    private void CreateGame()
    {
        Game dude = new ();
        
        string jsom = JsonUtility.ToJson(dude);

        db?.GetReference(GAMELOBBY).Child(GAMESESSION).Push().SetRawJsonValueAsync(jsom).ContinueWithOnMainThread(task =>
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
    private void ListenForPlayerJoin(string path)
    {
       
        db.GetReference(GAMELOBBY).Child(GAMESESSION).Child(path).Child("Player2").ValueChanged += StartGame;
       
    }
    private void StartGame(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log(args.Snapshot.GetRawJsonValue());
        if(args.Snapshot.GetRawJsonValue() == "none")
        {
            Debug.Log("Game Will Start " + args.Snapshot.GetRawJsonValue());
            db.GetReference(GAMELOBBY).Child(GAMESESSION).Child(myPath).Child("Player2").ValueChanged -= StartGame;
        }
           
    }
}

