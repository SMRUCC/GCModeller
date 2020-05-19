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

        for (let x of source) {
            if (predicate(x)) {
                takes.push(x);
            } else {
                break;
            }
        }

        return new IEnumerator<T>(takes);
    }

    export function Where<T>(source: T[], predicate: (e: T) => boolean): IEnumerator<T> {
        let takes: T[] = [];

        for (let o of source) {
            if (true == predicate(o)) {
                takes.push(o);
            }
        }

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
        for (let element of source) {
            if (!predicate(element)) {
                return false;
            }
        }

        return true;
    }

    export function Any<T>(source: T[], predicate: (e: T) => boolean): boolean {
        for (let element of source) {
            if (true == predicate(element)) {
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

        let tree = new algorithm.BTree.binaryTree<TKey, T[]>(compares);
        let key: TKey;
        let list: T[];

        for (let obj of source) {
            key = getKey(obj);
            list = tree.find(key);

            if (list) {
                list.push(obj);
            } else {
                tree.add(key, [obj]);
            }
        }

        // TypeScript.logging.log(tree);

        return tree
            .AsEnumerable()
            .Select(node => {
                return new Group<TKey, T>(node.key, node.value);
            });
    }

    export function AllKeys<T>(sequence: T[]): string[] {
        return $from(sequence)
            .Select(o => Object.keys(<any>o))
            .Unlist<string>()
            .Distinct()
            .ToArray();
    }   
}