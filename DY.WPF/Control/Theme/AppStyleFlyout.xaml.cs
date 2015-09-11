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

using MahApps.Metro.Controls;
using MahApps.Metro;

using DY.WPF.SYSTEM;

namespace DY.WPF
{
    /// <summary>
    /// ThemeFlyout.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AppStyleFlyout : UserControl
    {
        public AppStyleFlyout()
        {
            InitializeComponent();
            LoadAppStyle();
            PushListBoxItems();
            SelectCurrnetAppStyle();
            NThemeBox.SelectionChanged += OnThemeSelectChanged;
            NAccentBox.SelectionChanged += OnAccentSelectChanged;
        }

        private void LoadAppStyle()
        {
            Dictionary<string, string> dictionary = Json<Dictionary<string, string>>.Read("./theme_config.json");
            if (dictionary == null)
                return;
            if (dictionary.ContainsKey("Accent") && dictionary.ContainsKey("AppTheme"))
            {
                string accent = dictionary["Accent"];
                string theme = dictionary["AppTheme"];
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(accent), ThemeManager.GetAppTheme(theme));
            }
        }
        
        private void SaveAppStyle()
        {
            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle();
            string theme = appStyle.Item1.Name;
            string accent = appStyle.Item2.Name;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("AppTheme", theme);
            dictionary.Add("Accent", accent);
            Json<Dictionary<string, string>>.Write("./theme_config.json", dictionary);
        }

        private void OnThemeSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = e.AddedItems[0] as ListBoxItem;
            if (item == null)
                return;
            string name = GetColorName(item);
            ThemeManager.ChangeAppTheme(Application.Current, name);
            SaveAppStyle();
        }

        private void OnAccentSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = e.AddedItems[0] as ListBoxItem;
            if (item == null)
                return;
            string name = GetColorName(item);
            Accent accent = ThemeManager.GetAccent(name);
            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, accent, appStyle.Item1);
            SaveAppStyle();
        }

        private string GetColorName(ListBoxItem item)
        {
            StackPanel panel = item.Content as StackPanel;
            TextBlock textBlock = panel.Children[1] as TextBlock;
            string name = textBlock.Text;
            return name;
        }

        private void SelectCurrnetAppStyle()
        {
            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);
            AppTheme theme = appStyle.Item1 as AppTheme;
            Accent accent = appStyle.Item2 as Accent;

            foreach(var i in NThemeBox.Items)
            {
                ListBoxItem item = i as ListBoxItem;
                string name = GetColorName(item);
                if(theme.Name == name)
                {
                    NThemeBox.SelectedItem = item;
                    break;
                }
            }

            foreach (var i in NAccentBox.Items)
            {
                ListBoxItem item = i as ListBoxItem;
                string name = GetColorName(item);
                if (accent.Name == name)
                {
                    NAccentBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void PushListBoxItems()
        {
            foreach (var i in ThemeManager.AppThemes)
            {
                var border = i.Resources["BlackColorBrush"] as Brush;
                var color = i.Resources["WhiteColorBrush"] as Brush;
                var name = i.Name;

                Ellipse sample = new Ellipse() { Width = 16, Height = 16, StrokeThickness = 1, Stroke = border, Fill = color };
                TextBlock text = new TextBlock() { Text = name, Margin = new Thickness(5, 0, 0, 0) };

                StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
                panel.Children.Add(sample);
                panel.Children.Add(text);

                ListBoxItem item = new ListBoxItem() { Content = panel };
                NThemeBox.Items.Add(item);
            }

            foreach (var i in ThemeManager.Accents)
            {
                var color = i.Resources["AccentColorBrush"] as Brush;
                var name = i.Name;
                Ellipse sample = new Ellipse() { Width = 16, Height = 16, StrokeThickness = 0, Fill = color };
                TextBlock text = new TextBlock() { Text = name, Margin = new Thickness(5, 0, 0, 0) };

                StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
                panel.Children.Add(sample);
                panel.Children.Add(text);

                ListBoxItem item = new ListBoxItem() { Content = panel };
                NAccentBox.Items.Add(item);
            }
        }
    }
}
