using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Collections;

#region Messages
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
#endregion
public class FirebaseTest : MonoBehaviour //maybe turn this into a singleton
{
    public static FirebaseTest instance = null;
    #region Constants
    private const string GAMELOBBY = "GameLobby";
    private const string GAMESESSION = "Gamesession";
    private const string PLAYER1 = "Player1";
    private const string PLAYER2 = "Player2";
    #endregion
    #region Variables
    FirebaseDatabase db;
    FirebaseAuth auth;
    FirebaseApp app;
    bool gameWasFound = false;
    bool shouldListen = false;
    private static string myPath = "";
    public string mySpot = "";
    #endregion
    public static event System.Action<ProjectileInfo> OnProjectile;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
            Destroy(gameObject);
    }
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseDatabase.DefaultInstance;
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            TryJoinGame();
            
        }

    }
    private void SaveToFirebase(ProjectileInfo sendToFireBase) // sends projectileInfo on firebase which will be used to send the projectile on other players screen
    {
        string jsonString = JsonUtility.ToJson(sendToFireBase);
        //puts the json data in the "users/userId" part of the database.
        db.GetReference(GAMESESSION).Child(myPath).Child(mySpot).SetRawJsonValueAsync(jsonString).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.Log(task.Exception);
        });
    }

    #region SetUpGames
    public void TryJoinGame() // This should be done when player presses start game
    {
        db?.GetReference(GAMELOBBY).GetValueAsync().ContinueWithOnMainThread(task => //here we have a way of going thorugh each game session just replace the 1 with an int
        {
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }

            DataSnapshot snapshot = task.Result;

            if (snapshot.Value == null) // if there is no gamelobby up and running
            {
                CreateGame();
                TryJoinGame();
                return;
            }

            string bigJsonString = snapshot.GetRawJsonValue();

            string trimmedJsonString = bigJsonString.Replace("Player1", "").Replace("Player2", "");

            char[] delimitercharacters = { ':', ',' };
            string[] gameSessionAndValues = trimmedJsonString.Split(delimitercharacters);

            List<string> keys = new List<string>();


            foreach (string key in gameSessionAndValues)
            {
                string valueOrSession = key.Trim(new char[] { '{', '}', '"' }).Replace("angle", "").Replace("speed", "Taken");

                float check = 0;
                bool isFloat = float.TryParse(key.ToString(), out check);
                if (valueOrSession.Length > 3 && isFloat == false)
                {
                    Debug.Log(valueOrSession);
                    keys.Add(valueOrSession);
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
                    mySpot = PLAYER1;
                    myPath = gameSessions[i].path;

                    JoinGame(gameSessions[i].path, PLAYER1);//put function for joinin a game here

                    ListenForPlayerJoin(gameSessions[i].path);
                    return;
                }
                else if (gameSessions[i].spot2 == "none")
                {
                    // i am player2
                    gameWasFound = true;
                    Debug.Log("Joining " + gameSessions[i].path + " as player 2");
                    mySpot = PLAYER2;
                    myPath = gameSessions[i].path;
                    JoinGame(gameSessions[i].path, PLAYER2);

                    ListenForPlayerThrow(myPath, PLAYER1);
                    return;
                }
            }
        });
    }
    private void JoinGame(string path, string spot)
    { //change this
        db.GetReference(GAMELOBBY).Child(path).Child(spot).SetValueAsync("Taken").ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
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
        Game session = new();

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
    #endregion

    private void UpdateGameState(object sender, ValueChangedEventArgs args) //this gets called twice?
    {
        if(args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
       
        try {OnProjectile.Invoke(JsonUtility.FromJson<ProjectileInfo>(args.Snapshot.GetRawJsonValue()));}

        catch
        {
            Debug.Log("Shouldnt listen");
        }
        return;
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

            db.GetReference(GAMELOBBY).Child(myPath).Child(PLAYER2).ValueChanged -= StartGame;

            db.RootReference.Child(GAMESESSION).Child(myPath).SetRawJsonValueAsync(jsom).ContinueWithOnMainThread(task => // create a new session under gamesession
            {
                if (task.Exception != null)
                {
                    Debug.LogError(task.Exception);
                }
            });

            ListenForPlayerThrow(myPath, PLAYER2);
            
            StartCoroutine(DestroyLobby());

        } 
    }
    private IEnumerator DestroyLobby()
    {
        yield return new WaitForSeconds(1f);
        db.GetReference(GAMELOBBY).Child(myPath).RemoveValueAsync().ContinueWithOnMainThread(task => //remove this session from lobby
        {
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }
        });
    }
    #region listeners 
    private void ListenForPlayerThrow(string path, string spot)
    {
        db.GetReference(GAMESESSION).Child(path).Child(spot).ValueChanged += UpdateGameState;
    }
    private void ListenForPlayerJoin(string path)
    {
        db.GetReference(GAMELOBBY).Child(myPath).Child(PLAYER2).ValueChanged += StartGame; //listen to see if a player joins
    }
    #endregion
}

