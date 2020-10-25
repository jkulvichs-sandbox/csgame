using System.Windows.Forms;

namespace CSGame
{
    static class GameApp
    {
        static void Main()
        {
            var window = new GameWindow();
            Application.EnableVisualStyles();
            Application.Run(window);
        }
    }
}