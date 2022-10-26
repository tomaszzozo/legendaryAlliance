using UnityEngine;

public class SharedVariablesMb : MonoBehaviour
{
    public static void SetUsername(string username) { PlayerPrefs.SetString("username", username); }
    public static void DeleteUsername() { PlayerPrefs.DeleteKey("username"); }
    public static string GetUsername() { return PlayerPrefs.GetString("username"); }

    public static void SetRoomToJoin(string roomToJoinId) { PlayerPrefs.SetString("roomToJoinId", roomToJoinId); }
    public static string GetRoomToJoin() { return PlayerPrefs.GetString("roomToJoinId"); }
}

public static class SharedVariables
{
    public static object[] SharedData;

    public static void SetUsername(string username) { PlayerPrefs.SetString("username", username); }
    public static void DeleteUsername() { PlayerPrefs.DeleteKey("username"); }
    public static string GetUsername() { return PlayerPrefs.GetString("username"); }

    public static void SetRoomToJoin(string roomToJoinId) { PlayerPrefs.SetString("roomToJoinId", roomToJoinId); }
    public static string GetRoomToJoin() { return PlayerPrefs.GetString("roomToJoinId"); }
    
    public static void SetIsAdmin(bool isAdmin) { PlayerPrefs.SetInt("isAdmin", isAdmin ? 1:0); }
    public static bool GetIsAdmin() { return PlayerPrefs.GetInt("isAdmin") == 1; }
}
