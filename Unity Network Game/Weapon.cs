using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct Weapon : INetworkSerializable
{
    public int dmg, rpm, ammo, magSize, fireMode;
    public float reloadSpeed, spread;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref dmg);
        serializer.SerializeValue(ref rpm);
        serializer.SerializeValue(ref ammo);
        serializer.SerializeValue(ref magSize);
        serializer.SerializeValue(ref reloadSpeed);
        serializer.SerializeValue(ref fireMode);
        serializer.SerializeValue(ref spread);
    }
}
