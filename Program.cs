using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class Program
    {
        private ConsoleCanvas canva = null;             // 画布
        private List<Stage> stages = new List<Stage>(); // 关卡

        public delegate void StartGame();               
        public delegate void FinishGame();              
        StartGame StartGameFun;                         // 游戏开始前UI
        public Dictionary<GameState, StartGame> end = new Dictionary<GameState, StartGame>();   // 游戏结束后UI

        public void InitStage(string[] stageFile)       // 加载关卡，传入关卡数据文件名
        {
            StartGameFun = Start;
            end.Add(GameState.Dead, Dead);
            end.Add(GameState.Clear, Clear);
            foreach (string fileName in stageFile)
            {
                stages.Add(new Stage(fileName));
            }
        }

        public void Run()                               // 运行游戏
        {
            StartGameFun();
            Console.Clear();
            foreach (Stage stage in stages)             // 运行每一个关卡
            {
                GameLoop(stage);
                Console.Clear();
                end[stage.Flag]();
            }
        }

        private void Start()
        {
            canva = new ConsoleCanvas(27, 21);
            canva.ClearDoubleBuffer();
            canva.DrawBorder();
            string title = "PACMAN";
            string str1 = "按任意键开始游戏";
            for (int i = 0; i < title.Length; ++i)
                canva.Buffer[canva.Height / 2, canva.Width / 2 - 3 + i] = title[i];
            for (int i = 0; i < str1.Length; ++i)
                canva.Buffer[canva.Height / 2 + 1, canva.Width / 2 - 4 + i] = str1[i];
            canva.Refresh();

            Console.ReadKey();
        }

        private void Dead()
        {
            canva = new ConsoleCanvas(27, 21);
            canva.ClearDoubleBuffer();
            canva.DrawBorder();
            string title = "你死了";
            string str1 = "按任意键结束游戏";
            for (int i = 0; i < title.Length; ++i)
                canva.Buffer[canva.Height / 2, canva.Width / 2 - 1 + i] = title[i];
            for (int i = 0; i < str1.Length; ++i)
                canva.Buffer[canva.Height / 2 + 1, canva.Width / 2 - 4 + i] = str1[i];
            canva.Refresh();

            Console.ReadKey();
        }

        private void Clear()
        {
            canva = new ConsoleCanvas(27, 21);
            canva.ClearDoubleBuffer();
            canva.DrawBorder();
            string title = "恭喜你过关";
            string str1 = "按任意键结束游戏";
            for (int i = 0; i < title.Length; ++i)
                canva.Buffer[canva.Height / 2, canva.Width / 2 - 2 + i] = title[i];
            for (int i = 0; i < str1.Length; ++i)
                canva.Buffer[canva.Height / 2 + 1, canva.Width / 2 - 4 + i] = str1[i];
            canva.Refresh();

            Console.ReadKey();
        }

        private Direction Input()                   // 输入函数
        {
            var key = Console.ReadKey();
  
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    return Direction.Up;
                case ConsoleKey.DownArrow:
                    return Direction.Down;
                case ConsoleKey.LeftArrow:
                    return Direction.Left;
                case ConsoleKey.RightArrow:
                    return Direction.Right;
            }
            return Direction.None;
        }

        private void GameLoop(Stage stage)                              // 游戏循环
        {
            canva = new ConsoleCanvas(stage.Height, stage.Width);       // 创建画布
            stage.AddCreatures(new Player(10, 20, stage, 2));
            stage.AddCreatures(new Monster(10, 13, stage, MonsterColor.Red, 3));
            stage.AddCreatures(new Monster(10, 13, stage, MonsterColor.Green, 3));
            stage.AddCreatures(new Monster(10, 10, stage, MonsterColor.Blue, 3));
            stage.AddCreatures(new Monster(10, 13, stage, MonsterColor.Magenta, 3));

            while (!stage.CheckFinished())
            {
                Direction dir = Direction.None;
                while (Console.KeyAvailable)                            // 一帧内的输入只有最后一个有效
                {
                    dir = Input();
                }
                stage.Pacman.MoveDirection = dir;       

                stage.Update();                                         // 关卡状态更新

                canva.ClearDoubleBuffer();                              // 清理缓冲区
                for (int i = 0; i < stage.Height; i++)                  // 更新地图格子
                {
                    for (int j = 0; j < stage.Width; j++)
                    {
                        switch (stage.StageState[i, j])
                        {
                            case State.Wall: canva.Buffer[i, j] = '■'; break;
                            case State.Space: canva.Buffer[i, j] = ' '; break;
                            case State.Player: canva.Buffer[i, j] = 'P'; break;
                            case State.Bean: canva.Buffer[i, j] = '·'; break;
                        }
                    }
                }
                foreach (Monster monster in stage.monsters)
                    canva.Buffer[monster.PosY, monster.PosX] = 'M';

                Array.Copy(stage.ColorData, canva.ColorBuffer, stage.Height * stage.Width); // 更新地图格子颜色
                canva.Refresh();                                        // 渲染
                Thread.Sleep(100);
            }
        }
        
        public static void Main(string[] args)
        {
            try
            {
                Program game = new Program();
                string[] stageFiles = { "stageData.txt" };
                game.InitStage(stageFiles);
                game.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }
}
