using System.Drawing;
using System.Drawing.Imaging;

namespace CSGame.GamePlugins
{
    // Помощь при работе с изображениями
    // Некоторые функции могут быть достаточно ресурсоёмки и способны обвалить FPS, используйте с умом
    public static class ImageTools
    {
        // Изменяет прозрачность изображения
        public static Bitmap Opacity(Bitmap image, float opacity)
        {
            var newImage = new Bitmap(image.Width, image.Height);
            var gCtx = Graphics.FromImage(newImage);

            // Матрица трансформирования цвета
            ColorMatrix matrix = new ColorMatrix();
            matrix.Matrix33 = opacity;
            ImageAttributes attrs = new ImageAttributes();
            attrs.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            gCtx.DrawImage(
                image,
                new Rectangle(
                    0,
                    0,
                    image.Width,
                    image.Height
                ),
                0,
                0,
                image.Width,
                image.Height,
                GraphicsUnit.Pixel, attrs
            );
            gCtx.Dispose();

            return newImage;
        }
    }
}