namespace Events
{
    public enum EventTypes
    {
        UpdateRoomUI = 1,
        ClientClickedReady,
        GoToGameScene,
        RoomAlreadyInGameSignal,
        NextTurn
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
        public readonly int roomId;
        public readonly string[] usernames;
        public readonly string[] statuses;

        public UpdateRoomUi(int roomId, string[] usernames, string[] statuses) : base(EventTypes.UpdateRoomUI)
        {
            this.roomId = roomId;
            this.usernames = usernames;
            this.statuses = statuses;
        }

        public object[] Serialize()
        {
            return new object[] { roomId, usernames, statuses };
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
        public readonly string nickName;

        public ClientClickedReady(string nickName) : base(EventTypes.ClientClickedReady)
        {
            this.nickName = nickName;
        }

        public object[] Serialize()
        {
            return new object[] { nickName };
        }

        public static ClientClickedReady Deserialize(object[] content)
        {
            return new ClientClickedReady(
                (string)content[0]);
        }
    }
}