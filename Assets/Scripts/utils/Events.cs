using fields;

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
    BuyUnits
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