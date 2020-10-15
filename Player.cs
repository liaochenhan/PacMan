using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class Player : ICreature
    {
        public int PosX { get; set; }                   // 玩家位置坐标
        public int PosY { get; set; }
        
        private Direction direction = Direction.Left;   // 运动方向，初始向左运动
        public Direction MoveDirection
        {
            get{ return direction; }
            set
            {
                switch (value)
                {
                    case Direction.Up:
                        if (MyStage.StageState[PosY - 1, PosX] != State.Wall)
                            direction = value;
                        break;
                    case Direction.Down:
                        if (MyStage.StageState[PosY + 1, PosX] != State.Wall)
                            direction = value;
                        break;
                    case Direction.Left:
                        if (MyStage.StageState[PosY, PosX - 1] != State.Wall)
                            direction = value;
                        break;
                    case Direction.Right:
                        if (MyStage.StageState[PosY, PosX + 1] != State.Wall)
                            direction = value;
                        break;
                }
            }
        }
        
        public int Speed { get; set; }              // 运动速度
        private int Count { get; set; }             // 计数器
        public Stage MyStage { get; set; }          // 玩家所在关卡

        public Player(int posX, int posY, Stage stage, int speed = 1)   // 构造函数
        {
            PosX = posX;
            PosY = posY;
            MyStage = stage;
            Speed = speed;
        }

        public void Move()                          // 移动函数
        {
            if (Count++ % Speed != 0)               // 控制移动速度
                return;

            switch (MoveDirection)
            {
                case Direction.Up:
                    int upIndex = (PosY - 1 + MyStage.Height) % MyStage.Height;     // 超过边界后会出现在另一边的边界
                    if (MyStage.StageState[upIndex, PosX] == State.Wall) break;
                    else
                    {
                        MyStage.StageState[PosY, PosX] = State.Space;               // 移动后更新地图状态，移动前位置变为空格，
                        PosY = upIndex;                                             
                        MyStage.StageState[PosY, PosX] = State.Player;              // 移动后的位置变为玩家
                        break;
                    }
                case Direction.Down:
                    int downIndex = (PosY + 1 + MyStage.Height) % MyStage.Height;
                    if (MyStage.StageState[downIndex, PosX] == State.Wall) break;
                    else
                    {
                        MyStage.StageState[PosY, PosX] = State.Space;
                        PosY = downIndex;
                        MyStage.StageState[PosY, PosX] = State.Player;
                        break;
                    }
                case Direction.Left:
                    int leftIndex = (PosX - 1 + MyStage.Width) % MyStage.Width;
                    if (MyStage.StageState[PosY, leftIndex] == State.Wall) break;
                    else
                    {
                        MyStage.StageState[PosY, PosX] = State.Space;
                        PosX = leftIndex;
                        MyStage.StageState[PosY, PosX] = State.Player;
                        break;
                    }
                case Direction.Right:
                    int rightIndex = (PosX + 1 + MyStage.Width) % MyStage.Width;
                    if (MyStage.StageState[PosY, rightIndex] == State.Wall) break;
                    else
                    {
                        MyStage.StageState[PosY, PosX] = State.Space;
                        PosX = rightIndex;
                        MyStage.StageState[PosY, PosX] = State.Player;
                        break;
                    }
            }
        }
    }
}
