using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options
{
    public static readonly int ATTEMPTS_LIMIT_MAJOR = 2500;
    public static readonly int ATTEMPTS_LIMIT_MINOR = 250;
    public static readonly int ATTEMPTS_LIMIT_REALTIME = 125;

    public static readonly float UNOCCUPIED_ROOM_TICK_RATE = 0.25f;
    public static readonly float OCCUPIED_ROOM_TICK_RATE = 0.35f;

    public static readonly float ROOM_GRID_RAY_DIST = 0.5f;

    public static readonly int DUNGEON_ROOM_SPACING = 32;

    public static readonly float DMG_MARKER_LIFETIME = 1f;

    public static readonly float HEAVY_MOVE_PENALTY = .15f;

    public static readonly float HEALTH_SCALING = 1.03f;
    public static readonly float DAMAGE_SCALING = 1.03f;
}
