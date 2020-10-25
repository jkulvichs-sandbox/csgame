using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CSGame.GamePlugins;

namespace CSGame.Game.Objects
{
    // Класс отскакивающего объекта
    // Отскакивает от стенок экрана
    // От этого класса можно наследоваться как и от GameObject
    public class GameObjectBouncing : GameObject
    {
        // Размер игрока
        public float size = 0f;

        // Необходимо вызвать также и дочерний конструктор через base
        public GameObjectBouncing(PointF pos, float size) : base(pos)
        {
            this.size = size;
        }

        // Обновление данных отскакивающего объекта
        // base используется чтобы вызвать цепочку переопределённых методов
        public override void Update(State state, GameObject parent = null)
        {
            // Учитываем смещение родительского компонента
            var origin = new PointF(0, 0);
            if (parent != null) origin = parent.pos;

            // Отскакивание игрока от стен
            if (pos.X < 0 + size / 2f - origin.X)
            {
                pos.X = 0 + size / 2f - origin.X;
                acc.X *= -1;
            }

            if (pos.X > state.screen.Width - size / 2f + origin.X)
            {
                pos.X = state.screen.Width - size / 2f + origin.X;
                acc.X *= -1;
            }

            if (pos.Y < 0 + size / 2f - origin.Y)
            {
                pos.Y = 0 + size / 2f - origin.Y;
                acc.Y *= -1;
            }

            if (pos.Y > state.screen.Height - size / 2f + origin.Y)
            {
                pos.Y = state.screen.Height - size / 2f + origin.Y;
                acc.Y *= -1;
            }

            base.Update(state, parent);
        }
    }
}