using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    // Room globals
    public enum Direction { north, east, south, west };

    public const int MAX_ROOM_NO = 5; // Max amount of rooms allowed, at least 3 required
    public const int ROOM_AMOUNT_PER_AXIS = 3; // Amount of rooms that can be placed in a single line on a map
    public const float ROOM_SIZE = 20; // Maximum room size, the doors need to match up with the room size boundaries
}
