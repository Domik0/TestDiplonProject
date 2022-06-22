using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarterAssets;
using Unity.Collections;
using Unity.Netcode;

namespace Assets.Scripts
{
    public struct PlayerStat : INetworkSerializable, IEquatable<PlayerStat>
    {
        public int PlayerNum;
        public FixedString32Bytes PlayerName;
        public TimeSpan TimeTag;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerNum);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref TimeTag);
        }

        public bool Equals(PlayerStat other)
        {
            return PlayerName == other.PlayerName &&
                   TimeTag.Equals(other.TimeTag) && PlayerNum == other.PlayerNum;
        }
    }
}
