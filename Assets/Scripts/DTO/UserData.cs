using System;

namespace Game.DTO
{
    public struct UserData:IEquatable<UserData>
    {
        public string id;
        public string authKey;
        public string nickname;
        public string uniqueDeviceId;

        public static bool operator == (UserData user1, UserData user2)
        {
            return Equals(user1, user2);
        }

        public static bool operator !=(UserData user1, UserData user2)
        {
            return !(user1 == user2);
        }

        public bool Equals(UserData other)
        {
            return id == other.id && authKey == other.authKey && nickname == other.nickname && uniqueDeviceId == other.uniqueDeviceId;
        }

        public override bool Equals(object obj)
        {
            return obj is UserData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, authKey, nickname, uniqueDeviceId);
        }
    }
}