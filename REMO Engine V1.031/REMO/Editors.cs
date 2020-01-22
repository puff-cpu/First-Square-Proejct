using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using REMO_Engine_V1._031;


namespace REMO_Engine_V1._031
{

    public static class HashTextEditor
    {
        private static string AccountPath = "TestMap.txt";
        private static string Salt = "RMNTESTGAME_MONODRAWING";

        private static byte[] SHA256_toByte(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string SHA256_toString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in SHA256_toByte(inputString))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        private static bool Validate()
        {
            string[] GFH = GetNormalFileHash();
            if (GFH[0] == GFH[1])
                return true;
            return false;
        }

        private static string Hash(string s)
        {
            return SHA256_toString(s);
        }

        private static string[] GetNormalFileHash()
        {
            List<string> sList = File.ReadAllLines(AccountPath).ToList();
            string seed = Salt;
            if (sList.Count < 1)
            {
                string[] ss = { "Invalid", "-1" };
                return ss;
            }
            if (sList.Count == 2)
            {
                sList[0] = Hash(sList[0] + seed);
            }
            else if (sList.Count > 2)
            {
                while (sList.Count > 2)
                {
                    seed = Hash(seed);
                    sList[0] = Hash(sList[0] + seed + sList[1]);// Salt가 첨가된 형태의 Merkle-Damgard
                    sList.RemoveAt(1);
                }
            }
            string[] s = { sList[0], sList[1] };
            return s;
        }

        private static string GetFileHash()
        {
            List<string> sList = File.ReadAllLines(AccountPath).ToList();
            string seed = Salt;
            if (sList.Count == 1)
            {
                sList[0] = Hash(sList[0] + seed);
            }
            else
            {
                while (sList.Count > 1)
                {
                    seed = Hash(seed);
                    sList[0] = Hash(sList[0] + seed + sList[1]);// Merkle-Damgard
                    sList.RemoveAt(1);
                }

            }
            return sList[0];
        }


        public static void ReadLine(string Header, Action<int> ReadAct, Action ErrorAct)
        {
            if (Validate())
            {
                List<string> sList = File.ReadAllLines(AccountPath).ToList();
                for (int i = 0; i < sList.Count - 1; i++)
                {
                    string[] s = sList[i].Split('/');
                    if (s[0] == Header)
                    {
                        ReadAct(Int32.Parse(s[1]));
                    }
                }
            }
            else
            {
                ErrorAct();
            }


        }
        public static void AddLine(string Line)
        {

            string s = Line + "/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            if (!File.Exists(AccountPath))
            {
                File.Create(AccountPath).Close();
            }
            if (Validate())
            {
                var lines = System.IO.File.ReadAllLines(AccountPath);
                System.IO.File.WriteAllLines(AccountPath, lines.Take(lines.Length - 1).ToArray());//마지막 줄 자르기
                using (StreamWriter outputFile = new StreamWriter(AccountPath, true))
                {
                    outputFile.WriteLine(s);//마지막 줄 추가.
                }
                string h = GetFileHash();
                using (StreamWriter outputFile = new StreamWriter(AccountPath, true))
                {
                    outputFile.WriteLine(h);//해시값 추가.
                }
            }
            else
            {
                File.Delete(AccountPath);
                File.Create(AccountPath).Close();
                using (StreamWriter outputFile = new StreamWriter(AccountPath, true))
                {
                    outputFile.WriteLine(s);//마지막 줄 추가.
                }
                string h = GetFileHash();
                using (StreamWriter outputFile = new StreamWriter(AccountPath, true))
                {
                    outputFile.WriteLine(h);//해시값 추가.
                }
            }
        }


    }

    public static class TxtEditor //특정 디렉토리 내부의 Txt파일을 관리합니다.
    {

        public static string MakePath(string DirName, string FileName) //특정 디렉토리 내부의 txt파일의 경로를 불러옵니다.
        {
            return DirName + "\\" + FileName + ".txt";
        }



        public static void MakeTextFile(string DirName, string FileName) //특정 디렉토리에 특정 파일을 생성합니다.
        {
            DirectoryInfo di = new DirectoryInfo(DirName);
            if (di.Exists == false)
            {
                di.Create();
            }
            if (!File.Exists(MakePath(DirName, FileName)))
            {
                File.Create(MakePath(DirName, FileName)).Close();
            }
        }



        public static void DeleteFile(string DirName, string FileName)
        {
            FileInfo fileDel = new FileInfo(MakePath(DirName, FileName));
            if (fileDel.Exists)
            {
                fileDel.Delete();
            }
        }


        public static string[] ReadAllLines(string DirName, string FileName)
        {
            return File.ReadAllLines(MakePath(DirName, FileName));
        }


        public static void WriteAllLines(string DirName, string FileName, string[] contents)
        {
            File.WriteAllLines(MakePath(DirName, FileName), contents);
        }

        public static void AppendLines(string DirName, string FileName, string[] contents)
        {
            using (StreamWriter sw = File.AppendText(MakePath(DirName, FileName)))
            {
                for (int i = 0; i < contents.Length; i++)
                {
                    sw.WriteLine(contents[i]);
                    sw.WriteLine(contents[i], 0);
                }
            }
        }

        public static void AppendLinesToTop(string DirName, string FileName, string[] contents)//텍스트파일의 맨 위쪽에 새 라인들을 붙입니다.
        {
            string str;
            using (StreamReader sreader = new StreamReader(MakePath(DirName, FileName)))
            {
                str = sreader.ReadToEnd();
            }

            File.Delete(MakePath(DirName, FileName));

            using (StreamWriter swriter = new StreamWriter(MakePath(DirName, FileName), false))
            {
                for (int i = 0; i < contents.Length; i++)
                {
                    swriter.WriteLine(contents[i]);
                }
                swriter.WriteLine(str);
            }

        }

        public static void Clear(string DirName, string FileName) // 텍스트 파일의 내용을 전부 지웁니다.
        {
            File.WriteAllText(MakePath(DirName, FileName), String.Empty);
        }

        public static List<string> GetTxtListFrom(string DirName)//특정 디렉토리에 있는 텍스트 파일명들을 전부 불러옵니다.
        {
            List<string> strings = new List<string>();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(DirName);
            foreach (System.IO.FileInfo File in di.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".out") == 0)
                {
                    strings.Add(File.Name.Substring(0, File.Name.Length - 4));
                }
            }
            return strings;
        }


    }



    public class TLReader
    {
        // 매우 간단한 TL(Tag Language)을 읽어들이는 리더기입니다.
        // 문법은 <Tag>Statement로 작동합니다. Tag는 Statement에 대한 Action을 대응하기 위한 키워드입니다. 액션은 직접 지정하여 사용할 수 있습니다.
        // 예시 1 : <Tag1>State1<Tag2>State2<Tag3>State3
        // 예시 2 : <CharaName> Gin <Talk> Why don't you leave the place?
        // Tag는 대소문자를 구분하지 않습니다. 예를 들어 Tag로 Rabbit을 지정할 경우 RABbit 등도 똑같은 태그로 읽습니다. 또한 공백을 무시합니다.

        // Statement에 대한 간단한 형태의 Parse 도구를 지원합니다. State1%%State2%%State3의 형태로 읽을 경우, 각각 Parse(Statement)[0], [1],[2]로 읽는 것이 가능합니다.
        // 또한, ReadPoint와 ReadRect를 통해 간단하게 Point와 Rectangle을 읽어들일 수 있습니다. 각 함수는 공백을 무시합니다.

        private Dictionary<string, Action<string>> Rules = new Dictionary<string, Action<string>>();

        public string Embracer = "<>";//태그언어의 태그를 구분하는 Embracer입니다. <>, {},(),[]등 다양한 Embracer를 채택하는 것이 가능합니다.

        public TLReader()
        {
        }


        public TLReader(Dictionary<string, Action<string>> rules)
        {
            foreach (string rule in rules.Keys.ToList())
            {
                AddRule(rule, rules[rule]);
            }
        }

        public void ReadLine(string CodeLine)//특정 스트링을 읽어 명령줄을 실행합니다.
        {
            string[] Statements = CodeLine.Split(Embracer[0]);
            for (int i = 1; i < Statements.Length; i++)
            {
                ReadStatement(Statements[i]);
            }
        }

        public void ReadTxt(string DirName, string FileName) // 특정 텍스트파일 전체의 내용을 읽어 명령을 실행합니다.
        {
            string[] s = TxtEditor.ReadAllLines(DirName, FileName);
            for (int i = 0; i < s.Length; i++)
                ReadLine(s[i]);
        }

        public void AddRule(string Tag, Action<string> StringAction) // Tag가 달린 Statement에 대해 StringAction(Statement)를 실행합니다. 문법은 <Tag>Statement 
        {
            Rules.Add(Tag.Replace(" ", ""), StringAction);
        }

        public void Clear()
        {
            Rules.Clear();
        }

        private void ReadStatement(string Statement)
        {
            string[] ParsedStatement = Statement.Split(Embracer[1]);
            foreach (string Tag in Rules.Keys.ToList())
            {
                if (String.Compare(ParsedStatement[0].Replace(" ", ""), Tag, true) == 0)
                {
                    Rules[Tag](ParsedStatement[1]);
                }
            }
        }

        public static Point ReadPoint(string s) // 30,30 등의 Point 인수를 읽습니다.
        {
            string[] ps = s.Replace(" ", "").Split(',');
            return new Point(Int32.Parse(ps[0]), Int32.Parse(ps[1]));
        }

        public static Rectangle ReadRect(string s)// 0,0,50,50 등의 Rectangle 인수를 읽습니다.
        {
            string[] ps = s.Replace(" ", "").Split(',');
            return new Rectangle(Int32.Parse(ps[0]), Int32.Parse(ps[1]), Int32.Parse(ps[2]), Int32.Parse(ps[3]));
        }

        public static string[] Parse(string s) => Parse(s, "%%");


        public static string[] Parse(string s, string Parser)
        {
            return s.Split(new string[] { Parser }, StringSplitOptions.None);
        }




    }

    // 유저의 타이핑을 받아서 저장하는 클래스입니다. 
    // 기본적으론 알파벳과 숫자 입력을 받습니다. 그 외의 특수문자는 등록을 해서 사용하십시오. 가령, Space를 통한 공백 입력은 다음과 같이 지정합니다.
    // typeWriter.AddCustomType(Keys.Space, " ");
    // 이와 같이 입력 세팅이 다양한 이유는 파일명 입력 등을 처리하기 위해서입니다. 
    // 혹은, Code Injection 방어를 위해 오퍼레이터로 작동하는 특정 문자열의 입력을 막을 필요가 있습니다.
    // 생성자에서 Enter 입력에 의한 액션을 세팅할 수 있습니다.

    public class TypeWriter
    {
        private string typeLine = "";
        public string TypeLine
        {
            get { return typeLine; }
        }

        private int BackSpaceTimer = 0;
        private Action EnterAction;
        private Dictionary<Keys, string> CustomKeys = new Dictionary<Keys, string>();

        public TypeWriter() { }
        public TypeWriter(Action enterAction)
        {
            EnterAction = enterAction;
        }

        public void AddCustomType(Keys k, string TypeString)
        {
            CustomKeys.Add(k, TypeString);
        }

        public void AddWritingKeySet() // 공백, 스페이스, 콤마, 마침표, 따옴표를 인식할 수 있게 됩니다.
        {
            AddCustomType(Keys.OemMinus, "-");
            AddCustomType(Keys.Space, " ");
            AddCustomType(Keys.OemComma, ",");
            AddCustomType(Keys.OemPeriod, ".");
            AddCustomType(Keys.OemQuotes, "'");
        }

        public void Empty() => typeLine = "";

        public override string ToString()
        {
            return TypeLine;
        }

        public void Update()
        {
            Keys[] ks = Keyboard.GetState().GetPressedKeys();

            if (User.Pressing(Keys.Back))
            {
                BackSpaceTimer++;
            }
            else
            {
                BackSpaceTimer = 0;
            }
            foreach (Keys k in ks)
            {
                if (User.JustPressed(k))
                {
                    if (k.ToString().Length == 1)
                    {
                        if (k.ToString()[0] <= 'Z' && k.ToString()[0] >= 'A')
                        {
                            if (User.Pressing(Keys.LeftShift) || User.Pressing(Keys.RightShift))
                                typeLine += k.ToString().ToUpper();
                            else
                                typeLine += k.ToString().ToLower();
                        }
                    }
                    if (k.ToString().Length == 2 && k.ToString()[0] == 'D')
                    {
                        typeLine += k.ToString()[1];
                    }
                    if (k == Keys.Enter)
                    {
                        EnterAction?.Invoke();
                    }
                    foreach (Keys ck in CustomKeys.Keys.ToList())
                    {
                        if (k == ck)
                        {
                            typeLine += CustomKeys[ck];
                        }
                    }
                }

                if ((k == Keys.Back && BackSpaceTimer >= 15) || User.JustPressed(Keys.Back))
                {
                    if (typeLine.Length >= 1)
                    {
                        typeLine = typeLine.Substring(0, typeLine.Length - 1);
                    }
                }
            }
        }
    }



    //Function 성능을 측정하는 단위테스터입니다.
    //키보드 키들에 테스트하고 싶은 함수들을 할당할 수 있습니다.
    //Invoke를 통해 테스트하려는 함수를 호출합니다. 
    //1,2,3 키입력을 통해 루프횟수를 늘릴 수 있습니다. 4를 통해 Loop를 0으로 만듭니다.
    public class UnitTester
    {
        public int FuncCallCount = 100;
        private Action UnitFunc;
        private event Action UpdateEvent = null;


        public void RegisterActionKey(Keys k, Action kAction)//Key k에 테스트하고 싶은 함수 f를 할당합니다.
        {
            UpdateEvent +=
                () =>
                {
                    if (User.JustPressed(k))
                        UnitFunc = kAction;
                };
        }

        public void Update()
        {
            User.KeyJAct(Keys.D1, () =>
            {
                FuncCallCount += 10;
            });
            User.KeyJAct(Keys.D2, () =>
            {
                FuncCallCount += 100;
            });
            User.KeyJAct(Keys.D3, () =>
            {
                FuncCallCount += 1000;
            });
            User.KeyJAct(Keys.D4, () =>
            {
                FuncCallCount = 0;
            });


            UpdateEvent();
        }
        public void Automation(int Interval, int TargetMilsec)//특정 Milsec per frame(mpf)에 도달하는데 필요한 함수 호출량을 계산합니다.
        {
            if (StandAlone.ElapsedMillisec < TargetMilsec)
                FuncCallCount += Interval;
            if (StandAlone.ElapsedMillisec > TargetMilsec)
                FuncCallCount -= Interval;
        }

        public void Draw(Color c)
        {
            StandAlone.DrawString(FuncCallCount + " Loop,Time = " + StandAlone.ElapsedMillisec + "ms (per 1frame)", new Point(0, 0), c);
        }

        public void Invoke()
        {
            for (int i = 0; i < FuncCallCount; i++)
                UnitFunc?.Invoke();
        }


    }


    public static class REMOBluePrinter // 레모 내장 블루프린터입니다. UI 세팅을 할때 활용할 수 있습니다.
    {
        private static string CurrentLog = "";
        private static int SavedTimer = 0;
        private static Rectangle Bound = Rectangle.Empty;
        enum BluePrintMode
        {
            NULL,
            ShowCursorPos,
            ShowRectangle
        }
        private static BluePrintMode DebugMode = BluePrintMode.NULL;

        private static void BluePrintKeyAct(Keys k, Action a)
        {
            if (User.JustPressed(Keys.LeftControl, k))
                a();
        }

        private static void BluePrintModeAct(Keys k, BluePrintMode m)
        {
            BluePrintKeyAct(k, () =>
            {
                if (DebugMode == m)
                    DebugMode = BluePrintMode.NULL;
                else
                    DebugMode = m;
            });// 특정 키에 대해 디버그모드 m을 할당합니다.

        }

        private static void SaveLog() //로그를 저장합니다.
        {
            if (DebugMode != BluePrintMode.NULL)
                TxtEditor.AppendLinesToTop("Logs", "DebugLog", new string[] { LogNameWriter.TypeLine + "=" + CurrentLog });
            else
                TxtEditor.AppendLinesToTop("Logs", "DebugLog", new string[] { LogNameWriter.TypeLine });

            SavedTimer = 30;
        }

        public static Scene scn = new Scene(() =>
        {
            scn.InitOnce(() =>
            {
                TxtEditor.MakeTextFile("Logs", "DebugLog"); // 디버깅 기록을 로그할 텍스트파일을 만듭니다.
                LogNameWriter.AddCustomType(Keys.Space, " ");
            });

        }, () =>
        {
            if (SavedTimer > 0)
                SavedTimer--;
            switch (DebugMode)
            {
                case BluePrintMode.NULL:
                    CurrentLog = "";
                    break;
                case BluePrintMode.ShowCursorPos:
                    CurrentLog = "new Point(" + Cursor.Pos.X + "," + Cursor.Pos.Y + ")";
                    break;
                case BluePrintMode.ShowRectangle:
                    if (User.JustLeftClicked())
                        Bound = new Rectangle(Cursor.Pos, Point.Zero);
                    if (User.Pressing(MouseButtons.LeftMouseButton))
                    {
                        Bound = new Rectangle(Bound.Location, Cursor.Pos - Bound.Location);
                    }
                    CurrentLog = "new Rectangle(" + Bound.X + "," + Bound.Y + "," + Bound.Width + "," + Bound.Height + ")";

                    break;
            }
            BluePrintKeyAct(Keys.S, () =>
            {
                SaveLog();
                System.Diagnostics.Process.Start(TxtEditor.MakePath("Logs", "DebugLog"));
            });// Ctrl+S : 현재 로그를 저장하고 블루프린트 로그를 불러옵니다.
            BluePrintModeAct(Keys.Q, BluePrintMode.ShowCursorPos);// Ctrl+Q : 커서 위치를 보여주는 모드를 불러옵니다.
            BluePrintModeAct(Keys.W, BluePrintMode.ShowRectangle);// Ctrl+W : 사각형을 보여주는 모드를 불러옵니다.


            //로그에 주석을 달아주는 라이터 항목에 관한 업데이트입니다.
            if (WriterIsOn && !User.Pressing(Keys.LeftControl))
                LogNameWriter.Update();
            if (User.JustPressed(Keys.Enter))
            {
                if (WriterIsOn == true)
                    LogNameWriter.Empty();
                WriterIsOn = !WriterIsOn;

            }

        }, () =>
        {
            if (DebugMode == BluePrintMode.ShowRectangle)
            {
                Filter.Absolute(Bound, Color.Red * 0.5f);//사각형 영역을 표시합니다.
            }

            if (DebugMode != BluePrintMode.NULL) //NULL 모드가 아닐시 현재 로그를 불러옵니다.
            {
                StandAlone.DrawString(CurrentLog, Cursor.Pos + new Point(30, 0), Color.White, Color.Black);
            }
            if (SavedTimer > 0)
                StandAlone.DrawString("Saved", StandAlone.FullScreen.Center - new Point(30, 0), Color.White, Color.Black);

            if (WriterIsOn)
            {
                StandAlone.DrawString("> " + LogNameWriter.TypeLine, Cursor.Pos + new Point(30, 20), Color.SkyBlue, Color.Black);
            }

            StandAlone.DrawString("DEBUG", new Point(0, 0), Color.White, Color.Black);
        });
        private static bool WriterIsOn = false;
        private static TypeWriter LogNameWriter = new TypeWriter(); //로깅할 때 로그에 주석을 달 수 있습니다.

        public static void Enable() //이녀석을 업데이트 함수에 넣을 경우, Ctrl+Alt+Q를 통해 디버거를 로드할 수 있는 상태가 됩니다.
        {
            if (User.JustPressed(Keys.LeftControl, Keys.LeftAlt, Keys.Q))
            {
                if (!Projectors.SubProjector.Loaded(REMOBluePrinter.scn))
                {
                    Projectors.Projector.PauseAll();
                    Projectors.SubProjector.Load(REMOBluePrinter.scn);
                }
                else
                {
                    Projectors.Projector.ResumeAll();
                    Projectors.SubProjector.Unload(REMOBluePrinter.scn);
                }
            }
        }
    }



    public class Scripter
    {
        public SimpleAligned<SimpleAligned<GfxStr>> Script = new SimpleAligned<SimpleAligned<GfxStr>>(AlignMode.Vertical, Point.Zero, 0);
        public Point Pos
        {
            get
            {
                return Script.Pos;
            }
            set
            {
                Script.Pos = value;
            }
        }
        public int LetterSpacing = 0;
        public int LineSpacing
        {
            get { return Script.interval; }
            set { Script.interval = value; }
        }

        public int LineWidth = 400;

        public Scripter(Point _Pos, int _LetterSpacing, int _LineSpacing, int _LineWidth)
        {
            Pos = _Pos;
            LetterSpacing = _LetterSpacing;
            LineSpacing = _LineSpacing;
            LineWidth = _LineWidth;
        }

        public void BuildScript(string s) // Script에 문장을 넣습니다.
        {
            Script.Clear();
            string[] strs = s.Split(' ');

            SimpleAligned<GfxStr> Strings = new SimpleAligned<GfxStr>(AlignMode.Horizen, Point.Zero, LetterSpacing);
            int i = 0;
            for (; i < strs.Length; i++)
            {
                Strings.Add(new GfxStr(strs[i]));
                Strings.Align();
                if (Strings.Bound.Width > LineWidth && Strings.Count > 1)
                {
                    Strings.RemoveAt(Strings.Count - 1);
                    i--;
                    Script.Add(Strings);//빌드된 스트링을 포함하고,
                    Strings = new SimpleAligned<GfxStr>(AlignMode.Horizen, Point.Zero, LetterSpacing); //새로운 스트링을 씁니다.
                }
            }
            Script.Add(Strings);//마지막으로 포함 안된 스트링을 포함합니다.
            Script.Align();
        }





    }

}
