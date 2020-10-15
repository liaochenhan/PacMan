using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class Node : IComparable<Node>                   // A*寻路过程中所需的结点
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F { get; set; }
        public Node Parent { get; set; } = null;

        public Node(int posX, int posY)
        {
            PosX = posX;
            PosY = posY;
        }

        public int CompareTo(Node node)
        {
            return F - node.F;
        }
        public static bool operator==(Node node1, Node node2)
        {
            return node1.PosX == node2.PosX && node1.PosY == node2.PosY;
        }
        public static bool operator!=(Node node1, Node node2)
        {
            return !(node1 == node2);
        }
    }
    class AStarContainer
    {
        public Node DestNode { get; set; }
        private List<Node> closeList = new List<Node>();
        private LinkedList<Node> openList = new LinkedList<Node>();

        public AStarContainer(Node origin, Node dest)
        {
            DestNode = dest;

            origin.Parent = null;
            origin.G = 0;
            origin.H = Math.Abs(DestNode.PosX - origin.PosX) +
                Math.Abs(DestNode.PosY - origin.PosY);
            origin.F = origin.G + origin.H;
            openList.AddLast(origin);
        }
        public void PushOpenList(Node node) // 将结点加入openList
        {
            node.Parent = closeList.Last();
            node.G = node.Parent.G + 1;
            node.H = Math.Abs(DestNode.PosX - node.PosX) +
                Math.Abs(DestNode.PosY - node.PosY);
            node.F = node.G + node.H;
            if (node == DestNode)           // 如果该结点坐标与终点重合，则赋值给终点
                DestNode = node;
            openList.AddLast(node);
        }
        public Node GetMinNode()                 // 从openList中获取F值最小的结点，并加入closeList中
        {
            Node min = openList.Min();
            closeList.Add(min);
            openList.Remove(min);
            return min;
        }
        public bool CheckContained(Node node)   // 判断closeList是否包含给定Node
        {
            foreach (Node temp in closeList)
            {
                if (node == temp)
                    return true;
            }
            return false;
        }
    }
}
