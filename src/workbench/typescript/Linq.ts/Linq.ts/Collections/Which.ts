/**
 * 序列之中的元素下标的操作方法集合
*/
namespace Which {

    /**
     * 查找出所给定的逻辑值集合之中的所有true的下标值
    */
    export function Is(booleans: boolean[] | IEnumerator<boolean>): IEnumerator<number> {
        if (Array.isArray(booleans)) {
            booleans = new IEnumerator<boolean>(booleans)
        }

        return booleans
            .Select((flag, i) => {
                return {
                    flag: flag, index: i
                };
            })
            .Where(t => t.flag)
            .Select(t => t.index);
    }

    /**
     * 默认的通用类型的比较器对象
    */
    export class DefaultCompares<T> {

        /**
         * 一个用于比较通用类型的数值转换器对象
        */
        private as_numeric: (x: T) => number = null;

        public compares(a: T, b: T): number {
            if (!this.as_numeric) {
                this.as_numeric = Strings.AsNumeric(a);

                if (!this.as_numeric) {
                    this.as_numeric = Strings.AsNumeric(b);
                }
            }

            if (!this.as_numeric) {
                // a 和 b 都是null或者undefined
                // 认为这两个空值是相等的
                // 则this.as_numeric会在下一个循环之中被赋值
                return 0;
            } else {
                return this.as_numeric(a) - this.as_numeric(b);
            }
        }

        public static default<T>(): (a: T, b: T) => number {
            return new DefaultCompares().compares;
        }
    }

    /**
     * 查找出序列之中最大的元素的序列下标编号
     * 
     * @param x 所给定的数据序列
     * @param compare 默认是将x序列之中的元素转换为数值进行大小的比较的
    */
    export function Max<T>(x: IEnumerator<T>, compare: (a: T, b: T) => number = DefaultCompares.default<T>()): number {
        var xMax: T = null;
        var iMax: number = 0;

        for (var i: number = 0; i < x.Count; i++) {
            if (compare(x.ElementAt(i), xMax) > 0) {
                // x > xMax
                xMax = x.ElementAt(i);
                iMax = i;
            }
        }

        return iMax;
    }

    /**
     * 查找出序列之中最小的元素的序列下标编号
     * 
     * @param x 所给定的数据序列
     * @param compare 默认是将x序列之中的元素转换为数值进行大小的比较的
    */
    export function Min<T>(x: IEnumerator<T>, compare: (a: T, b: T) => number = DefaultCompares.default<T>()): number {
        return Max<T>(x, (a, b) => - compare(a, b));
    }
}