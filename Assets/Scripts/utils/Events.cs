using System.Collections.Generic;

public enum EventTypes
{
    UpdateRoomUI = 1,
    ClientClickedReady,
    GoToGameScene,
    RoomAlreadyInGameSignal,
    NextTurn,
    OnlineSelectedFieldChange,
    OnlineDeselectField,
    CapitalSelected,
    RequestRoomData,
    BuyUnits,
    AfterAttackUpdateFields,
    SomeoneWon,
    SendNotificationEvent,
    PlaySound,
    ObjectChanged,
    SellEverything,
}

public class Event
{
    private readonly EventTypes _eventType;

    protected Event(EventTypes eventType)
    {
        _eventType = eventType;
    }

    public byte GetEventType()
    {
        return (byte)_eventType;
    }
}

public class UpdateRoomUi : Event
{
    public readonly int RoomId;
    public readonly string[] Statuses;
    public readonly string[] Usernames;

    public UpdateRoomUi(int roomId, string[] usernames, string[] statuses) : base(EventTypes.UpdateRoomUI)
    {
        RoomId = roomId;
        Usernames = usernames;
        Statuses = statuses;
    }

    public object[] Serialize()
    {
        return new object[] { RoomId, Usernames, Statuses };
    }

    public static UpdateRoomUi Deserialize(object[] content)
    {
        return new UpdateRoomUi(
            (int)content[0],
            (string[])content[1],
            (string[])content[2]);
    }
}

public class ClientClickedReady : Event
{
    public readonly string NickName;

    public ClientClickedReady(string nickName) : base(EventTypes.ClientClickedReady)
    {
        NickName = nickName;
    }

    public object[] Serialize()
    {
        return new object[] { NickName };
    }

    public static ClientClickedReady Deserialize(object[] content)
    {
        return new ClientClickedReady(
            (string)content[0]);
    }
}

public class OnlineSelectedFieldChange : Event
{
    public readonly string FieldName;

    public OnlineSelectedFieldChange(string fieldName) : base(EventTypes.OnlineSelectedFieldChange)
    {
        FieldName = fieldName;
    }

    public object[] Serialize()
    {
        return new object[] { FieldName };
    }

    public static OnlineSelectedFieldChange Deserialize(object[] content)
    {
        return new OnlineSelectedFieldChange((string)content[0]);
    }
}

public class CapitalSelected : Event
{
    public readonly string FieldName;
    public readonly string Owner;

    public CapitalSelected(string fieldName, string owner) : base(EventTypes.CapitalSelected)
    {
        FieldName = fieldName;
        Owner = owner;
    }

    public object[] Serialize()
    {
        return new object[] { FieldName, Owner };
    }

    public static CapitalSelected Deserialize(object[] content)
    {
        return new CapitalSelected((string)content[0], (string)content[1]);
    }
}

public class BuyUnits : Event
{
    public readonly int AllUnits;
    public readonly int AvailableUnits;
    public readonly string FieldName;
    public readonly int Gold;
    public readonly string Owner;

    public BuyUnits(string fieldName, int availableUnits, int allUnits, string owner, int gold) : base(
        EventTypes.BuyUnits)
    {
        FieldName = fieldName;
        AvailableUnits = availableUnits;
        AllUnits = allUnits;
        Owner = owner;
        Gold = gold;
    }

    public object[] Serialize()
    {
        return new object[] { FieldName, AvailableUnits, AllUnits, Owner, Gold };
    }

    public static BuyUnits Deserialize(object[] content)
    {
        return new BuyUnits((string)content[0], (int)content[1], (int)content[2], (string)content[3],
            (int)content[4]);
    }
}

public class AfterAttackUpdateFields : Event
{
    /// <summary>
    ///     First field is the one under display, other fields are neighbours to that field
    /// </summary>
    public readonly List<FieldUpdatedData> FieldsUpdatedData;

    public AfterAttackUpdateFields(List<FieldUpdatedData> updatedData) : base(EventTypes.AfterAttackUpdateFields)
    {
        FieldsUpdatedData = updatedData;
    }

    public object[] Serialize()
    {
        var array = new object [FieldsUpdatedData.Count * 4];
        for (int i = 0, d = 0; i < array.Length; i += 4, d++)
        {
            array[i] = FieldsUpdatedData[d].FieldName;
            array[i + 1] = FieldsUpdatedData[d].AllUnits;
            array[i + 2] = FieldsUpdatedData[d].AvailableUnits;
            array[i + 3] = FieldsUpdatedData[d].NewOwner;
        }

        return array;
    }

    public static AfterAttackUpdateFields Deserialize(object[] content)
    {
        var updatedData = new List<FieldUpdatedData>();

        for (var i = 0; i < content.Length; i += 4)
            updatedData.Add(new FieldUpdatedData
            {
                FieldName = content[i] as string, AllUnits = (int)content[i + 1], AvailableUnits = (int)content[i + 2],
                NewOwner = content[i + 3] as string
            });

        return new AfterAttackUpdateFields(updatedData);
    }

    public struct FieldUpdatedData
    {
        public string FieldName;
        public int AllUnits;
        public int AvailableUnits;
        public string NewOwner;
    }
}

public class SomeoneWon : Event
{
    public readonly string WinnerNickName;

    public SomeoneWon(string winner) : base(EventTypes.SomeoneWon)
    {
        WinnerNickName = winner;
    }

    public object[] Serialize()
    {
        return new object[] { WinnerNickName };
    }

    public static SomeoneWon Deserialize(object[] content)
    {
        return new SomeoneWon(content[0] as string);
    }
}

public class SendNotificationEvent : Event
{
    public readonly string Message;

    public SendNotificationEvent(string message) : base(EventTypes.SendNotificationEvent)
    {
        Message = message;
    }

    public object[] Serialize()
    {
        return new object[] { Message };
    }

    public static SendNotificationEvent Deserialize(object[] content)
    {
        return new SendNotificationEvent(content[0] as string);
    }
}

public class PlaySound : Event
{
    public readonly string SoundName;

    public PlaySound(string soundName) : base(EventTypes.PlaySound)
    {
        SoundName = soundName;
    }

    public object[] Serialize()
    {
        return new object[] { SoundName };
    }

    public static PlaySound Deserialize(object[] content)
    {
        return new PlaySound(content[0] as string);
    }
}

public class ObjectChanged : Event
{
    public enum ObjectType
    {
        Trenches,
        Lab
    }

    public readonly string FieldName;
    public readonly ObjectType ObjectName;
    public readonly bool Sold;

    public ObjectChanged(string fieldName, ObjectType objectName, bool sold = false) : base(EventTypes.ObjectChanged)
    {
        FieldName = fieldName;
        ObjectName = objectName;
        Sold = sold;
    }

    public object[] Serialize()
    {
        return new object[] { FieldName, (int)ObjectName, Sold};
    }

    public static ObjectChanged Deserialize(object[] content)
    {
        return new ObjectChanged(content[0] as string, (ObjectType)content[1], (bool)content[2]);
    }
}

public class SellEverything : Event
{
    public readonly string PlayerName;

    public SellEverything(string playerName) : base(EventTypes.SellEverything)
    {
        PlayerName = playerName;
    }
    
    public object[] Serialize()
    {
        return new object[] { PlayerName };
    }

    public static SellEverything Deserialize(object[] content)
    {
        return new SellEverything(content[0] as string);
    }
}
