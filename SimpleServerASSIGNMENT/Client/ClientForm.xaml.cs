using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Client
{
    /// <summary>
    /// Interaction logic for ClientForm.xaml
    /// </summary>
    public partial class ClientForm : Window
    {
        
        private Client m_client;


        public ClientForm(Client Client)
        {
            m_client = Client;
            InitializeComponent();
        }

        
        public void UpdateChatWindow(string message)
        {
            MessageWindow.Dispatcher.Invoke(() =>
            {
                MessageWindow.Text += message + Environment.NewLine;
                MessageWindow.ScrollToEnd();
            });
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            m_client.SendMessage(InputWindow.Text);
            InputWindow.Text = "";
        }
    }
}
