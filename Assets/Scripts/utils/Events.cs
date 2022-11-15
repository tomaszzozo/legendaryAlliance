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
    SendNotificationEvent
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
    public readonly string[] Usernames;
    public readonly string[] Statuses;

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
    public readonly string FieldName;
    public readonly int AvailableUnits;
    public readonly int AllUnits;
    public readonly string Owner;
    public readonly int Gold;

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
    public struct FieldUpdatedData
    {
        public string FieldName;
        public int AllUnits;
        public int AvailableUnits;
    }

    public readonly List<FieldUpdatedData> FieldsUpdatedData;
    public readonly string NewOwner;

    public AfterAttackUpdateFields(List<FieldUpdatedData> updatedData, string newOwner) : base(EventTypes.AfterAttackUpdateFields)
    {
        FieldsUpdatedData = updatedData;
        NewOwner = newOwner;
    }

    public object[] Serialize()
    {
        var array = new object [FieldsUpdatedData.Count*3 + 1];
        array[0] = NewOwner;
        for (int i = 1, d = 0; i < array.Length; i+=3, d++)
        {
            array[i] = FieldsUpdatedData[d].FieldName;
            array[i+1] = FieldsUpdatedData[d].AllUnits;
            array[i+2] = FieldsUpdatedData[d].AvailableUnits;
        }

        return array;
    }

    public static AfterAttackUpdateFields Deserialize(object[] content)
    {
        var newOwner = content[0] as string;
        var updatedData = new List<FieldUpdatedData>();
        
        for (var i = 1; i < content.Length; i+=3)
        {
            updatedData.Add(new FieldUpdatedData{FieldName = content[i] as string, AllUnits = (int)content[i+1], AvailableUnits = (int)content[i+2]});
        }

        return new AfterAttackUpdateFields(updatedData, newOwner);
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
