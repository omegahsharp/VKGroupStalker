using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using VkNet;
using VkNet.Categories;
using VkNet.Exception;
using VkNet.Utils;
using VkNet.Enums.Filters;
using VkNet.Properties;
using VkNet.Model.RequestParams;
using System.Diagnostics;

namespace GroupStalker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        VkApi vk = new VkApi();
        public static Hashtable Total = new Hashtable();
        public static Hashtable Map = new Hashtable();
        public static bool isconnected = false;

        public static bool VkAuth(VkApi vk)
        {
            try
            {
            string cfgtxt = File.ReadAllText("config.txt");
            string[] cfg = SplitIt(cfgtxt, ":");
            ulong appID = UInt64.Parse(cfg[1]);
            string email = cfg[3];
            string pass = cfg[5];
            Settings scope = Settings.All;
            
                vk.Authorize(new ApiAuthParams
                {
                    ApplicationId = appID,
                    Login = email,
                    Password = pass,
                    Settings = scope
                });
                return true;
            }
            catch (VkApiException err) { return false; }
        }

        public static string[] SplitIt(string buf, string sep)
        {
            string[] seps = new string[] { sep, Environment.NewLine };
            string[] buf1 = buf.Split(seps, StringSplitOptions.RemoveEmptyEntries);
            return buf1;
        }


        private void ConnectButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            if ((VkAuth(vk) == true) && (isconnected == false))
            { ConnectButton.Content = "Connected"; isconnected = true; }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (isconnected == true)
            {
                NextButton.IsEnabled = false;
                avatar.Source = null;
                Random rnd = new Random();
                Grlist.Items.Clear();
                Map = new Hashtable();

                var gids = new long[int.Parse(iterbox.Text)];
                int cou1 = 0;
                try
                {
                    while (cou1 < int.Parse(iterbox.Text))
                    {
                        long rou = rnd.Next(int.Parse(minbox.Text), int.Parse(maxbox.Text));
                        if (gids.Contains(rou) == false)
                        {
                            gids[cou1] = rou;
                            cou1++;
                        }
                    }
                }
                catch (Exception err) { }

                var groups = vk.Groups.GetById(gids, GroupsFields.All).ToList();
                if (groups.Count > 0)
                {
                    foreach (var grp in groups)
                    {
                        if (grp.Name != null)
                        {
                            if ((grp.MembersCount >= int.Parse(minusrbox.Text)) && (grp.MembersCount < int.Parse(maxusrbox.Text)) && (Total.ContainsKey(grp.Id) == false))
                            {
                                try
                                {
                                    string tmpname = grp.Name.ToString();
                                    tmpname = tmpname.Replace(Environment.NewLine, " ");
                                    tmpname = "[" + grp.MembersCount.ToString() + "] " + tmpname;

                                    ListBoxItem itm = new ListBoxItem();
                                    itm.Content = tmpname;
                                    Grlist.Items.Add(itm);
                                    Total.Add(grp.Id, grp.Photo100);
                                    Map.Add(Grlist.Items.Count - 1, grp.Id);
                                }
                                catch (Exception err) { }
                            }
                        }

                    }
                }
                NextButton.IsEnabled = true;
            }
        }

        private void Grlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int si = Grlist.SelectedIndex;
                string ImgUri = Total[Map[si]].ToString();
                if (ImgUri.Length > 10)
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(ImgUri, UriKind.RelativeOrAbsolute);
                    bitmap.EndInit();
                    avatar.Source = bitmap;
                }
            }
            catch (Exception err) { }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (Grlist.SelectedIndex > -1)
            {
                int si = Grlist.SelectedIndex;
                string GroupUri = @"https://vk.com/club" + Map[si].ToString();
                System.Diagnostics.Process.Start(GroupUri);
            }
        }

      

     
    }
}
