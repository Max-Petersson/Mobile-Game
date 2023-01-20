using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Collections;

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
    bool gameWasFound = false;
    private string myPath = "";
    public static string mySpot = "";
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseDatabase.DefaultInstance;
            app = FirebaseApp.Create();
            FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
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
            

        if (Input.GetKeyDown(KeyCode.D))
        {

        }
           

        if (Input.GetKeyDown(KeyCode.S))
        {
            TryJoinGame();
            
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            db.GetReference(GAMESESSION).SetValueAsync("Empty").ContinueWithOnMainThread(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogError(task.Exception);
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateGame();
        }
    }
    private void SaveToFirebase(ProjectileInfo sendToFireBase) // sends projectileInfo on firebase which will be used to send the projectile on other players screen
    {
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        string jsonString = JsonUtility.ToJson(sendToFireBase);
        //puts the json data in the "users/userId" part of the database.
        db.GetReference(GAMESESSION).Child(myPath).Child(mySpot).SetRawJsonValueAsync(jsonString).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.Log(task.Exception);
        });
       
    }

    private void TryJoinGame() // This should be done when player presses start game
    {
        db?.GetReference(GAMELOBBY).GetValueAsync().ContinueWithOnMainThread(task => //here we have a way of going thorugh each game session just replace the 1 with an int
        {
            if(task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }

            DataSnapshot snapshot = task.Result;
           
            string blo = snapshot.GetRawJsonValue();
           
            string theObject = blo.Replace("Player1", "").Replace("Player2", "");

            char[] delimitercharacters = { ':', ',' };
            string[] keysAndValues = theObject.Split(delimitercharacters);

            List<string> keys = new List<string>();
           
           
            foreach (string key in keysAndValues)
            {
                string blue = key.Trim(new char[] { '{', '}', '"' }).Replace("angle", "").Replace("speed", "Taken");
                
                float check = 0;
                bool isFloat = float.TryParse(key.ToString(), out check);
                if (blue.Length > 3 && isFloat == false)
                {
                    Debug.Log(blue);
                    keys.Add(blue);
                }
            }

            string spot1 = "";
            string spot2 = "";
            string path = "";

            Debug.Log(keys.Count);

            List<GameSessions> gameSessions = new List<GameSessions>();
            for (int i = 0; i < keys.Count; i++)
            {
                if (i == 0 || i % 3 == 0)
                {
                    path = keys[i];
                    //its a key
                }
                else if (i == 1 || i % 3 == 1)
                {
                    spot1 = keys[i];
                    //spot 2
                }
                else if (i == 2 || i % 3 == 2)
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
                    gameWasFound = true;
                    Debug.Log("Joining " + gameSessions[i].path + " as player 1");
                    //i am player 1
                    mySpot = "Player1";
                    myPath = gameSessions[i].path;

                    JoinGame(gameSessions[i].path, "Player1");//put function for joinin a game here

                    ListenForPlayerJoin(gameSessions[i].path);
                    return;
                }
                else if (gameSessions[i].spot2 == "none")
                {
                    // i am player2
                    gameWasFound = true;
                    Debug.Log("Joining " + gameSessions[i].path + " as player 2");
                    mySpot = "Player2";
                    myPath = gameSessions[i].path;
                    JoinGame(gameSessions[i].path, "Player2");
                    return;
                }
            }
            

        });
        if(gameWasFound == false)
        {
            CreateGame();
            //gameWasFound = true;
        }
       gameWasFound = false;
    }
    private void JoinGame(string path, string spot)
    { //change this
        db.GetReference(GAMELOBBY).Child(path).Child(spot).SetValueAsync("Taken").ContinueWithOnMainThread(task => 
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
        Game session = new ();
        
        string jsom = JsonUtility.ToJson(session);
        //change
        db?.GetReference(GAMELOBBY).Push().SetRawJsonValueAsync(jsom).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogWarning(task.Exception);
            }
        });
    }
    private void ListenForPlayerThrow(string path, string spot)
    {
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId; // change this
        db.GetReference(GAMESESSION).Child(path).Child(spot).ValueChanged += UpdateGameState;
    }
    private void UpdateGameState(object sender, ValueChangedEventArgs args)
    {
        if(args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        ProjectileInfo info = JsonUtility.FromJson<ProjectileInfo>(args.Snapshot.GetRawJsonValue());
        Debug.Log(info.angle + " " + info.speed);
        return ;
        
    }
    private void ListenForPlayerJoin(string path)
    {
        //change this
        db.GetReference(GAMELOBBY).Child(myPath).Child("Player2").ValueChanged += StartGame; //listen to see if a player joins
    }
    private void StartGame(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log(args.Snapshot.GetRawJsonValue());

        if (args.Snapshot.GetRawJsonValue()?.Trim(new char[] {'"'}) == "Taken")
        {
            Game session = new();
            string jsom = JsonUtility.ToJson(session);

            db.GetReference(GAMELOBBY).Child(myPath).Child("Player2").ValueChanged -= StartGame;

            db.RootReference.Child(GAMESESSION).Child(myPath).SetRawJsonValueAsync(jsom).ContinueWithOnMainThread(task => // create a new session under gamesession
            {
                if (task.Exception != null)
                {
                    Debug.LogError(task.Exception);
                }
            });

            StartCoroutine(DestroyLobby());
           // Debug.Log("Game Will Start " + args.Snapshot.GetRawJsonValue());
             //change this
        } 
    }
    private IEnumerator DestroyLobby()
    {
        yield return new WaitForSeconds(1f);
        db.GetReference(GAMELOBBY).Child(myPath).RemoveValueAsync().ContinueWithOnMainThread(task => //remove this session from lobby
        {
            db.GetReference(GAMELOBBY).Child(myPath).Child("Player2").ValueChanged -= StartGame;
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }
        });
    }
}

