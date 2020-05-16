using System;
using System.Collections.Generic;
using System.Linq;
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
using static expressionWpfTest1.Expression;

namespace expressionWpfTest1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        StringToValue exp;
        VariableCollection variableCollection;
        List<Label> namesList;
        List<TextBox> rangesList;
        List<Slider> slidersList;
        List<Label> valuesList;
        ScrollViewer sv1, sv2, sv3, sv4;
        public MainWindow()
        {

            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(Slider), Slider.MouseMoveEvent, new RoutedEventHandler(undateslide));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.KeyDownEvent, new RoutedEventHandler(undaterange));
            namesList = new List<Label>();
            rangesList = new List<TextBox>();
            slidersList = new List<Slider>();
            valuesList = new List<Label>();
            

            

        }
        
        private void undaterange(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != input)
            {
                string str = input.Text;
                StringToValue exp = new StringToValue(str);
                int i = 0;
                foreach (TextBox t1 in listBox2.Items)
                {
                    i++;
                    if (t1 == textBox)
                    {
                        break;
                    }
                }
                try
                {
                    double temp = Convert.ToDouble(textBox.Text);
                    double td = ((slidersList[i - 1].Value - 5) * temp) / 5;
                    valuesList[i - 1].Content = td;
                    variableCollection.vlist[i - 1].value = td;
                    exp.setVariables(variableCollection);
                    result.Content = exp.getValue();
                }
                catch (FormatException ee)
                {
                    MessageBox.Show("只能输入数字或者数字过大");
                }
            }
        }

 

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sv1 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.listBox1, 0), 0) as ScrollViewer;
            sv2 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.listBox2, 0), 0) as ScrollViewer;
            sv3 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.listBox3, 0), 0) as ScrollViewer;
            sv4 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.listBox4, 0), 0) as ScrollViewer;

            sv4.ScrollChanged += new ScrollChangedEventHandler(listBox4_ScrollChanged);
            sv3.ScrollChanged += new ScrollChangedEventHandler(listBox3_ScrollChanged);
            sv2.ScrollChanged += new ScrollChangedEventHandler(listBox2_ScrollChanged);
            sv1.ScrollChanged += new ScrollChangedEventHandler(listBox1_ScrollChanged);
        }



        private void listBox1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sv1 != null)
            {
                sv2.ScrollToVerticalOffset(sv1.VerticalOffset);
                sv3.ScrollToVerticalOffset(sv1.VerticalOffset);
                sv4.ScrollToVerticalOffset(sv1.VerticalOffset);
            }
        }

        private void listBox2_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sv2 != null)
            {
                sv1.ScrollToVerticalOffset(sv2.VerticalOffset);
                sv3.ScrollToVerticalOffset(sv2.VerticalOffset);
                sv4.ScrollToVerticalOffset(sv2.VerticalOffset);
            }
        }

        private void listBox3_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sv3 != null)
            {
                sv1.ScrollToVerticalOffset(sv3.VerticalOffset);
                sv2.ScrollToVerticalOffset(sv3.VerticalOffset);
                sv4.ScrollToVerticalOffset(sv3.VerticalOffset);
            }
        }

        private void listBox4_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            if (sv4 != null)
            {
                sv3.ScrollToVerticalOffset(sv4.VerticalOffset);
                sv2.ScrollToVerticalOffset(sv4.VerticalOffset);
                sv1.ScrollToVerticalOffset(sv4.VerticalOffset);
            }
        }

        private void undateslide(object sender, RoutedEventArgs e)
        {
            string str = input.Text;
            StringToValue exp = new StringToValue(str);

            Slider s = sender as Slider;
            int i = 0;
            foreach (Slider s1 in listBox3.Items)
            {
                i++;
                if (s1 == s)
                {
                    break;
                }
            }
            try
            {
                if (i <= listBox4.Items.Count)
                {
                    double td = (slidersList[i-1].Value - 5) * Convert.ToDouble(rangesList[i-1].Text);
                    td = td / 5;
                    valuesList[i-1].Content = td;
                    variableCollection.vlist[i-1].value = td;
                    exp.setVariables(variableCollection);
                    result.Content = exp.getValue();
                    //MessageBox.Show(exp.getValue().ToString());
                }
            }
            catch(Exception e1)
            {
                
                //MessageBox.Show(e1.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string s = input.Text;
            exp = new StringToValue(s);
            variableCollection = exp.GetVariables();
            result.Content =exp.getValue();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            namesList.Clear();
            rangesList.Clear();
            slidersList.Clear();
            valuesList.Clear();
            if (variableCollection.vlist != null)
            {
                for (int i = 0; i < variableCollection.vlist.Length; i++)
                {
                    Label name = new Label();
                    name.Content = variableCollection.vlist[i].name;
                    name.Height = 30;

                    TextBox textBox = new TextBox();
                    textBox.Text = "5";
                    textBox.Height = 30;
                    textBox.Width = listBox1.Width;
                    textBox.VerticalAlignment = VerticalAlignment.Center;

                    Slider slider1 = new Slider();
                    slider1.Height = 30;
                    slider1.Value = 6;
                    slider1.HorizontalAlignment = HorizontalAlignment.Center;
                    slider1.Width = 200;

                    Label var_value = new Label();
                    var_value.Content = "1";
                    var_value.Height = 30;

                    namesList.Add(name);
                    rangesList.Add(textBox);
                    slidersList.Add(slider1);
                    valuesList.Add(var_value);

                    listBox1.Items.Add(name);
                    listBox2.Items.Add(textBox);
                    listBox3.Items.Add(slider1);
                    listBox4.Items.Add(var_value);
                }
            }

           
        }


        
    }
}
