using System;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace CSGame.Game.Objects
{
    // Класс врага который летает по сцене
    public class Enemy : GameObjectBouncing
    {
        // Враг будет менять траекторию по прошествии кадого отрезка этого времени
        public float trackChangeAfter = 40f;

        // По истечении этого таймера враг будет менять траекторию
        public float trackChangeTimer = 0f;

        // Скорость движения противника
        public float speed = 10f;

        // Затухание скорости
        public float speedFade = 0.95f;

        public Enemy(PointF pos) : base(pos, 15)
        {
        }

        public override void Update(State state, GameObject parent = null)
        {
            base.Update(state, parent);

            // Меняем траекторию врага
            if (trackChangeTimer <= 0)
            {
                // Выбираем произвольный вектор траектории
                var angle = (float) state.rand.NextDouble() * (float) Math.PI * 2f;

                // Строим вектор
                var vect = new PointF(
                    (float) Math.Sin(angle),
                    (float) Math.Cos(angle)
                );
                
                // Прибавляем вектор траектории
                acc.X += vect.X * speed;
                acc.Y += vect.Y * speed;

                trackChangeTimer = trackChangeAfter;
            }

            // Затухание скорости
            acc.X *= speedFade;
            acc.Y *= speedFade;

            trackChangeTimer--;
        }

        public override void Draw(State state, GameObject parent = null)
        {
            base.Draw(state, parent);
            state.gCtx.FillEllipse(
                state.res.brushViolet,
                pos.X - size / 2f,
                pos.Y - size / 2f,
                size,
                size
            );
        }
    }
}