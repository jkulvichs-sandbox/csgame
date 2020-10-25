using System.Drawing;
using CSGame.GamePlugins;

namespace CSGame.Game.Objects
{
    // Одна частица
    // Самый мелкий компонент на экране
    // Нужен преимущественно для создания различных эффектов
    public class Particle : GameObjectBouncing
    {
        // Увет частицы
        public Color color;

        // Максимальное время жизни частицы
        public float maxLifeTime = 0f;

        // Время сколько частица уже живёт
        public float lifeTime = 0f;

        // Время жизни после которого частица начинает затухать
        // Если 0 - частица не тухнет, а исчезает после истечения времени жизни
        public float fadeAfter = 0f;

        public Particle(PointF pos, PointF acc, Color color, float maxLifeTime, float fadeAfter = 0) : base(pos, 2)
        {
            this.acc = acc;
            this.color = color;
            this.maxLifeTime = maxLifeTime;
        }

        // Логика жизни частицы
        public override void Update(State state, GameObject parent = null)
        {
            base.Update(state, parent);

            // Увеличиваем время жизни
            lifeTime++;

            // Если частица живёт дольше чем максимальное время - ставим флаг на удаление
            if (lifeTime > maxLifeTime) deleted = true;
        }

        // Отрисовка частицы
        public override void Draw(State state, GameObject parent = null)
        {
            base.Draw(state, parent);

            // Если у частицы есть родитель - берём его кординаты как точку отсчёта для отрисовки
            PointF origin = new PointF(0, 0);
            if (parent != null) origin = parent.pos;

            // Изменяем прозрачность частицы от времени её жизни
            var opacity = 1f;
            if (lifeTime > fadeAfter)
            {
                // Сколько ещё времени проивёт частица максимум
                var maxAfterFade = maxLifeTime - fadeAfter;
                // Сколько времени частица уже прожила от этого
                var lifeAfterFade = lifeTime - fadeAfter;
                opacity = 1f - lifeAfterFade / maxAfterFade;
            }

            // Защита на случай если частица не удалится сразу и математика уйдёт в минус
            if (opacity < 0) opacity = 0;

            // Рисуем частицу
            state.gCtx.FillRectangle(
                new SolidBrush(Color.FromArgb((int) (255f * opacity), color)),
                origin.X + pos.X, origin.Y + pos.Y, size, size
            );
        }
    }
}