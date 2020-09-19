using System;
using System.IO;
using System.Text;

namespace BokujoMessage4.Format
{
    public class msg4u
    {
        #region Variable
        private string _GCType = "Msg"; // Config identifier
        private GameSeries _GSeries; // Series bokujo monogatari game

        private byte[] MsgData; // Data overall will store in here

        public bool isHaveMessage; // for type 2 on us version

        private bool isJapaneseMessage; // for identified is Japanese Version
        private bool isEroupeMessage; // for identified is Eroupe Version
        private bool isUSMessage;

        private MemoryStream _mstream = null;
        private BinaryReader _binstream = null;

        public MsgConfigHeader _CHeader;
        #endregion

        public msg4u()
        {
            // it will do nothing
        }

        public msg4u(byte[] input, GameSeries series)
        {
            _GSeries = series;

            MsgData = input; // place input data to byte array data container

            CheckingVersioning();
            LoadMessage();
        }

        private void CheckingVersioning()
        {
            isEroupeMessage = false;
            isJapaneseMessage = false;
            isUSMessage = false;

            switch (_GSeries)
            {
                case GameSeries.SOSEU:
                    isEroupeMessage = true;
                    break;
                case GameSeries.SOSUS:
                    isUSMessage = true;
                    break;
                default: // Since ANB EU/JP/US and SOS JP used same format so thow all of them on this ...
                    isJapaneseMessage = true;
                    break;
            }
        }

        private void LoadMessage()
        {
            _mstream = new MemoryStream(MsgData);
            _binstream = new BinaryReader(_mstream);

            _CHeader = new MsgConfigHeader();
            _binstream.BaseStream.Position = 0;

            _CHeader.FileLenght = _binstream.ReadUInt32();
            _CHeader.FileCount = _binstream.ReadUInt32();

            if (_CHeader.FileCount != 0)
            {
                if (isUSMessage && _CHeader.FileCount == 2)
                {
                    isHaveMessage = false;
                }
                else
                {
                    isHaveMessage = true;
                }

                _CHeader.FileOffset = new uint[_CHeader.FileCount];

                for (int i = 0; i < _CHeader.FileCount; i++)
                {
                    _CHeader.FileOffset[i] = _binstream.ReadUInt32();
                }

                _binstream.BaseStream.Position = (int)_CHeader.FileOffset[0];
            }

            _mstream.Flush();
            _mstream.Close();
            _binstream.Close();
        }

        public string GetTextSection()
        {
            _mstream = new MemoryStream(MsgData);
            _binstream = new BinaryReader(_mstream);
            string tempinp = "";
            int MsgLenght = 0;

            if (isHaveMessage == false || _CHeader.FileCount == 0)
            {
                return ""; //No needed to check since don't have text section
            }

            try
            {
                if (isUSMessage)
                {
                    MsgLenght = (int)(MsgData.Length - _CHeader.FileOffset[2]);
                    _binstream.BaseStream.Position = _CHeader.FileOffset[2];
                    tempinp = Encoding.Unicode.GetString(_binstream.ReadBytes(MsgLenght)).Replace("\0", "");
                }

                if (isJapaneseMessage)
                {
                    MsgLenght = (int)(_CHeader.FileOffset[2] - _CHeader.FileOffset[1]);
                    _binstream.BaseStream.Position = _CHeader.FileOffset[1];
                    tempinp = Encoding.Unicode.GetString(_binstream.ReadBytes(MsgLenght)).Replace("\0", "");
                }

                if (isEroupeMessage)
                {
                    MsgLenght = (int)(_CHeader.FileOffset[3] - _CHeader.FileOffset[2]);
                    _binstream.BaseStream.Position = _CHeader.FileOffset[2];
                    tempinp = Encoding.Unicode.GetString(_binstream.ReadBytes(MsgLenght)).Replace("\0", "");
                }
            }
            catch
            {
                Console.WriteLine("Wrong Region Selection ...");
            }

            _mstream.Flush();
            _mstream.Close();
            _binstream.Close();
            return tempinp;
        }

        public string GetVariableSection()
        {
            _mstream = new MemoryStream(MsgData);
            _binstream = new BinaryReader(_mstream);
            string tempinp = "";
            int MsgVarLenght = 0;

            if (_CHeader.FileCount == 0)
            {
                return "";
            }

            try
            {
                if (isHaveMessage == false)
                {
                    MsgVarLenght = (int) (MsgData.Length - _CHeader.FileOffset[1]);
                    _binstream.BaseStream.Position = _CHeader.FileOffset[1];
                    return Encoding.GetEncoding(932).GetString(_binstream.ReadBytes(MsgVarLenght)).Replace("\0", "");
                }

                if (isUSMessage)
                {
                    MsgVarLenght = (int) (_CHeader.FileOffset[2] - _CHeader.FileOffset[1]);
                    _binstream.BaseStream.Position = _CHeader.FileOffset[1];
                    tempinp = Encoding.GetEncoding(932).GetString(_binstream.ReadBytes(MsgVarLenght)).Replace("\0", "");
                }

                if (isJapaneseMessage)
                {
                    MsgVarLenght = (int) (MsgData.Length - _CHeader.FileOffset[2]);

                    _binstream.BaseStream.Position = _CHeader.FileOffset[2];
                    tempinp = Encoding.GetEncoding(932).GetString(_binstream.ReadBytes(MsgVarLenght)).Replace("\0", "");
                }

                if (isEroupeMessage)
                {
                    MsgVarLenght = (int) (_CHeader.FileOffset[2] - _CHeader.FileOffset[1]);
                    _binstream.BaseStream.Position = _CHeader.FileOffset[1];
                    tempinp = Encoding.GetEncoding(932).GetString(_binstream.ReadBytes(MsgVarLenght)).Replace("\0", "");
                }
            }
            catch
            {
                Console.WriteLine("Wrong Region Selection ...");
            }

            _mstream.Flush();
            _mstream.Close();
            _binstream.Close();
            return tempinp;
        }

        public byte[] GetEUSection()
        {
            try
            {
                if (MsgData != null)
                {
                    _mstream = new MemoryStream(MsgData);
                    _binstream = new BinaryReader(_mstream);

                    int DummyLenght = 0;

                    if (isEroupeMessage)
                    {
                        DummyLenght = (int)(_mstream.Length - _CHeader.FileOffset[3]);

                        return _binstream.ReadBytes(DummyLenght);
                    }
                }

                if (isEroupeMessage == false)
                {
                    return new byte[0];
                }

                return new byte[] { 0x26, 0x20, 0x0, 0x0 };
            }
            catch
            {
                Console.WriteLine("Wrong Region Selection ...");
                return new byte[0];
            }
        }

        public void Create(GameSeries series, string varinput, string textinput, bool hasmessage = true)
        {
            _GSeries = series;
            CheckingVersioning();

            byte[] variablestring = Encoding.GetEncoding(932).GetBytes((string)varinput);
            byte[] messagestring = Encoding.Unicode.GetBytes((string)textinput);

            uint ExtraSectionEU = 0;
            
            uint varcountleft = (uint)Utils.RecountLenght(variablestring.Length); // calculate lenght of variable string with padding 2^2
            uint msgcountleft = (uint)Utils.RecountLenght(messagestring.Length); // calculate lenght of text string with padding 2^2

            int configlenght = Encoding.ASCII.GetBytes(_GCType).Length;
            uint typecountleft = (uint)Utils.RecountLenght(configlenght); // calculate lenght of GConfig

            uint SectionNumber = 0;

            if (isJapaneseMessage || isUSMessage)
            {
                SectionNumber = 3;
            }

            if (isUSMessage && hasmessage == false)
            {
                SectionNumber = 2;
            }

            if (isEroupeMessage)
            {
                SectionNumber = 4;
                ExtraSectionEU = 4;
            }

            uint lenght = 
                4 // File Lenght (4 byte)
                + 4 // File Count (4 byte)
                + (4* SectionNumber) // FAT section lenght (4 byte per each)
                + (uint) configlenght 
                + (uint) variablestring.Length
                + (uint) messagestring.Length
                + varcountleft
                + msgcountleft
                + typecountleft
                + ExtraSectionEU;

            byte[] binnew = new byte[lenght];

            _mstream = new MemoryStream(binnew);
            BinaryWriter spwri = new BinaryWriter(_mstream);

            if (isUSMessage)
            {
                if (hasmessage)
                {
                    spwri.Write(BitConverter.GetBytes(lenght), 0, 4);
                    spwri.Write(BitConverter.GetBytes(3), 0, 4);
                    spwri.Write(BitConverter.GetBytes(20), 0, 4);
                }
                else
                {
                    spwri.Write(BitConverter.GetBytes(lenght), 0, 4);
                    spwri.Write(BitConverter.GetBytes(3), 0, 4);
                    spwri.Write(BitConverter.GetBytes(14), 0, 4);
                }
            }

            if (isJapaneseMessage)
            {
                spwri.Write(BitConverter.GetBytes(lenght), 0, 4);
                spwri.Write(BitConverter.GetBytes(3), 0, 4);
                spwri.Write(BitConverter.GetBytes(20), 0, 4);
            }

            if (isEroupeMessage)
            {
                spwri.Write(BitConverter.GetBytes(lenght), 0, 4);
                spwri.Write(BitConverter.GetBytes(4), 0, 4);
                spwri.Write(BitConverter.GetBytes(24), 0, 4);
            }

            uint secondleft = 20 + (uint)configlenght + typecountleft;

            if (isUSMessage)
            {
                if (hasmessage)
                {
                    spwri.Write(BitConverter.GetBytes(secondleft), 0, 4);
                }
                else
                {
                    spwri.Write(BitConverter.GetBytes(secondleft-4), 0, 4);
                }
            }

            if (isJapaneseMessage)
            {
                spwri.Write(BitConverter.GetBytes(secondleft), 0, 4);
            }

            if (isEroupeMessage)
            {
                spwri.Write(BitConverter.GetBytes(secondleft+4), 0, 4);
            }

            byte[] tp = null;

            if (isUSMessage)
            {
                if (hasmessage)
                {
                    spwri.Write(BitConverter.GetBytes(secondleft + variablestring.Length + varcountleft), 0, 4);
                    tp = Encoding.ASCII.GetBytes(_GCType);
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
                    tp = variablestring;
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
                    tp = messagestring;
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
                }
                else
                {
                    spwri.Write(BitConverter.GetBytes(secondleft + variablestring.Length + varcountleft), 0, 4);
                    tp = Encoding.ASCII.GetBytes(_GCType);
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
                    tp = variablestring;
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
                }
            }

            if (isJapaneseMessage)
            {
                spwri.Write(BitConverter.GetBytes(secondleft + messagestring.Length + msgcountleft), 0, 4);
                tp = Encoding.ASCII.GetBytes(_GCType);
                spwri.Write(tp, 0, tp.Length);
                spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
                tp = messagestring;
                spwri.Write(tp, 0, tp.Length);
                spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
                tp = variablestring;
                spwri.Write(tp, 0, tp.Length);
                spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
            }

            if (isEroupeMessage)
            {
                spwri.Write(BitConverter.GetBytes(secondleft + variablestring.Length + varcountleft + 4), 0, 4);
                spwri.Write(BitConverter.GetBytes(secondleft + variablestring.Length + varcountleft + 4 + messagestring.Length + msgcountleft), 0, 4);
                tp = Encoding.ASCII.GetBytes(_GCType);
                spwri.Write(tp, 0, tp.Length);
                spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
                tp = variablestring;
                spwri.Write(tp, 0, tp.Length);
                spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
                tp = messagestring;
                spwri.Write(tp, 0, tp.Length);
                spwri.Write(new byte[Utils.RecountLenght(tp.Length)], 0, Utils.RecountLenght(tp.Length));
                spwri.Write(BitConverter.GetBytes((int)lenght));
            }

            MsgData = _mstream.ToArray();
            _mstream.Close();
            spwri.Close();
            LoadMessage();
        }

        public byte[] GetMSGData()
        {
            return MsgData;
        }

        public long getMSGLenght()
        {
            return MsgData.Length;
        }
    }
}
