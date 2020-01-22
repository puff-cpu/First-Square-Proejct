﻿using System;
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
{   //각종 StandAlone계열 클래스들을 담당. 게임 씬과 무관한 기본 기능들을 수행하는 컴포넌트들을 처리합니다.
    public static class StandAlone
    {


        public static int FrameTimer = 0;//지나간 프레임수를 측정합니다.
        public static int ElapsedMillisec = 0; // 한 프레임에서 다음 프레임으로 넘어가는 밀리초를 측정합니다.

        public static void InternalUpdate() // 이것은 함부로 불러서는 안됩니다.
        {
            FrameTimer++;
            Fader.Update();
            MusicBox.Update();
        }

        public static void InternalDraw() // 이것은 함부로 불러서는 안됩니다.
        {
            Game1.Painter.OpenCanvas(() =>
            {
                Fader.DrawAll();
            });
        }

        public static void DrawString(string s, Point p, Color c)
        {
            GfxStr t = new GfxStr(s, p);
            t.Draw(c);
        }
        public static void DrawString(string s, Point p, Color c, Color BackGroundColor)
        {
            GfxStr t = new GfxStr(s, p, 3);
            Filter.Absolute(t, BackGroundColor);
            t.Draw(c);
        }


        public static Rectangle FullScreen //현재 게임 풀스크린을 다룹니다.
        {
            set
            {
                Game1.graphics.PreferredBackBufferWidth = value.Width;
                Game1.graphics.PreferredBackBufferHeight = value.Height;
                Game1.graphics.ApplyChanges();
            }
            get { return new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height); }
        }

        public static void ToggleFullScreen() // 윈도우 상단바가 없는 풀스크린 모드로 전환합니다.
        {
            Game1.graphics.ToggleFullScreen();
        }

        public static void DrawFullScreen(string SpriteName)
        {
            Gfx2D g = new Gfx2D(SpriteName, FullScreen);
            g.Draw();
        }


        public static void DrawFullScreen(string SpriteName, Color c)
        {
            Gfx2D g = new Gfx2D(SpriteName, FullScreen);
            g.Draw(c);
        }

        public static readonly Rectangle GameScreen = FullScreen;
        //고정된 사이즈의 게임스크린을 설정하고, 이후 이 스크린을 카메라를 통해 매핑하는 형식으로 해상도를 조정하는 것이 좋습니다. 게임스크린 사이즈가 해상도보다 커야 합니다.



        private static Random random = new Random();
        public static int Random(int x, int y)
        {
            return random.Next(Math.Min(x, y), Math.Max(x, y));
        }

        public static double Random()
        {
            return random.NextDouble();
        }
        /// <summary>
        /// 리스트 아이템 중 한개를 랜덤하게 픽합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Ts"></param>
        /// <returns></returns>
        public static T RandomPick<T>(List<T> Ts) //리스트 아이템 중 한개를 랜덤하게 픽합니다.
        {
            double r = StandAlone.Random();
            double m = 1.0 / Ts.Count;
            for (int i = 0; i < Ts.Count; i++)
            {
                if (r >= m * i && r < m * (i + 1))
                {
                    return Ts[i];
                }
            }
            return Ts[0];
        }

    }
}
