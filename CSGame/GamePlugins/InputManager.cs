using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CSGame.GamePlugins
{
    // Помогает работать с устройствами ввода
    public class InputManager
    {
        // Хранит состояние нажатых и отпущенных клавиш
        private Dictionary<Keys, bool> keysState = new Dictionary<Keys, bool>();

        // Вызывает нажатие клавиши
        public void DispatchKeyDown(Keys key)
        {
            keysState[key] = true;
        }

        // Вызывает отпускание клавиши
        public void DispatchKeyUp(Keys key)
        {
            keysState[key] = false;
        }

        // Проверяет нажата ли клавиша
        public bool IsKeyPressed(Keys key)
        {
            return keysState.ContainsKey(key) && keysState[key];
        }
    }
}