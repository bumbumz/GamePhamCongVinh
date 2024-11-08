﻿using System;

namespace TetrisGame
{
    public class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBLock(),
            new TBlock(),
            new ZBlock(),
        };// chỉ gán cho một đối tượng và ko thể đc thay đổi 

        private readonly Random random= new Random();

        public Block NextBlock { get; private set; }

        public BlockQueue()
        {
            NextBlock = RandomBlock();

        }
        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public Block GetAndUpdate()
        {
            Block block=  NextBlock;

            do
            {
                NextBlock= RandomBlock();
            }
            while (block.Id == NextBlock.Id);
            return block;
        }



    }
}
