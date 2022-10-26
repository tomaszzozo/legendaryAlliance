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

    public static void SetCameraPosition(Vector3 position)
    {
        PlayerPrefs.SetFloat("cameraPositionX", position.x);
        PlayerPrefs.SetFloat("cameraPositionY", position.y);
    }

    public static Vector3 GetCameraPosition()
    {
        return new Vector3(PlayerPrefs.GetFloat("cameraPositionX"), PlayerPrefs.GetFloat("cameraPositionY"), -10);
    }
    
    public static void SetCameraSize(float size) { PlayerPrefs.SetFloat("cameraSize", size); }
    public static float GetCameraSize() { return PlayerPrefs.GetFloat("cameraSize"); }
}
