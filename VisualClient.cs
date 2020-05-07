using System;
using System.Collections.Generic;
using Senses;
using System.Net.Sockets;
using System.Threading;

namespace Learn
{
    class VisualClient
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
        private Application application;
        private Window window;
        private TextVisual text;
        private TextInput input;
        private Theme themeA;
        private Container mainView;
        TextVisual turnVisual;
        TextVisual indication;
        TextVisual lastPlay;
        Thread gameLogicThread;
        public void OnPlay1Clicked(object sender, EventArgs e)
        {
            if (isMyTurn && play < 0)
            {
                play = 0;
            }
        }
        public void OnPlay2Clicked(object sender, EventArgs e)
        {
            if (isMyTurn && play < 0)
            {
                play = 1;
            }
        }
        public void OnPlay3Clicked(object sender, EventArgs e)
        {
            if (isMyTurn && play < 0)
            {
                play = 2;
            }
        }
        public void OnPlay4Clicked(object sender, EventArgs e)
        {
            if (isMyTurn && play < 0)
            {
                play = 3;
            }
        }
        public void GameLoop()
        {
            int lastPlayNumber = -1;
            printGameAttributes();
            if (playerIndex == 0)
            {
                isMyTurn = true;
            }
        Play:
            play = -1;
            turnVisual.Text = "turn " + turn.ToString();
            if (isMyTurn)
            {
                indication.Text = "Your turn";
                while (play < 0){
                    Thread.Sleep(16);
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
                indication.Text = "Enemy's turn";
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
            string lastPlayText = "Last plays: " + (play + 1).ToString()+", ";
            if (lastPlayNumber < 0)
            {
                lastPlayText += "none";
            }
            else
            {
                lastPlayText += (lastPlayNumber + 1).ToString();
            }
            lastPlayNumber = play;
            lastPlay.Text = lastPlayText;
            if (winner >= 0)
            {
                if (winner < Game.PlayersCount)
                {
                    Console.WriteLine("Game winner: player {0}", winner);
                    indication.Text = "Player " + (winner+1).ToString() + " wins";

                }
                else
                {
                    Console.WriteLine("Game ended on draw, no one winnned");
                    indication.Text = "Draw";
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
        public void OnConnectClicked(object sender, EventArgs e)
        {
            text.Text = "Connecting...";
            Console.WriteLine("OnConnectClicked");
            Console.WriteLine("Connecting to server: {0} ...", input.Input);
            Int32 port = 13000;
            client = new TcpClient(input.Input, port);

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
            mainView = new Container(Orientation.Vertical);
            window.Container = mainView;
            for (int i = 0; i < 4; i++)
            {
                window.Container.Add(new Container(Orientation.Horizontal));
            }
            Container dummy;
            dummy = (Container)window.Container.Visuals[0];
            dummy.Add(new TextVisual("Player: " + (playerIndex + 1).ToString()));
            turnVisual = new TextVisual("Turn: " + turn);
            dummy.Add(turnVisual);
            indication = new TextVisual("Indications");
            dummy.Add(indication);
            lastPlay = new TextVisual("Last plays: none, none");
            dummy.Add(lastPlay);
            dummy = (Container)window.Container.Visuals[1];
            for (int i = 0; i < Player.AttributesCount * Game.PlayersCount; i++)
            {
                dummy.Add(new Bar(1.0));
            }
            dummy = (Container)window.Container.Visuals[2];
            for (int i = 0; i < Player.AttributesCount * Game.PlayersCount; i++)
            {
                dummy.Add(new TextVisual(32.ToString()));
            }
            dummy = (Container)window.Container.Visuals[3];

            Button play1Button = new Button("Play 1");
            play1Button.Theme = themeA;
            play1Button.OnPressed += OnPlay1Clicked;
            Button play2Button = new Button("Play 2");
            play2Button.OnPressed += OnPlay2Clicked;
            Button play3Button = new Button("Play 3");
            play3Button.Theme = themeA;
            play3Button.OnPressed += OnPlay3Clicked;
            Button play4Button = new Button("Play 4");
            play4Button.OnPressed += OnPlay4Clicked;

            dummy.Add(play1Button);
            dummy.Add(play2Button);
            dummy.Add(play3Button);
            dummy.Add(play4Button);
            gameLogicThread = new Thread(new ThreadStart(GameLoop));
            gameLogicThread.Start();
        }
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
            Container barsContainer = (Container)mainView.Visuals[1];
            Container attributesContainer = (Container)mainView.Visuals[2];
            Bar bar;
            TextVisual text;
            //Console.WriteLine("Game attributes:");
            long maximum = 0;
            for (int i = 0; i < Game.PlayersCount * Player.AttributesCount; i++)
            {
                if (gameAttributes[i] > maximum)
                {
                    maximum = gameAttributes[i];
                }
            }
            for (int player = 0; player < Game.PlayersCount; player++)
            {
                //Console.WriteLine("Player {0} attributes:", player);
                for (int attribute = 0; attribute < Player.AttributesCount; attribute++)
                {   
                    int index = player * Player.AttributesCount + attribute;
                    bar = (Bar)barsContainer.Visuals[index];
                    text = (TextVisual)attributesContainer.Visuals[index];
                    //Console.WriteLine("attribute no.{0} = {1}", attribute, gameAttributes[index]);
                    text.Text = gameAttributes[index].ToString();
                    double value = gameAttributes[index];
                    value /= maximum;
                    bar.Level = value;
                }
            }
        }
        public VisualClient()
        {
            application = new Application();
            window = new Window("learn", 1280, 720, Orientation.Vertical);
            themeA = new Theme(new Color(255/8, 255/8, 255 /8), new Color(255, 255, 255), Window.Theme.Font);
            application.Window = window;
            text = new TextVisual("Waiting...");
            input = new TextInput("Server");
            input.Input = "127.0.0.1";
            Button button = new Button("Connect");
            button.Theme = themeA;
            button.OnPressed += OnConnectClicked;
            window.Container.Add(text);
            window.Container.Add(input);
            window.Container.Add(button);
            application.Run();
            gameLogicThread.Join();
        }
    }
}
