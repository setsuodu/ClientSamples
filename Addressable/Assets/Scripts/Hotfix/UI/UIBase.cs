using UnityEngine;
//using LiteNetLib;
//using LiteNetLib.Utils;
//using Code.Shared;
//using Code.Client;

namespace HotFix
{
    public abstract class UIBase : MonoBehaviour
    {
        public virtual void Pop()
        {
            UIManager.Get().Pop(this);
        }

        //public virtual void OnNetCallback(PacketType eventID, INetSerializable reader, NetPeer peer) { }
        //public virtual void OnUserCallback(PlayerStatus status) { }

        public virtual void ApplyLanguage() { }

        //public virtual ClientPlayer LocalPlayer => ClientNet.Get.m_PlayerManager.LocalPlayer;
    }
}