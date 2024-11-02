using System.Collections.Generic;
using System.Xml.Serialization;
namespace TetrisGame
{
    public abstract class Block
    {
        protected abstract Position[][] Tiles { get; }// mảng hai chiều 

        protected abstract Position StartOffset { get; }// vị trí khởi đầu của khối
        public abstract int Id {get;}

        public int rotationState;
        public Position offset;//lưu trữ vị trí hiện tại của một khối

        public Block()
        {
            offset=new Position(StartOffset.Row, StartOffset.Column);
        }

        public IEnumerable<Position> TilePositions( )
        {
            foreach (Position p in Tiles[rotationState]) 
            {
                yield return new Position(p.Row + offset.Row, p.Column + offset.Column);
            }
        }

        public void RotateCW()//xoay theo chiều kim đồng hồ
        {
            rotationState=(rotationState+1)%Tiles.Length;
        }

        public void RotateCCW()// xoay ngược chiều kim đồng hồ
        {
            if(rotationState==0)
            {
                rotationState=Tiles.Length-1;
            }
            else
            {
                rotationState--;
            }
        }

        public void Move(int rows,int columns)// duy chuyền chuyền và thay đổi vị trí của khối

        {
            offset.Row += rows;
            offset.Column += columns;
        }

        public void Reset()
        {
            rotationState = 0;
            offset.Row = StartOffset.Row;
            offset.Column = StartOffset.Column;
        }

    }
}
