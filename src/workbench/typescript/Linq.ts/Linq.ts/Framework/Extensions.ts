namespace Framework.Extensions {

    /**
     * 确保所传递进来的参数输出的是一个序列集合对象
    */
    export function EnsureCollection<T>(data: T | T[] | IEnumerator<T>, n = -1): IEnumerator<T> {
        return new IEnumerator<T>(EnsureArray(data, n));
    }

    /**
     * 确保随传递进来的参数所输出的是一个数组对象
     * 
     * @param data 如果这个参数是一个数组，则在这个函数之中会执行复制操作
     * @param n 如果data数据序列长度不足，则会使用null进行补充，n为任何小于data长度的正实数都不会进行补充操作，
     *     相反只会返回前n个元素，如果n是负数，则不进行任何操作
    */
    export function EnsureArray<T>(data: T | T[] | IEnumerator<T>, n = -1): T[] {
        var type = $ts.typeof(<any>data);
        var array: T[];

        if (type.isEnumerator) {
            array = (<IEnumerator<T>>data).ToArray();
        } else if (type.isArray) {
            array = [...<T[]>data];
        } else {
            var x = <T>data;

            if (n <= 0) {
                array = [x];
            } else {
                array = [];

                for (var i: number = 0; i < n; i++) {
                    array.push(x);
                }
            }
        }

        if (1 <= n) {
            if (n < array.length) {
                array = array.slice(0, n);
            } else if (n > array.length) {
                var len: number = array.length;

                for (var i: number = len; i < n; i++) {
                    array.push(null);
                }
            } else {
                // n 和 array 等长，不做任何事
            }
        }

        return array;
    }

    /**
     * Extends `from` object with members from `to`.     
     * 
     * > https://stackoverflow.com/questions/122102/what-is-the-most-efficient-way-to-deep-clone-an-object-in-javascript
     * 
     * @param to If `to` is null, a deep clone of `from` is returned
    */
    export function extend<V>(from: V, to: V = null): V {
        if (from == null || typeof from != "object") return from;
        if (from.constructor != Object && from.constructor != Array) return from;
        if (from.constructor == Date ||
            from.constructor == RegExp ||
            from.constructor == Function ||
            from.constructor == String ||
            from.constructor == Number ||
            from.constructor == Boolean)

            return new (<any>from).constructor(from);

        to = to || new (<any>from).constructor();

        for (var name in from) {
            to[name] = typeof to[name] == "undefined" ? extend(from[name], null) : to[name];
        }

        return to;
    }
}

namespace TypeScript {

    export function gc() {
        return garbageCollect.handler();
    }
}