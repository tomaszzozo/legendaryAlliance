using UnityEngine;

public class GlobalVariablesMB : MonoBehaviour
{
    public static void SetUsername(string username) { PlayerPrefs.SetString("username", username); }
    public static void DeleteUsername() { PlayerPrefs.DeleteKey("username"); }
    public static string GetUsername() { return PlayerPrefs.GetString("username"); }

    public static void SetRoomToJoin(string roomToJoinId) { PlayerPrefs.SetString("roomToJoinId", roomToJoinId); }
    public static string GetRoomToJoin() { return PlayerPrefs.GetString("roomToJoinId"); }
}

public static class GlobalVariables
{
    public static object[] SharedData;

    public static void SetUsername(string username) { PlayerPrefs.SetString("username", username); }
    public static void DeleteUsername() { PlayerPrefs.DeleteKey("username"); }
    public static string GetUsername() { return PlayerPrefs.GetString("username"); }

    public static void SetRoomToJoin(string roomToJoinId) { PlayerPrefs.SetString("roomToJoinId", roomToJoinId); }
    public static string GetRoomToJoin() { return PlayerPrefs.GetString("roomToJoinId"); }
}
