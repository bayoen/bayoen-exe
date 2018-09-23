using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using js = Newtonsoft.Json;
using jl = Newtonsoft.Json.Linq;
using System.Windows.Threading;

namespace bayoen
{
    public partial class MainWindow
    {
        public class PPTimer : DispatcherTimer
        {
            public const int DefaultUnit = 10;  // 100 times in a second

            public readonly int UnitMilliSeconds;
            private readonly TimeSpan TickTime;
            public readonly int StepAUnit;
            public readonly int StepBUnit;
            public readonly int RingCut;

            public int _ring;
            public int Ring
            {
                get => this._ring;
                set
                {
                    this._ring = (this.RingCut <= value) ? (0) : (value);
                }
            }

            public bool StepAFlag
            {
                get
                {
                    return (this.Ring % this.StepAUnit == 0);
                }
            }
            public bool StepBFlag
            {
                get
                {
                    return (this.Ring % this.StepBUnit == 0);
                }
            }


            /// <summary>
            /// Create new PPT timer with 10 [ms]
            /// </summary>
            /// <param name="finding"></param>
            /// <param name="capture"></param>
            public PPTimer(int finding, int capture) : this(DefaultUnit, finding, capture) { }

            /// <summary>
            /// Create new PPT timer with custum timer
            /// </summary>
            /// <param name="clk"></param>
            /// <param name="unitA"></param>
            /// <param name="unitB"></param>
            public PPTimer(int clk, int unitA, int unitB)
            {
                this.UnitMilliSeconds = clk;
                this.StepAUnit = unitA;
                this.StepBUnit = unitB;

                this.RingCut = LCM2(unitA, unitB);
                this._ring = 0;

                this.TickTime = new TimeSpan(0, 0, 0, 0, UnitMilliSeconds);

                this.Interval = this.TickTime;
            }

            /// <summary>
            /// Get the Least Common Multiple of given 2 numbers
            /// </summary>
            /// <param name="a">a number A</param>
            /// <param name="b">a number B</param>
            /// <returns></returns>
            private static int LCM2(int a, int b)
            {
                int num1, num2;

                if (a > b)
                {
                    num1 = a; num2 = b;
                }
                else
                {
                    num1 = b; num2 = a;
                }

                for (int i = 1; i < num2; i++)
                {
                    if ((num1 * i) % num2 == 0)
                    {
                        return i * num1;
                    }
                }

                return num1 * num2;
            }
        }
    }

}