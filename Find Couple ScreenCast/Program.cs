using System;
using SFML.Learning;
using SFML.System;
using SFML.Window;

namespace Find_Couple_ScreenCast
{
    internal class Program : Game
    {
        static string[] iconsName;
        static int[,] cards;

        static int cardCount;
        static int cardWidth = 100;
        static int cardHeight = 100;

        static int countPerLine = 5;
        static int space = 40;
        static int leftOffset = 70;
        static int topOffset = 20;

        static bool winLose = false;

        static string[,] levels = { {"50", "200", "Легкий (10 карточек и 30 секунд времени)", "500", "50" },
                                   {"50", "250", "Средний (14 карточек и 30 секунд времени)", "510", "50" },
                                   {"50", "300", "Тяжелый (20 карточек и 30 секунд времени)", "520", "50"} 
        };

        static string open_first_icon = LoadSound("open_first_icon.wav");
        static string open_second_icon = LoadSound("open_second_icon.wav");
        static string you_win = LoadSound("you_win.wav");

        static void LoadIcons()
        {
            iconsName = new string[7];
            iconsName[0] = LoadTexture("icon_close.png");
            for (int i = 1; i < iconsName.Length; i++)
            {
                iconsName[i] = LoadTexture("icon_" + (i).ToString() + ".png");
            }
        }
        static void Shuffle(int[] arr)
        {
            Random rand = new Random();

            for (int i = arr.Length - 1; i >= 1; i--)
            {
                int j = rand.Next(1, i + 1);
                int temp = arr[j];
                arr[j] = arr[i];
                arr[i] = temp;
            }
        }

        static void InitCard()
        {
            Random rnd = new Random();
            cards = new int[cardCount, 6];
            int[] iconId = new int[cards.GetLength(0)];
            int id = 0;
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                if (i % 2 == 0)
                {
                    id = rnd.Next(1, 7);
                }
                iconId[i] = id;
            }
            Shuffle(iconId);
            Shuffle(iconId);
            Shuffle(iconId);
            for (int i = 0; i < iconId.Length; i++)
            {
                cards[i, 0] = 1;
                cards[i, 1] = (i % countPerLine) * (cardWidth + space) + leftOffset;
                cards[i, 2] = (i / countPerLine) * (cardHeight + space) + topOffset;
                cards[i, 3] = cardWidth;
                cards[i, 4] = cardHeight;
                cards[i, 5] = iconId[i];
            }
        }

        static void SetStateToAllCards(int state)
        {
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                cards[i, 0] = state;
            }
        }

        static void DrawCard()
        {
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                if (cards[i, 0] == 1)
                {
                    DrawSprite(iconsName[cards[i, 5]], cards[i, 1], cards[i, 2]);

                }
                if (cards[i, 0] == 0)
                {
                    DrawSprite(iconsName[0], cards[i, 1], cards[i, 2]);
                }
            }
        }
        static int GetIndexCardByMousePosition()
        {
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                if (MouseX >= cards[i, 1] 
                    && MouseX <= cards[i, 1] + cards[i, 3] 
                    && MouseY >= cards[i, 2] 
                    && MouseY <= cards[i, 2] + cards[i, 4])
                {
                    if (cards[i, 0] != -1)
                    {
                        PlaySound(open_first_icon, cardCount);
                        return i;
                    }
                }
            }
            return -1;
        }
        static int GetIndexLevelByCursorPosition()
        {
            for (int i = 0; i < levels.GetLength(0); i++)
            {
                if (MouseX>=50 && MouseY>=200
                    &&MouseX >= Int32.Parse(levels[i,0])
                    &&MouseX <= Int32.Parse(levels[i, 0]) + Int32.Parse(levels[i, 3])
                    && MouseY <= Int32.Parse(levels[i, 1]) + Int32.Parse(levels[i, 4]))
                {
                    Console.WriteLine(MouseX + " " + MouseY);
                    return i;
                }
            }
            return -1;
        }
        static void Main(string[] args)
        {
            int openCardAmount = 0;
            int firstOpenCardIndex = -1;
            int secondOpenCardIndex = -1;

            LoadIcons();
            SetFont("Comic Sans MS.ttf");

            InitWindow(800, 600, "Find Couple");

            while (true)
            {
                DispatchEvents();
                ClearWindow();
                DrawText(300, 50, "Игра \"Найди пару\" ", 24);
                DrawText(250, 100, "Выбери уровень сложности: ", 24);
                DrawText(Int32.Parse(levels[0, 0]), Int32.Parse(levels[0, 1]), levels[0, 2], 24);
                DrawText(Int32.Parse(levels[1, 0]), Int32.Parse(levels[1, 1]), levels[1, 2], 24);
                DrawText(Int32.Parse(levels[2, 0]), Int32.Parse(levels[2, 1]), levels[2, 2], 24);

                if (GetMouseButton(0) == true)
                {
                    int index = GetIndexLevelByCursorPosition();
                    if (index == 0)
                    {
                        cardCount = 10;
                        break;
                    }
                    if (index == 1)
                    {
                        cardCount = 14;
                        break;
                    }
                    if (index == 2)
                    {
                        cardCount = 20;
                        break;
                    }
                }
                DisplayWindow();
                Delay(1);
            }
            int remainingCard = cardCount;

            InitCard();
            SetStateToAllCards(1);
            ClearWindow(26, 46, 92);
            DrawCard();
            DisplayWindow();
            Delay(2000);
            SetStateToAllCards(0);

            DateTime time = DateTime.Now;//текущее время
            DateTime timer = time.AddSeconds(30);//таймер 30 секунд

            while (true)
            {
                DispatchEvents();
                time = DateTime.Now;
                string str = (timer - time).ToString();
                Console.WriteLine(time + "\t" + timer + "\t\t" + str);

                if (time > timer)
                {
                    break;
                }

                if (remainingCard == 0)
                {
                    winLose = true;
                    break;
                }
                if (openCardAmount == 2)
                {
                    if (cards[firstOpenCardIndex, 5] == cards[secondOpenCardIndex, 5])
                    {
                        cards[firstOpenCardIndex, 0] = -1;
                        cards[secondOpenCardIndex, 0] = -1;

                        remainingCard -= 2;
                        PlaySound(open_second_icon, 20);
                    }
                    else
                    {
                        cards[firstOpenCardIndex, 0] = 0;
                        cards[secondOpenCardIndex, 0] = 0;
                    }
                    firstOpenCardIndex = -1;
                    secondOpenCardIndex = -1;
                    openCardAmount = 0;

                    Delay(1000);
                }

                if (GetMouseButtonDown(0) == true)
                {
                    int index = GetIndexCardByMousePosition();
                    if (index != -1 && index != firstOpenCardIndex)
                    {
                        cards[index, 0] = 1;
                        openCardAmount++;

                        if (openCardAmount == 1)
                        {
                            firstOpenCardIndex = index;
                        }
                        if (openCardAmount == 2)
                        {
                            secondOpenCardIndex = index;
                        }
                    }
                }

                ClearWindow(26, 46, 92);

                DrawCard();
                DrawText(550, 550, str, 24);

                DisplayWindow();

                Delay(1);
            }
            ClearWindow();
            SetFillColor(255, 255, 255);
            if (winLose)
            {
                PlaySound(you_win, 50);
                DrawText(200, 300, "Поздравляю! Ты открыл все карты!", 24);
            }
            else
            {
                DrawText(200, 300, "Время вышло. Вы Проиграли!!!", 24);
            }
            DisplayWindow();
            Delay(2000);
        }
    }
}
