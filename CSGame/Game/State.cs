using System;
using System.Drawing;
using CSGame.Game.Objects;
using CSGame.GamePlugins;

namespace CSGame.Game
{
    // Класс хранящий состояние игры
    // Все глобальные игровые переменные следует располагать здесь
    // Например, здоровье игрока, массив позиций противника итп
    public class State
    {
        // Ресурсы игры 
        public Resources res = new Resources();

        // Игровой экран
        public Bitmap screen;

        // КОнтекст рисования графики на экране
        public Graphics gCtx;

        // Менеджер ввода
        public InputManager input;
        
        // Количество логических тактов с начала игры
        // Подобная переменная необходима для различных анимаций
        public float gameTime = 0f;
        
        // Часто используется в играх рандомизатор, добавляем его
        public Random rand = new Random();

        // Информация об игроке
        public GameObject player;
        
        // Главная игровая сцена со всеми объектами
        public GameObject scene = new GameObject(new PointF(0, 0));
        
        // Сцена эффектов (частицы и прочие не игровые объекты)
        public GameObject sceneEffects = new GameObject(new PointF(0, 0));
        
        // Сцена игрока и его дочерних объектов
        public GameObject scenePlayer = new GameObject(new PointF(0, 0));
        
        // Сцена - контейнер для врагов
        public GameObject sceneEnemies = new GameObject(new PointF(0, 0));
    }
}