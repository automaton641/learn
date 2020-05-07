using System;
using System.Net.Sockets;

namespace Learn
{
    class Client
    {
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
        public Client(string server)
        {
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
            Choose:
                Console.WriteLine("Choose a play:");
                for (int playIndex = 0; playIndex < Game.PlaysCount; playIndex++)
                {
                    Console.WriteLine("\tplay: {0}", playIndex);
                }
                Console.Write("Your play: ");
                string playString = Console.ReadLine();
                try
                {
                    play = Int32.Parse(playString);
                    if (play < 0 || play >= Game.PlaysCount)
                    {
                        Console.WriteLine("Play must be a number between 0 and {0} inclusive, try again...", Game.PlaysCount-1);
                        goto Choose;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Play must be a number between 0 and {0} inclusive, try again...", Game.PlaysCount-1);
                    goto Choose;
                }
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
            stream.Close();    
            client.Close();
        }
    }
}
