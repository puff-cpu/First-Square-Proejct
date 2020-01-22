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
    public static class ExampleScene1 // 사각형이 튕기는 씬입니다.
    {
        public static Gfx2D sqr = new Gfx2D(new Rectangle(200, 200, 50, 50));
        public static Gfx2D sqr2 = new Gfx2D(new Rectangle(200, 400, 300, 20));

        public static Vector2 v = new Vector2(0, 0);
        public static Vector2 g = new Vector2(0, 0.2f);
        public static double speed = 0;
        public static void MoveSquare() => sqr.Pos += v.ToPoint();


        public static Scene scn = new Scene(
            () =>
            {

            },
            () =>
            {
                v += g;//이 물체는 중력을 받는다.

                MoveSquare();//속도벡터에 의해 물체를 움직인다.
                if (Rectangle.Intersect(sqr.Bound, sqr2.Bound) != Rectangle.Empty)//충돌 판정
                {
                    v = -v;//바에 충돌할 경우, 속도벡터가 바뀐다.
                    MoveSquare();
                }
            },
            () =>
            {
                sqr.Draw(Color.White);
                sqr2.Draw(Color.Red);

            }
            );

    }

    public static class ExampleGame1 // 점프하며 사각형들을 피하는 게임입니다.
    {
        public static Gfx2D sqr = new Gfx2D(new Rectangle(50, 300, 50, 50));
        public static Vector2 v = new Vector2(0, 0);
        public static Vector2 g = new Vector2(0, 1);
        public static Gfx2D Ground = new Gfx2D(new Rectangle(0, 350, 1000, 350));
        public static void MoveSqr() => sqr.Pos += v.ToPoint();
        public static List<Gfx> Enemies = new List<Gfx>();
        public static int Score = 0;
        public static int GameOverTimer = 0;
        public static Scene scn = new Scene(() => { }, () =>
        {
            if (GameOverTimer > 0)
                GameOverTimer--;
            if (sqr.Pos.Y < 300)//스퀘어가 공중에 떴을 때
                v += g;
            MoveSqr();
            if (sqr.Pos.Y > 300)
                sqr.Pos = new Point(50, 300);//스퀘어는 바닥을 뚫을 수 없다.
            if (sqr.Pos.Y < 0)
                sqr.Pos = new Point(50, 0);//스퀘어는 천장을 뚫을 수 없다.

            if (User.Pressing(Keys.Space))
                v = new Vector2(0, -15);
            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemies[i].MoveByVector(new Point(-1, 0), 3 + 0.1 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                if (Rectangle.Intersect(Enemies[i].Bound, sqr.Bound) != Rectangle.Empty)//적과 부딪치면 스코어가 초기화됩니다.
                {
                    Score = 0;
                    GameOverTimer = 30;
                }

                if (Enemies[i].Pos.X < -30)//이미 지나간 적은 제거되어 점수가 됩니다.
                {
                    Enemies.RemoveAt(i);
                    i--;
                    Score += 10;
                }
            }
            if (StandAlone.FrameTimer % Math.Max(20, 70 - StandAlone.FrameTimer / 100) == 0)
            {
                Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(0, 300), 30, 30))); // 적들을 생성합니다.
            }




        }, () =>
        {
            sqr.Draw(Color.White, Color.Red * (GameOverTimer * 0.1f));
            for (int i = 0; i < Enemies.Count; i++)
                Enemies[i].Draw(Color.White, Color.Red * GameOverTimer * 0.1f);
            Ground.Draw(Color.White, Color.Red * GameOverTimer * 0.1f);
            StandAlone.DrawString("SCORE : " + Score, new Point(200, 400), Color.Black);
        });


    }
    public static class FlickerScene
    {
        public static Gfx2D sqr = new Gfx2D(new Rectangle(0, 300, 5, 5));
        public static Gfx2D sqr2 = new Gfx2D(new Rectangle(500, 400, 100, 100));
        public static Gfx2D sqr3 = new Gfx2D(new Rectangle(300, 400, 100, 100));
        public static Scene scn = new Scene(() =>
        {
        }, () =>
        {
            sqr.Pos += new Point(1, 0);
            //sqr.MoveByVector(new Point(1,0), 1.0);
            sqr.Pos = new Point(sqr.Pos.X, (int)(-Fader.Flicker(100) * 100) + 300);
            Fader.Add(new Gfx2D(sqr.Bound), 1000, Color.White);
        }, () =>
        {
            sqr.Draw(Color.White);
            sqr2.Draw(Color.White, Color.Red * Fader.Flicker(100));
            sqr3.Draw(Color.White, Color.Green * Fader.Flicker(100));
            Fader.Draw(Color.White);
        });
    }
}
