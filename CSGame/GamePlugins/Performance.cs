using System;
using System.Drawing;
using System.Windows.Forms;

namespace CSGame.GamePlugins
{
    // Замеряет FPS и выводит информацию о производительности на экран
    public class Performance : IDisposable
    {
        // Количество секунд на основе которых выстраивается метрика
        private const int metricsTime = 30;

        // Экран производительности для вывода информации
        private Bitmap screen = new Bitmap(200, 100);

        // Рендер контекст для экрана
        private Graphics gCtx;

        // Таймер для замера производительности
        private Timer perfTimer = new Timer();

        // Счётчик количества кадров прошедших с момента последней проверки
        private int graphicalFPS = 0;

        // Счётчик количества кадров логического цикла
        private int logicalFPS = 0;

        // Хранилище последних кадров за последние секунды
        private int[] graphicalFPSHistory = new int[metricsTime];

        // Хранилище последних логических кадров
        private int[] logicalFPSHistory = new int[metricsTime];

        // Ресурсы для отображения
        private Font fontMain = new Font(FontFamily.GenericMonospace, 8);
        private Brush brushMain = new SolidBrush(Color.White);
        private Pen penAccent = new Pen(Color.Red, 1);

        public Performance()
        {
            gCtx = Graphics.FromImage(this.screen);
            // Запускаем таймер на каждую секунду для проверки значений
            perfTimer.Interval = 1000;
            perfTimer.Tick += OnPerfTimerTick;
            perfTimer.Start();
        }

        // Освобождение ресурсов
        public void Dispose()
        {
            gCtx.Dispose();
            screen.Dispose();
            perfTimer.Dispose();
            fontMain.Dispose();
            brushMain.Dispose();
            penAccent.Dispose();
        }

        // Таймер проверки производительности
        private void OnPerfTimerTick(object sender, EventArgs e)
        {
            // Сдвиг данных в массиве метрик
            for (var i = 1; i < metricsTime; i++)
            {
                graphicalFPSHistory[i - 1] = graphicalFPSHistory[i];
                logicalFPSHistory[i - 1] = logicalFPSHistory[i];
            }

            // Добавление нового значение я в историю метрики
            graphicalFPSHistory[metricsTime - 1] = graphicalFPS;
            logicalFPSHistory[metricsTime - 1] = logicalFPS;
            // Очистка счётчика FPS
            graphicalFPS = 0;
            logicalFPS = 0;
        }

        // Должен вызываться функцией которая замеряет производительность
        // и инкрементирует количество прошедших кадров
        public void GraphicalRenderTick() => this.graphicalFPS++;

        // Должен вызываться функцией которая замеряет производительность
        // кадров для логического цикла
        public void LogicalRenderTick() => this.logicalFPS++;

        // Возвращает отрендеренный экран производительности
        public Bitmap RenderPerfScreen()
        {
            gCtx.Clear(Color.Black);

            // Получение значения последнего замеренного FPS
            var graphFPS = graphicalFPSHistory[metricsTime - 1];
            var logicFPS = logicalFPSHistory[metricsTime - 1];
            // Получение значения медианного FPS
            var graphMedianFPS = 0;
            var logicMedianFPS = 0;
            for (var i = 0; i < metricsTime; i++)
            {
                graphMedianFPS += graphicalFPSHistory[i];
                logicMedianFPS += logicalFPSHistory[i];
            }

            graphMedianFPS /= metricsTime;
            logicMedianFPS /= metricsTime;

            // Вывод последнего замеренного значения кадров и медианного
            gCtx.DrawString(
                "GRAPHICAL FPS " + graphFPS.ToString().PadRight(2) + " MEDIAN " + graphMedianFPS.ToString(),
                fontMain,
                brushMain,
                0,
                screen.Height - 30
            );
            gCtx.DrawString(
                "  LOGICAL FPS " + logicFPS.ToString().PadRight(2) + " MEDIAN " + logicMedianFPS.ToString(),
                fontMain,
                brushMain,
                0,
                screen.Height - 15
            );

            // Вывод графика истории FPS
            var graphicalColumns = new RectangleF[metricsTime];
            var logicalColumns = new RectangleF[metricsTime];
            for (var i = 0; i < metricsTime; i++)
            {
                graphicalColumns[i] = new RectangleF(i * 3, 0, 2, graphicalFPSHistory[i]);
                logicalColumns[i] = new RectangleF(screen.Width - metricsTime * 3 + i * 3, 0, 2, logicalFPSHistory[i]);
            }

            gCtx.FillRectangles(brushMain, graphicalColumns);
            gCtx.FillRectangles(brushMain, logicalColumns);

            // Вывод линии медианного FPS
            gCtx.DrawLine(penAccent, 0, graphMedianFPS, metricsTime * 3, graphMedianFPS);
            gCtx.DrawLine(penAccent, screen.Width - metricsTime * 3, logicMedianFPS, screen.Width, logicMedianFPS);

            return this.screen;
        }
    }
}