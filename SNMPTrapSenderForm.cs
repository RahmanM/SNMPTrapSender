using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNMP.Trap.Sender
{
    public partial class SNMPTrapSenderForm : Form
    {
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

            var message = GetMessage();
            txtMessage.Text = message;
        }

        private string GetMessage()
        {
            var message =
$@"Source: {GetLocalIPAddress()} 
Node: {GetLocalIPAddress()} 
Type: Event 
Description: 
Severity: Critical
Resource:
Date and Time: {DateTime.Now.ToString()}
";

            return message;
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            try
            {
                SendTrap(txtMessage.Text, txtHost.Text, Int32.Parse(txtPort.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
