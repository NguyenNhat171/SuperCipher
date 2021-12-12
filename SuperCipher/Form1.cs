using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace SuperCipher
{
    public partial class Form1 : Form
    {
        private String filepath;
        private String extension;

        public Form1()
        {
            InitializeComponent();
            textBox3.ReadOnly = true;
            this.filepath = "harus.diganti";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //hilangkan IV
            label2.Visible = true;
            ivBox.Visible = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //hilangkan IV
            label2.Visible = false;
            ivBox.Visible = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            //tampilkan IV
            if (label2.Visible == false)
            {
                label2.Visible = true;
            }
            if (ivBox.Visible == false)
            {
                ivBox.Visible = true;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            //tampilkan IV
            if (label2.Visible == false)
            {
                label2.Visible = true;
            }
            if (ivBox.Visible == false)
            {
                ivBox.Visible = true;
            }
        }
        //Chọn file để giải mã
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                labelText.Text = "";
                keyBox.Text = "";
                ivBox.Text = "";
                MessageBox.Show("File: " + openFileDialog1.FileName, "Confirm", MessageBoxButtons.YesNo);
                
                    this.filepath = openFileDialog1.FileName;
                    Console.WriteLine("filepath:{0}", this.filepath);
                    labelText.Text += openFileDialog1.FileName;
                
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //savefile
            String path = System.IO.Directory.GetCurrentDirectory();
            System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "/" + "default.txt", textBox3.Text);
        }
        
        //encript
        private void button2_Click(object sender, EventArgs e)
        {
            //menyimpan hasil paling akhir
            String result;

            //menyimpan hasil tiap mode
            byte[] modeResult = null;
            String mode = "";

            //check input file
            if (openFileDialog1.FileName.Equals("openFileDialog1"))
            {
                MessageBox.Show("Input file", "Message", MessageBoxButtons.OK);
                return;
            }

            //validate key and IV
            Regex reg = new Regex("^[a-zA-Z0-9]*$");
            //key.length >= 8-byte
            if (keyBox.Text.Length < 8)
            {
                MessageBox.Show("Key chua du do dai", "Message", MessageBoxButtons.OK);
                return;
            }
            else if(!reg.IsMatch(keyBox.Text))
            {
                MessageBox.Show("Chua nhap Key", "Message", MessageBoxButtons.OK);
                return;
            }
            else
            {
                //IV.length = key.length
                if ((!radioButton2.Checked && !radioButton1.Checked) && (ivBox.Text.Length != keyBox.Text.Length))
                {
                    MessageBox.Show("Key chua du do dai", "Message", MessageBoxButtons.OK);
                    return;
                }
                else if ((!radioButton2.Checked && !radioButton1.Checked) && (ivBox.Text.Contains(".")))
                {
                    MessageBox.Show("Key chua du do dai \".\"", "Message", MessageBoxButtons.OK);
                    return;
                }
            }

            //read file content (plaintext)
            String dialogfilename = openFileDialog1.FileName;
            byte[] plain = System.IO.File.ReadAllBytes(dialogfilename);

            //check used mode
            if (radioButton1.Checked)
            {
                //ECB mode
                mode = "ECB";
                ECB ecb = new ECB (plain, null, keyBox.Text, ivBox.Text);
                modeResult = ecb.encrypt();
            }
            else if (radioButton2.Checked)
            {
                //generate IV
                //IV==key
                //CBC mode
                mode = "CBC";
                CBC cbc = new CBC(Encoding.ASCII.GetString(plain), "", keyBox.Text, ivBox.Text);
                ivBox.Text = keyBox.Text;
                modeResult = cbc.encipher(plain);
            }
            else if (radioButton3.Checked)
            {
                //CFB mode
                mode = "CFB";
                CFB cfb = new CFB(plain, null ,keyBox.Text,ivBox.Text);
                modeResult = cfb.encrypt();
            }
            else if (radioButton4.Checked)
            {
                //OFB mode
                mode = "OFB";
                OFB ofb = new OFB(plain, null, keyBox.Text, ivBox.Text);
                modeResult = ofb.encrypt();
            }
            //convert byte to hex
            result = ByteArrayToString(modeResult);
            textBox3.Text = result;
            //set header
            result += "." + ivBox.Text + "." + Path.GetFileNameWithoutExtension(filepath) + Path.GetExtension(filepath) + "." + mode + "." + (keyBox.Text.Length- (plain.Length % keyBox.Text.Length)).ToString();
            Print_TXT(sender, e, result,mode);

           
            
            

        }
        private void Print_TXT(object sender, EventArgs e,String ketqua,String mode)
        {
            
            string dirParameter = AppDomain.CurrentDomain.BaseDirectory + @"\"+ mode + "Encript.txt";
            FileStream fParameter = new FileStream(dirParameter, FileMode.Create, FileAccess.Write);
            StreamWriter m_WriterParameter = new StreamWriter(fParameter);
            m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
            m_WriterParameter.Write(ketqua);
            m_WriterParameter.Flush();
            m_WriterParameter.Close();
        }

        //decry
        private void button3_Click(object sender, EventArgs e)
        {
            //header variable
            String iv ;
            int padding;
            byte[] content = new byte[1];
            String mode = "";
            
            //instead of keyBox.Text use this
            String newKey = keyBox.Text;

            //result for each mode
            byte[] modeResult = new byte[keyBox.Text.Length];

            //validate input file
            if (openFileDialog1.FileName.Equals("openFileDialog1"))
            {
                MessageBox.Show("input file", "Message", MessageBoxButtons.OK);
                return;
            }

            //validasi kunci
            //key.length >= 8-byte
            if (keyBox.Text.Length < 8)
            {
                MessageBox.Show("Key chua du do dai", "Message", MessageBoxButtons.OK);
                return;
            }

            ////read file content (cipher)
            String dialogfilename = openFileDialog1.FileName;
            String cipher = System.IO.File.ReadAllText(dialogfilename);
            //String cipher = textBox3.Text;
            //no need for iv
            //int kt = 1;
            //get header
            String[] header = cipher.Split('.');
            if (header.Length != 6 )
            {
                MessageBox.Show("Tep khong giai ma duoc", "Message", MessageBoxButtons.OK);
                return;
            }
            else 
            {
                filepath = header[2];
                iv = header[1];
                mode = header[4];
                padding = Int32.Parse(header[5]);
                content = StringToByteArray(header[0]);
                extension = header[3];



                //validate key 
                if (content.Length % keyBox.Text.Length != 0 && !mode.Equals("CBC"))
                {
                    newKey = "";
                    Random rnd = new Random(content.Length);
                    for (int i = 0; i < (content.Length); i++)
                    {
                        newKey += rnd.Next() % 255;
                    }
                }

                if (mode.Equals("ECB"))
                {
                    //ECB mode
                    ECB ecb = new ECB(null, content, newKey, iv);
                    byte[] pbytes = ecb.decrypt();      // Giải mã ECB
                    Console.WriteLine("decrypt: {0}", ByteArrayToString(pbytes));

                    textBox3.Text = ByteArrayToString(pbytes);

                    if (extension.Equals("txt"))
                    {
                        //show plaintext if using text extension
                        textBox3.Text += Environment.NewLine + Environment.NewLine + Encoding.ASCII.GetString(pbytes);
                        Console.WriteLine("textbox3 {0}", textBox3.Text);
                    }
                    else
                    {
                        //savefile
                        String path = System.IO.Directory.GetCurrentDirectory();
                        System.IO.File.WriteAllBytes(System.IO.Directory.GetCurrentDirectory() + "/" + filepath + "." + extension, pbytes);
                    }
                }
                else
                if (mode.Equals("CBC"))
                {
                    //Generate IV
                    //CBC mode Encoding.ASCII.GetString(content)
                    CBC cbc = new CBC("", Encoding.ASCII.GetString(content), newKey, iv);
                    byte[] pbytes = cbc.decipher(content);
                    Console.WriteLine("decrypt: {0}", ByteArrayToString(pbytes));
                    textBox3.Text = ByteArrayToString(pbytes);
                    MessageBox.Show(textBox3.Text.ToString(), "Message", MessageBoxButtons.OK);


                    if (extension.Equals("txt"))
                    {
                        //show plaintext if using text extension
                        textBox3.Text += Environment.NewLine + Environment.NewLine + Encoding.ASCII.GetString(pbytes);
                        MessageBox.Show(Encoding.ASCII.GetString(pbytes), "Message", MessageBoxButtons.OK);

                        Console.WriteLine("textbox3 {0}", textBox3.Text);
                    }
                    else
                    {
                        //savefile
                        String path = System.IO.Directory.GetCurrentDirectory();
                        System.IO.File.WriteAllBytes(System.IO.Directory.GetCurrentDirectory() + "/" + filepath + "." + extension, pbytes);
                    }
                }
                else
                if (mode.Equals("CFB"))
                {
                    //CFB mode
                    CFB cfb = new CFB(null, content, keyBox.Text, iv);
                    byte[] pbytes = cfb.decrypt();
                    Console.WriteLine("decrypt: {0}", ByteArrayToString(pbytes));
                    textBox3.Text = ByteArrayToString(pbytes);

                    if (extension.Equals("txt"))
                    {
                        //show plaintext if using text extension
                        textBox3.Text += Environment.NewLine + Environment.NewLine + Encoding.ASCII.GetString(pbytes);
                        Console.WriteLine("textbox3 {0}",textBox3.Text);
                    }
                    else
                    {
                        //savefile
                        String path = System.IO.Directory.GetCurrentDirectory();
                        System.IO.File.WriteAllBytes(System.IO.Directory.GetCurrentDirectory() + "/" + filepath + "." + extension,pbytes);
                    }
                }
                else
                if (mode.Equals("OFB"))
                {
                    //OFB mode
                    OFB ofb = new OFB(null, content, keyBox.Text, iv);
                    Console.WriteLine(iv);
                    byte[] pbytes = ofb.decrypt();
                    Console.WriteLine("decrypt: {0}", ByteArrayToString(pbytes));
                    textBox3.Text = ByteArrayToString(pbytes);

                    if (extension.Equals("txt"))
                    {
                        //show plaintext if using text extension
                        textBox3.Text += Environment.NewLine + Environment.NewLine + Encoding.ASCII.GetString(pbytes);
                        Console.WriteLine("textbox3 {0}", textBox3.Text);
                    }
                    else
                    {
                        //savefile
                        String path = System.IO.Directory.GetCurrentDirectory();
                        System.IO.File.WriteAllBytes(System.IO.Directory.GetCurrentDirectory() + "/" + filepath + "." + extension, pbytes);
                    }
                }
            }
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
