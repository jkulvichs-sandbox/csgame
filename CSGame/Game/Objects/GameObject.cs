using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CSGame.GamePlugins;

namespace CSGame.Game.Objects
{
    // Игровой объект на сцене, все прочие объекты должны от него наследоваться
    public class GameObject
    {
        // Позиция объекта на экране
        public PointF pos = new PointF();

        // Вектор ускорения объекта, будет прибавляться к позиции на каждый тик
        public PointF acc = new PointF();

        // Объект устанавливает это значение когда хочет быть удалён
        // Позже в обновлении все помеченные таким образом дочерние объекты будут удалены
        public bool deleted = false;

        // Дочерние объекты от этого
        // Все такие объекты будут как бы объединены в одну группу которой можно
        // комплексно управлять
        // Например, в одну группу можно установить объекты HUD'а, во вторую врагов
        // Словарь здесь используется для более быстрого поиска объекта
        public Dictionary<GameObject, GameObject> childs = new Dictionary<GameObject, GameObject>();

        // Конструктор объекта
        public GameObject(PointF pos)
        {
            this.pos = pos;
        }

        // Шаг логики объекта
        // Для базового объекта тут базовая логика ускорения
        // Должен переопределяться для объектов
        public virtual void Update(State state, GameObject parent = null)
        {
            // Блокируем объект и оставляем его в единоличное пользование только в этом потоке
            // Чтобы другие не попытались получить к нему доступ во время работы и не упали
            lock (childs)
            {
                // Обновление состояния всех дочерних компонентов
                foreach (var obj in childs)
                    if (!obj.Value.deleted)
                        obj.Value.Update(state, this);
            }

            // Обновление собственного состояния
            pos.X += acc.X;
            pos.Y += acc.Y;
        }

        // Шаг отрисовки объекта
        // Логика того, что объект сам себя нарисует зная информацию о себе и из глобального состояния
        // Должен быть перегружен для дочернего объекта
        public virtual void Draw(State state, GameObject parent = null)
        {
            lock (childs)
            {
                // Отрисовка всех дочерних компонентов
                foreach (var obj in childs)
                    if (!obj.Value.deleted)
                        obj.Value.Draw(state, this);
            }
        }

        // Добавляет объект как дочерний
        public void AddChild(GameObject child)
        {
            lock (childs)
            {
                childs[child] = child;
            }
        }

        // Добавляет массив объектов как дочерних
        public void AddChilds(GameObject[] childArray)
        {
            lock (childs)
            {
                for (var i = 0; i < childArray.Length; i++)
                    childs[childArray[i]] = childArray[i];
            }
        }

        // Удаляет дочерний объект
        public void RemoveChild(GameObject child)
        {
            lock (childs)
            {
                childs.Remove(child);
            }
        }

        // Удаляет все дочерние объекты
        public void ClearChilds()
        {
            lock (childs)
            {
                childs.Clear();
            }
        }

        // Проверяет, содержится ли объект другой дочерний объект
        public bool ContainsObject(GameObject child)
        {
            return childs.ContainsKey(child);
        }

        // Запускает рекурсивную очистку дочерних объектов которые пометили себя к удалению
        public int ClearDeleted()
        {
            // Считаем количество удалённых объектов
            int removed = 0;

            // Блокируем объект говоря, что мы работаем с объектом коллекций
            lock (childs)
            {
                // Получаем список объектов чтобы удалять по индексу
                var list = childs.ToList();
                for (var i = 0; i < list.Count; i++)
                {
                    // Удаляем все дочерние рекурсивно
                    removed += list[i].Value.ClearDeleted();
                    // Удаляем дочерние этого компонента
                    if (list[i].Value.deleted)
                    {
                        childs.Remove(list[i].Value);
                        removed++;
                    }
                }
            }

            return removed;
        }
    }
}