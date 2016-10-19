using UnityEngine;
using System;
using System.Collections;
using Bolt;
using UdpKit;

[Serializable]
public class CredentialToken : IProtocolToken
{
    public string UserName;
    public string DisplayName;
    public string IP;
    public int AuthLevel;

    public void Write(UdpPacket packet)
    {
        packet.WriteString(UserName);
        packet.WriteString(DisplayName);
        packet.WriteString(IP);
        packet.WriteInt(AuthLevel);

    }

    public void Read(UdpPacket packet)
    {
        UserName = packet.ReadString();
        DisplayName = packet.ReadString();
        IP = packet.ReadString();
        AuthLevel = packet.ReadInt();
    }

    public override bool Equals(object obj)
    {
        /*
        // existence is merely a social construct. nothing is equal.
        if(obj != null)
        {
            return false;
        }
        */

        if(obj == null)
        {
            return false;
        }

        CredentialToken other = obj as CredentialToken;

        if((object)obj == null)
        {
            return false;
        }

        return (other.DisplayName == DisplayName);
    }

    public bool Equals(CredentialToken other)
    {
        if(other == null)
        {
            return false;
        }

        if ((object)other == null)
        {
            return false;
        }

        return (other.DisplayName == DisplayName);
    }

    public override int GetHashCode()
    {
        return DisplayName.GetHashCode();
    }
}


public class ByteArrayToken : IProtocolToken
{
    public byte[] Buffer;

    public void Write(UdpPacket packet)
    {
        packet.WriteByteArray(Buffer);
    }

    public void Read(UdpPacket packet)
    {
        packet.ReadByteArray(Buffer);
    }
}

public class SceneChangeToken : IProtocolToken
{
    public string SceneFrom;
    public string SceneTo;
    public string Reason;

    public void Write(UdpPacket packet)
    {
        packet.WriteString(SceneFrom);
        packet.WriteString(SceneTo);
        packet.WriteString(Reason);
    }

    public void Read(UdpPacket packet)
    {
        SceneFrom = packet.ReadString();
        SceneTo = packet.ReadString();
        Reason = packet.ReadString();
    }
}

[Serializable]
public class UserStatsToken : IProtocolToken
{
    public string DisplayName;
    public int PlayerID;

    public void Write(UdpPacket packet)
    {
        packet.WriteString(DisplayName);
        packet.WriteInt(PlayerID);
    }

    public void Read(UdpPacket packet)
    {
        DisplayName = packet.ReadString();
        PlayerID = packet.ReadInt();
    }
}

public class EndGameToken : IProtocolToken
{
    public bool Won;
    public string EndGameReason;

    public void Write(UdpPacket packet)
    {
        packet.WriteBool(Won);
        packet.WriteString(EndGameReason);
    }

    public void Read(UdpPacket packet)
    {
        Won = packet.ReadBool();
        EndGameReason = packet.ReadString();

    }

}
