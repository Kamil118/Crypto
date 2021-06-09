using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
namespace Crypto
{
    class FileClass : IDisposable
    {

        private FileStream fi;
        private BinaryReader fird;
        private FileStream fo;
        private BinaryWriter ford;

        private String inputPath;
        private String outputPath;
        public FileClass(String inputPath, String outputPath)
        {
            this.inputPath = inputPath;
            this.outputPath = outputPath;
        }


        //decrypt and encrypt should calculate hash of decrypted file as they are processing the data
        public void encrypt(RSACryptoServiceProvider rsa)
        {

            string fileName;
            fi = File.Open(inputPath, FileMode.Open);
            fileName = Path.GetExtension(inputPath);
            fo = File.Open(outputPath, FileMode.CreateNew);
            fird = new BinaryReader(fi);
            ford = new BinaryWriter(fo);
            try
            {
                byte[] hashbuffer = new byte[20];
                byte[] hashbuffer1 = new byte[256];
                ford.Write(hashbuffer);
                var aes = Aes.Create();
                aes.IV = Encoding.ASCII.GetBytes("1234567812345678");
                aes.Mode = CipherMode.CBC;
                byte[] encryptedKey = rsa.Encrypt(aes.Key, true);
                ford.Write(encryptedKey); //ENCRYPT THIS
                
                int filenamelength = fileName.Length;
                ford.Write(Encoding.ASCII.GetBytes(fileName));
                ford.BaseStream.Seek(8 - filenamelength, SeekOrigin.Current);
                HashAlgorithm hash = SHA1.Create();

                var encryptor = aes.CreateEncryptor();
                var cs = new CryptoStream(fo, encryptor, CryptoStreamMode.Write);
                while (fi.Position != fi.Length)
                {

                    byte[] buf = fird.ReadBytes(16);
                    cs.Write(buf);

                    hashbuffer1 = hash.ComputeHash(buf);
                    for (int j = 0; j < 20; ++j)
                    {
                        hashbuffer[j] = (byte)(hashbuffer[j] ^ hashbuffer1[j]);
                    }

                }
                cs.FlushFinalBlock();
                fo.Seek(0, SeekOrigin.Begin);
                ford.Write(hashbuffer);
                fo.Close();
            }
            catch (CryptographicException ex)
            {

            }
        }

        public void decrypt(RSACryptoServiceProvider rsa, string filename)
        {
            fi = File.Open(inputPath, FileMode.Open);

            fird = new BinaryReader(fi);

            try
            {
                HashAlgorithm hash = SHA1.Create();
                int size = 256;
                byte[] decryptedbuffer = new byte[size];
                byte[] buffer = new byte[size];
                byte[] hashbuffer1 = new byte[256];
                byte[] hashbuffer = new byte[256];
                byte[] hashbuffer2;
                string fileext;

                hashbuffer2 = fird.ReadBytes(20);

                var encryptedkey = fird.ReadBytes(256);//decrypt this

                byte[] decrytpedkey = rsa.Decrypt(encryptedkey,true);//wstaw odszyfrowny klucz do tej zmiennej

                int extsize = 0;
                byte[] tmp = fird.ReadBytes(8);
                for (int i = 0; i < 8; ++i)
                {
                    if (tmp[i] != 0)
                    {
                        extsize++;

                    }
                    if (tmp[i] == 0)
                    {
                        break;
                    }
                }
                byte[] tmp2 = new byte[extsize];
                for (int i = 0; i < extsize; ++i)
                {
                    tmp2[i] = tmp[i];
                }
                string fileName1 = Encoding.ASCII.GetString(tmp2);
                filename += fileName1;
                fo = File.Open(outputPath + filename, FileMode.CreateNew);
                ford = new BinaryWriter(fo);
                string filename2 = filename;

                var aes = Aes.Create();
                aes.IV = Encoding.ASCII.GetBytes("1234567812345678");
                aes.Mode = CipherMode.CBC;
                aes.Key = decrytpedkey;

                var decryptor = aes.CreateDecryptor();
                var cs = new CryptoStream(fi, decryptor, CryptoStreamMode.Read);
                var csr = new BinaryReader(cs);
                while (fi.Position != fi.Length)
                {

                    var buf = csr.ReadBytes(16);
                    ford.Write(buf);

                    hashbuffer1 = hash.ComputeHash(buf);
                    for (int j = 0; j < 20; ++j)
                    {
                        hashbuffer[j] = (byte)(hashbuffer[j] ^ hashbuffer1[j]);
                    }

                }

                for (int i = 0; i < 20; ++i)
                {
                    if (hashbuffer[i] != hashbuffer2[i])
                    {
                        break;
                        System.Windows.MessageBox.Show("Hash doesn't match", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (CryptographicException ex)
            {
            }
        }

        public void Dispose()
        {
            fi.Close();
            fo.Close();
        }
    }
}
