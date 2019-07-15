//// <reference path="Enumerator.ts" />

/**
 * The linq pipline implements at here. (在这个模块之中实现具体的数据序列算法)
*/
module Enumerable {

    export function Range(from: number, to: number, steps: number = 1): number[] {
        return new data.NumericRange(from, to).PopulateNumbers(steps);
    }

    export function Min(...v: number[]): number {
        let min: number = 99999999999;

        for (let x of v) {
            if (x < min) {
                min = x;
            }
        }

        return min;
    }

    /**
     * 进行数据序列的投影操作
     * 
    */
    export function Select<T, TOut>(source: T[], project: (e: T, i: number) => TOut): IEnumerator<TOut> {
        var projections: TOut[] = [];

        source.forEach((o, i) => {
            projections.push(project(o, i));
        });

        return new IEnumerator<TOut>(projections);
    }

    /**
     * 进行数据序列的排序操作
     * 
    */
    export function OrderBy<T>(source: T[], key: (e: T) => number): IEnumerator<T> {
        // array clone
        var clone: T[] = [...source];

        clone.sort((a, b) => {
            // a - b
            return key(a) - key(b);
        });
        // console.log("clone");
        // console.log(clone);
        return new IEnumerator<T>(clone);
    }

    export function OrderByDescending<T>(source: T[], key: (e: T) => number): IEnumerator<T> {
        return Enumerable.OrderBy(source, (e) => {
            // b - a
            return -key(e);
        });
    }

    export function Take<T>(source: T[], n: number): IEnumerator<T> {
        var takes: T[] = [];
        var len: number = source.length;

        if (len <= n) {
            takes = source;
        } else {
            takes = [];

            for (var i = 0; i < n; i++) {
                if (i >= len) {
                    break;
                } else {
                    takes.push(source[i]);
                }
            }
        }

        return new IEnumerator<T>(takes);
    }

    export function Skip<T>(source: T[], n: number): IEnumerator<T> {
        var takes: T[] = [];

        if (n >= source.length) {
            return new IEnumerator<T>([]);
        }

        for (var i = n; i < source.length; i++) {
            takes.push(source[i]);
        }

        return new IEnumerator<T>(takes);
    }

    export function TakeWhile<T>(source: T[], predicate: (e: T) => boolean): IEnumerator<T> {
        var takes: T[] = [];

        for (var i: number = 0; i < source.length; i++) {
            if (predicate(source[i])) {
                takes.push(source[i]);
            } else {
                break;
            }
        }

        return new IEnumerator<T>(takes);
    }

    export function Where<T>(source: T[], predicate: (e: T) => boolean): IEnumerator<T> {
        var takes: T[] = [];

        source.forEach(o => {
            if (true == predicate(o)) {
                takes.push(o);
            }
        });

        return new IEnumerator<T>(takes);
    }

    export function SkipWhile<T>(source: T[], predicate: (e: T) => boolean): IEnumerator<T> {
        for (var i: number = 0; i < source.length; i++) {
            if (true == predicate(source[i])) {
                // skip
            } else {
                // end skip
                return Enumerable.Skip(source, i);
            }
        }

        // skip all
        return new IEnumerator<T>([]);
    }

    export function All<T>(source: T[], predicate: (e: T) => boolean): boolean {
        for (var i = 0; i < source.length; i++) {
            if (!predicate(source[i])) {
                return false;
            }
        }

        return true;
    }

    export function Any<T>(source: T[], predicate: (e: T) => boolean): boolean {
        for (var i = 0; i < source.length; i++) {
            if (true == predicate(source[i])) {
                return true;
            }
        }

        return false;
    }

    /**
     * Implements a ``group by`` operation by binary tree data structure.
    */
    export function GroupBy<T, TKey>(source: T[],
        getKey: (e: T) => TKey,
        compares: (a: TKey, b: TKey) => number): IEnumerator<Group<TKey, T>> {

        var tree = new algorithm.BTree.binaryTree<TKey, T[]>(compares);

        source.forEach(obj => {
            var key: TKey = getKey(obj);
            var list: T[] = tree.find(key);

            if (list) {
                list.push(obj);
            } else {
                tree.add(key, [obj]);
            }
        });

        console.log(tree);

        return tree.AsEnumerable().Select(node => {
            return new Group<TKey, T>(node.key, node.value);
        });
    }

    export function AllKeys<T>(sequence: T[]): string[] {
        return From(sequence)
            .Select(o => Object.keys(o))
            .Unlist<string>()
            .Distinct()
            .ToArray();
    }

    export class JoinHelper<T, U> {

        private xset: T[];
        private yset: U[];
        private keysT: string[];
        private keysU: string[];

        public constructor(x: T[], y: U[]) {
            this.xset = x;
            this.yset = y;
            this.keysT = AllKeys(x);
            this.keysU = AllKeys(y);
        }

        public JoinProject<V>(x: T, y: U): V {
            var out: object = {};

            this.keysT.forEach(k => out[k] = x[k]);
            this.keysU.forEach(k => out[k] = y[k]);

            return <V><any>out;
        }

        public Union<K, V>(
            tKey: (x: T) => K,
            uKey: (x: U) => K,
            compare: (a: K, b: K) => number,
            project: (x: T, y: U) => V = this.JoinProject): IEnumerator<V> {

            var tree = this.buildUtree(uKey, compare);
            var output: V[] = [];
            var keyX = new algorithm.BTree.binaryTree<K, K>(compare);

            this.xset.forEach(x => {
                var key: K = tKey(x);
                var list: U[] = tree.find(key);

                if (list) {
                    // 有交集，则进行叠加投影
                    list.forEach(y => output.push(project(x, y)));

                    if (!keyX.find(key)) {
                        keyX.add(key);
                    }
                } else {
                    // 没有交集，则投影空对象
                    output.push(project(x, <U>{}));
                }
            });
            this.yset.forEach(y => {
                var key: K = uKey(y);

                if (!keyX.find(key)) {
                    // 没有和X进行join，则需要union到最终的结果之中
                    // 这个y是找不到对应的x元素的
                    output.push(project(<T>{}, y));
                }
            });

            return new IEnumerator<V>(output);
        }

        private buildUtree<K>(uKey: (x: U) => K, compare: (a: K, b: K) => number): algorithm.BTree.binaryTree<K, U[]> {
            var tree = new algorithm.BTree.binaryTree<K, U[]>(compare);

            this.yset.forEach(obj => {
                var key: K = uKey(obj);
                var list: U[] = tree.find(key);

                if (list) {
                    list.push(obj);
                } else {
                    tree.add(key, [obj]);
                }
            });

            return tree;
        }

        public LeftJoin<K, V>(
            tKey: (x: T) => K,
            uKey: (x: U) => K,
            compare: (a: K, b: K) => number,
            project: (x: T, y: U) => V = this.JoinProject): IEnumerator<V> {

            var tree = this.buildUtree(uKey, compare);
            var output: V[] = [];

            this.xset.forEach(x => {
                var key: K = tKey(x);
                var list: U[] = tree.find(key);

                if (list) {
                    // 有交集，则进行叠加投影
                    list.forEach(y => output.push(project(x, y)));
                } else {
                    // 没有交集，则投影空对象
                    output.push(project(x, <U>{}));
                }
            });

            return new IEnumerator<V>(output);
        }
    }
}