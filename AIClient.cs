using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
namespace Learn
{
    class MemoryBlock
    {
        private int play;
        public int Play 
        {
            get {return play;}
        }
        private int turn;
        public int Turn 
        {
            get {return turn;}
        }
        private int winner;
        public int Winner 
        {
            get {return winner;}
            set {winner = value;}
        }
        private long[,] attributes;
        public long[,] Attributes
        {
            get {return attributes;}
        }
        public MemoryBlock(int turn, int winner, int play, long[] gameAttributes)
        {
            this.turn = turn;
            this.winner = winner;
            this.play = play;
            attributes = new long[Game.PlayersCount, Player.AttributesCount];
            for (int playerIndex = 0; playerIndex < Game.PlayersCount; playerIndex++)
            {
                for (int attributeIndex = 0; attributeIndex < Player.AttributesCount; attributeIndex++)
                {
                    int index = playerIndex * Player.AttributesCount + attributeIndex;
                    attributes[playerIndex, attributeIndex] =  gameAttributes[index];
                }
            }
        }
    }
    class AIClient
    {
        List<MemoryBlock> memory = new List<MemoryBlock>();
        List<MemoryBlock> gameMemory = new List<MemoryBlock>();
        Random random = new Random();
        private string aIName;
        private TcpClient client;
        private NetworkStream stream;
        private long[] gameAttributes = new long[Player.AttributesCount * Game.PlayersCount];
        private byte[] playerIndexBuffer = new byte[4];
        private int playerIndex;
        private byte[] playBuffer = new byte[4];
        private int play;
        private int turn = 0;
        private int winner;
        private byte[] winnerBuffer = new byte[4];
        private bool isMyTurn = false;
        private byte[] attributesBuffer = new byte[Player.AttributesCount * Game.PlayersCount * 8];
        private void fillGameAttributes()
        {
            for (int i = 0; i < gameAttributes.Length; i++) 
            {
                byte[] longBuffer = new byte[8];
                for (int j = 0; j < 8; j++)
                {
                    longBuffer[j] = attributesBuffer[j + i * 8];
                }
                if (BitConverter.IsLittleEndian) 
                {
                    Array.Reverse(longBuffer);
                }
                gameAttributes[i] = BitConverter.ToInt64(longBuffer, 0);
            }
        }
        private void printGameAttributes()
        {
            Console.WriteLine("Game attributes:");
            for (int player = 0; player < Game.PlayersCount; player++)
            {
                Console.WriteLine("Player {0} attributes:", player);
                for (int attribute = 0; attribute < Player.AttributesCount; attribute++)
                {      
                    Console.WriteLine("attribute no.{0} = {1}", attribute, gameAttributes[player * Player.AttributesCount + attribute]);
                }
            }
        }
        public int Play()
        {
            if (memory.Count == 0)
            {
                return random.Next(Game.PlaysCount);
            }
            long[] weights = new long[Game.PlaysCount];
            long w;
            foreach (MemoryBlock memoryBlock in memory)
            {
                if (playerIndex == 0)
                {
                    if (memoryBlock.Turn % 2 == playerIndex)
                    {
                        if (memoryBlock.Winner == 0) 
                        {
                            w = 4;
                        }
                        else if (memoryBlock.Winner == 1)
                        {
                            w = -4;
                        }
                        else
                        {
                            w = -1;
                        }
                        for (int j = 0; j < Player.AttributesCount; j++)
                        {
                            weights[memoryBlock.Play] += w*256/(Math.Distance(memoryBlock.Attributes[0, j], gameAttributes[j])+512);
                            weights[memoryBlock.Play] += w*256/(Math.Distance(memoryBlock.Attributes[1, j], gameAttributes[j+Player.AttributesCount])+512);
                        }
                    }
                }
                else
                {
                    if (memoryBlock.Turn % 2 == playerIndex)
                    {
                        if (memoryBlock.Winner == 0) 
                        {
                            w = -4;
                        }
                        else if (memoryBlock.Winner == 1)
                        {
                            w = 4;
                        }
                        else
                        {
                            w = -1;
                        }
                        for (int j = 0; j < Player.AttributesCount; j++)
                        {
                            weights[memoryBlock.Play] += w*256/(Math.Distance(memoryBlock.Attributes[0, j], gameAttributes[j])+512);
                            weights[memoryBlock.Play] += w*256/(Math.Distance(memoryBlock.Attributes[1, j], gameAttributes[j+Player.AttributesCount])+512);
                        }
                    }
                }
                  
            }
            int bestPlay = 0;
            for (int i = 0; i < Game.PlaysCount; i++)
            {
                if (weights[bestPlay] < weights[i])
                {
                    bestPlay = i;
                }
            }
            return bestPlay;
        }
        private void SavePlay()
        {
            MemoryBlock memoryBlock = new MemoryBlock(turn, winner, play, gameAttributes);
            gameMemory.Add(memoryBlock);
        }
        private void LoadMemory()
        {
            try
            {

                using (FileStream stream = new FileStream(aIName+".memory", FileMode.Open, FileAccess.Read))
                {

                    byte[] turnBuffer = new byte[4];
                Read:
                    int result = stream.Read(winnerBuffer, 0, winnerBuffer.Length);
                    if (result == 0)
                    {
                        return;
                    }
                    stream.Read(turnBuffer, 0, turnBuffer.Length);
                    stream.Read(playBuffer, 0, playBuffer.Length);
                    stream.Read(attributesBuffer, 0, attributesBuffer.Length);
                    if (BitConverter.IsLittleEndian) 
                    {
                        Array.Reverse(winnerBuffer);
                        Array.Reverse(turnBuffer);
                        Array.Reverse(playBuffer);
                    }
                    winner = BitConverter.ToInt32(winnerBuffer, 0);
                    turn = BitConverter.ToInt32(turnBuffer, 0);
                    play = BitConverter.ToInt32(playBuffer, 0);
                    fillGameAttributes();
                    memory.Add(new MemoryBlock(turn, winner, play, gameAttributes));
                    goto Read;
                    // Read may return anything from 0 to numBytesToRead.
                    //int n = stream.Read(bytes, numBytesRead, numBytesToRead);
                }
            }
            catch (FileNotFoundException ioEx)
            {
                Console.WriteLine(ioEx.Message);
            }
        }
        private void SaveGame()
        {
            foreach (MemoryBlock memoryBlock in gameMemory)
            {
                memoryBlock.Winner = winner;
            }
            using (var stream = new FileStream(aIName+".memory", FileMode.Append))
            {
                byte[] buffer;
                foreach (MemoryBlock memoryBlock in gameMemory)
                {
                    buffer = BitConverter.GetBytes(memoryBlock.Winner);
                    if (BitConverter.IsLittleEndian) {Array.Reverse(buffer);}
                    stream.Write(buffer, 0, buffer.Length);

                    buffer = BitConverter.GetBytes(memoryBlock.Turn);
                    if (BitConverter.IsLittleEndian) {Array.Reverse(buffer);}
                    stream.Write(buffer, 0, buffer.Length);

                    buffer = BitConverter.GetBytes(memoryBlock.Play);
                    if (BitConverter.IsLittleEndian) {Array.Reverse(buffer);}
                    stream.Write(buffer, 0, buffer.Length);

                    foreach (long attribute in memoryBlock.Attributes)
                    {
                        buffer = BitConverter.GetBytes(attribute);
                        if (BitConverter.IsLittleEndian) {Array.Reverse(buffer);}
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }
        public AIClient(string server, string aIName)
        {
            this.aIName = aIName;
            
            Console.WriteLine("Connecting to server: {0} ...", server);
            Int32 port = 13000;
            client = new TcpClient(server, port);
            stream = client.GetStream();
            stream.Read(playerIndexBuffer, 0, playerIndexBuffer.Length);
            if (BitConverter.IsLittleEndian) 
            {
                Array.Reverse(playerIndexBuffer);
            }
            playerIndex = BitConverter.ToInt32(playerIndexBuffer, 0);
            Console.WriteLine("Player index: {0}", playerIndex);
            LoadMemory();
            turn = 0;
            if (playerIndex == 0)
            {
                isMyTurn = true;
            }
            stream.Read(attributesBuffer, 0, attributesBuffer.Length);
            fillGameAttributes();
            printGameAttributes();
        Play:
            Console.WriteLine("Turn: {0}", turn);
            if (isMyTurn)
            {
                play = Play();
                byte[] playBuffer = BitConverter.GetBytes(play);
                if (BitConverter.IsLittleEndian) 
                {
                    Array.Reverse(playBuffer);
                }
                stream.Write(playBuffer, 0, playBuffer.Length);
            }
            else
            {
                Console.WriteLine("Waiting enemys play...");
            }
            stream.Read(playBuffer, 0, playBuffer.Length);
            stream.Read(winnerBuffer, 0, winnerBuffer.Length);
            stream.Read(attributesBuffer, 0, attributesBuffer.Length);
            if (BitConverter.IsLittleEndian) 
            {
                Array.Reverse(winnerBuffer);
                Array.Reverse(playBuffer);
            }
            winner = BitConverter.ToInt32(winnerBuffer, 0);
            play = BitConverter.ToInt32(playBuffer, 0);
            fillGameAttributes();
            SavePlay();
            if (!isMyTurn)
            {
                Console.WriteLine("Enemy's play: {0}", play);
            }
            printGameAttributes();
            if (winner > 0)
            {
                if (winner < Game.PlayersCount)
                {
                    Console.WriteLine("Game winner: player {0}", winner);
                }
                else
                {
                    Console.WriteLine("Game ended on draw, no one winnned");
                }
                goto Exit;
            }
            isMyTurn = !isMyTurn;
            turn++;
            goto Play;
        Exit:
            SaveGame();
            stream.Close();    
            client.Close();
        }
    }
}
