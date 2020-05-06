using System;
using System.Net;
using System.Net.Sockets;

namespace Learn
{
    class Server
    {
        private NetworkStream[] streams = new NetworkStream[Game.PlayersCount];
        private TcpListener server;
        private Game game;
        private Int32 port = 13000;
        private int winner = -1;
        byte[] winnerBuffer;
        int play;
        byte[] playBuffer = new byte[Game.PlaysCount];
        private long[] gameAttributes = new long[Player.AttributesCount * Game.PlayersCount];
        private byte[] attributesBuffer = new byte[Player.AttributesCount * Game.PlayersCount * 8];
        private TcpClient[] clients = new TcpClient[Game.PlayersCount];
        private IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        private void fillGameAttributes()
        {
            for (int i = 0; i < gameAttributes.Length; i++)
            {
                gameAttributes[i] = game.GetPlayerAttribute(i / (gameAttributes.Length / Game.PlayersCount), i % Player.AttributesCount);
            }
            int offset = 0;
            foreach (long attribute in gameAttributes) 
            {
                byte[] buffer = BitConverter.GetBytes(attribute);
                if (BitConverter.IsLittleEndian) 
                {
                    Array.Reverse(buffer);
                }
                buffer.CopyTo(attributesBuffer, offset);
                offset += 8;
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
        public Server()
        {
            server = new TcpListener(localAddress, port);
            Byte[] bytes = new Byte[256];
            server.Start();
            Console.WriteLine("Waiting for a connection... ");
            for (int playerIndex = 0; playerIndex < Game.PlayersCount; playerIndex++)
            {
                clients[playerIndex] = server.AcceptTcpClient();
                streams[playerIndex] = clients[playerIndex].GetStream();
                Console.WriteLine("Player index {0} connected", playerIndex);
            }
            game = new Game();
            fillGameAttributes();
            printGameAttributes();
            winnerBuffer = BitConverter.GetBytes(winner);
            if (BitConverter.IsLittleEndian) 
            {
                Array.Reverse(winnerBuffer);
            }
            for (int playerIndex = 0; playerIndex < Game.PlayersCount; playerIndex++)
            {
                byte[] playerIndexBuffer = BitConverter.GetBytes(playerIndex);
                if (BitConverter.IsLittleEndian) 
                {
                    Array.Reverse(playerIndexBuffer);
                }
                streams[playerIndex].Write(playerIndexBuffer, 0, playerIndexBuffer.Length);
                streams[playerIndex].Write(winnerBuffer, 0, winnerBuffer.Length);
                streams[playerIndex].Write(attributesBuffer, 0, attributesBuffer.Length);
            }
            int currentPLayerTurn = 0;
        Play:    
            streams[currentPLayerTurn].Read(playBuffer, 0, playBuffer.Length);
            if (BitConverter.IsLittleEndian) 
            {
                Array.Reverse(playBuffer);
            }
            play = BitConverter.ToInt32(playBuffer, 0);
            winner = game.Play(play);
            winnerBuffer = BitConverter.GetBytes(winner);
            if (BitConverter.IsLittleEndian) 
            {
                Array.Reverse(winnerBuffer);
            }
            fillGameAttributes();
            playBuffer = BitConverter.GetBytes(play);
            if (BitConverter.IsLittleEndian) 
            {
                Array.Reverse(playBuffer);
            }
            for (int playerIndex = 0; playerIndex < Game.PlayersCount; playerIndex++)
            {
                streams[playerIndex].Write(playBuffer, 0, playBuffer.Length);
                streams[playerIndex].Write(winnerBuffer, 0, winnerBuffer.Length);
                streams[playerIndex].Write(attributesBuffer, 0, attributesBuffer.Length);
            }
            currentPLayerTurn = (currentPLayerTurn + 1) % Game.PlayersCount; 
            if (winner >= 0)
            {
                Console.WriteLine("Winner: {0}", winner);
                goto Exit;
            }
            else
            {
                goto Play;
            }
        Exit:
            for (int playerIndex = 0; playerIndex < Game.PlayersCount; playerIndex++)
            {
                clients[playerIndex].Close();
            }
            server.Stop();
        }
    }
}
