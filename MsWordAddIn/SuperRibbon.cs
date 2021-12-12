using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace MsWordAddIn
{
    public partial class SuperRibbon
    {
        private String defaultkey = "12345678";
        private String defaultIV = "09876543";

        private void SuperRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void encrypt_Click(object sender, RibbonControlEventArgs e)
        {
            //Word.Document doc
            String plain = Globals.ThisAddIn.Application.Selection.Text;

            CFB cfb = new CFB(Encoding.ASCII.GetBytes(plain), null, defaultkey, defaultIV);
            byte[] resbyte = cfb.encrypt();
            Globals.ThisAddIn.Application.Selection.Text = ByteArrayToString(resbyte);
        }

        private void decrypt_Click(object sender, RibbonControlEventArgs e)
        {
            String cipher = Globals.ThisAddIn.Application.Selection.Text;

            CFB cfb = new CFB(null, StringToByteArray(cipher), defaultkey, defaultIV);
            byte[] resbyte = cfb.decrypt();

            Globals.ThisAddIn.Application.Selection.Text = Encoding.ASCII.GetString(resbyte);
        }

        private String ByteArrayToString(byte[] b)
        {
            StringBuilder hex = new StringBuilder(b.Length * 2);
            foreach (byte a in b)
            {
                hex.AppendFormat("{0:x2}", a);
            }
            return hex.ToString();
        }

        private byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
