using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public interface ICreature
    {
        int PosX { get; set; }      // 生物的坐标
        int PosY { get; set; }
        int Speed { get; set; }     // 移动速度
        Stage MyStage { get; set; } // 所在关卡
        void Move();                // 移动函数
    }
}
