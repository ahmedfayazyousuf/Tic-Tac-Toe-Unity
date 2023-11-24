using TMPro;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DatabaseManager : MonoBehaviour
{
    public TMP_InputField Name;
    public TMP_InputField Email;
    private string userID;
    private DatabaseReference dbreference;
    public static DatabaseManager instance;

    void Start()
    {
        //userID = SystemInfo.deviceUniqueIdentifier; //this command shall assign a new unique id to every new user
        InitializeUser();
    }

    public void InitializeUser()
    {
        userID = DateTime.Now.ToString("yyMMddHHmmssff");
        dbreference = FirebaseDatabase.DefaultInstance.RootReference;
        if (instance == null)
            instance = this;
    }
    public void CreateUser()
    {
        GameController newUser = new GameController(Name.text, Email.text);
        string json = JsonUtility.ToJson(newUser);
        Debug.Log(userID);
        dbreference.Child("users").Child(userID).SetRawJsonValueAsync(json);
    }
}