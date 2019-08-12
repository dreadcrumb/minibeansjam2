namespace Assets.Scripts
{
    public static class Const
    {
        public class Animations
        {
            public const string MeatbagEaten = "People_gefressenwerden";
            public const string DinoEating = "Dino_Eat";
        }

        public class File
        {
            public const string LevelButtonPrefab = "Prefabs/LevelButtonPrefab";
            public const string MIDI_FileExtension = ".mid";
            public const string MIDI_Location = "Assets/MIDI_Music";
        }

        public static class AudioSources
        {

            public const string Eating = "fressen";
            public const string Bite = "schnapp";
        }

        public static class Coroutine
        {
            public const string ApplyHunger = "ApplyHunger";
            public const string FadeNote = "FadeNote";
            public const string Instrument = "Instrument";
        }

        public static class Duration
        {
            public const int NoteFade = 1;
        }

        public static class Scenes
        {
            public static int PlayScene = 1;
        }

        public static class Tags
        {
            public const string GameController = "GameController";
            public const string Head = "Head";
            public const string Killbox = "Killbox";
            public const string Level = "Level";
            public const string LevelPanel = "LevelPanel";
            public const string MidiPlayer = "MidiPlayer";
            public const string Note = "Note";
            public const string NoteHitArea = "NoteHitArea";
            public const string NoteSpawn = "NoteSpawn";
            public const string Player = "Player";
            public const string Spawn = "Spawn";
            public const string StartButton = "StartButton";
            public const string UIController = "UIController";
        }

        public static class UI
        {
            public const string MenuReset = "TriggerMenuReset";
        }
    }
}
