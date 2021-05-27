using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Crypto
{
    class FileClass : IDisposable
    {

        private FileStream fi;
        private BinaryReader fird;
        private FileStream fo;
        private BinaryReader ford;

        public FileClass(String inputPath, String outputPath)
        {
            fi = File.Open(inputPath, FileMode.Open);
            fo = File.Open(outputPath, FileMode.CreateNew);
            fird = new BinaryReader(fi);
            ford = new BinaryReader(fo);
        }


        //decrypt and encrypt should calculate hash of decrypted file as they are processing the data
        public void encrypt(String key)
        {
            
        }

        public void decrypt(String key)
        {

        }
        public void Dispose()
        {
            fi.Close();
            fo.Close();
        }
    }
}
