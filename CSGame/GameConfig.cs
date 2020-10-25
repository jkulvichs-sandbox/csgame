using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CSGame
{
    [Serializable]
    public class GameConfig
    {
        // Настройки формы
        public WindowConfig Window { get; set; } = new WindowConfig();

        [Serializable]
        public class WindowConfig
        {
            public string Title { get; set; } = "CSGame";
            public int X { get; set; } = 0;
            public int Y { get; set; } = 0;
            public int Width { get; set; } = 500;
            public int Height { get; set; } = 500;
            public bool IsMaximized { get; set; } = false;
            public int MinWidth { get; set; } = 400;
            public int MinHeight { get; set; } = 400;
            public int MaxWidth { get; set; } = 800;
            public int MaxHeight { get; set; } = 600;
            public bool IsResizable { get; set; } = true;
            public bool AllowMaximizing { get; set; } = false;
        }

        // Настройки графического движка
        public EngineConfig Engine { get; set; } = new EngineConfig();

        [Serializable]
        public class EngineConfig
        {
            public CompositingQuality CompositingQuality { get; set; } = CompositingQuality.HighSpeed;
            public InterpolationMode InterpolationQuality { get; set; } = InterpolationMode.Low;
            public int VScreenWidth { get; set; } = 500;
            public int VScreenHeight { get; set; } = 500;
            public bool ShowPerformanceScreen { get; set; } = true;

            // FPS потока вывода графики
            public int GraphicalFPS { get; set; } = 60;

            // FPS потока вычислений
            public int LogicalFPS { get; set; } = 30;
        }

        // Настройки игры
        public GameData Game { get; set; } = new GameData();

        [Serializable]
        public class GameData
        {
            
        }
    }
}