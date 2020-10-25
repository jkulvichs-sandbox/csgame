using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CSGame.GamePlugins;

namespace CSGame.Game.Objects
{
    // Пример класса игрока
    public class Player : GameObjectBouncing
    {
        // Скорость игрока
        public float speed = 1.5f;

        // Коэффициент затухания скорости
        public float speedFade = 0.9f;

        // Перезарядка оружия игрока
        public float reload = 0f;

        // Скорость перезарядки
        public float reloadSpeed = 0.05f;

        // Сила сброса ракеты
        public float rocketDropSpeed = 5f;

        // Необходимо вызвать также и дочерний конструктор через base
        public Player(PointF pos) : base(pos, 20)
        {
        }

        // Обновление данных игрока
        // base используется чтобы вызвать цепочку переопределённых методов
        public override void Update(State state, GameObject parent = null)
        {
            // Создаём вектор движения игрока
            var vect = new PointF(0, 0);

            // Меняем вектор с логикой нажатых клавиш
            if (state.input.IsKeyPressed(Keys.W)) vect.Y -= 1;
            if (state.input.IsKeyPressed(Keys.S)) vect.Y += 1;
            if (state.input.IsKeyPressed(Keys.A)) vect.X -= 1;
            if (state.input.IsKeyPressed(Keys.D)) vect.X += 1;

            // Нормируем вектор
            // Нужно чтобы когда мы жмём одновременно перемещение по X и Y
            // игрок не перемещался быстрее чем он это делает двигаясь только по одной оси
            // Такая проблема была в Half-Life
            float length = (float) Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y);
            if (length > 0)
            {
                vect.X /= length;
                vect.Y /= length;
            }

            // Теперь умножаем вектор на скорость игрока
            vect.X *= speed;
            vect.Y *= speed;

            // И прибавляем этот вектор к ускорению игрока
            acc.X += vect.X;
            acc.Y += vect.Y;

            // После этого применяем затухание скорости
            acc.X *= speedFade;
            acc.Y *= speedFade;

            // Перезарядка оружия
            if (reload < 1f) reload += reloadSpeed;

            // Логика выстрела
            if (state.input.IsKeyPressed(Keys.Space) && reload >= 1f)
            {
                // Выбираем произвольную цель из сцены с противниками
                var enemies = state.sceneEnemies.childs.ToList();
                if (enemies.Count > 0)
                {
                    reload = 0f;

                    var target = enemies[state.rand.Next(0, enemies.Count - 1)].Value;

                    // Выбор произвольного угла сброса ракеты
                    var angle = (float) state.rand.NextDouble() * (float) Math.PI * 2f;

                    // Расчёт вектора сброса
                    var dropAngle = new PointF(
                        (float) Math.Sin(angle) * rocketDropSpeed,
                        (float) Math.Cos(angle) * rocketDropSpeed
                    );

                    // Создание ракеты и применение вектора сброса
                    var rocket = new Rocket(pos, target);
                    rocket.acc.X += dropAngle.X;
                    rocket.acc.Y += dropAngle.Y;

                    // Добавление к сцене
                    state.scenePlayer.AddChild(rocket);
                }
            }

            base.Update(state, parent);
        }

        // Отрисовка игрока
        // base используется чтобы вызвать цепочку переопределённых методов
        public override void Draw(State state, GameObject parent = null)
        {
            // Отрисовка объекта игрока
            state.gCtx.FillEllipse(state.res.brushWhite,
                new RectangleF(pos.X - size / 2f, pos.Y - size / 2f, size, size));

            // Отрисовка индикатора перезарядки
            var indicatorMaxSize = size - 4f;
            var indicatorSize = (1f - reload) * indicatorMaxSize;
            state.gCtx.FillEllipse(
                state.res.brushBlack,
                pos.X - indicatorSize / 2f,
                pos.Y - indicatorSize / 2f,
                indicatorSize,
                indicatorSize
            );

            base.Draw(state, parent);
        }
    }
}