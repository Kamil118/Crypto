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
                int size = (rsa.KeySize - 384) / 8 ;
                int iter = (int)fi.Length / size;
                byte[] encryptedbuffer = new byte[size];
                byte[] buffer = new byte[size];
                byte[] hashbuffer = new byte[20];
                byte[] hashbuffer1 = new byte[256];
                ford.Write(hashbuffer);
                int filenamelength = fileName.Length;
                ford.Write(Encoding.ASCII.GetBytes(fileName));
                ford.BaseStream.Seek(8 - filenamelength, SeekOrigin.Current);
                HashAlgorithm hash = SHA1.Create();
                for (int i = 0; i <= iter; i++)
                {
                    buffer = fird.ReadBytes(size);
                    hashbuffer1 = hash.ComputeHash(buffer);
                    for (int j = 0; j < 20; ++j)
                    {
                        hashbuffer[j] = (byte)(hashbuffer[j] ^ hashbuffer1[j]);
                    }
                    encryptedbuffer = rsa.Encrypt(buffer, true);
                    ford.Write(encryptedbuffer);
                }

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


                int iter = ((int)fi.Length - 28) / size;
                for (int i = 0; i < iter; i++)
                {
                    buffer = fird.ReadBytes(size);
                    decryptedbuffer = rsa.Decrypt(buffer, true);
                    hashbuffer1 = hash.ComputeHash(decryptedbuffer);
                    for (int j = 0; j < 20; ++j)
                    {
                        hashbuffer[j] = (byte)(hashbuffer[j] ^ hashbuffer1[j]);
                    }
                    ford.Write(decryptedbuffer);
                }
                bool f = true;
                for (int i = 0; i < 20; ++i)
                {
                    if (hashbuffer[i] != hashbuffer2[i])
                    {
                        f = false;
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
