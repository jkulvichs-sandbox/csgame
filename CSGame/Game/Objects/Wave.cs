using System.Drawing;

namespace CSGame.Game.Objects
{
    // Whoop эффект расходящейся волны
    // Чем-то схож с Splash, но делает меньше нагрузки и не состоит из частиц
    public class Wave : GameObject
    {
        public Color color;
        public float lifeTime = 0;
        public float maxLifeTime;
        public float lifeTimeSpeed = 1.5f;
        public float size;

        public Wave(PointF pos, float strength, Color color) : base(pos)
        {
            this.pos = pos;
            this.color = color;
            this.size = strength * 4f;
            this.maxLifeTime = strength;
        }

        public override void Update(State state, GameObject parent = null)
        {
            base.Update(state, parent);
            lifeTime += lifeTimeSpeed;
            if (lifeTime >= maxLifeTime) deleted = true;
        }

        public override void Draw(State state, GameObject parent = null)
        {
            base.Draw(state, parent);

            var lifePrc = lifeTime / maxLifeTime;
            var currentSize = size * lifePrc;

            state.gCtx.FillEllipse(
                new SolidBrush(Color.FromArgb((int) ((1f - lifePrc) * 255f), color)),
                pos.X - currentSize / 2f,
                pos.Y - currentSize / 2f,
                currentSize,
                currentSize
            );
        }
    }
}