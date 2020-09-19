using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace BokujoMessage4.Format
{
    public class xbb
    {
        private BinaryReader _binstream;
        private XBBDAT[] _DAT;
        private XBBFAT[] _FAT;
        private uint _filecount;
        private string _magicheader = "XBB";
        private MemoryStream _mstream;
        private XBBUQ[] _XBBUQ;

        public xbb()
        {
        }

        public xbb(byte[] input)
        {
            _mstream = new MemoryStream(input);

            LoadXBB();
        }

        public XBBUQ getselectedUniq(int index)
        {
            return _XBBUQ[index];
        }

        public XBBUQ[] getselectedUniq()
        {
            return _XBBUQ;
        }

        public void Load(byte[] input)
        {
            _mstream = new MemoryStream(input);

            LoadXBB();
        }

        public void LoadXBB()
        {
            _binstream = new BinaryReader(_mstream);

            _binstream.BaseStream.Position = 4;

            _filecount = _binstream.ReadUInt32();

            _FAT = new XBBFAT[_filecount];
            _DAT = new XBBDAT[_filecount];
            _XBBUQ = new XBBUQ[_filecount];

            _binstream.BaseStream.Position = 0x20;

            for (var i = 0; i < _filecount; i++)
            {
                _FAT[i].Offset = _binstream.ReadUInt32();
                _FAT[i].Lenght = _binstream.ReadUInt32();
                _FAT[i].OffsetName = _binstream.ReadUInt32();
                _FAT[i].uniqueId = _binstream.ReadBytes(4);
            }

            for (var i = 0; i < _filecount; i++)
            {
                _XBBUQ[i].uniqueId = _binstream.ReadBytes(4);
                _XBBUQ[i].id = _binstream.ReadUInt32();
            }

            for (var i = 0; i < _filecount; i++)
            {
                if (i < _filecount - 1)
                {
                    _binstream.BaseStream.Position = _FAT[i].OffsetName;
                    _DAT[i].filename =
                        Encoding.ASCII.GetString(
                            _binstream.ReadBytes((int) _FAT[i + 1].OffsetName - (int) _FAT[i].OffsetName));
                    _binstream.BaseStream.Position = _FAT[i].Offset;
                    _DAT[i].data = _binstream.ReadBytes((int) _FAT[i].Lenght);
                }
                else
                {
                    _binstream.BaseStream.Position = _FAT[i].OffsetName;
                    _DAT[i].filename =
                        Encoding.ASCII.GetString(_binstream.ReadBytes((int) _FAT[0].Offset - (int) _FAT[i].OffsetName));
                    _binstream.BaseStream.Position = _FAT[i].Offset;
                    _DAT[i].data = _binstream.ReadBytes((int) _FAT[i].Lenght);
                }
            }
        }

        public void RecreateContent()
        {
            var lenght = (int) (32 + (16*_filecount) + (8*_filecount));
            var filenamelenght = 0;

            for (var i = 0; i < _DAT.Count(); i++)
            {
                filenamelenght += _DAT[i].filename.Replace("\0", "").Length;
            }

            filenamelenght += (1*_DAT.Count());
            var recount = recountlenght(filenamelenght);
            filenamelenght += recountlenght(filenamelenght);

            lenght += filenamelenght;

            for (var i = 0; i < _DAT.Count(); i++)
            {
                lenght += _DAT[i].data.Length;
            }

            var temp = new byte[lenght];
            _mstream = new MemoryStream(temp);
            var spwri = new BinaryWriter(_mstream);

            spwri.Write(Encoding.ASCII.GetBytes("XBB"), 0, 3);
            spwri.Write(BitConverter.GetBytes(1), 0, 1);
            spwri.Write(BitConverter.GetBytes(_DAT.Count()));
            spwri.Write(new byte[24], 0, 24);

            var startfileoffset = 32 + ((16*_DAT.Count()) + (8*_DAT.Count()))
                                  + filenamelenght;
            var startfilenameoffset = 32 + ((16*_DAT.Count()) + (8*_DAT.Count()));

            for (var i = 0; i < _DAT.Count(); i++)
            {
                if (i > 0)
                {
                    startfileoffset += _DAT[i - 1].data.Length;
                    startfilenameoffset += _DAT[i - 1].filename.Replace("\0", "").Length + 1;

                    spwri.Write(BitConverter.GetBytes(startfileoffset), 0, 4);
                    spwri.Write(BitConverter.GetBytes(_DAT[i].data.Length), 0, 4);
                    spwri.Write(BitConverter.GetBytes(startfilenameoffset), 0, 4);
                    spwri.Write(_FAT[i].uniqueId, 0, 4);
                }
                else
                {
                    spwri.Write(BitConverter.GetBytes(startfileoffset), 0, 4);
                    spwri.Write(BitConverter.GetBytes(_DAT[i].data.Length), 0, 4);
                    spwri.Write(BitConverter.GetBytes(startfilenameoffset), 0, 4);
                    spwri.Write(_FAT[i].uniqueId, 0, 4);
                }
            }

            for (var i = 0; i < _DAT.Count(); i++)
            {
                spwri.Write(_XBBUQ[i].uniqueId, 0, 4);
                spwri.Write(BitConverter.GetBytes(_XBBUQ[i].id), 0, 4);
            }

            for (var i = 0; i < _DAT.Count(); i++)
            {
                spwri.Write(Encoding.ASCII.GetBytes(_DAT[i].filename.Replace("\0", "")));
                spwri.Write(new byte[1], 0, 1);
            }

            spwri.Write(new byte[recount], 0, recount);

            for (var i = 0; i < _DAT.Count(); i++)
            {
                spwri.Write(_DAT[i].data);
            }
        }

        private static int recountlenght(int lenght)
        {
            var inp0 = lenght%4;
            return 4 - inp0;
        }

        public static void Pack(string savePath, string[] inputPath, XBBUQ[] uniqueValue)
        {
            int _filecount = inputPath.Length;

            /*var lenght = (int)(32 + (16 * _filecount) + (8 * _filecount));*/
            var filenamelenght = 0;

            XBBUQ[] tempuq = uniqueValue.OrderBy(x => x.id).ToArray();

            for (var i = 0; i < _filecount; i++)
            {
                filenamelenght += Path.GetFileName(inputPath[i]).Length;
            }

            filenamelenght += (1 * _filecount);
            var recount = recountlenght(filenamelenght);
            filenamelenght += recountlenght(filenamelenght);
            /*
            lenght += filenamelenght;

            for (var i = 0; i < _filecount; i++)
            {
                lenght += (int)(new FileInfo(inputPath[i])).Length;
            }

            var temp = new byte[lenght];*/

            FileStream _mstream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var spwri = new BinaryWriter(_mstream);

            spwri.Write(Encoding.ASCII.GetBytes("XBB"), 0, 3);
            spwri.Write(BitConverter.GetBytes(1), 0, 1);
            spwri.Write(BitConverter.GetBytes(_filecount));
            spwri.Write(new byte[24], 0, 24);

            var startfileoffset = 32 + ((16 * _filecount) + (8 * _filecount))
                                  + filenamelenght;
            var startfilenameoffset = 32 + ((16 * _filecount) + (8 * _filecount));

            for (var i = 0; i < _filecount; i++)
            {
                if (i > 0)
                {
                    startfileoffset += (int)(new FileInfo(inputPath[i-1])).Length;
                    startfilenameoffset += Path.GetFileName(inputPath[i]).Length + 1;

                    spwri.Write(BitConverter.GetBytes(startfileoffset), 0, 4);
                    spwri.Write(BitConverter.GetBytes((int)(new FileInfo(inputPath[i])).Length), 0, 4);
                    spwri.Write(BitConverter.GetBytes(startfilenameoffset), 0, 4);
                    spwri.Write(tempuq[i].uniqueId, 0, 4);
                }
                else
                {
                    spwri.Write(BitConverter.GetBytes(startfileoffset), 0, 4);
                    spwri.Write(BitConverter.GetBytes((int)(new FileInfo(inputPath[i])).Length), 0, 4);
                    spwri.Write(BitConverter.GetBytes(startfilenameoffset), 0, 4);
                    spwri.Write(tempuq[i].uniqueId, 0, 4);
                }
            }

            tempuq = uniqueValue.OrderBy(x => x.uniqueId).ToArray();

            for (var i = 0; i < _filecount; i++)
            {
                spwri.Write(tempuq[i].uniqueId, 0, 4);
                spwri.Write(BitConverter.GetBytes(tempuq[i].id), 0, 4);
            }

            for (var i = 0; i < _filecount; i++)
            {
                spwri.Write(Path.GetFileName(inputPath[i]));
                spwri.Write(new byte[1], 0, 1);
            }

            spwri.Write(new byte[recount], 0, recount);

            for (var i = 0; i < _filecount; i++)
            {
                spwri.Write(File.ReadAllBytes(inputPath[i]));
            }
        }

        public static void unPack(string openPath, string outfolder)
        {
            FileStream strread = new FileStream(openPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader binread = new BinaryReader(strread);

            binread.BaseStream.Position = 4;

            uint _filecount = binread.ReadUInt32();

            XBBFAT[] _FAT = new XBBFAT[_filecount];
            XBBDAT[] _DAT = new XBBDAT[_filecount];
            XBBUQ[] _XBBUQ = new XBBUQ[_filecount];

            binread.BaseStream.Position = 0x20;

            for (var i = 0; i < _filecount; i++)
            {
                _FAT[i].Offset = binread.ReadUInt32();
                _FAT[i].Lenght = binread.ReadUInt32();
                _FAT[i].OffsetName = binread.ReadUInt32();
                _FAT[i].uniqueId = binread.ReadBytes(4);
            }

            for (var i = 0; i < _filecount; i++)
            {
                _XBBUQ[i].uniqueId = binread.ReadBytes(4);
                _XBBUQ[i].id = binread.ReadUInt32();
            }

            for (var i = 0; i < _filecount; i++)
            {
                if (i < _filecount - 1)
                {
                    binread.BaseStream.Position = _FAT[i].OffsetName;
                    _DAT[i].filename =
                        Encoding.ASCII.GetString(
                            binread.ReadBytes((int)_FAT[i + 1].OffsetName - (int)_FAT[i].OffsetName));
                    binread.BaseStream.Position = _FAT[i].Offset;
                    _DAT[i].data = binread.ReadBytes((int)_FAT[i].Lenght);
                }
                else
                {
                    binread.BaseStream.Position = _FAT[i].OffsetName;
                    _DAT[i].filename =
                        Encoding.ASCII.GetString(binread.ReadBytes((int)_FAT[0].Offset - (int)_FAT[i].OffsetName));
                    binread.BaseStream.Position = _FAT[i].Offset;

                    File.WriteAllBytes(outfolder + @"\" + _DAT[i].filename, binread.ReadBytes((int)_FAT[i].Lenght));
               }
            }

            File.WriteAllText(outfolder + @"\" + "uniqueIDList" + ".json", JsonConvert.SerializeObject(_XBBUQ, Formatting.Indented), Encoding.UTF8);
        }

        #region set/get data

        public byte[] Data
        {
            get { return _mstream.ToArray(); }
            set
            {
                _mstream = new MemoryStream(value);
                LoadXBB(); // make sure it will reload all entry data based input data.
            }
        }

        public string getFileName(int index)
        {
            return _DAT[index].filename;
        }

        public uint getCount()
        {
            return _filecount;
        }

        public byte[] getSelectedfile(int index)
        {
            return _DAT[index].data;
        }

        public void setSelectedfile(int index, byte[] input)
        {
            _DAT[index].data = input;
        }

        #endregion
    }


    public struct XBBFAT
    {
        public byte[] file;
        public uint Lenght;
        public uint Offset;
        public uint OffsetName;
        public byte[] uniqueId;
    }

    public struct XBBDAT
    {
        public byte[] data;
        public string filename;
    }

    public struct XBBUQ
    {
        public uint id;
        public byte[] uniqueId;
    }
}