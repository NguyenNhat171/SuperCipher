using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCipher
{
    public class CFB
    {
        //private String plain;
        //private String cipher;
        private String key;
        private string iv;

        private byte[] plain;
        private byte[] cipher;

        public CFB(byte[] plain, byte[] cipher, String key, String iv)
        {
            this.plain = plain;
            //Console.WriteLine("plain: {0}",plain[0]);

            this.key = key;
            this.iv = iv;
            this.cipher = cipher;
        }

        public byte[] encrypt()
        {
            //form shift register
            byte[] register = Encoding.ASCII.GetBytes(this.iv);
            byte[] ikey = new byte[register.Length];

            //start encryption
            Encry enkripsi = new Encry();

            Int32 blok = Encoding.ASCII.GetBytes(this.key).Length;
            byte[] pbyte = this.plain;
            byte[] cbyte = new byte[pbyte.Length];

            for (int i = 0; i <= (pbyte.Length / blok); i++)
            {
                //get internal key
                ikey = enkripsi.encrypt(Encoding.ASCII.GetBytes(key), register);
                //ikey = register;

                //get block-sized plaintext
                byte[] ci;
                byte[] pi;

                if(pbyte.Length - (i * blok) >= blok)
                {
                    ci = new byte[blok];
                    pi = new byte[blok];
                    pi = pbyte.Skip(i * blok).Take(blok).ToArray();
                }
                else
                {
                    pi = new byte[pbyte.Length - (i * blok)];
                    ci = new byte[pbyte.Length - (i * blok)];
                    pi = pbyte.Skip(i * blok).Take(pbyte.Length - (i * blok)).ToArray();
                }

                ikey = ikey.Skip(0).Take(pi.Length).ToArray();
                ci = xor(pi, ikey);
                for (int m = 0; m < ci.Length; m++)
                {
                    cbyte[i * blok + m] = ci[m];
                }

                //wrap register
                register = ci;
            }

            this.cipher = new byte[cbyte.Length];
            this.cipher = cbyte;
            return this.cipher;
        }

        //i.Length = key.Length
        public byte[] xor(byte[] i, byte[] key)
        {
            byte[] res = new byte[i.Length];
            for (int j = 0; j < i.Length; j++)
            {
                res[j] = (byte)(i[j] ^ key[j]);
            }
            
            return res;
        }

        public byte[] decrypt()
        {
            //form shift register
            byte[] register = Encoding.ASCII.GetBytes(this.iv);
            byte[] ikey = new byte[register.Length];

            //start decryption
            Encry enkripsi = new Encry();

            Int32 blok = Encoding.ASCII.GetBytes(this.key).Length;
            byte[] cbyte = this.cipher;
            byte[] pbyte = new byte[cbyte.Length];

            for (int i = 0; i <= (cbyte.Length / blok); i++)
            {
                //get internal key
                ikey = enkripsi.encrypt(Encoding.ASCII.GetBytes(key), register);
                //ikey = register;

                byte[] ci;
                byte[] pi;

                if (cbyte.Length - (i * blok) >= blok)
                {
                    ci = new byte[blok];
                    pi = new byte[blok];
                    ci = cbyte.Skip(i * blok).Take(blok).ToArray();
                }
                else
                {
                    pi = new byte[cbyte.Length - (i * blok)];
                    ci = new byte[cbyte.Length - (i * blok)];
                    ci = cbyte.Skip(i * blok).Take(cbyte.Length - (i * blok)).ToArray();
                }
                ikey = ikey.Skip(0).Take(ci.Length).ToArray();
                //Console.WriteLine("ci: {0}", ci[0]);
                //Console.WriteLine("ikey: {0}", ikey[0]);
                pi = xor(ci, ikey);
                //Console.WriteLine("pi: {0}", pi[0]);
                for (int m = 0; m < pi.Length; m++)
                {
                    pbyte[i * blok + m] = pi[m];
                }

                //wrap register
                register = ci;
            }

            //this.plain = Encoding.ASCII.GetString(pbyte);
            this.plain = new byte[pbyte.Length];
            this.plain = pbyte;
            //Console.WriteLine("plain[0]: {0}", this.plain[0]);
            return this.plain;
        }
    }
}
