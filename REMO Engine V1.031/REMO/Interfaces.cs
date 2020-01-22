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
using REMO_Engine_V1._031;

namespace REMO_Engine_V1._031
{
    public interface IMovable
    {
        Point Pos { get; set; }
        void MoveTo(Point p);//(x,y)로 옮긴다.
        void MoveTo(Point p, double speed); // (x,y)를 향해 등속운동.
        void MoveByVector(Point v, double speed); // 벡터 v의 방향으로 speed의 속도로 등속운동한다.
    }


    public abstract class Movable //물체의 위치와 위치를 이동시키는 함수를 포함하는 추상클래스입니다.
    {
        public abstract Point Pos { get; set; }
        public void MoveTo(int x, int y, double speed)// (x,y)를 향해 등속운동.
        {
            double N = Method2D.Distance(new Point(x, y), Pos);//두 물체 사이의 거리
            if (N < speed)//거리가 스피드보다 가까우면 도착.
            {
                Pos = new Point(x, y);
                return;
            }

            Vector2 v = new Vector2(x - Pos.X, y - Pos.Y);
            v.Normalize();
            v = new Vector2((float)speed * v.X, (float)speed * v.Y);
            Pos = new Point(Pos.X + (int)(v.X), Pos.Y + (int)(v.Y));
        }
        public void MoveByVector(Point v, double speed)// 벡터 v의 방향으로 speed의 속도로 등속운동한다.
        {
            double N = Method2D.Distance(new Point(0, 0), v);
            int Dis_X = (int)(v.X * speed / N);
            int Dis_Y = (int)(v.Y * speed / N);
            Pos = new Point(Pos.X + Dis_X, Pos.Y + Dis_Y);
        }
    }

    public interface IDrawable
    {
        void Draw();
        void RegisterDrawAct(Action a);
    }

    public interface IBoundable
    {
        Rectangle Bound { get; }
    }

    public interface IInitiable//이 클래스를 상속받은 객체는 단 한번 InitOnce 함수를 통해 액션을 호출할 기회를 얻습니다. 그 이후 InitOnce는 작동하지 않습니다. 주로 커스터마이즈된 이니셜라이즈 함수를 구현할 때 쓰게 됩니다.실제 구현은 REMOC을 참조하십시오.
    {
        void InitOnce(Action a);
    }

    //주로 씬의 Init Block 안에서 이것을 활용합니다. 기본적으로 씬의 init block은 씬을 load할 때마다 발생하는데, 그중 단 한번만 init하고 싶은 경우가 있습니다.
    public abstract class HInitOnce : IInitiable //IInitiable을 구현하는 추상 클래스입니다. 씬에서 이것을 유용하게 활용할 수 있습니다.
    {
        //InitOnce 구현.
        private bool isInited = false;
        public void InitOnce(Action a)
        {
            if (!isInited)
            {
                a();
                isInited = true;
            }

        }
    }

    public interface IClickable : IBoundable
    {
        void ClickAct();
        void RegisterClickAct(Action a);
        bool ContainsCursor();
        bool CursorClickedThis();
        bool Contains(Point p);
    }



}
