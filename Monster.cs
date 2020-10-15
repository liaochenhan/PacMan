using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class Monster : ICreature
    {
        public int PosX { get; set; }                   // 生物的位置
        public int PosY { get; set; }
        public int Speed { get; set; }                  // 移动速度
        private int Count { get; set; }                 // 计数器
        public Stage MyStage { get; set; }              // 所在关卡
        private Direction direction = Direction.Left;   // 运动方向

        delegate void MonsterMove();                    // 怪物移动函数字典，不同颜色怪物的移动行为不同
        private Dictionary<MonsterColor, MonsterMove> moves 
            = new Dictionary<MonsterColor, MonsterMove>();
        
        public MonsterColor Color { get; set; }         // 怪物颜色

        public Monster(int posX, int posY, Stage stage, MonsterColor color, int speed = 1)  // 构造函数
        {
            PosX = posX;
            PosY = posY;
            MyStage = stage;
            Speed = speed;
            Color = color;

            moves.Add(MonsterColor.Red, RedMove);
            moves.Add(MonsterColor.Green, GreenMove);
            moves.Add(MonsterColor.Blue, BlueMove);
            moves.Add(MonsterColor.Magenta, MagentaMove);
        }

        private void AStarFind(Node origin, Node dest)                      // A*寻路，起点是玩家，终点是怪物
        {
            AStarContainer container = new AStarContainer(origin, dest);    // 要保证origin和dest不重合
            while (true)
            {
                Node node = container.GetMinNode();                         // 获取openList中F最小的结点，并加入closeList

                int up = node.PosY - 1;
                int down = node.PosY + 1;
                int left = node.PosX - 1;
                int right = node.PosX + 1;
                Node nextNode;                                                      // 向openList中加入周围结点nextNode
                if (up >= 0 && MyStage.StageState[up, node.PosX] != State.Wall &&   // 要满足三个条件
                    !container.CheckContained(nextNode = new Node(node.PosX, up)))  // 分别是边界条件，周围结点不能是墙，结点不包含在closeList中
                {                                                                   
                    container.PushOpenList(nextNode);
                    if (nextNode == dest) break;                                    // 当周围结点包括了终点时，则完成寻路
                }
                if (down < MyStage.Height && MyStage.StageState[down, node.PosX] != State.Wall &&
                    !container.CheckContained(nextNode = new Node(node.PosX, down)))
                {
                    container.PushOpenList(nextNode);
                    if (nextNode == dest) break;
                }
                if (left >= 0 && MyStage.StageState[node.PosY, left] != State.Wall &&
                    !container.CheckContained(nextNode = new Node(left, node.PosY)))
                {
                    container.PushOpenList(nextNode);
                    if (nextNode == dest) break;
                }
                if (right < MyStage.Width && MyStage.StageState[node.PosY, right] != State.Wall &&
                    !container.CheckContained(nextNode = new Node(right, node.PosY)))
                {
                    container.PushOpenList(nextNode);
                    if (nextNode == dest) break;
                }
            }

            Node next = container.DestNode.Parent;  // 获取怪物要运动到的下一个结点
            switch (next.PosX - dest.PosX)          // 确定怪物的运动方向
            {
                case 1: direction = Direction.Right; break;
                case -1: direction = Direction.Left; break;
            }
            switch (next.PosY - dest.PosY)
            {
                case 1: direction = Direction.Down; break;
                case -1: direction = Direction.Up; break;
            }
        }
        
        private void RedMove()          // 红色怪物移动函数，以最短路径追玩家
        {
            if (Count++ % Speed != 0)   // 控制移动速度
                return;

            Node player = new Node(MyStage.Pacman.PosX, MyStage.Pacman.PosY);
            Node monster = new Node(PosX, PosY);
            AStarFind(player, monster);
            switch (direction)
            {
                case Direction.Up: PosY--; break;
                case Direction.Down: PosY++; break;
                case Direction.Left: PosX--; break;
                case Direction.Right: PosX++; break;
            }
        }

        private void GreenMove()        // 绿色怪物移动函数，以最短路径追击玩家，当靠近玩家时散开
        {
            if (Count++ % Speed != 0)   // 控制移动速度
                return;

            Node dest = new Node(MyStage.Pacman.PosX, MyStage.Pacman.PosY);
            if (Math.Abs(PosX - MyStage.Pacman.PosX) < 6 && Math.Abs(PosY - MyStage.Pacman.PosY) < 6)           // 如果怪物已接近玩家附近
                                                                                                                // 区域，则向角落移动
            {
                Node corner;
                if (MyStage.Pacman.PosX < MyStage.Width / 2 && MyStage.Pacman.PosY < MyStage.Height / 2)        // 玩家处于左上角区域
                    corner = new Node(MyStage.Width - 2, MyStage.Height - 2);
                else if (MyStage.Pacman.PosX < MyStage.Width / 2 && MyStage.Pacman.PosY > MyStage.Height / 2)   // 玩家处于左下角区域
                    corner = new Node(MyStage.Width - 2, 1);
                else if (MyStage.Pacman.PosX > MyStage.Width / 2 && MyStage.Pacman.PosY < MyStage.Height / 2)   // 玩家处于右上角区域
                    corner = new Node(1, MyStage.Height - 2);
                else                                                                                            // 玩家处于右下角区域
                    corner = new Node(1, 1);
                dest = corner;
            }

            Node monster = new Node(PosX, PosY);
            AStarFind(dest, monster);
            switch (direction)
            {
                case Direction.Up: PosY--; break;
                case Direction.Down: PosY++; break;
                case Direction.Left: PosX--; break;
                case Direction.Right: PosX++; break;
            }
        }

        private void BlueMove()         // 蓝色怪物移动函数，随机移动，直到碰到墙后改变移动方向
        {
            if (Count++ % Speed != 0)   // 控制移动速度
                return;

            if (CanMove(direction))     // 判断是否能移动
            {
                switch (direction)
                {
                    case Direction.Up: PosY--; break;
                    case Direction.Down: PosY++; break;
                    case Direction.Left: PosX--; break;
                    case Direction.Right: PosX++; break;
                }
            }
            else
            {
                Direction dir = Direction.None;
                do
                {
                    Random r = new Random();
                    int corner = r.Next() % 4;
                    switch (corner)
                    {
                        case 0: dir = Direction.Up; break;
                        case 1: dir = Direction.Down; break;
                        case 2: dir = Direction.Left; break;
                        case 3: dir = Direction.Right; break;
                    }

                } while (!CanMove(dir));
                direction = dir;
            }
        }

        private void MagentaMove()      // 粉色怪物移动函数，随机瞄准玩家周围4个角落追击
        {
            if (Count++ % Speed != 0)   // 控制移动速度
                return;

            Node monster = new Node(PosX, PosY);

            int destX = MyStage.Pacman.PosX;
            int destY = MyStage.Pacman.PosY;
            Node dest = new Node(destX, destY);

            Random r = new Random();
            int corner = r.Next() % 4;
            switch (corner)
            {
                case 0:
                    destX = Clamp(1, MyStage.Width - 2, destX - 6);
                    destY = Clamp(1, MyStage.Height - 2, destY - 6);
                    break;
                case 1:
                    destX = Clamp(1, MyStage.Width - 2, destX + 6);
                    destY = Clamp(1, MyStage.Height - 2, destY - 6);
                    break;
                case 2:
                    destX = Clamp(1, MyStage.Width - 2, destX + 6);
                    destY = Clamp(1, MyStage.Height - 2, destY + 6);
                    break;
                case 3:
                    destX = Clamp(1, MyStage.Width - 2, destX - 6);
                    destY = Clamp(1, MyStage.Height - 2, destY + 6);
                    break;
            }
            
            if (MyStage.StageState[destY, destX] != State.Wall && destX != PosX && destY != PosY) // 如果计算的目标是墙，那么dest结点仍然为玩家
                dest = new Node(destX, destY);

            AStarFind(dest, monster);
            switch (direction)
            {
                case Direction.Up: PosY--; break;
                case Direction.Down: PosY++; break;
                case Direction.Left: PosX--; break;
                case Direction.Right: PosX++; break;
            }
        }
        
        private int Clamp(int min, int max, int param)  // 钳制函数
        {
            if (param < min) return min;
            else if (param > max) return max;
            else return param;
        }

        private bool CanMove(Direction dir)             // 判断怪物是否能移动
        {
            bool move = true;
            switch (dir)
            {
                case Direction.Up:
                    if (PosY - 1 < 0 || MyStage.StageState[PosY - 1, PosX] == State.Wall) move = false; break;
                case Direction.Down:
                    if (PosY + 1 >= MyStage.Height || MyStage.StageState[PosY + 1, PosX] == State.Wall) move = false; break;
                case Direction.Left:
                    if (PosX - 1 < 0 || MyStage.StageState[PosY, PosX - 1] == State.Wall) move = false; break;
                case Direction.Right:
                    if (PosX + 1 >= MyStage.Width || MyStage.StageState[PosY, PosX + 1] == State.Wall) move = false; break;
                case Direction.None: move = false; break;
            }
            return move;
        }

        public void Move()          // 移动行为      
        {
            moves[Color]();         // 调用对应颜色的怪物移动函数
        }
    }
}
