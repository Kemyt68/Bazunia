using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Path = System.IO.Path;

namespace Bazunia



{
    public struct Parametry
    {
        public StreamReader? sur;
        public StreamWriter baza;
        public string bazaPath;
        public string surPath;
        public int maxOdpCount;
        public int pytCount;

    };



    public partial class MainWindow : Window
    {
        public const int PYTANIE = 1;
        public const int ODP = 3;
        public const int SKIP= 4;
        public const int SKIP_PYT = 5;  
        public const string FAUTY_LINE_MSG= "Niezidentyfikowana linie:";
        public const int ILE_POL_Q_ = 6;

        List<ComboBox> PytAComboBoxList;
        List<ComboBox> PytBComboBoxList;
        public static List<string> ListaOpcjiPyt = new List<string>{
        ".",
        ":",
        "Numer",
        "<tab>",
        "<spacja>",
        "Tekst",
        "Dowolny"
        };

        List<ComboBox> OdpAComboBoxList;
        List<ComboBox> OdpBComboBoxList;
        public static List<string> ListaOpcjiOdp = new List<string>{
        "ABCD",
        ".",
        ":",
        "Numer",
        "<tab>",
        "<spacja>",
        "Tekst",
        ")",
        "(",
        "Dowolny"
        };

        public string faultyLine = FAUTY_LINE_MSG;
        public int numerLinii = 0;
        public int tmp_pytCount = 0;


        public bool FindPyt(string str) {

            const int FALSE = 0;
            const int TRUE = 1;
            const int SKIP = 2;
            int i = 0;
            
            int A = FALSE;
            int B = FALSE;

            //Wybiera przypadek wybrany w Combobox, sprawdza czy wystepuje i przesuwa iterator dla kolejnego Combobox-a
            // Zwraca: FALSE nie znaleziono, TRUE znaleziono, SKIP Combobox jest pusty
            int Find(ComboBox comboBox)
            {
                string? wybranaOpcja = comboBox.SelectedItem as string;
                switch (wybranaOpcja)
                {
                    case "Dowolny":
                        return TRUE;

                    case ".":
                        try
                        {
                            if (str[i] != '.')
                                return FALSE;
                            else
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case ",":
                        try
                        {
                            if (str[i] != ',')
                                return FALSE;
                            else
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case "Numer":
                        try
                        {
                            if (char.IsDigit(str[i]))
                                i++;
                            else
                                return FALSE;

                            while (char.IsDigit(str[i]))
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }

                        break;
                    case "<tab>":
                        try
                        {
                            if (str[i] != '\t')
                                return FALSE;
                            else
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case "<spacja>":
                        try
                        {
                            if (str[i] != ' ')
                                return FALSE;
                            else
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case "Tekst":
                        try
                        {
                            string tekst = "";
                            Dictionary<ComboBox, TextBox> map = new Dictionary<ComboBox, TextBox>
                        {
                            { Pyt1, PytTxt1 },
                            { Pyt2, PytTxt2 },
                            { Pyt3, PytTxt3 },
                            { Pyt4, PytTxt4 },
                                                        
                            { Pyt5, PytTxt5 },
                            { Pyt6, PytTxt6 },
                            { Pyt7, PytTxt7 },
                            { Pyt8, PytTxt8 }
                        };

                            if (map.ContainsKey(comboBox))
                            {
                                TextBox textBox = map[comboBox];
                                tekst = textBox.Text;
                            }

                            if (str.Substring(i, tekst.Length) == tekst)
                                i = i + tekst.Length;
                            else
                                return FALSE;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;

                    default:
                        return SKIP;
                }
                return TRUE;
            }

            foreach (ComboBox comboBox in PytAComboBoxList)
            {
                switch (Find(comboBox))
                {
                    case FALSE:
                        A = FALSE;
                        break;

                    case TRUE:
                        A = TRUE;
                        break;

                    case SKIP:
                        break;
                }
                if (A == FALSE)
                    break;
            }

            if (A == TRUE)
            {
                return true;
            }

            i = 0;
            foreach (ComboBox comboBox in PytBComboBoxList)
            {
                switch (Find(comboBox))
                {
                    case FALSE:
                        B = FALSE;
                        break;

                    case TRUE:
                        B = TRUE;
                        break;

                    case SKIP:
                        break;
                }
                if (B == FALSE)
                    break;
            }

            if (B == FALSE)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool FindOdp(string str)
        {

            //Warunek niespełniony
            const int FALSE = 0;
            //Warunek spełniony
            const int TRUE = 1;
            const int SKIP = 2;
            int i = 0;

            int A = FALSE;
            int B = FALSE;

            //Wybiera przypadek wybrany w Combobox, sprawdza czy wystepuje i przesuwa iterator dla kolejnego Combobox-a
            // Zwraca: FALSE nie znaleziono, TRUE znaleziono, SKIP Combobox jest pusty
            int Find(ComboBox comboBox)
            {
                string? wybranaOpcja = comboBox.SelectedItem as string;
                switch (wybranaOpcja)
                {
                    case "Dowolny":
                        return TRUE;
                    case ".":
                        try
                        {
                            if (str[i] != '.')
                                return FALSE;
                            else
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case ",":
                        try
                        {
                            if (str[i] != ',')
                                return FALSE;
                            else
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case "Numer":
                        try
                        {
                            if (char.IsDigit(str[i]))
                                i++;
                            else
                                return FALSE;

                            while (char.IsDigit(str[i]))
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }

                        break;
                    case "<tab>":
                        try
                        {
                            if (str[i] != '\t')
                                return FALSE;
                            else
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case "<spacja>":
                        try
                        {
                            if (str[i] != ' ')
                                return FALSE;
                            else
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case "Tekst":
                        try
                        {
                            string tekst = "";
                            Dictionary<ComboBox, TextBox> map = new Dictionary<ComboBox, TextBox>
                        {
                            { Odp1, OdpTxt1 },
                            { Odp2, OdpTxt2 },
                            { Odp3, OdpTxt3 },
                            { Odp4, OdpTxt4 },

                            { Odp5, OdpTxt5 },
                            { Odp6, OdpTxt6 },
                            { Odp7, OdpTxt7 },
                            { Odp8, OdpTxt8 }
                        };

                            if (map.ContainsKey(comboBox))
                            {
                                TextBox textBox = map[comboBox];
                                tekst = textBox.Text;
                            }

                            if (str.Substring(i, tekst.Length) == tekst)
                                i = i + tekst.Length;
                            else
                                return FALSE;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case ")":
                        try
                        {
                            if (str[i] != ')')
                                return FALSE;
                            else
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case "(":
                        try
                        {
                            if (str[i] != '(')
                                return FALSE;
                            else
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    case "ABCD":

                        try
                        {
                            string tekst = "";
                            Dictionary<ComboBox, TextBox> map = new Dictionary<ComboBox, TextBox>
                        {
                            { Odp1, OdpTxt1 },
                            { Odp2, OdpTxt2 },
                            { Odp3, OdpTxt3 },
                            { Odp4, OdpTxt4 },

                            { Odp5, OdpTxt5 },
                            { Odp6, OdpTxt6 },
                            { Odp7, OdpTxt7 },
                            { Odp8, OdpTxt8 }
                        };

                            if (map.ContainsKey(comboBox))
                            {
                                TextBox textBox = map[comboBox];
                                tekst = textBox.Text;
                            }


                            if (char.IsLetter(str[i]))
                                i++;
                            else
                                return FALSE;

                            while (char.IsLetter(str[i]))
                                i++;
                        }
                        catch (Exception)
                        {
                            return FALSE;
                        }
                        break;
                    default:
                        return SKIP;
                }
                return TRUE;
            }

            foreach (ComboBox comboBox in OdpAComboBoxList)
            {
                switch (Find(comboBox))
                {
                    case FALSE:
                        A = FALSE;
                        break;

                    case TRUE:
                        A = TRUE;
                        break;

                    case SKIP:
                        break;
                }
                if (A == FALSE)
                    break;
            }

            if (A == TRUE)
            {
                return true;
            }

            i = 0;
            foreach (ComboBox comboBox in OdpBComboBoxList)
            {
                switch (Find(comboBox))
                {
                    case FALSE:
                        B = FALSE;
                        break;

                    case TRUE:
                        B = TRUE;
                        break;

                    case SKIP:
                        break;
                }
                if (B == FALSE)
                    break;
            }

            if (B == FALSE)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int DecodeLine(string line)
        {
            //osobna zmienna wynik, aby ogarnąć przypadek, gdy pytanie jest "dowolne" to żeby szukał pytania, a nie od razu zwracał pytanie
            int wynik = 0;

            if (FindPyt(line))
            {
                wynik = PYTANIE;
            }
            if (FindOdp(line))
            {
                wynik = ODP;
            }



            if (line.Length < 2)
            {
                wynik = SKIP;
            }
            else if(wynik!=PYTANIE&&wynik!=ODP) 
            {
                foreach (char c in line)
                {
                    if (Char.IsLetter(c))
                    {
                        faultyLine = faultyLine + "\nNr. lini " + numerLinii + "\t" + line;
                        return 69; // Znaleziono literę
                    }
                }
                wynik = SKIP; // Brak liter
            }
            return wynik;
        }

        public static string ZabezpieczLinie(string line)
        {
            line = line.Replace("\"", "\"\"");
            line = '\"' + line + '\"';
            return line;
        }
        public bool ZliczPytOdp(StreamReader sur)
        {
            string? line;
            int lineType, prevLineType = PYTANIE;
            p.pytCount = 0;
            p.maxOdpCount = 0;
            int tmp_maxOdpCount = 0;

            // Odczytujemy plik linia po linii
            while ((line = PobierzLinie()) != null)
            {

                lineType = DecodeLine(line);


                switch (lineType)
                {

                    case PYTANIE:
                        {

                            if (PytNastLin.IsChecked == true)
                            {
                                line = PobierzLinie();
                            }
                            p.pytCount++;
                            if (prevLineType == ODP)
                            {
                                if(p.maxOdpCount< tmp_maxOdpCount)
                                {
                                    p.maxOdpCount = tmp_maxOdpCount;
                                }
                            }
                            tmp_maxOdpCount = 0;
                            break;
                        }
                    case ODP:
                        {
                            tmp_maxOdpCount++;
                            break;
                        }
                    case SKIP:
                        lineType = prevLineType;
                        break;
                }
                prevLineType = lineType;


            }

            //jeśli najwięcej odpowiedzi jest w ostatnim pytaniu w pliku
            if (prevLineType == ODP)
            {
                if (p.maxOdpCount < tmp_maxOdpCount)
                {
                    p.maxOdpCount = tmp_maxOdpCount;
                }
            }

            if (faultyLine != FAUTY_LINE_MSG) {
                MessageBox.Show(faultyLine, "Błąd odczytu lini", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Click_BrowseBaza(object sender, RoutedEventArgs e)
        {


            p.sur = OpenFile();
            if (p.sur == null)
            {
                return;
            }

            File.WriteAllText(p.bazaPath, "#separator:;\n#html:true\n");
            //MessageBox.Show(bazaPath, "path", MessageBoxButton.OK, MessageBoxImage.None);
            //string bazaPath = "C:\Users\Acer\Desktop";

            if (FiszkaCheckBox.IsChecked==true)
            {
                PrzygotujBazeFiszki();
            }
            else
            {
                PrzygotujBazeABCD();
            }

        }

        public static string UsunABCD(string line)
        {
            return line.Substring(3); // usuń na sztywno trzy pierwsze znaki z dodpowiedzi
        }
        
        private void WstawOdpABCD(int ileWstawionych)
        {
            int ile = p.maxOdpCount;
            //Wstaw ; po ostatniej wczytanej odpowiedzi
            File.AppendAllText(p.bazaPath, ";");


            //uzupełnij brakujące pola do ILE_POL_Q_
            while (ileWstawionych < ILE_POL_Q_)
            {
                File.AppendAllText(p.bazaPath, ";");
                ileWstawionych++;
            }

            int i = 0;
            while (i < ILE_POL_Q_)
            {
                File.AppendAllText(p.bazaPath, "0 ");
                i++;
            }

            //ile = 5 - ile;
            //for (int i = 1; i < 6; i++)
            //{
            //    if (ile > 0)
            //    {
            //                                File.AppendAllText(p.bazaPath, ";");
            //        ile--;
            //    }
            //    else
            //    {
            //                                File.AppendAllText(p.bazaPath, "0 ");
            //    }
            //}
            //// baza << ' ';
            ///
            File.AppendAllText(p.bazaPath, ";;;;");

        }

        public string PobierzLinie()
        {
            numerLinii++;
            return p.sur.ReadLine();
        }
        private void PrzygotujBazeABCD()
        {
            string? line;
            // streampos prevline;
            int lineType, prevLineType = PYTANIE;
            int ileOdp = 0;
            // Która z wymienionych struktur znajduje się w uchu środkowym ? ;;2;Błędnik.;Wrzecionko.;Przychłonka.;Aparat przedsionkowy.;Kosteczki słuchowe.;0 1 0 0 0;;;

            //zlicza ile maksymalnie jest opdowiedzi pod którymś pytaniem
            //potrzebne do określenia liczby pól dla importu do 
            if (p.sur!=null && ZliczPytOdp(p.sur))
            {
                if (p.maxOdpCount < 7)
                    MessageBox.Show("Liczba pytań: " + p.pytCount + "\nMax liczba odp: " + p.maxOdpCount, "Wynik", MessageBoxButton.OK, MessageBoxImage.None);
                else
                    MessageBox.Show("Liczba pytań: " + p.pytCount + "\nMax liczba odp: " + p.maxOdpCount + "UWAGA więcej niż 6 pytań kontakt do tymon", "Wynik", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else
            {
                //MessageBox.Show("Liczba pytań: " + p.pytCount + "\nMax liczba odp: " + p.maxOdpCount, "Wynik", MessageBoxButton.OK, MessageBoxImage.None);
            }

            p.sur = new StreamReader(p.surPath);

            while (true)
            {
                line = PobierzLinie();
                if (line == null) { break; }

                // cout << line << endl;
                lineType = DecodeLine(line);

               


                switch (lineType)
                {
                    case PYTANIE:
                        // Jeśli pytanie ma być w następnej linii, to ją pobierz
                        if (PytNastLin.IsChecked == true)
                        {
                            line = PobierzLinie();
                        }

                        if (prevLineType == ODP)
                        {
                            // koniec odpowiedzi


                            WstawOdpABCD(ileOdp);
                            // klucz = "";
                        }

                        // usunNumer(line);
                        // getline(sur, line);

                        line = ZabezpieczLinie(line);

                        File.AppendAllText(p.bazaPath, "\n"+line);
                        break;


                    case ODP:
                        if (prevLineType == PYTANIE)
                        {
                            ileOdp = 1;
                                                    File.AppendAllText(p.bazaPath, ";;2;");
                            line = UsunABCD(line);
                            // isCorrect(line, klucz);
                        }
                        else if (prevLineType == ODP)
                        {
                            ileOdp++;
                                                    File.AppendAllText(p.bazaPath, ";");
                            line = UsunABCD(line);
                            // isCorrect(line, klucz);
                        }
                        line = ZabezpieczLinie(line);
                        File.AppendAllText(p.bazaPath, line);

                        break;

                    case SKIP:
                        lineType = prevLineType;
                        break;

                    default:
                        break;
                }

                prevLineType = lineType;

            }

            // koniec odpowiedzi
            WstawOdpABCD(ileOdp);


        }

        private void PrzygotujBazeFiszki()
        {
            string? line;
            int lineType, prevLineType = PYTANIE;

            //p.sur = new StreamReader(p.surPath);


            while (true)
            {
                line = PobierzLinie();
                if (line == null) { break; }

                // cout << line << endl;
                lineType = DecodeLine(line);




                switch (lineType)
                {
                    case PYTANIE:
                        // Jeśli pytanie ma być w następnej linii, to ją pobierz
                        if (PytNastLin.IsChecked == true)
                        {
                            line = PobierzLinie();
                        }

                        line = ZabezpieczLinie(line);

                        File.AppendAllText(p.bazaPath, "\n" + line);
                        break;


                    case ODP:
                        if (prevLineType == PYTANIE)
                        {
                            //wstaw ; 
                            File.AppendAllText(p.bazaPath, ";");
                        }

                        line = ZabezpieczLinie(line);
                        File.AppendAllText(p.bazaPath, line);

                        break;

                    case SKIP:
                        lineType = prevLineType;
                        break;

                    default:
                        break;
                }

                prevLineType = lineType;

            }


        }
        private StreamReader? OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Wybierz plik",
                Filter = "Pliki tekstowe|*.txt",
                InitialDirectory = "C:\\"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                    p.surPath = openFileDialog.FileName;
                    return new StreamReader(fileStream); // Zwracamy otwartego StreamReadera
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Błąd dostępu do pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? combo = sender as ComboBox;
            string? selected = combo.SelectedItem as string;

            // Mapowanie ComboBox -> odpowiadający TextBox
            Dictionary<ComboBox, TextBox> map = new Dictionary<ComboBox, TextBox>
    {
        { Pyt1, PytTxt1 },
        { Pyt2, PytTxt2 },
        { Pyt3, PytTxt3 },
        { Pyt4, PytTxt4 },

        { Pyt5, PytTxt5 },
        { Pyt6, PytTxt6 },
        { Pyt7, PytTxt7 },
        { Pyt8, PytTxt8 },

        { Odp1, OdpTxt1 },
        { Odp2, OdpTxt2 },
        { Odp3, OdpTxt3 },
        { Odp4, OdpTxt4 },

        { Odp5, OdpTxt5 },
        { Odp6, OdpTxt6 },
        { Odp7, OdpTxt7 },
        { Odp8, OdpTxt8 },
    };

            if (map.ContainsKey(combo))
            {
                TextBox textBox = map[combo];
                if (selected == "Tekst")
                {
                    textBox.Visibility = Visibility.Visible;
                }
                else
                {
                    textBox.Visibility = Visibility.Collapsed;
                }
            }
        }

        private Parametry p;



        public MainWindow()
        {
            InitializeComponent();
            p.sur?.Close();

            PytAComboBoxList = new List<ComboBox> { Pyt1, Pyt2, Pyt3, Pyt4 };
            PytBComboBoxList = new List<ComboBox> { Pyt5, Pyt6, Pyt7, Pyt8 };
            OdpAComboBoxList = new List<ComboBox> { Odp1, Odp2, Odp3, Odp4 };
            OdpBComboBoxList = new List<ComboBox> { Odp5, Odp6, Odp7, Odp8 };

            foreach (string opcja in ListaOpcjiPyt)
            {
                foreach (ComboBox comboBox in PytAComboBoxList)
                {
                    comboBox.Items.Add(opcja);
                }
                foreach (ComboBox comboBox in PytBComboBoxList)
                {
                    comboBox.Items.Add(opcja);
                }
            }
            foreach (string opcja in ListaOpcjiOdp)
            {
                foreach (ComboBox comboBox in OdpAComboBoxList)
                {
                    comboBox.Items.Add(opcja);
                }
                foreach (ComboBox comboBox in OdpBComboBoxList)
                {
                    comboBox.Items.Add(opcja);
                }
            }

            p.bazaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "baza.txt");




        }


    }
}
