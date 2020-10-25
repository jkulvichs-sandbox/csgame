using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSGame.GamePlugins;
using Json.Net;

namespace CSGame
{
    // Класс инициализирует окно и запускает игровые циклы
    // Занимается перерисовкой графики на окне
    // ps. прощу прощенья за код, он не совсем шарповский по стилю, отпечатался JS :)
    public class GameWindow : Form
    {
        private string configPath = "config.json";

        private GameConfig config = new GameConfig();

        // Виртуальный игровой экран
        private Bitmap vScreen;

        // Объект отрисовки на экране
        private Graphics vGCtx;

        // Таймер перерисовки окна
        private Timer graphicalLoop = new Timer();

        // Анализатор производительности
        private Performance performance = new Performance();

        // Контроллер управления
        private InputManager input = new InputManager();

        // Состояние игры
        private Game.Game game = new Game.Game();

        // Загрузка первичных ресурсов
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Проверяем существование файла конфигурации
            if (File.Exists(configPath))
                // Загрузка конфигурации, в противном случае останется переменная по умолчанию
                this.config = JsonNet.Deserialize<GameConfig>(File.ReadAllText(configPath));

            // Настраиваем параметры окна
            // Двойная буферизация для предотвращения мерцания при обновлении изображения
            this.DoubleBuffered = true;

            // Настраиваем параметры окна из конфигурации
            this.Text = config.Window.Title;
            this.Left = config.Window.X;
            this.Top = config.Window.Y;
            this.ClientSize = new Size(config.Window.Width, config.Window.Height);
            Console.WriteLine(this.Bounds);
            this.WindowState = config.Window.IsMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
            this.MaximumSize = new Size(config.Window.MaxWidth, config.Window.MaxHeight);
            this.MinimumSize = new Size(config.Window.MinWidth, config.Window.MinHeight);
            this.FormBorderStyle = config.Window.IsResizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog;
            this.MaximizeBox = config.Window.AllowMaximizing;

            // Настраиваем параметры движка из конфигурации
            this.graphicalLoop.Interval = (int) (1000f / (float) this.config.Engine.GraphicalFPS);

            // Запускаем движок
            // Добавляем делегат который будет дёргать перерисовку формы
            this.graphicalLoop.Tick += (sender, args) => this.Refresh();
            this.graphicalLoop.Start();

            // Создаём новый логический поток и запускаем таймер на шаг
            Task.Run(() =>
            {
                var logicalLoop = new System.Timers.Timer(1000f / (float) this.config.Engine.LogicalFPS);
                logicalLoop.AutoReset = true;
                logicalLoop.Elapsed += (sender, args) =>
                {
                    // Обновление состояния игры
                    this.game.OnUpdate();
                    this.performance.LogicalRenderTick();
                };
                logicalLoop.Start();
            });

            // Настройка параметров виртуального экрана
            this.vScreen = new Bitmap(config.Engine.VScreenWidth, config.Engine.VScreenHeight);
            this.vGCtx = Graphics.FromImage(this.vScreen);

            // Говорим игре что готовы работать
            this.game.OnLoad(this.config.Game, this.vScreen, this.vGCtx, this.input);
        }

        // Финализация и закрытие программы
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // Сохранение конфигурации
            File.WriteAllText(configPath, JsonNet.Serialize(config));
        }

        // Меняем конфигурацию размеров окна
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.config.Window.Width = this.ClientSize.Width;
            this.config.Window.Height = this.ClientSize.Height;
            this.config.Window.IsMaximized = this.WindowState == FormWindowState.Maximized;
        }

        // Меняем конфигурацию позиции окна
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            this.config.Window.X = this.Left;
            this.config.Window.Y = this.Top;
        }

        // Если какая-либо клавиша нажата
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            this.input.DispatchKeyDown(e.KeyCode);
        }

        // Если какая-либо клавиша отпущена
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            this.input.DispatchKeyUp(e.KeyCode);
        }

        // Возвращает масштабирование виртуального экрана
        private float GetVScreenScale()
        {
            // Определяем во сколько раз реальный экран больше виртуального по каждой из сторон
            var scaleX = (float) this.ClientSize.Width / (float) this.vScreen.Width;
            var scaleY = (float) this.ClientSize.Height / (float) this.vScreen.Height;
            // Выбираем наименьшее значение и это будет коэффициент на который надо домножить каждую сторону
            // При выводе виртуального экрана чтобы вписать его в реальный
            return Math.Min(scaleX, scaleY);
        }

        // Смещение виртуального экрана
        private PointF GetVScreenOffset()
        {
            var scale = this.GetVScreenScale();

            // Определяем сдвиги для центрирования вывода виртуального экрана
            var offsetX = ((float) this.ClientSize.Width - (float) this.vScreen.Width * scale) / 2f;
            var offsetY = ((float) this.ClientSize.Height - (float) this.vScreen.Height * scale) / 2f;
            return new PointF(offsetX, offsetY);
        }

        // Событие когда форма хочет перерисоваться
        protected override void OnPaint(PaintEventArgs e)
        {
            // Получение контекста для отрисовки на форме
            var gCtx = e.Graphics;

            // Настройки качества графики
            gCtx.CompositingQuality = config.Engine.CompositingQuality;
            gCtx.InterpolationMode = config.Engine.InterpolationQuality;

            // Если виртуальный экран доступен - отрисовываем
            if (this.vScreen != null)
            {
                // Очистка экрана
                gCtx.Clear(Color.FromArgb(255, 20, 20, 20));

                // Масштабирование виртуального экрана
                var scale = this.GetVScreenScale();

                // Смещение виртуального экрана
                var offset = this.GetVScreenOffset();

                // Отрисовываем отмасштабированный и отцентрированный виртуальный экран если доступен
                if (this.vScreen != null)
                    gCtx.DrawImage(
                        this.game.OnDraw(),
                        offset.X,
                        offset.Y,
                        (float) this.vScreen.Width * scale,
                        (float) this.vScreen.Height * scale
                    );

                // Отрисовка экрана производительности
                if (this.config.Engine.ShowPerformanceScreen)
                {
                    var perfScreen = this.performance.RenderPerfScreen();
                    gCtx.DrawImage(ImageTools.Opacity(perfScreen, 0.5f), 0, 0);
                }

                // Сообщаем наблюдателю производительности о том, что мы отрисовали кадр
                this.performance.GraphicalRenderTick();
            }

            base.OnPaint(e);
        }
    }
}