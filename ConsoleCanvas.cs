using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class ConsoleCanvas
    {
        // Canvas的尺寸
        public int Height { get; }
        public int Width { get; }
        // 游戏画面缓冲区，分为前缓冲区和后缓冲区
        public char[,] Buffer { get; set; }
        public char[,] BackBuffer { get; }
        // 地图中字符的颜色缓冲区
        public ConsoleColor[,] ColorBuffer { get; set; }
        // 地图空白处对应的字符
        char empty;
        // 构造函数
        public ConsoleCanvas(int height, int width, char _empty = ' ')
        {
            Height = height;
            Width = width;
            empty = _empty;

            Buffer = new char[height, width];
            BackBuffer = new char[height, width];
            ColorBuffer = new ConsoleColor[height, width];

            Console.CursorVisible = false;
            ClearBuffer();
        }
        // 清理前缓冲区和颜色缓冲区
        private void ClearBuffer()
        {
            for (int i = 0; i < Buffer.GetLength(0); i++)
            {
                for (int j = 0; j < Buffer.GetLength(1); j++)
                {
                    Buffer[i, j] = empty;
                    ColorBuffer[i, j] = ConsoleColor.Gray;
                }
            }
        }
        // 将上一帧的图像存储在后缓冲区中，再清理前缓冲区和颜色缓冲区
        public void ClearDoubleBuffer()
        {
            Array.Copy(Buffer, BackBuffer, Height * Width);
            ClearBuffer();
        }
        // 刷新屏幕
        public void Refresh()
        {
            for (int i = 0; i < Height; i++)
            {
                int end = 0;
                // 找到每一行末尾空白前字符的索引，使后续不用处理末尾的空白
                for (int j = Width - 1; j >= 0; j--)
                {
                    // 如果前一帧不是空白也需要处理，因为要覆盖掉前一帧的字符
                    if (Buffer[i, j] != empty || BackBuffer[i, j] != empty)
                    {
                        end = j + 1;
                        break;
                    }
                }

                for (int j = 0; j < end; j++)
                {
                    if (Buffer[i, j] != BackBuffer[i, j])
                    {
                        Console.ForegroundColor = ColorBuffer[i, j];
                        Console.SetCursorPosition(j * 2, i);        // 每个字符占两个空格,保证兼容非ASCII码字符
                        Console.Write(Buffer[i, j]);
                    }
                    
                }
            }
            Console.SetCursorPosition(Width * 2, Height);           // 将光标设置到地图右下角，防止读取的键显示在控制台后覆盖地图
        }

        public void DrawBorder()
        {
            for (int i = 0; i < Height; i++)    // 左边
                Buffer[i, 0] = '*';
            for (int i = 0; i < Height; i++)    // 右边
                Buffer[i, Width - 1] = '*';
            for (int i = 0; i < Width; i++)     // 上边
                Buffer[0, i] = '*';
            for (int i = 0; i < Width; i++)    // 下边
                Buffer[Height - 1, i] = '*';
        }
    }
}
