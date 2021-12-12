using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCipher
{
    class CBC
    {
        private String key;
        private string iv;

        private byte[] plain;
        private byte[] cipher;

        public CBC(string plain, string cipher, String key, String iv)
        {
            this.plain = ConvertToBinary(plain);
            this.key = key;
            this.iv = iv;
            this.cipher = ConvertToBinary(cipher);
        }

        public byte[] xor(byte[] i, byte[] key)
        {
            byte[] res = new byte[i.Length];

            for (int j = 0; j < i.Length; j++)
            {
                res[j] = (byte)(i[j] ^ key[j]);
            }

            return res;
        }

        public byte[] ConvertToBinary(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public byte[][] SplitText(byte[] p, int mod){
            byte[][] res = new byte[p.Length/mod][];
            int a = 0;

            for (int i = 0; i < res.Length; i++) {
                res[i] = new byte[mod];
                for (int x = 0; x < res[i].Length; x++) {
                    res[i][x] = p[a];
                    a++;
                }
            }
            return res;
        }

        public byte[] encipher(byte[] plaintext) {
            Console.WriteLine("CBC Encipher");

            int blockLength = ConvertToBinary(this.key).Length;
            string _iv = this.iv;
            byte[] res = new byte[plaintext.Length];
            byte[][] ciphers = new byte[plaintext.Length / blockLength][];
            byte[][] split = SplitText(plaintext, blockLength);

            Console.WriteLine("plaintext conversion result: ");
            Console.WriteLine(" ");
            for (int i = 0; i < split.Length; i++)
            {
                for (int j = 0; j < split[i].Length; j++)
                {
                    Console.WriteLine(split[i][j].ToString());
                }
            }
            
            //operasi pertama cbc
            ciphers[0] = xor(ConvertToBinary(this.key), xor(split[0], ConvertToBinary(_iv)));
            //operasi ke-2 s/d n
            for (int i = 1; i < split.Length; i++) {
                ciphers[i] = xor(ConvertToBinary(this.key), xor(split[i], ciphers[i-1]));
            }

            int a = 0;
            for (int i = 0; i < ciphers.Length; i++)
            {
                for (int j = 0; j < ciphers[i].Length; j++)
                {
                    res[a] = ciphers[i][j];
                    a++;
                }
            }

            Console.WriteLine(" ");
            Console.WriteLine("CBC result: ");
            Console.WriteLine(" ");
            for (int i = 0; i < res.Length; i++)
            {
                Console.WriteLine(res[i].ToString());
            }

            return res;
        }

        public byte[] decipher(byte[] ciphertext) {
            Console.WriteLine("CBC Decipher");

            int blockLength = ConvertToBinary(this.key).Length;
            string _iv = this.iv;
            byte[] res = new byte[ciphertext.Length];
            byte[][] plain = new byte[ciphertext.Length/blockLength][];
            byte[][] split = SplitText(ciphertext, blockLength);

            //operasi pertama cbc
            plain[0] = xor(ConvertToBinary(_iv), xor(split[0], ConvertToBinary(this.key)));
            //operasi ke-2 s/d n
            for (int i = 1; i < split.Length; i++)
            {
                plain[i] = xor(split[i-1], xor(ConvertToBinary(this.key), split[i]));
            }

            //isi ke result
            int a = 0;
            for (int i = 0; i < plain.Length; i++)
            {
                for (int j = 0; j < plain[i].Length; j++)
                {
                    res[a] = plain[i][j];
                    a++;
                }
            }

            Console.WriteLine(" ");
            Console.WriteLine("CBC decipher result: ");
            Console.WriteLine(" ");
            for (int i = 0; i < res.Length; i++)
            {
                Console.WriteLine(res[i].ToString());
            }

            return res;
        }
    }
}
