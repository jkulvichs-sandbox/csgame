using System;
using System.Drawing;
using CSGame.GamePlugins;

namespace CSGame.Game.Objects
{
    // Класс ракеты которыми стреляет игрок
    public class Rocket : GameObjectBouncing
    {
        // Цель ракеты
        public GameObject target;

        // Время которое ракета бездействует
        public float coolDown = 10f;

        // Скорость ракеты
        public float speed = 1f;

        // Затухание скорости
        public float speedFade = 0.95f;

        // Радиус при сближении на который ракета может уничтожить противника
        public float destroyDistance = 15f;

        // Время жизни ракеты после которого она взрывается
        public float lifeTime = 50f;

        public Rocket(PointF pos, GameObject target) : base(pos, 5)
        {
            this.target = target;
        }

        public override void Update(State state, GameObject parent = null)
        {
            base.Update(state, parent);

            acc.X *= speedFade;
            acc.Y *= speedFade;

            coolDown--;
            lifeTime--;

            if (coolDown <= 0)
            {
                if (!target.deleted)
                {
                    // Расчёт и нормализация вектора до цели
                    var vect = new PointF(
                        target.pos.X - pos.X,
                        target.pos.Y - pos.Y
                    );
                    var vectLen = (float) Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y);
                    vect.X = vect.X / vectLen * speed;
                    vect.Y = vect.Y / vectLen * speed;
                    acc.X += vect.X;
                    acc.Y += vect.Y;

                    // Если цель находится в радиусе уничтожения
                    // Взрываемся уничтожая цель
                    var distance = (float) Math.Sqrt(
                        Math.Pow(pos.X - target.pos.X, 2) +
                        Math.Pow(pos.Y - target.pos.Y, 2)
                    );
                    if (distance <= destroyDistance)
                    {
                        lifeTime = 0;
                        target.deleted = true;
                        state.sceneEffects.AddChild(new Splash(pos, 20, Color.Violet, state.rand));
                        state.sceneEffects.AddChild(new Wave(pos, 15, Color.Aqua));
                    }
                }
                else
                {
                    // Если цель удалилась и больше не существует на сцене
                    // уничтожаем ракету
                    lifeTime = 0;
                }

                // Добавление нестаблиьности ракетам
                // Чтобы траектория выглядела интереснее
                acc.X += (float) (state.rand.NextDouble() * 2f - 1f);
                acc.Y += (float) (state.rand.NextDouble() * 2f - 1f);
            }

            // Взрыв ракеты по истечении времени жизни
            if (lifeTime <= 0)
            {
                deleted = true;
                state.sceneEffects.AddChild(new Splash(new PointF(pos.X, pos.Y), 15, Color.White, state.rand));
            }
        }

        public override void Draw(State state, GameObject parent = null)
        {
            base.Draw(state, parent);
            state.gCtx.FillEllipse(state.res.brushWhite, pos.X - size / 2f, pos.Y - size / 2f, size, size);
        }
    }
}