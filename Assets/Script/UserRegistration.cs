using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    public string username;
    public string email;
    public string password;
}

[System.Serializable]
public class UserDataList
{
    public List<UserData> users = new List<UserData>();
}

public class UserRegistration : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button submitButton;

    private string filePath;

    void Start()
    {
        // Get the path to the 'Assets/Script' directory not absolute so works all the time
        string scriptDirectory = Path.Combine(Application.dataPath, "Script");

        // Set the file path for 'users.json' inside the 'Script' directory
        filePath = Path.Combine(scriptDirectory, "users.json");

        // Check if the submitButton is assigned and set up the listener
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmit);
        else
            Debug.LogError("Submit button is not assigned!");
    }

    void OnSubmit()
    {
        // Create the new user data from input fields
        UserData newUser = new UserData
        {
            username = usernameInput.text,
            email = emailInput.text,
            password = passwordInput.text
        };

        // Load the existing users data from the file if it exists
        UserDataList userList;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            userList = JsonUtility.FromJson<UserDataList>(json);
        }
        else
        {
            userList = new UserDataList();
        }

        // Check if the username or email is already taken
        foreach (UserData user in userList.users)
        {
            if (user.username == newUser.username)
            {
                Debug.LogError("Username is already taken!");
                return; // Exit the method without saving
            }
            if (user.email == newUser.email)
            {
                Debug.LogError("Email is already registered!");
                return; // Exit the method without saving
            }
        }

        // Add the new user to the list
        userList.users.Add(newUser);

        // Convert the updated user list back to JSON
        string newJson = JsonUtility.ToJson(userList, true);

        // Save the updated list to the file
        File.WriteAllText(filePath, newJson);

        // Log the success
        Debug.Log("User saved to: " + filePath);
    }
}
