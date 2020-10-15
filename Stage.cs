using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class Stage
    {
        public State[,] StageState { get; set; }                    // 关卡数据
        public ConsoleColor[,] ColorData { get; set; }              // 颜色数据
        public GameState Flag { get; set; }
        private string stageFile;                                   // 关卡数据文件名

        public int Height { get; set; }                             // 关卡尺寸
        public int Width { get; set; }

        public Player Pacman { get; private set; }                  // 玩家
        public List<Monster> monsters = new List<Monster>();       // 地图中所有怪物

        public Stage(string _stageFile)
        {
            stageFile = _stageFile;
            LoadMap();
        }
        
        private void LoadMap()                                      // 加载关卡
        {
            using (StreamReader sr = new StreamReader(stageFile))   // 从文件中读取数据
            {
                string temp;
                string data = string.Empty;
                while ((temp = sr.ReadLine()) != null)
                {
                    Width = temp.Length;
                    Height++;
                    data += temp;
                }

                StageState = new State[Height, Width];
                ColorData = new ConsoleColor[Height, Width];
                for (int i = 0; i < Height; i++)                    // 设置关卡状态
                {
                    for (int j = 0; j < Width; j++)
                    {
                        char ch = data[i * Width + j];
                        if (ch == '*')
                            StageState[i, j] = State.Wall;
                        else if (ch == '.')
                            StageState[i, j] = State.Bean;
                        else
                            StageState[i, j] = State.Space;
                    }
                }
            }
        }
        
        public void AddCreatures(ICreature creature)        // 添加生物
        {
            if (creature is Player)
            {
                Pacman = creature as Player;
                StageState[Pacman.PosY, Pacman.PosX] = State.Player;
            }
            else
                monsters.Add(creature as Monster);
        }

        private void ClearColorData()
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    ColorData[i, j] = ConsoleColor.Gray;
        }

        public void Update()                                // 更新地图
        {
            ClearColorData();
            Pacman.Move();                                  // 更新玩家位置
            
            for (int i = 0; i < Height; i++)                // 更新地图颜色状态
            {
                for (int j = 0; j < Width; j++)
                {
                    switch (StageState[i, j])
                    {
                        case State.Wall:
                        case State.Space: ColorData[i, j] = ConsoleColor.Gray; break;
                        case State.Player: ColorData[i, j] = ConsoleColor.Yellow; break;
                        case State.Bean: ColorData[i, j] = ConsoleColor.Green; break;
                    }
                }
            }

            foreach (Monster monster in monsters)           // 更新怪物位置和对应地图颜色状态
            {
                monster.Move();
                switch (monster.Color)
                {
                    case MonsterColor.Red: ColorData[monster.PosY, monster.PosX] = ConsoleColor.Red; break;
                    case MonsterColor.Green: ColorData[monster.PosY, monster.PosX] = ConsoleColor.Green; break;
                    case MonsterColor.Blue: ColorData[monster.PosY, monster.PosX] = ConsoleColor.Blue; break;
                    case MonsterColor.Magenta: ColorData[monster.PosY, monster.PosX] = ConsoleColor.Magenta; break;
                }
            }
        }

        public bool CheckFinished()                         // 关卡是否结束
        {
            bool flag1 = false;
            bool flag2 = true;
            foreach (var monster in monsters)               // 如果玩家和怪物的位置重叠，则游戏结束
            {
                if (Pacman.PosX == monster.PosX && Pacman.PosY == monster.PosY)
                {
                    flag1 = true;
                    Flag = GameState.Dead;
                }
            }

            for (int i = 0; i < Height; i++)                // 如果地图中没有豆子，则游戏结束
            {
                for (int j = 0; j < Width; j++)
                {
                    if (StageState[i, j] == State.Bean)
                        flag2 = false;
                }
            }
            if (flag2 == true) Flag = GameState.Clear;

            return flag1 || flag2;
        }
    }
}
