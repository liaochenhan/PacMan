using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

    public enum MonsterColor
    {
        Red,
        Green,
        Blue,
        Magenta,
    }

    public enum State
    {
        Space,
        Wall,
        Bean,
        Player,
    }

    public enum GameState
    {
        Clear,
        Dead,
    }
}
