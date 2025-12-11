namespace vier_gewinnt
{
    internal class Program
    {
        static void draw_spielfeld(string[,] spielfeld)
        {
            Console.WriteLine("");
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Console.Write($"{spielfeld[i,j]} ");
                }
                Console.WriteLine();
            }
        }

        static bool check_if_coloum_free(string[,] spielfeld, int spalte)
        {
            for (int reihe = 0; reihe < 6; reihe++)
            {
                if (spielfeld[reihe,spalte] == ".")
                {
                    return true;

                }
            }
            return false;
        }

        static string[,] init_spielfeld(string[,] spielfeld )
        {
            for (int i = 0;i < 6;i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    spielfeld[i, j] = "."; 
                }
            }
            return spielfeld;
        }

        static (int, bool, bool) eingabe(string[,] spielfeld, string spieler)
        {
            bool stop = false;
            bool del = false;
            int spalte = 0;
            int entry = 0;
            while (true)
            {
                Console.Write($"\nSpieler \"{spieler}\", in welche Spalte moechtest du den Stein einwerfen? ");
                string entry_string = Console.ReadLine();
                if (entry_string == "!")
                {
                    stop = true;
                    return (spalte, stop, del);
                }
                if (((entry_string == "O") && (spieler == "X")) || ((entry_string == "X") && (spieler == "O")))
                {
                    del = true;
                    return (spalte, stop, del);
                } 
                try
                {
                    entry = Convert.ToInt32(entry_string);
                    spalte = entry - 1;
                    if (spalte < 0)
                    {
                        Console.WriteLine($"Gebe eine positive Zahl ein und nicht \"{entry}\"");
                        continue;
                    }
                    if (spalte > 6)
                    {
                        Console.WriteLine($"Es gibt nur 7 Spalten!");
                        continue;
                    }
                    if (check_if_coloum_free(spielfeld, spalte))
                        return (spalte, stop, del);
                    else
                    {
                        Console.WriteLine($"Spalte {entry} ist bereits voll.");
                        continue;
                    }
                }
                catch
                {
                    Console.WriteLine($"Gebe eine Zahl ein und nicht \"{entry_string}\"!");
                    continue;
                }
            }
        }

        static (string[,], int, int) place_move(string[,] spielfeld, string spieler, int spalte)
        {
            int reihe = 5;
            for (int i = 0; i < 6; i++)
            {
                if (spielfeld[reihe,spalte] != ".")
                {
                    reihe--;
                }
            }
            spielfeld[reihe, spalte] = spieler;
            return (spielfeld, reihe, spalte);
        }

        static void print_results(int plays, List<List<int>> results)
        {
            Console.WriteLine("\nDas Spiel ist wie folgt abgelaufen:");
            for (int i = 0; i < results.Count; i++)
            {
                string player = "O";
                if (results[i][0] == 0)
                {
                    player = "X";
                }
                if ((results[i][2] == -1) && (results[i][1] == -1))
                    Console.WriteLine($"{player} hat einen Zug rückgängig gemacht.");
                else
                    Console.WriteLine($"{player}:({results[i][2] + 1},{results[i][1] + 1}) -- Spalte {results[i][2] + 1}, {results[i][1] + 1}-te Zeile");
            }
            Console.WriteLine($"\nDas Spiel wurde nach {plays} Zügen beendet.");
        }

        static (bool, bool) check_for_win(string[,] spielfeld, string spieler)
        {

            int counter_rows = 0;
            int counter_columns = 0;
            int counter_points = 0;
            for (int reihe = 0;reihe < 6;reihe++)
            {
                for (int spalte = 0; spalte < 7; spalte++)
                {
                    // Unentschieden
                    if (spielfeld[reihe,spalte] == ".")
                    {
                        counter_points++;
                    }
                    // Reihe
                    if (spielfeld[reihe, spalte] == spieler)
                    {
                        counter_rows++;
                        if (counter_rows == 4)
                            return (true, false);
                    }
                    else
                        counter_rows = 0;
                    // Spalte
                    if (spalte < 6)
                    {
                        if (spielfeld[spalte, reihe] == spieler)
                        {
                            counter_columns++;
                            if (counter_columns == 4)
                                return (true, false);
                        }
                        else
                            counter_columns = 0;
                    // Diagonal nach unten rechts
                    if (((-1 < spalte) && (spalte < 4)) && ((-1 < reihe) && (reihe < 3)) && spielfeld[reihe,spalte] == spieler)
                    {
                        string one = spielfeld[reihe, spalte];
                        string two = spielfeld[reihe+1, spalte+1];
                        string three = spielfeld[reihe+2, spalte+2];
                        string four = spielfeld[reihe+3, spalte+3];
                        if (one == two && one == three && one == four)
                        {
                            return (true, false);
                        }
                    }
                    // Diagonal nach unten links
                    if (((2 < spalte) && (spalte < 7)) && ((-1 < reihe) && (reihe < 3)) && spielfeld[reihe, spalte] == spieler)
                    {
                        string one = spielfeld[reihe, spalte];
                        string two = spielfeld[reihe + 1, spalte - 1];
                        string three = spielfeld[reihe + 2, spalte - 2];
                        string four = spielfeld[reihe + 3, spalte - 3];
                        if (one == two && one == three && one == four)
                        {
                            return (true, false);
                        }
                    }
                }
            }
            }
            if (counter_points == 0)
            {
                return (true, true);
            }
            return (false,false);
        }

        static void delete_move(string[,] spielfeld,int reihe, int spalte)
        {
            spielfeld[reihe, spalte] = ".";
        }

        static void Main(string[] args)
        {
            string[,] spielfeld = new string[6,7];
            spielfeld = init_spielfeld(spielfeld);
            draw_spielfeld(spielfeld);
            string[] spieler = ["X", "O"];
            int spieler_dran = 0;
            int plays = 1;
            int last_reihe = 0;
            int last_spalte = 0;
            bool already_del = false;
            List<List<int>> spielzüge = new List<List<int>>();
            List<int> spielzug = new List<int>();

            while (true)
            {
                (int entry, bool stop, bool del) = eingabe(spielfeld, spieler[spieler_dran]);
                if (stop)
                {
                    print_results(plays - 1, spielzüge);
                    break;
                }
                spielzug.Clear();
                if (del)
                {
                    if (already_del)
                    {
                        Console.WriteLine("Du kannst nicht zweimal den letzten Zug rückgängig machen!");
                        continue;
                    }
                    spielzug.Add(spieler_dran);
                    spielzug.Add(-1);
                    spielzug.Add(-1);
                    delete_move(spielfeld, last_reihe, last_spalte);
                    already_del = true;
                }
                else
                {
                    (spielfeld, last_reihe, last_spalte) = place_move(spielfeld, spieler[spieler_dran], entry);
                    already_del = false;
                    spielzug.Add(spieler_dran);
                    spielzug.Add(last_reihe);
                    spielzug.Add(last_spalte);
                }
                spielzüge.Add(new List<int> { spielzug[0], spielzug[1], spielzug[2] });
                (bool win, bool draw) = check_for_win(spielfeld, spieler[spieler_dran]);
                if (win)
                {
                    if (!draw)
                    {
                        Console.Clear();
                        draw_spielfeld(spielfeld);
                        Console.WriteLine($"\nSpieler \"{spieler[spieler_dran]}\" gewinnt!");
                        print_results(plays, spielzüge);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\nUnentschieden!");
                        print_results(plays, spielzüge);
                        break;
                    }
                }
                Console.Clear();
                draw_spielfeld(spielfeld);
                plays++;
                spieler_dran = (spieler_dran + 1) % 2;
            }
        }
    }
}
