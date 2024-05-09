using UnityEngine;
namespace NuiN.NExtensions
{
    public static class Strings
    {
        public static class Tags
        {
            public static readonly string Untagged = "Untagged";
            public static readonly string Respawn = "Respawn";
            public static readonly string Finish = "Finish";
            public static readonly string EditorOnly = "EditorOnly";
            public static readonly string MainCamera = "MainCamera";
            public static readonly string Player = "Player";
            public static readonly string GameController = "GameController";
            
        }

        public static class Layers
        {
            public static readonly int Default = LayerMask.NameToLayer("Default");
            public static readonly int TransparentFX = LayerMask.NameToLayer("TransparentFX");
            public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
            public static readonly int Water = LayerMask.NameToLayer("Water");
            public static readonly int UI = LayerMask.NameToLayer("UI");
            public static readonly int Player = LayerMask.NameToLayer("Player");
            public static readonly int SceneMesh = LayerMask.NameToLayer("SceneMesh");
            
        }

        public static class Scenes
        {
            public static readonly string Testing_Cai = "Testing_Cai";
            public static readonly string Netcode_David = "Netcode_David";
            public static readonly string Game = "Game";
            public static readonly string Netcode_David_ = "Netcode_David_";
            
        }
    }
}