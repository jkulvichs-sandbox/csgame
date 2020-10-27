using System;
using System.Drawing;
using System.Windows.Forms;
using CSGame.Game.Objects;
using CSGame.GamePlugins;

namespace CSGame.Game
{
    // Сама игра должна быть написана здесь
    public class Game
    {
        private State state = new State();

        // Вызывается один раз перед запуском игры
        // Здесь лучше загружать и инициализировать ресурсы
        public void OnLoad(GameConfig.GameData data, Bitmap screen, Graphics gCtx, InputManager input)
        {
            state.screen = screen;
            state.gCtx = gCtx;
            state.input = input;

            // Инициализация ресурсов
            state.res.Init();

            // Композиция сцен
            state.scene.AddChild(state.sceneEffects);
            state.scene.AddChild(state.scenePlayer);
            state.scene.AddChild(state.sceneEnemies);

            // Создание игрока в центре сцены
            state.player = new Player(new PointF(screen.Width / 2f, screen.Height / 2f));
            state.scenePlayer.AddChild(state.player);
        }

        // Шаг логического цикла
        public void OnUpdate()
        {
            // Обновляем все объекты на сцене
            state.scene.Update(state, null);
            // Чтобы снизить нагрузку каждые 10 тактов будем удалять объекты которые этого просят
            if (state.gameTime % 10 == 0)
                state.scene.ClearDeleted();

            // Спавним новых врагов
            if (state.sceneEnemies.childs.Count < 5)
            {
                if (state.rand.Next(0, state.sceneEnemies.childs.Count * 400) == 0)
                {
                    var pos = new PointF(
                        (float) (state.rand.NextDouble() * state.screen.Width),
                        (float) (state.rand.NextDouble() * state.screen.Height)
                    );
                    state.sceneEnemies.AddChild(new Enemy(pos));
                    state.sceneEffects.AddChild(new Wave(pos, 20, Color.Violet));
                }
            }

            // Увеличиваем переменную времени игры
            state.gameTime++;
        }

        // Шаг графического цикла 
        public Bitmap OnDraw()
        {
            // Очищаем сцену полупрозрачным цветом
            // Таким образом добиваемся эффекта следа
            state.gCtx.FillRectangle(state.res.brushBackground, 0, 0, state.screen.Width, state.screen.Height);

            // Рисуем все объекты на сцене
            state.scene.Draw(state, null);

            return state.screen;
        }
    }
}