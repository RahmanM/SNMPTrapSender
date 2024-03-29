﻿using SnmpSharpNet;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace SNMP.Trap.Sender
{
    public partial class SNMPTrapSenderForm : Form
    {
        private string _key;

        public SNMPTrapSenderForm()
        {
            InitializeComponent();
        }

        private static void SendTrap(string message, string ipAddressToSend, int portToSend)
        {
            TrapAgent agent = new TrapAgent();

            // Variable Binding collection to send with the trap
            VbCollection col = new VbCollection();
            col.Add(new Oid("1.3.6.1.2.1.1.1.0"), new OctetString(message));
            //col.Add(new Oid("1.3.6.1.2.1.1.2.0"), new Oid("1.3.6.1.9.1.1.0"));
            //col.Add(new Oid("1.3.6.1.2.1.1.3.0"), new TimeTicks(2324));
            col.Add(new Oid("1.3.6.1.2.1.1.4.0"), new OctetString(DateTime.Now.ToString()));
            col.Add(new Oid("1.3.6.1.2.1.1.1.0"), new OctetString("Critical"));

            // Send the trap to the localhost port 162
            agent.SendV1Trap(new IpAddress(ipAddressToSend), portToSend, "public",
                             new Oid("1.3.6.1.2.1.1.1.0"), new IpAddress(GetLocalIPAddress()),
                             SnmpConstants.WarmStart, 0, 13432, col);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void SNMPTrapSenderForm_Load(object sender, EventArgs e)
        {
            txtHost.Text = GetLocalIPAddress();
            txtPort.Text = 162.ToString();
            _key = Guid.NewGuid().ToString();

            var message = GetMessage();
            txtMessage.Text = message;

        }

        private string GetMessage()
        {
            var message =
$@"Source: {GetLocalIPAddress()} 
Node: {GetLocalIPAddress()} 
Type: Event 
Description: {txtDescription.Text}
Severity: Critical
Resource: {txtResource.Text}
Date and Time: {DateTime.Now.ToString()}
Key: {_key}
";

            return message;
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtDescription.Text)) throw new Exception("Please provide a description for the message to be sent.");
                if (string.IsNullOrEmpty(txtResource.Text)) throw new Exception("Please provide a resource name e.g. Customers database etc");

                SendTrap(txtMessage.Text, txtHost.Text, Int32.Parse(txtPort.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            var message = GetMessage();
            txtMessage.Text = message;
        }

        private void txtResource_TextChanged(object sender, EventArgs e)
        {
            var message = GetMessage();
            txtMessage.Text = message;
        }
    }
}
