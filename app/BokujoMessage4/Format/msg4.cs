using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BokujoMessage4.Format
{
    public class msg4
    {
        #region Variable
        private string _inType;
        private GameSeries _series;

        private string _inputVariableName;
        private string _inputMessageContent;
        public byte[] _inputdummybyteEU;
        private bool _noMsgData = false;

        public int TextLenght;

        private MemoryStream _mstream;
        private BinaryReader _binstream;

        public MsgConfigHeader ConfigHeader;
        #endregion

        public msg4() { }

        #region Get Return values
        public string VariableName()
        {
            return _inputVariableName;
        }

        public string MessageContent()
        {
            return _inputMessageContent;
        }

        /*
         *  Get / Set MessageData Raw ... 
         *
         */
        public byte[] MessageData
        {
            get
            {
                return _mstream.ToArray();
            }
            set
            {
                _mstream = new MemoryStream(value);
                LoadMessage(); // make sure it will reload all entry data based input data.
            }
        }

        public long MessageDataLenght()
        {
            return _mstream.Length;
        }
        #endregion

        #region LoadSection

        public msg4(byte[] input, GameSeries series)
        {
            _series = series;

            _mstream = new MemoryStream(input);
            LoadMessage();
        }

        public void setGameSeries(GameSeries series)
        {
            _series = series;
        }

        public void LoadMessage(byte[] input, GameSeries series)
        {
            _series = series;

            _mstream = new MemoryStream(input);
            LoadMessage();
        }

        public void LoadMessage(byte[] input)
        {
            // this function must set what gameseries being used.

            _mstream = new MemoryStream(input);
            LoadMessage();
        }

        public void CloseIt()
        {
            _binstream.Close();
            _mstream.Close();
        }

        private void LoadMessage()
        {
            _binstream = new BinaryReader(_mstream);
            ConfigHeader = new MsgConfigHeader();
            _binstream.BaseStream.Position = 0;

            ConfigHeader.FileLenght = _binstream.ReadUInt32();
            ConfigHeader.FileCount = _binstream.ReadUInt32();

            ConfigHeader.FileOffset = new uint[ConfigHeader.FileCount];

            for (int i = 0; i < ConfigHeader.FileCount; i++)
            {
                ConfigHeader.FileOffset[i] = _binstream.ReadUInt32();
            }

            _binstream.BaseStream.Position = (int)ConfigHeader.FileOffset[0];
            uint lenght = ConfigHeader.FileOffset[1] - ConfigHeader.FileOffset[0];

            _inType = Encoding.ASCII.GetString(_binstream.ReadBytes((int)lenght)).Replace("\0", "");

            LoadContent();
        }

        private void LoadContent()
        {
            int MsgVarLenght = 0;
            int MsgLenght = 0;
            int DummyLenght = 0;

            switch (_series)
            {
                case GameSeries.SOSUS:
                    if (ConfigHeader.FileCount == 3)
                    {
                        MsgVarLenght = (int)(ConfigHeader.FileOffset[2] - ConfigHeader.FileOffset[1]);
                        _binstream.BaseStream.Position = ConfigHeader.FileOffset[1];
                        _inputVariableName = Encoding.GetEncoding(932).GetString(_binstream.ReadBytes(MsgVarLenght)).Replace("\0", "");

                        MsgLenght = (int) (_mstream.Length - ConfigHeader.FileOffset[2]);
                        _binstream.BaseStream.Position = ConfigHeader.FileOffset[2];
                        _inputMessageContent = Encoding.Unicode.GetString(_binstream.ReadBytes(MsgLenght))
                            .Replace("\0", "");

                        _noMsgData = false;
                    }
                    else
                    {
                        MsgVarLenght = (int)(_mstream.Length - ConfigHeader.FileOffset[1]);
                        _binstream.BaseStream.Position = ConfigHeader.FileOffset[1];
                        _inputVariableName = Encoding.GetEncoding(932).GetString(_binstream.ReadBytes(MsgVarLenght)).Replace("\0", "");

                        _inputMessageContent = "";

                        _noMsgData = true;
                    }
                    break;
                case GameSeries.SOSEU:
                    MsgVarLenght = (int)(ConfigHeader.FileOffset[2] - ConfigHeader.FileOffset[1]);
                    _binstream.BaseStream.Position = ConfigHeader.FileOffset[1];
                    _inputVariableName = Encoding.GetEncoding(932).GetString(_binstream.ReadBytes(MsgVarLenght)).Replace("\0", "");

                    MsgLenght = (int)(ConfigHeader.FileOffset[3] - ConfigHeader.FileOffset[2]);
                    _binstream.BaseStream.Position = ConfigHeader.FileOffset[2];
                    _inputMessageContent = Encoding.Unicode.GetString(_binstream.ReadBytes(MsgLenght))
                        .Replace("\0", "");

                    DummyLenght = (int)(_mstream.Length - ConfigHeader.FileOffset[3]);
                    
                    _inputdummybyteEU = _binstream.ReadBytes(DummyLenght);
                    _noMsgData = false;
                    break;
                case GameSeries.SOSJP:
                case GameSeries.ANBUS:
                case GameSeries.ANBJP:
                    MsgLenght = (int)(ConfigHeader.FileOffset[2] - ConfigHeader.FileOffset[1]);
                    MsgVarLenght = (int)(_mstream.Length - ConfigHeader.FileOffset[2]);

                    _binstream.BaseStream.Position = ConfigHeader.FileOffset[2];
                    _inputVariableName = Encoding.GetEncoding(932).GetString(_binstream.ReadBytes(MsgVarLenght)).Replace("\0", "");
                    _binstream.BaseStream.Position = ConfigHeader.FileOffset[1];
                    _inputMessageContent = Encoding.Unicode.GetString(_binstream.ReadBytes(MsgLenght)).Replace("\0", "");

                    _noMsgData = false;
                    break;
                case GameSeries.SOSKOR:

                    _noMsgData = false;
                    break;
            }

            TextLenght = MsgLenght;
        }
        #endregion

        public void CreateMessage(GameSeries series, params object[] input)
        {
            _series = series;
            _inType = "Msg";

            _MsgConfigWrite(input);
        }

        private void _MsgConfigWrite(params object[] input)
        {
            byte[] variablestring = Encoding.GetEncoding(932).GetBytes((string)input[0]);
            byte[] messagestring = Encoding.Unicode.GetBytes((string)input[1]);

            _inputVariableName= (string)input[0];
            _inputMessageContent= (string)input[1];

            if (input.Count() >2)
            {
                _inputdummybyteEU = (byte[]) input[2];
            }
            else
            {
                _inputdummybyteEU = new byte[] { 0x26, 0x20, 0x0, 0x0 };
            }

            uint varcountleft = (uint) recountlenght(variablestring.Length);
            uint msgcountleft = (uint) recountlenght(messagestring.Length);
            uint typecountleft = (uint) recountlenght(Encoding.ASCII.GetBytes(_inType).Length);

            uint lenght = 4 + 4 + (12) + (uint)Encoding.ASCII.GetBytes(_inType).Length + (uint)variablestring.Length
                + (uint)messagestring.Length
                + varcountleft
                + msgcountleft
                + typecountleft;

            if (_series == GameSeries.SOSEU)
            {
                lenght += (uint) _inputdummybyteEU.Length+4;
            }
            
            byte[] binnew = new byte[lenght];

            _mstream = new MemoryStream(binnew);
            BinaryWriter spwri = new BinaryWriter(_mstream);

            if (_series == GameSeries.SOSEU)
            {
                spwri.Write(BitConverter.GetBytes(lenght), 0, 4);
                spwri.Write(BitConverter.GetBytes(4), 0, 4);
                spwri.Write(BitConverter.GetBytes(24), 0, 4);
            }
            else
            {
                spwri.Write(BitConverter.GetBytes(lenght), 0, 4);
                spwri.Write(BitConverter.GetBytes(3), 0, 4);
                spwri.Write(BitConverter.GetBytes(20), 0, 4);
            }

            uint secondleft = 20 + (uint) Encoding.ASCII.GetBytes(_inType).Length + typecountleft;

            if (_series == GameSeries.SOSEU)
            {
                spwri.Write(BitConverter.GetBytes(secondleft+4), 0, 4);
            }
            else
            {
                spwri.Write(BitConverter.GetBytes(secondleft), 0, 4);
            }


            byte[] tp=null;
            switch (_series)
            {
                case GameSeries.SOSUS:
                    spwri.Write(BitConverter.GetBytes(secondleft + variablestring.Length+ varcountleft), 0, 4);
                    tp = Encoding.ASCII.GetBytes(_inType);
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[recountlenght(tp.Length)], 0, recountlenght(tp.Length));
                    tp = variablestring;
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[recountlenght(tp.Length)], 0, recountlenght(tp.Length));
                    tp = messagestring;
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[recountlenght(tp.Length)], 0, recountlenght(tp.Length));
                    break;
                case GameSeries.SOSEU:
                    spwri.Write(BitConverter.GetBytes(secondleft + variablestring.Length + varcountleft + 4), 0, 4);
                    spwri.Write(BitConverter.GetBytes(secondleft + variablestring.Length + varcountleft + 4 + messagestring.Length + msgcountleft), 0, 4);
                    tp = Encoding.ASCII.GetBytes(_inType);
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[recountlenght(tp.Length)], 0, recountlenght(tp.Length));
                    tp = variablestring;
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[recountlenght(tp.Length)], 0, recountlenght(tp.Length));
                    tp = messagestring;
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[recountlenght(tp.Length)], 0, recountlenght(tp.Length));
                    spwri.Write(_inputdummybyteEU);
                    break;
                case GameSeries.SOSJP:
                case GameSeries.ANBUS:
                case GameSeries.ANBJP:
                    spwri.Write(BitConverter.GetBytes(secondleft + messagestring.Length+ msgcountleft), 0, 4);
                    tp = Encoding.ASCII.GetBytes(_inType);
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[recountlenght(tp.Length)], 0, recountlenght(tp.Length));
                    tp = messagestring;
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[recountlenght(tp.Length)], 0, recountlenght(tp.Length));
                    tp = variablestring;
                    spwri.Write(tp, 0, tp.Length);
                    spwri.Write(new byte[recountlenght(tp.Length)], 0, recountlenght(tp.Length));
                    break;
            }
        }

        private int recountlenght(int lenght)
        {
            int inp0 = lenght % 4;
            return 4 - inp0;
        }
    }

    public struct MsgConfigHeader
    {
        public uint FileLenght;
        public uint FileCount;
        public uint[] FileOffset;
    }
}
