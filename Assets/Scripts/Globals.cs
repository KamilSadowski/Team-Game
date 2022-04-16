using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    // Room globals
    public enum Direction { north, east, south, west };

    public struct WeaponData { public int prefabID; }; // Stores all of the data needed for weapons to persist between levels and save files

    public struct Grid2D 
    { 
        public int x; 
        public int y;

        public Grid2D(int gridX, int gridY)
        {
            x = gridX;
            y = gridY;
        }
    };

    public const int MAX_ROOM_NO = 15; // Max amount of rooms allowed, at least 3 required
    public const int MAP_GRID_SIZE = 10; // Size of the map grid that is used for map creation
    public const float ROOM_SIZE = 20; // Maximum room size, the doors need to match up with the room size boundaries
    public const int MIN_CORRIDOR_DIST = 3; // How long a corridor has to be away from the room to avoid wall clipping
    public const int ROOM_PLACEMENT_OFFSET = 5; // Distance between room grid fields, needs to exist to avoid wall clipping and allow corridors

    public const float SPRITE_Z = -0.05f; // How much to offset sprites to cast shadows

    public const int COLOURS_PER_SHADER = 3; // How many colours the colouring shader supports

    // Game manager globals
    public enum Scenes { MainMenu, HubWorld, Dungeon };
    public static string[] SceneNames = new string[3] { "MainMenu", "HubWorld", "Dungeon" };



    const bool IS_WEAPON_LOG_OUTPUT = true;
}
