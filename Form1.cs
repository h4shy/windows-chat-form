using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// using ChatService;

namespace chat_app
{
    public partial class Form1 : Form
    {
        private ServiceAPI service;
        private ConnectionChannel channel;

        public Form1()
        {
            InitializeComponent();
            // Initialize the chat service after the form
            InitializeService();
            this.FormClosing += Form1_FormClosing;
        }

        public void InitializeService(string apiKey = "API_KEY")
        {
            // Connect to the chat service with your API key -- just for testing -- don't give out your app with your api key hardcoded
            service = new ServiceAPI(apiKey);
            service.Connect(args =>
            {
                this.Invoke((MethodInvoker)delegate {
                    ChatBox1.Items.Add("Connected Successfully!");
                });
            });
            // Connect to a channel and register a message listener
            channel = service.GetChannel("Channel1");
            channel.Listener(Channel_MessageReceived);

            service.Disconnect(Connection_Closed);
        }

        private void Channel_MessageReceived(Message messageData)
        {
            string messageContent = messageData.ToString();
            // Display incoming messages
            this.Invoke((MethodInvoker)delegate {
                ChatBox1.Items.Add(messageContent);
            });
        }

        private void Connection_Closed()
        {
            this.Invoke((MethodInvoker)delegate {
                ChatBox1.Items.Clear();
                ChatBox1.Items.Add("Closed Connection.");
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            service.Connect();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            service.Close();
        }

        private void richTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                // Prevent new line
                e.SuppressKeyPress = true;

                // Send via Enter
                SendButton.PerformClick(); 
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (ChatInputBox1.Text.Length < 1 || NameInputBox1.Text.Length < 3)
            {
                ChatBox1.Items.Add("ERROR: MISSING REQUIRED FIELD");
                return;
            }
            if (ChatInputBox1.Text == "!clear")
            {
                ChatBox1.Items.Clear();
            }
            // Publish a message
            string senderName = NameInputBox1.Text;
            string messageContent = ChatInputBox1.Text;
            string fullMessage = $"{senderName}: {messageContent}";
            channel.Publish("Channel1", fullMessage);
            ChatInputBox1.Text = "";
        }
    }
}
