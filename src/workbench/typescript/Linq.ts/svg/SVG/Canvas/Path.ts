namespace Canvas {

    /**
     * ``path``元素是用来定义形状的通用元素。所有的基本形状都可以
     * 用``path``元素来创建。
    */
    export class Path {

        private pathStack: string[];

        /**
         * 获取SVG的path字符串结果
        */
        public get d(): string {
            return this.pathStack.join(" ");
        }
        
        public constructor() {
            this.pathStack = [];
        }

        public toString(): string {
            return this.d;
        }
        
        /**
         * 从给定的（x,y）坐标开启一个新的子路径或路径。M表示后面跟随的是绝对坐标值。
         * m表示后面跟随的是一个相对坐标值。如果"moveto"指令后面跟随着多个坐标值，那么
         * 这多个坐标值之间会被当做用线段连接。因此如果moveto是相对的，那么lineto也将会
         * 是相对的，反之也成立。如果一个相对的moveto出现在path的第一个位置，那么它会
         * 被认为是一个绝对的坐标。在这种情况下，子序列的坐标将会被当做相对的坐标，即便
         * 它在初始化的时候是绝对坐标。
        */
        public MoveTo(x: number, y: number, relative: boolean = false): Path {
            if (relative) {
                this.pathStack.push(`m ${x},${y}`);
            } else {
                this.pathStack.push(`M ${x},${y}`);
            }

            return this;
        }

        /**
         * 从（cpx,cpy）画一个水平线到（x,cpy）。H表示后面跟随的参数是绝对的坐标，h表示
         * 后面跟随的参数是相对坐标。可以为其提供多个x值作为参数。在指令执行结束后，
         * 最新的当前点将是参数提供的最后值（x，cpy）
        */
        public HorizontalTo(x: number, relative: boolean = false): Path {
            if (relative) {
                this.pathStack.push(`h ${x}`);
            } else {
                this.pathStack.push(`H ${x}`);
            }

            return this;
        }

        /**
         * 从当前点（cpx，cpy）到（cpx，y）画一条竖直线段。V表示后面的参数是绝对坐标
         * 值，v表示后面跟着的参数是相对坐标值。可以供多个y值作为参数使用。在指令的最
         * 后，根据最后的参数y值最新的当前点的坐标值是（cpx,y）.
        */
        public VerticalTo(y: number, relative: boolean = false): Path {
            if (relative) {
                this.pathStack.push(`v ${y}`);
            } else {
                this.pathStack.push(`V ${y}`);
            }

            return this;
        }

        /**
         * 画一条从当前点到给定的（x,y）坐标，这个给定的坐标将变为新的当前点。L表示后面
         * 跟随的参数将是绝对坐标值；l表示后面跟随的参数将是相对坐标值。可以通过指定一系
         * 列的坐标来描绘折线。在命令执行后，新的当前点将会被设置成被提供坐标序列的最后
         * 一个坐标。
        */
        public LineTo(x: number, y: number, relative: boolean = false): Path {
            if (relative) {
                this.pathStack.push(`l ${x} ${y}`);
            } else {
                this.pathStack.push(`L ${x} ${y}`);
            }

            return this;
        }

        /**
         * 在曲线开始的时候，用（x1，y1）作为当前点（x，y）的控制点，
         * 在曲线结束的时候，用（x2，y2）作为当前点的控制点，
         * 画一段立方体的贝塞尔曲线。C表示后面跟随的参数是绝对坐标值；
         * c表示后面跟随的参数是相对坐标值。可以为贝塞尔函数提供多个参数
         * 值。在指令执行完毕后，最后的当前点将变为在贝塞尔函数中只用的
         * 最后的（x，y）坐标值
        */
        public CurveTo(x1: number, y1: number, x2: number, y2: number, endX: number, endY: number, relative: boolean = false): Path {
            if (relative) {
                this.pathStack.push(`c ${x1} ${y1} ${x2} ${y2} ${endX} ${endY}`);
            } else {
                this.pathStack.push(`C ${x1} ${y1} ${x2} ${y2} ${endX} ${endY}`);
            }

            return this;
        }

        /**
         * 从当前点（x，y）画一个立方体的贝塞尔曲线。相对于当前点，
         * 第一个控制点被认为是前面命令的第二个控制点的反射。（如果
         * 前面没有指令或者指令不是C, c, S 或者s，那么就认定当前点和
         * 第一个控制点是一致的。）（x2，y2）是第二个控制点，控制
         * 着曲线结束时的变化。S表示后面跟随的参数是绝对的坐标值。
         * s表示后面跟随的参数是相对的坐标值。多个值可以作为
         * 贝塞尔函数的参数。在执行执行完后，最新的当前点是在贝塞尔函数中
         * 使用的最后的（x，y）坐标值。
        */
        public SmoothCurveTo(x2: number, y2: number, endX: number, endY: number, relative: boolean = false): Path {
            if (relative) {
                this.pathStack.push(`s ${x2} ${y2} ${endY} ${endY}`);
            } else {
                this.pathStack.push(`S ${x2} ${y2} ${endY} ${endY}`);
            }

            return this;
        }

        /**
         * 从当前点（x，y）开始，以（x1，y1）为控制点，画出一个二次贝塞尔曲线。
         * Q表示后面跟随的参数是绝对坐标值，q表示后面跟随的参数是相对坐标值。
         * 可以为贝塞尔函数指定多个参数值。在指令执行结束后，新的当前点是贝塞尔曲线调用参数中最后一个坐标值（x，y）。
        */
        public QuadraticBelzier(x: number, y: number, endX: number, endY: number, relative: boolean = false): Path {
            if (relative) {
                this.pathStack.push(`q ${x} ${y} ${endX} ${endY}`);
            } else {
                this.pathStack.push(`Q ${x} ${y} ${endX} ${endY}`);
            }

            return this;
        }

        /**
         * 用来从当前点（x，y）来画出一个椭圆弧曲线。曲线的形状和方向通过椭圆半径（rx，ry）
         * 和一个沿X轴旋转度来指明椭圆作为一个整体在当前坐标系下旋转的情形。椭圆的中心
         * （cx，cy）是通过满足其他参数的约束自动计算出来的。large-arc-flag和sweep-flag决定了计算和帮助要画的弧度大小。
        */
        public EllipticalArc(rX: number, rY: number, xrotation: number, flag1: number, flag2: number, x: number, y: number, relative: boolean = false): Path {
            if (relative) {
                this.pathStack.push(`a ${rX} ${rY} ${xrotation} ${flag1} ${flag2} ${x} ${y}`);
            } else {
                this.pathStack.push(`A ${rX} ${rY} ${xrotation} ${flag1} ${flag2} ${x} ${y}`);
            }

            return this;
        }

        /**
         * ClosePath命令将在当前路径从，从当前点到第一个点简单画一条直线。它是最简单的命令，
         * 而且不带有任何参数。它沿着到开始点的最短的线性路径，如果别的路径落在这路上，将
         * 可能路径相交。句法是``Z``或``z``，两种写法作用都一样。
        */
        public ClosePath(): Path {
            this.pathStack.push("Z");
            return this;
        }
    }
}
