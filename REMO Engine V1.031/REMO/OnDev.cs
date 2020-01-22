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

namespace REMO_Engine_V1._031
{
    //On Developing. 아직 개발중인 컴포넌트들은 이 클래스 내부에 포함됩니다.
    public static class OnDev
    {

    }
    public static class TestScene2_Disposed
    {

        public class CustomTLReader : TLReader
        {
            public CustomTLReader() : base()
            {
                this.Embracer = "[]";
                this.AddRule("Script", (s) => { StandAlone.DrawString(s, new Point(100, 200), Color.White); });
            }
        }



        public static CustomTLReader t = new CustomTLReader();


        public static Scene scn = new Scene(() =>
        {

        }, () =>
        {

        }, () =>
        {
            t.ReadLine("[Script] My life is so great");
            //Strings.Draw();
            Cursor.Draw(Color.White);
        });

    }


    public static class TestScene4_Disposed
    {
        public static Scripter s = new Scripter(new Point(100, 100), 10, 0, 20);

        public static Scene scn = new Scene(() =>
        {
            s.BuildScript("Yesterday, love is such an easy game to play, now I need a place to hide away. ");


        }, () =>
        {



        }, () =>
        {
            s.Script.Draw();
            Cursor.Draw(Color.White);
        });




    }

}
