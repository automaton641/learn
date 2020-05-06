using System;

namespace Learn
{
    class Game
    {
        public const int PlaysCount = 4;
        public const int PlayersCount = 2;
        private int currentPlayerIndex = 0;
        private int currentEnemyIndex = 1;
        private int winner = -1;
        private int turn = 0;
        private Player[] players = new Player[PlayersCount];
        public Game()
        {
            for (int i = 0; i < PlayersCount; i++)
            {
                players[i] = new Player(i);
            }
        }
        public long GetPlayerAttribute(int playerIndex, int attributeIndex)
        {
            return players[playerIndex].GetAttribute(attributeIndex);
        }
        private void changeAttribute(int playerIndex, int attributeIndex, long offset)
        {
            players[playerIndex].ChangeAttribute(attributeIndex, offset);
        }
        private void fightDraw(long life0, long life1)
        {
            if (life0 == life1)
            {
                long sum0 = 0;
                long sum1 = 0;
                for (int i = 1; i < Player.AttributesCount; i++)
                {
                    sum0 += GetPlayerAttribute(0, i);
                    sum1 += GetPlayerAttribute(1, i);
                }
                if (sum0 == sum1)
                {
                    winner = 2;
                    return;
                }
                else
                {
                    if (sum0 > sum1)
                    {
                        winner = 0;
                        return;
                    }
                    else
                    {
                        winner = 1;
                        return;
                    }
                }
            }
            else
            {
                if (life0 > life1)
                {
                    winner = 0;
                    return;
                }
                else
                {
                    winner = 1;
                    return;
                }
            }
        }
        private void checkWinner()
        {
            long life0 = GetPlayerAttribute(0, 0);
            long life1 = GetPlayerAttribute(1, 0);
            if (life0 <= 0)
            {
                if (life1 <= 0)
                {
                    fightDraw(life0, life1);
                    return;
                }
                else
                {
                    winner = 1;
                    return;
                }
            }
            else if (life1 <= 0)
            {
                winner = 0;
                return;
            }
            else if (turn >= 16)
            {
                fightDraw(life0, life1);
                return;
            }
        }
        public int Play(int play)
        {
            if (winner < 0)
            {
                currentPlayerIndex = turn % Game.PlayersCount;
                currentEnemyIndex = (currentPlayerIndex + 1) % Game.PlayersCount;
                switch (play)
                {
                    case 0:
                        long d = Math.Distance(GetPlayerAttribute(currentPlayerIndex, 0), GetPlayerAttribute(currentEnemyIndex, 0));
                        changeAttribute(currentPlayerIndex, 0, d + 256);
                        changeAttribute(currentEnemyIndex, 2, -32); 
                        changeAttribute(currentEnemyIndex, 3, -32);
                        break;
                    case 1:
                        changeAttribute(currentPlayerIndex, 2, 16); 
                        changeAttribute(currentPlayerIndex, 3, 32 + 4);
                        break;
                    case 2:
                        changeAttribute(currentPlayerIndex, 1, 8); 
                        changeAttribute(currentPlayerIndex, 2, 32);
                        changeAttribute(currentEnemyIndex, 1, -8); 
                        break;
                    case 3:
                        long a = (GetPlayerAttribute(currentPlayerIndex, 2) * 24 + GetPlayerAttribute(currentPlayerIndex, 3) * 24) * 32 -
                            (GetPlayerAttribute(currentEnemyIndex, 1) * 12 + GetPlayerAttribute(currentEnemyIndex, 2) * 12) * 16;
                        changeAttribute(currentEnemyIndex, 0, -a * (64 + 32));
                        break;
                    default:
                        Console.WriteLine("Error: unknown play");
                        break;
                }
                checkWinner();
                turn++;
            }
            return winner;
        }
    }
}
