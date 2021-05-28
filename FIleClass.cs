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
        
        public FileClass(String inputPath, String outputPath)
        {
            fi = File.Open(inputPath, FileMode.Open);
            fo = File.Open(outputPath , FileMode.CreateNew);            
            fird = new BinaryReader(fi);
            ford = new BinaryWriter(fo);                    
        }


        //decrypt and encrypt should calculate hash of decrypted file as they are processing the data
        public void encrypt(byte[] publicKey,byte[] privateKey)
        {           
            var rsa = new RSACryptoServiceProvider();
            int keysize;          
            try
            {
                rsa.ImportRSAPublicKey(publicKey, out keysize);
                rsa.ImportRSAPrivateKey(privateKey, out keysize); 
                int size = (rsa.KeySize - 384) / 8 + 37;
                int iter = (int)fi.Length / size;
                byte[] encryptedbuffer = new byte[size];
                byte[] buffer = new byte[size];
                byte[] hashbuffer = new byte[256];
                byte[] hashbuffer1 = new byte[256];
                ford.Write(hashbuffer);
                for (int i = 0; i <= iter; i++)
                {
                    buffer = fird.ReadBytes(size);
                    hashbuffer1 =  rsa.SignData(buffer, SHA1.Create());
                    for(int j = 0; j < 256; ++j)
                    {
                        hashbuffer[j] = (byte)(hashbuffer[j] ^ hashbuffer1[j]);
                    }
                    encryptedbuffer = rsa.Encrypt(buffer, false);
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

        public void decrypt(byte[] publicKey, byte[] privateKey)
        {
            int keysize;
            var rsa = new RSACryptoServiceProvider();
            try
            {
                rsa.ImportRSAPrivateKey(privateKey, out keysize);
               // rsa.ImportRSAPublicKey(publicKey, out keysize);
                int size = rsa.KeySize / 8;
                byte[] decryptedbuffer = new byte[size];
                byte[] buffer = new byte[size];
                byte[] hashbuffer1 = new byte[256];
                byte[] hashbuffer = new byte[256];
                byte[] hashbuffer2;
                hashbuffer2 = fird.ReadBytes(256);
                int iter = ((int)fi.Length-256) / size;
                for (int i = 0; i < iter; i++)
                {
                    buffer = fird.ReadBytes(size);
                    decryptedbuffer = rsa.Decrypt(buffer, false);
                    hashbuffer1 = rsa.SignData(decryptedbuffer, SHA1.Create());
                    for (int j = 0; j < 256; ++j)
                    {
                        hashbuffer[j] = (byte)(hashbuffer[j] ^ hashbuffer1[j]);
                    }
                    ford.Write(decryptedbuffer);
                }
                bool f = true;
                for (int i = 0; i < 256; ++i)
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
