using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BokujoMessage4.Format
{
    public class papa
    {
        private string _magicheader = "PAPA";
        private uint _fatoffsethead = 0xC;
        private uint _fatlenght;
        private uint _filecount;

        private FATPP[] _FAT;
        private List<byte[]> data; 

        private MemoryStream _mstream;
        private BinaryReader _binstream;

        public papa() {}

        public papa(byte[] input)
        {
            _mstream = new MemoryStream(input);
            LoadPP();
        }

        #region Return/set data
        public byte[] Data
        {
            get
            {
                return _mstream.ToArray();
            }
            set
            {
                _mstream = new MemoryStream(value);
                LoadPP(); // make sure it will reload all entry data based input data.
            }
        }

        public uint Count
        {
            get
            {
                return _filecount;
            }
        }

        public byte[] getSelectedData(int index)
        {
            return data[index];
        }

        public void setSelectedData(int index, byte[] input)
        {
            data[index] = input;
        }

        public void removeSelectedfile(int index)
        {
            data.RemoveAt(index);
            RecreateContent();
            LoadPP();
        }

        public void addSelectedfile(byte[] inputfile)
        {
            byte[] lastfile = data[data.Count-1];
            data.RemoveAt(data.Count-1);
            data.Add(inputfile);
            data.Add(lastfile);
            RecreateContent();
            LoadPP();
        }
        #endregion

        public void LoadData(byte[] input)
        {
            _mstream = new MemoryStream(input);
            LoadPP();
        }

        private void LoadPP()
        {
            _binstream = new BinaryReader(_mstream);

            _binstream.BaseStream.Position = 8;

            _fatoffsethead = _binstream.ReadUInt32();
            _fatlenght = _binstream.ReadUInt32();
            _filecount = _binstream.ReadUInt32();
            _FAT = new FATPP[_filecount];

            for (int i = 0; i < _filecount; i++)
            {
                _FAT[i].Offset = _binstream.ReadUInt32();
            }

            for (int i = 0; i < _filecount; i++)
            {
                if (i <_filecount - 1)
                {
                    _FAT[i].Lenght = _FAT[i + 1].Offset - _FAT[i].Offset;
                    
                }
                else
                {
                    _FAT[i].Lenght = (uint)_binstream.BaseStream.Length - _FAT[i].Offset;
                }
            }

            data = new List<byte[]>();

            _binstream.BaseStream.Position = _fatlenght + _fatoffsethead;

            for (int i = 0; i < _filecount; i++)
            {
                data.Add(_binstream.ReadBytes((int)_FAT[i].Lenght));
            }
        }

        public void RecreateContent()
        {
            int fcount = 8+12+(data.Count*4);

            for (int i = 0; i < data.Count; i++)
            {
                fcount += data[i].Length;
            }

            byte[] temp = new byte[fcount];
            _mstream = new MemoryStream(temp);
            BinaryWriter spwri = new BinaryWriter(_mstream);
            
            spwri.Write(Encoding.ASCII.GetBytes("PAPA"),0,4);
            spwri.Write(new byte[4]);
            spwri.Write(BitConverter.GetBytes(12),0,4);
            spwri.Write(BitConverter.GetBytes((data.Count * 4)+8), 0, 4);
            spwri.Write(BitConverter.GetBytes(data.Count), 0, 4);

            int tempval1 = ((data.Count*4)) + 8 + 12;

            for (int i = 0; i < data.Count; i++)
            {
                if (i > 0)
                {
                    tempval1 += data[i-1].Length;
                    spwri.Write(BitConverter.GetBytes(tempval1), 0, 4);
                }
                else
                {
                    spwri.Write(BitConverter.GetBytes(tempval1), 0, 4);
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                spwri.Write(data[i]);
            }
        }

        public static void Pack(string savePath, string[] inputPath)
        {
            /*
            int fcount = 8 + 12 + (inputPath.Length * 4);

            for (int i = 0; i < inputPath.Length; i++)
            {
                fcount += (int) (new FileInfo(inputPath[i])).Length;
            }

            byte[] temp = new byte[fcount];*/

            FileStream _stmstr = new FileStream(savePath,FileMode.OpenOrCreate,FileAccess.ReadWrite);
            BinaryWriter spwri = new BinaryWriter(_stmstr);

            spwri.Write(Encoding.ASCII.GetBytes("PAPA"), 0, 4);
            spwri.Write(new byte[4]);
            spwri.Write(BitConverter.GetBytes(12), 0, 4);
            spwri.Write(BitConverter.GetBytes((inputPath.Length * 4) + 8), 0, 4);
            spwri.Write(BitConverter.GetBytes(inputPath.Length), 0, 4);

            int tempval1 = ((inputPath.Length * 4)) + 8 + 12;

            for (int i = 0; i < inputPath.Length; i++)
            {
                if (i > 0)
                {
                    tempval1 += (int)(new FileInfo(inputPath[i-1])).Length;
                    spwri.Write(BitConverter.GetBytes(tempval1), 0, 4);
                }
                else
                {
                    spwri.Write(BitConverter.GetBytes(tempval1), 0, 4);
                }
            }

            for (int i = 0; i < inputPath.Length; i++)
            {
                spwri.Write(File.ReadAllBytes(inputPath[i]));
            }

            spwri.Close();
            _stmstr.Close();
        }

        public static void unPack(string openPath, string destFolder)
        {
            FileStream strread = new FileStream(openPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader binread = new BinaryReader(strread);

            binread.BaseStream.Position = 8;

            uint _fatoff = binread.ReadUInt32();
            uint _fatlght = binread.ReadUInt32();
            uint _fcount = binread.ReadUInt32();

            FATPP[] _FATo = new FATPP[_fcount];

            for (int i = 0; i < _fcount; i++)
            {
                _FATo[i].Offset = binread.ReadUInt32();
            }

            for (int i = 0; i < _fcount; i++)
            {
                if (i < _fcount - 1)
                {
                    _FATo[i].Lenght = _FATo[i + 1].Offset - _FATo[i].Offset;

                }
                else
                {
                    _FATo[i].Lenght = (uint)binread.BaseStream.Length - _FATo[i].Offset;
                }
            }

            binread.BaseStream.Position = _fatlght + _fatoff;

            for (int i = 0; i < _fcount; i++)
            {
                File.WriteAllBytes(destFolder+@"\"+i+".bin", binread.ReadBytes((int)_FATo[i].Lenght));
            }

            binread.Close();
            strread.Close();
        }
    }

    public struct FATPP
    {
        public uint Offset;
        public uint Lenght;
    }
}
