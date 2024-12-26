using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BayoCosTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private MainWindowViewModel _viewModel;
        private string file_name = "";
        private Cos? cos_file;
        private List<float[,]> work_values = new List<float[,]>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Nullable<bool> result;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            dlg.Filter = "Cos file (*.cos)|*.cos|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            result = dlg.ShowDialog();
            // User didn't select a file so don't do anything
            if (result == false)
                return;

            // Load the file the user selected  
            Console.WriteLine(dlg.FileName);
            Load_Cos(dlg.FileName);
            return;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            return;
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (cos_file != null)
            {
                cos_file = null;
                work_values.Clear();
                this.Title = "Bayonetta Palette Tool";
                CloseBtn.IsEnabled = false;
                SaveBtn.IsEnabled = false;
                SaveAsBtn.IsEnabled = false;
                ClearUI();
            }
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void Load_Cos(string filename)
        {
            FileStream fileStream;
            EndianBinaryReader reader;
            try
            {
                using (fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (reader = new EndianBinaryReader(fileStream))
                    {
                        reader.IsBigEndian = false;
                        // Check if the file has at least 164 bytes (header + 1 struct) for a valid size
                        if (fileStream.Length < ((UInt16)Globals.HEADER_SIZE + (UInt16)Globals.STRUCT_SIZE))
                        {
                            Console.WriteLine("The file is too short.");
                            return;
                        }

                        Load_Header(reader);
                        Load_Entries(reader);
                    }
                    fileStream.Close();

                    Load_WorkValues();
                    file_name = filename;
                    this.Title = "Bayonetta Palette Tool - " + System.IO.Path.GetFileName(filename);
                    CloseBtn.IsEnabled = true;
                    SaveBtn.IsEnabled = true;
                    SaveAsBtn.IsEnabled = true;
                    LoadToUI();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("The file was not found.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An I/O error occurred: {ex.Message}");
            }
        }

        private void Load_Header(EndianBinaryReader reader)
        {
            uint size;
            uint version;
            uint offsetEntries;
            uint numEntries;

            size = reader.ReadUInt32();
            version = reader.ReadUInt32();
            offsetEntries = reader.ReadUInt32();
            numEntries = reader.ReadUInt32();

            cos_file = new Cos(size, version, offsetEntries, numEntries);
            reader.BaseStream.Seek(offsetEntries, 0);

            // Check if there's a single header
            if ((size != 0) && (numEntries != 0))
                return;

            // Not returning means there's a second header to add
            size = reader.ReadUInt32();
            version = reader.ReadUInt32();
            numEntries = reader.ReadUInt32(); //Second header has number of entries before the offset
            offsetEntries = reader.ReadUInt32();

            cos_file.SetHeader2(new CosHeader(size, version, offsetEntries, numEntries));
        }

        private void Load_Entries(EndianBinaryReader reader)
        {
            if (cos_file != null)
            {
                List<CosEntry> entries;
                CosEntry entry;
                CosColor color;
                CosColor[] colors;
                sbyte[] name;
                CosHeader header;
                uint offset;
                int i, j;

                entries = cos_file.GetEntries();
                header = cos_file.GetHeader(0);
                offset = header.GetOffset();

                // Check if the file has 2 headers
                if (cos_file.GetHeader(1) != null)
                {
                    header = cos_file.GetHeader(1);
                    offset += header.GetOffset();
                }
                entries.Capacity = (int)header.GetNumOfEntries();

                // Reads and adds as many entries as the header says to the list
                for (i = 0; i < (int)header.GetNumOfEntries(); i++)
                {
                    entry = new CosEntry();
                    colors = entry.GetColors();
                    for (j = 0; j < 7; j++)
                    {
                        color = new CosColor(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        colors[j] = color;
                    }
                    name = entry.GetName();
                    for (j = 0; j < 64; j++)
                    {
                        name[j] = reader.ReadSByte();
                    }

                    entries.Add(entry);
                }
            }
        }

        private void Load_WorkValues()
        {
            float[,] values;
            CosColor[] colors;
            int i;
            if (cos_file != null)
            {
                // Clears current working values
                work_values.Clear();

                // Gets the values from the file and copies them to the working values
                List<CosEntry> entries = cos_file.GetEntries();
                foreach (CosEntry entry in entries)
                {
                    colors = entry.GetColors();

                    // Creates an array to contain the 3 components of all 7 colors of the current entry and adds them to the working values
                    values = new float[7, 3];
                    for (i = 0; i < 7; i++)
                    {
                        values[i, 0] = colors[i].Red;
                        values[i, 1] = colors[i].Green;
                        values[i, 2] = colors[i].Blue;
                    }

                    work_values.Add(values);
                }
            }
        }

        private void LoadToUI()
        {
            BindEntries();
            LoadValues(0);
            for (short i = 0; i < 7; i++)
                UpdateColor(i);
            RedBox0.IsEnabled = true;
            RedBox1.IsEnabled = true;
            RedBox2.IsEnabled = true;
            RedBox3.IsEnabled = true;
            RedBox4.IsEnabled = true;
            RedBox5.IsEnabled = true;
            RedBox6.IsEnabled = true;
            GreenBox0.IsEnabled = true;
            GreenBox1.IsEnabled = true;
            GreenBox2.IsEnabled = true;
            GreenBox3.IsEnabled = true;
            GreenBox4.IsEnabled = true;
            GreenBox5.IsEnabled = true;
            GreenBox6.IsEnabled = true;
            BlueBox0.IsEnabled = true;
            BlueBox1.IsEnabled = true;
            BlueBox2.IsEnabled = true;
            BlueBox3.IsEnabled = true;
            BlueBox4.IsEnabled = true;
            BlueBox5.IsEnabled = true;
            BlueBox6.IsEnabled = true;
            Color0.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            Color1.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            Color2.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            Color3.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            Color4.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            Color5.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            Color6.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
        }

        private void BindEntries()
        {
            if (cos_file != null)
            {
                // Creates a new list with the amount of elements as the amount of entries to bind it to the combobox
                List<CosEntry> entries = cos_file.GetEntries();
                EntriesComboBox.ItemsSource = entries.Select((entry, index) => $"Entry #{index + 1}").ToList();
                EntriesComboBox.IsEnabled = true;
                EntriesComboBox.SelectedIndex = 0;
            }
        }

        private void LoadValues(int index)
        {
            float[,] values;

            if (cos_file != null)
            {
                if (index < work_values.Count)
                {
                    values = work_values[index];

                    RedBox0.Text = string.Format("{0:N7}", values[0, 0] > 0 ? values[0, 0] : 0);
                    RedBox1.Text = string.Format("{0:N7}", values[1, 0] > 0 ? values[1, 0] : 0);
                    RedBox2.Text = string.Format("{0:N7}", values[2, 0] > 0 ? values[2, 0] : 0);
                    RedBox3.Text = string.Format("{0:N7}", values[3, 0] > 0 ? values[3, 0] : 0);
                    RedBox4.Text = string.Format("{0:N7}", values[4, 0] > 0 ? values[4, 0] : 0);
                    RedBox5.Text = string.Format("{0:N7}", values[5, 0] > 0 ? values[5, 0] : 0);
                    RedBox6.Text = string.Format("{0:N7}", values[6, 0] > 0 ? values[6, 0] : 0);
                    GreenBox0.Text = string.Format("{0:N7}", values[0, 1] > 0 ? values[0, 1] : 0);
                    GreenBox1.Text = string.Format("{0:N7}", values[1, 1] > 0 ? values[1, 1] : 0);
                    GreenBox2.Text = string.Format("{0:N7}", values[2, 1] > 0 ? values[2, 1] : 0);
                    GreenBox3.Text = string.Format("{0:N7}", values[3, 1] > 0 ? values[3, 1] : 0);
                    GreenBox4.Text = string.Format("{0:N7}", values[4, 1] > 0 ? values[4, 1] : 0);
                    GreenBox5.Text = string.Format("{0:N7}", values[5, 1] > 0 ? values[5, 1] : 0);
                    GreenBox6.Text = string.Format("{0:N7}", values[6, 1] > 0 ? values[6, 1] : 0);
                    BlueBox0.Text = string.Format("{0:N7}", values[0, 2] > 0 ? values[0, 2] : 0);
                    BlueBox1.Text = string.Format("{0:N7}", values[1, 2] > 0 ? values[1, 2] : 0);
                    BlueBox2.Text = string.Format("{0:N7}", values[2, 2] > 0 ? values[2, 2] : 0);
                    BlueBox3.Text = string.Format("{0:N7}", values[3, 2] > 0 ? values[3, 2] : 0);
                    BlueBox4.Text = string.Format("{0:N7}", values[4, 2] > 0 ? values[4, 2] : 0);
                    BlueBox5.Text = string.Format("{0:N7}", values[5, 2] > 0 ? values[5, 2] : 0);
                    BlueBox6.Text = string.Format("{0:N7}", values[6, 2] > 0 ? values[6, 2] : 0);
                }
            }
        }

        /// <summary>
        /// Clears and disables the UI components
        /// </summary>
        private void ClearUI()
        {
            EntriesComboBox.ItemsSource = null;
            EntriesComboBox.IsEnabled = false;
            RedBox0.Text = "";
            RedBox1.Text = "";
            RedBox2.Text = "";
            RedBox3.Text = "";
            RedBox4.Text = "";
            RedBox5.Text = "";
            RedBox6.Text = "";
            GreenBox0.Text = "";
            GreenBox1.Text = "";
            GreenBox2.Text = "";
            GreenBox3.Text = "";
            GreenBox4.Text = "";
            GreenBox5.Text = "";
            GreenBox6.Text = "";
            BlueBox0.Text = "";
            BlueBox1.Text = "";
            BlueBox2.Text = "";
            BlueBox3.Text = "";
            BlueBox4.Text = "";
            BlueBox5.Text = "";
            BlueBox6.Text = "";
            RedBox0.IsEnabled = false;
            RedBox1.IsEnabled = false;
            RedBox2.IsEnabled = false;
            RedBox3.IsEnabled = false;
            RedBox4.IsEnabled = false;
            RedBox5.IsEnabled = false;
            RedBox6.IsEnabled = false;
            GreenBox0.IsEnabled = false;
            GreenBox1.IsEnabled = false;
            GreenBox2.IsEnabled = false;
            GreenBox3.IsEnabled = false;
            GreenBox4.IsEnabled = false;
            GreenBox5.IsEnabled = false;
            GreenBox6.IsEnabled = false;
            BlueBox0.IsEnabled = false;
            BlueBox1.IsEnabled = false;
            BlueBox2.IsEnabled = false;
            BlueBox3.IsEnabled = false;
            BlueBox4.IsEnabled = false;
            BlueBox5.IsEnabled = false;
            BlueBox6.IsEnabled = false;
            Color0.Fill = null;
            Color1.Fill = null;
            Color2.Fill = null;
            Color3.Fill = null;
            Color4.Fill = null;
            Color5.Fill = null;
            Color6.Fill = null;
            Color0.Stroke = null;
            Color1.Stroke = null;
            Color2.Stroke = null;
            Color3.Stroke = null;
            Color4.Stroke = null;
            Color5.Stroke = null;
            Color6.Stroke = null;
        }

        /// <summary>
        /// Converts the decimal value of the RGB component to its equivalent value from 0 to 255.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private short RGBto255(float color)
        {
            short rgb = (short)Math.Round(Math.Pow(color, 1 / 2.2) * 255);

            return rgb;
        }

        private float RGBtoDecimal(short color)
        {
            float rgb = (float)Math.Pow((color / 255), 2.2);

            return rgb;
        }

        private float[] GetRGB(short index)
        {
            float red, green, blue;

            switch (index)
            {
                case 1:
                    red = float.Parse(RedBox1.Text);
                    green = float.Parse(GreenBox1.Text);
                    blue = float.Parse(BlueBox1.Text);
                    break;
                case 2:
                    red = float.Parse(RedBox2.Text);
                    green = float.Parse(GreenBox2.Text);
                    blue = float.Parse(BlueBox2.Text);
                    break;
                case 3:
                    red = float.Parse(RedBox3.Text);
                    green = float.Parse(GreenBox3.Text);
                    blue = float.Parse(BlueBox3.Text);
                    break;
                case 4:
                    red = float.Parse(RedBox4.Text);
                    green = float.Parse(GreenBox4.Text);
                    blue = float.Parse(BlueBox4.Text);
                    break;
                case 5:
                    red = float.Parse(RedBox5.Text);
                    green = float.Parse(GreenBox5.Text);
                    blue = float.Parse(BlueBox5.Text);
                    break;
                case 6:
                    red = float.Parse(RedBox6.Text);
                    green = float.Parse(GreenBox6.Text);
                    blue = float.Parse(BlueBox6.Text);
                    break;
                default:
                    red = float.Parse(RedBox0.Text);
                    green = float.Parse(GreenBox0.Text);
                    blue = float.Parse(BlueBox0.Text);
                    break;
            }

            return new float[] { red, green, blue };
        }

        private void UpdateColor(short index)
        {
            float[] rgb;

            rgb = GetRGB(index);
            switch (index)
            {
                case 1:
                    Color1.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)RGBto255(rgb[0]), (byte)RGBto255(rgb[1]), (byte)RGBto255(rgb[2])));
                    break;
                case 2:
                    Color2.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)RGBto255(rgb[0]), (byte)RGBto255(rgb[1]), (byte)RGBto255(rgb[2])));
                    break;
                case 3:
                    Color3.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)RGBto255(rgb[0]), (byte)RGBto255(rgb[1]), (byte)RGBto255(rgb[2])));
                    break;
                case 4:
                    Color4.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)RGBto255(rgb[0]), (byte)RGBto255(rgb[1]), (byte)RGBto255(rgb[2])));
                    break;
                case 5:
                    Color5.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)RGBto255(rgb[0]), (byte)RGBto255(rgb[1]), (byte)RGBto255(rgb[2])));
                    break;
                case 6:
                    Color6.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)RGBto255(rgb[0]), (byte)RGBto255(rgb[1]), (byte)RGBto255(rgb[2])));
                    break;
                default:
                    Color0.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)RGBto255(rgb[0]), (byte)RGBto255(rgb[1]), (byte)RGBto255(rgb[2])));
                    break;
            }
        }

        private void EntriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadValues(EntriesComboBox.SelectedIndex);
            for (short i = 0; i < 7; i++)
                UpdateColor(i);
        }

        private void TextBox0_KeyUP(object sender, KeyEventArgs e)
        {
            UpdateColor(0);
        }

        private void TextBox1_KeyUP(object sender, KeyEventArgs e)
        {
            UpdateColor(1);
        }

        private void TextBox2_KeyUP(object sender, KeyEventArgs e)
        {
            UpdateColor(2);
        }
        private void TextBox3_KeyUP(object sender, KeyEventArgs e)
        {
            UpdateColor(3);
        }
        private void TextBox4_KeyUP(object sender, KeyEventArgs e)
        {
            UpdateColor(4);
        }
        private void TextBox5_KeyUP(object sender, KeyEventArgs e)
        {
            UpdateColor(5);
        }
        private void TextBox6_KeyUP(object sender, KeyEventArgs e)
        {
            UpdateColor(6);
        }
    }
}
