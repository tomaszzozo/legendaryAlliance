﻿namespace Events
{
    public enum EventTypes
    {
        UPDATE_ROOM_UI = 1,
        CLIENT_CLICKED_READY,
    }

    public class Event
    {
        public readonly EventTypes eventType;

        public Event(EventTypes eventType)
        {
            this.eventType = eventType;
        }

        public byte GetEventType()
        {
            return (byte)eventType;
        }
    }

    public class UpdateRoomUi : Event
    {
        public readonly int roomId;
        public readonly string[] usernames;
        public readonly string[] statuses;

        public UpdateRoomUi(int roomId, string[] usernames, string[] statuses) : base(EventTypes.UPDATE_ROOM_UI)
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

        public ClientClickedReady(string nickName) : base(EventTypes.CLIENT_CLICKED_READY)
        {
            this.nickName = nickName;
        }

        public object[] Serialize()
        {
            return new object[] { nickName};
        }

        public static ClientClickedReady Deserialize(object[] content)
        {
            return new ClientClickedReady(
                (string)content[0]);
        }
    }
}