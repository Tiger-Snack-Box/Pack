using UnityEngine;
using UnityEngine.Networking;

public class CognitoHostedUI : MonoBehaviour
{
    void Start()
    {
        Application.deepLinkActivated += OnDeepLinkActivated;

        if (!string.IsNullOrEmpty(Application.absoluteURL))
            OnDeepLinkActivated(Application.absoluteURL);

        OnLogin();
    }

    public void OnLogin()
    {
        string domain = "https://us-east-2ykkrod1ta.auth.us-east-2.amazoncognito.com";
        string clientId = "3oo0i5cqng88m1tcouuo2uk1ls";
        string redirectUri = "pack";
        string responseType = "token"; 

        string authUrl = $"{domain}/login/continue?client_id={clientId}&redirect_uri={redirectUri}%3A%2F%2Fcallback&response_type={responseType}&scope=email+openid+phone";

        Application.OpenURL(authUrl);
    }

    void OnDeepLinkActivated(string url)
    {
        Debug.Log("Deep link: " + url);

        if (url.Contains("id_token="))
        {
            string idToken = ExtractToken(url, "id_token");
            string accessToken = ExtractToken(url, "access_token");
            string refreshToken = ExtractToken(url, "refresh_token");

            PlayerPrefs.SetString("id_token", idToken);
            PlayerPrefs.SetString("access_token", accessToken);
            if (!string.IsNullOrEmpty(refreshToken))
                PlayerPrefs.SetString("refresh_token", refreshToken);

            PlayerPrefs.Save();
            Debug.Log("Tokens saved");
        }
    }

    string ExtractToken(string url, string key)
    {
        int keyIndex = url.IndexOf($"{key}=");
        if (keyIndex == -1) return null;

        int start = keyIndex + key.Length + 1;
        int end = url.IndexOf('&', start);
        if (end == -1) end = url.Length;

        return UnityWebRequest.UnEscapeURL(url.Substring(start, end - start));
    }
}
