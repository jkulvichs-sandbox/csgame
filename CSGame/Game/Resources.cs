using System.Drawing;

namespace CSGame.Game
{
    // Класс хранящий игровые ресурсы
    public class Resources
    {
        public Brush brushBackground = new SolidBrush(Color.FromArgb(20, 0, 0, 0));
        public Brush brushBlack = new SolidBrush(Color.Black);
        public Brush brushWhite = new SolidBrush(Color.White);
        public Brush brushViolet = new SolidBrush(Color.Violet);

        public Pen penBlack2 = new Pen(Color.Black, 2);
        public Pen penWhite2 = new Pen(Color.White, 2);

        // При необходимости здесь можно подгрузить ресурсы
        // Например, картинки
        public void Init()
        {
        }
    }
}