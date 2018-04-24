using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections.Specialized;

namespace WpfCursus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WindowMetRibbon : RibbonWindow
    {
        public WindowMetRibbon()
        {
            InitializeComponent();
            LeesMRU();
            StringCollection qatLijst = new StringCollection();
            if (WpfCurcus.Properties.Settings.Default.qat != null)
            {
                qatLijst = (StringCollection)WpfCurcus.Properties.Settings.Default.qat;
                int lijnNr = 0;
                while (lijnNr < qatLijst.Count)
                {
                    String commando = qatLijst[lijnNr];
                    String png = qatLijst[lijnNr + 1];
                    RibbonButton nieuweKnop = new RibbonButton();
                    BitmapImage icon = new BitmapImage();
                    icon.BeginInit();
                    icon.UriSource = new Uri(png);
                    icon.EndInit();
                    nieuweKnop.SmallImageSource = icon;

                    CommandBindingCollection ccol = this.CommandBindings;
                    foreach(CommandBinding cb in ccol)
                    {
                        RoutedUICommand rcb = (RoutedUICommand)cb.Command;
                        if (rcb.Text == commando)
                            nieuweKnop.Command = rcb;
                    }
                    Qat.Items.Add(nieuweKnop);
                    lijnNr += 2;
                }
            }
        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StringCollection qatLijst = new StringCollection();
            if (WpfCurcus.Properties.Settings.Default.qat != null)
                WpfCurcus.Properties.Settings.Default.qat.Clear();
            foreach (Object li in Qat.Items)
            {
                if (li.GetType() == typeof(RibbonButton))
                {
                    RibbonButton knop = (RibbonButton)li;
                    RoutedUICommand commando = (RoutedUICommand)knop.Command;
                    qatLijst.Add(commando.Text);
                    qatLijst.Add(knop.SmallImageSource.ToString());
                }
            }
            if (qatLijst.Count > 0)
                WpfCurcus.Properties.Settings.Default.qat = qatLijst;
            WpfCurcus.Properties.Settings.Default.Save();
        }

        
        private void LeesBestand(string bestandsnaam)
        {
            try
            {
                using (StreamReader bestand = new StreamReader(bestandsnaam))
                {
                    TextBoxVoorbeeld.Text = bestand.ReadLine();
                }
                BijwerkenMRU(bestandsnaam);
            }
            catch (Exception ex)
            {
                MessageBox.Show("openen mislukt : " + ex.Message);
            }
        }

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".rvb";
            dlg.Filter = "Ribbon documents |*.rvb";

            if (dlg.ShowDialog() == true)
            {
                LeesBestand(dlg.FileName);
            }
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".rvb";
                dlg.Filter = "Ribbon documents |*.rvb";

                if (dlg.ShowDialog() == true)
                {
                    using (StreamWriter bestand = new StreamWriter(dlg.FileName))
                    {
                        bestand.WriteLine(TextBoxVoorbeeld.Text);
                    }
                }
                BijwerkenMRU(dlg.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("opslaan mislukt : " + ex.Message);
            }
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TextBoxVoorbeeld.Text = string.Empty;
        }

        private void PrintExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog afdrukken = new PrintDialog();
            if (afdrukken.ShowDialog() == true)
            {
                MessageBox.Show("Hier zou worden afgedrukt");
            }
        }

        private void PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Hier zou een afdrukvoorbeeld moeten verschijnen");
        }

        private void HelpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Dit is helpscherm", "Help", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        private void MRUGallery_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LeesBestand(MRUGallery.SelectedValue.ToString());
        }

        private void Radio_Click(object sender, RoutedEventArgs e)
        {
            RadioButton keuze = (RibbonRadioButton)sender;
            BrushConverter bc = new BrushConverter();
            SolidColorBrush kleur = (SolidColorBrush)bc.ConvertFromString(keuze.Tag.ToString());
            TextBoxVoorbeeld.Foreground = kleur;
        }

        private void LeesMRU()
        {
            StringCollection mruLijst = new StringCollection();
            MRUGalleryCat.Items.Clear();
            if (WpfCurcus.Properties.Settings.Default.mru != null)
            {
                mruLijst = (StringCollection)WpfCurcus.Properties.Settings.Default.mru;
                for (int lijnNr = 0; lijnNr < mruLijst.Count; lijnNr++)
                {
                    MRUGalleryCat.Items.Add(mruLijst[lijnNr]);
                }
            }
        }
        private void BijwerkenMRU(string bestandsnaam)
        {
            StringCollection mruLijst = new StringCollection();
            if (WpfCurcus.Properties.Settings.Default.mru != null)
            {
                mruLijst = (StringCollection)WpfCurcus.Properties.Settings.Default.mru;
                int positie = mruLijst.IndexOf(bestandsnaam);
                if (positie >= 0)
                    mruLijst.RemoveAt(positie);
                else
                    if (mruLijst.Count >= 6) mruLijst.RemoveAt(5);
            }
            mruLijst.Insert(0, bestandsnaam);
            WpfCurcus.Properties.Settings.Default.mru = mruLijst;
            WpfCurcus.Properties.Settings.Default.Save();
            LeesMRU();
        }
    }
}

