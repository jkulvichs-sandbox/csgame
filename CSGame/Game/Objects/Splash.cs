using System;
using System.Drawing;
using CSGame.GamePlugins;

namespace CSGame.Game.Objects
{
    // Представляет объект взрыва частиц разлетающихся в разные стороны
    public class Splash : GameObject
    {
        // Обратный отсчёт времени жзни
        // Когда он истечёт объект будет уничтожен
        public float lifeTime = 0f;

        public Splash(PointF pos, int strength, Color color, Random rand) : base(pos)
        {
            // strength определяет в том числе количество и дальность разлёта частиц
            // С этими константами можно поиграться для лучшего эффекта и производительности
            var count = strength * 5;
            var maxSpeed = (float) strength / 3f;

            // Задаём время жизни объекта
            // И такое же значение улетит частицам
            lifeTime = strength * 3f;

            // Генерируем частицы
            var particles = new Particle[count];
            for (var i = 0; i < count; i++)
            {
                // Выбираем произвольное направление полёта
                var direction = (float) rand.NextDouble() * (float) Math.PI * 2f;

                // Создаём вектор полёта
                var vect = new PointF((float) Math.Sin(direction), (float) Math.Cos(direction));

                // Случайно выбираем силу полёта от 0 до максимума
                var speed = (float) rand.NextDouble() * maxSpeed;

                // Умножаем вектор на скорость полёта
                vect.X *= speed;
                vect.Y *= speed;

                // Кординаты всех частиц нулевые, т.к. они будут брать
                // этот компонент к вачестве родителя и точки отсчёта
                particles[i] = new Particle(
                    new PointF(0, 0),
                    vect,
                    color,
                    (float) lifeTime * ((float) rand.NextDouble() * 0.8f + 0.2f),
                    (float) strength * 2f
                );
            }

            // Добавляем частицы себе в дочерние
            AddChilds(particles);
        }

        // Удаление объекта по окончанию времени жазни
        public override void Update(State state, GameObject parent = null)
        {
            base.Update(state, parent);

            lifeTime--;
            if (lifeTime <= 0) deleted = true;
        }
    }
}