/// <reference path="Iterator.ts" />
/// <reference path="Enumerable.ts" />

/**
 * Provides a set of static (Shared in Visual Basic) methods for querying 
 * objects that implement ``System.Collections.Generic.IEnumerable<T>``.
 * 
 * (这个枚举器类型是构建出一个Linq查询表达式所必须的基础类型，这是一个静态的集合，不会发生元素的动态添加或者删除)
*/
class IEnumerator<T> extends LINQIterator<T> {

    //#region "readonly property"

    /**
     * 获取序列的元素类型
    */
    public get ElementType(): TypeScript.Reflection.TypeInfo {
        return $ts.typeof(<any>this.First);
    };

    /**
     * Get the element value at a given index position 
     * of this data sequence.
     * 
     * @param index index value should be an integer value.
    */
    public ElementAt(index: string | number = null): T {
        if (!index) {
            index = 0;
        } else if (typeof index == "string") {
            throw `Item index='${index}' must be an integer!`;
        }

        return this.sequence[index];
    }

    //#endregion

    /**
     * 可以从一个数组或者枚举器构建出一个Linq序列
     * 
     * @param source The enumerator data source, this constructor will perform 
     *       a sequence copy action on this given data source sequence at here.
    */
    constructor(source: T[] | IEnumerator<T>) {
        super(IEnumerator.getArray(source));
    }

    /**
     * 在明确类型信息的情况下进行强制类型转换
    */
    public ctype<U>(): IEnumerator<U> {
        return new IEnumerator<U>(<any>[...this.sequence]);
    }

    private static getArray<T>(source: T[] | IEnumerator<T>): T[] {
        if (!source) {
            return [];
        } else if (Array.isArray(source)) {
            // 2018-07-31 为了防止外部修改source导致sequence数组被修改
            // 在这里进行数组复制，防止出现这种情况
            return [...source];
        } else {
            return [...source.sequence];
        }
    }

    public indexOf(x: T): number {
        return this.sequence.indexOf(x);
    }

    /**
     * Get the first element in this sequence 
    */
    public get First(): T {
        return this.sequence[0];
    }

    /**
     * Get the last element in this sequence 
    */
    public get Last(): T {
        return this.sequence[this.Count - 1];
    }

    /**
     * If the sequence length is zero, then returns the default value.
    */
    public FirstOrDefault(Default: T = null): T {
        if (this.Count == 0) {
            return Default;
        } else {
            return this.sequence[0];
        }
    }

    /**
     * 两个序列求总和
    */
    public Union<U, K, V>(another: IEnumerator<U> | U[],
        tKey: (x: T) => K,
        uKey: (x: U) => K,
        compare: (a: K, b: K) => number,
        project: (x: T, y: U) => V = null): IEnumerator<V> {

        if (!Array.isArray(another)) {
            another = another.ToArray();
        }

        var join = new Enumerable.JoinHelper<T, U>(
            this.sequence, another
        );

        return join.Union(
            tKey, uKey,
            compare,
            project
        );
    }

    /**
     * 如果在another序列之中找不到对应的对象，则当前序列会和一个空对象合并
     * 如果another序列之中有多余的元素，即该元素在当前序列之中找不到的元素，会被扔弃
     * 
     * @param project 如果这个参数被忽略掉了的话，将会直接进行属性的合并
    */
    public Join<U, K, V>(another: IEnumerator<U> | U[],
        tKey: (x: T) => K,
        uKey: (x: U) => K,
        compare: (a: K, b: K) => number,
        project: (x: T, y: U) => V = null): IEnumerator<V> {

        if (!Array.isArray(another)) {
            another = another.ToArray();
        }

        var join = new Enumerable.JoinHelper<T, U>(
            this.sequence, another
        );

        return join.LeftJoin(
            tKey, uKey,
            compare,
            project
        );
    }

    /**
     * Projects each element of a sequence into a new form.
     * 
     * @typedef TOut The type of the value returned by selector.
     * 
     * @param selector A transform function to apply to each element.
     * 
     * @returns An ``System.Collections.Generic.IEnumerable<T>`` 
     *          whose elements are the result of invoking the 
     *          transform function on each element of source.
    */
    public Select<TOut>(selector: (o: T, i: number) => TOut): IEnumerator<TOut> {
        return Enumerable.Select<T, TOut>(this.sequence, selector);
    }

    /**
     * Groups the elements of a sequence according to a key selector function. 
     * The keys are compared by using a comparer and each group's elements 
     * are projected by using a specified function.
     * 
     * @param compares 注意，javascript在进行中文字符串的比较的时候存在bug，如果当key的类型是字符串的时候，
     *                 在这里需要将key转换为数值进行比较，遇到中文字符串可能会出现bug
    */
    public GroupBy<TKey>(
        keySelector: (o: T) => TKey,
        compares?: (a: TKey, b: TKey) => number): IEnumerator<Group<TKey, T>> {

        if (isNullOrUndefined(compares)) {
            let x = keySelector(this.First);

            switch (typeof x) {
                case "string": compares = <any>Strings.CompareTo; break;
                case "number": compares = <any>((x, y) => x - y); break;
                case "boolean":
                    compares = <any>(function (x, y) {
                        if (x == y) {
                            return 0;
                        }

                        // 有一个肯定是false
                        if (x == true) {
                            return 1;
                        } else {
                            return -1
                        }
                    });
                    break;
                default:
                    if (this.Count == 0) {
                        return <any>$from([]);
                    } else {
                        throw "No element comparer was specific!";
                    }
            }
        }

        return Enumerable.GroupBy(this.sequence, keySelector, compares);
    }

    /**
     * Filters a sequence of values based on a predicate.
     * 
     * @param predicate A test condition function.
     * 
     * @returns Sub sequence of the current sequence with all 
     *     element test pass by the ``predicate`` function.
    */
    public Where(predicate: (e: T) => boolean): IEnumerator<T> {
        return Enumerable.Where(this.sequence, predicate);
    }

    public Which(predicate: (e: T) => boolean, first: boolean = true): number | IEnumerator<number> {
        let index: number[]

        if (!first) {
            index = [];
        }

        for (var i = 0; i < this.sequence.length; i++) {
            if (predicate(this.sequence[i])) {
                if (first) {
                    return i;
                } else {
                    index.push(i);
                }
            }
        }

        return new IEnumerator<number>(index);
    }

    /**
     * Get the min value in current sequence.
     * (求取这个序列集合的最小元素，使用这个函数要求序列之中的元素都必须能够被转换为数值)
    */
    public Min(project: (e: T) => number = (e) => Strings.as_numeric(e)): T {
        return Enumerable.OrderBy(this.sequence, project).First;
    }

    /**
     * Get the max value in current sequence.
     * (求取这个序列集合的最大元素，使用这个函数要求序列之中的元素都必须能够被转换为数值)
    */
    public Max(project: (e: T) => number = (e) => Strings.as_numeric(e)): T {
        return Enumerable.OrderByDescending(this.sequence, project).First;
    }

    /**
     * 求取这个序列集合的平均值，使用这个函数要求序列之中的元素都必须能够被转换为数值
    */
    public Average(project: (e: T) => number = null): number {
        if (this.Count == 0) {
            return 0;
        } else {
            return this.Sum(project) / this.sequence.length;
        }
    }

    /**
     * 求取这个序列集合的和，使用这个函数要求序列之中的元素都必须能够被转换为数值
    */
    public Sum(project: (e: T) => number = null): number {
        var x: number = 0;

        if (!project) project = (e) => {
            return Number(e);
        }

        for (let val of this.sequence) {
            x += project(val);
        }

        return x;
    }

    /**
     * Sorts the elements of a sequence in ascending order according to a key.
     * 
     * @param key A function to extract a key from an element.
     *
     * @returns An ``System.Linq.IOrderedEnumerable<T>`` whose elements are 
     *          sorted according to a key.
    */
    public OrderBy(key: (e: T) => number): IEnumerator<T> {
        return Enumerable.OrderBy(this.sequence, key);
    }

    /**
     * Sorts the elements of a sequence in descending order according to a key.
     * 
     * @param key A function to extract a key from an element.
     * 
     * @returns An ``System.Linq.IOrderedEnumerable<T>`` whose elements are 
     *          sorted in descending order according to a key.
    */
    public OrderByDescending(key: (e: T) => number): IEnumerator<T> {
        return Enumerable.OrderByDescending(this.sequence, key);
    }

    /**
     * Split a sequence by elements count
    */
    public Split(size: number): IEnumerator<T[]> {
        let seq: T[][] = [];
        let row: T[] = [];

        for (let element of this.sequence) {
            if (row.length < size) {
                row.push(element);
            } else {
                seq.push(row);
                row = [];
            }
        }

        if (row.length > 0) {
            seq.push(row);
        }

        return new IEnumerator<T[]>(seq);
    }

    public subset(indexer: number[] | boolean[]): IEnumerator<T> {
        let index: number[];

        if (typeof indexer[0] == "boolean") {
            index = [];

            for (let i = 0; i < indexer.length; i++) {
                if (<boolean>indexer[i]) {
                    index.push(i);
                }
            }
        } else {
            index = <number[]>indexer;
        }

        let subsetOutput: T[] = [];

        for (let i of index) {
            subsetOutput.push(this.sequence[i]);
        }

        return new IEnumerator<T>(subsetOutput);
    }

    /**
     * 取出序列之中的前n个元素
    */
    public Take(n: number): IEnumerator<T> {
        return Enumerable.Take(this.sequence, n);
    }

    /**
     * 跳过序列的前n个元素之后返回序列之中的所有剩余元素
    */
    public Skip(n: number): IEnumerator<T> {
        return Enumerable.Skip(this.sequence, n);
    }

    /**
     * 序列元素的位置反转
    */
    public Reverse(): IEnumerator<T> {
        var rseq = this.ToArray().reverse();
        var seq = new IEnumerator<T>(rseq);

        return seq;
    }

    /**
     * Returns elements from a sequence as long as a specified condition is true.
     * (与Where类似，只不过这个函数只要遇到第一个不符合条件的，就会立刻终止迭代)
    */
    public TakeWhile(predicate: (e: T) => boolean): IEnumerator<T> {
        return Enumerable.TakeWhile(this.sequence, predicate);
    }

    /**
     * Bypasses elements in a sequence as long as a specified condition is true 
     * and then returns the remaining elements.
    */
    public SkipWhile(predicate: (e: T) => boolean): IEnumerator<T> {
        return Enumerable.SkipWhile(this.sequence, predicate);
    }

    /**
     * 判断这个序列之中的所有元素是否都满足特定条件
    */
    public All(predicate: (e: T) => boolean): boolean {
        return Enumerable.All(this.sequence, predicate);
    }

    /**
     * 判断这个序列之中的任意一个元素是否满足特定的条件
    */
    public Any(predicate: (e: T) => boolean = null): boolean {
        if (predicate) {
            return Enumerable.Any(this.sequence, predicate);
        } else {
            if (!this.sequence || this.sequence.length == 0) {
                return false;
            } else {
                return true;
            }
        }
    }

    /**
     * 对序列中的元素进行去重
    */
    public Distinct(key: (o: T) => string = o => o.toString()): IEnumerator<T> {
        return this
            .GroupBy(key, Strings.CompareTo)
            .Select(group => group.First);
    }

    /**
     * 将序列按照符合条件的元素分成区块
     * 
     * @param isDelimiter 一个用于判断当前的元素是否是分割元素的函数
     * @param reserve 是否保留下这个分割对象？默认不保留
    */
    public ChunkWith(isDelimiter: (x: T) => boolean, reserve: boolean = false): IEnumerator<T[]> {
        var chunks: List<T[]> = new List<T[]>();
        var buffer: T[] = [];

        for (let x of this.sequence) {
            if (isDelimiter(x)) {
                chunks.Add(buffer);

                if (reserve) {
                    buffer = [x];
                } else {
                    buffer = [];
                }
            } else {
                buffer.push(x);
            }
        }

        if (buffer.length > 0) {
            chunks.Add(buffer);
        }

        return chunks;
    }

    /**
     * Performs the specified action for each element in an array.
     * 
     * @param callbackfn  A function that accepts up to three arguments. forEach 
     * calls the callbackfn function one time for each element in the array.
     * 
    */
    public ForEach(callbackfn: (x: T, index: number) => void) {
        this.sequence.forEach(function (value, index, array) {
            callbackfn(value, index);
        });
    }

    /**
     * Contract the data sequence to string
     * 
     * @param deli Delimiter string that using for the string.join function
     * @param toString A lambda that describ how to convert the generic type object to string token 
     * 
     * @returns A contract string.
    */
    public JoinBy(
        deli: string,
        toString: (x: T) => string = (x: T) => {
            if (typeof x === "string") {
                return <string><any>x;
            } else {
                return x.toString();
            }
        }): string {

        return this.Select(x => toString(x))
            .ToArray()
            .join(deli);
    }

    /**
     * 如果当前的这个数据序列之中的元素的类型是某一种元素类型的集合，或者该元素
     * 可以描述为另一种类型的元素的集合，则可以通过这个函数来进行降维操作处理。
     * 
     * @param project 这个投影函数描述了如何将某一种类型的元素降维至另外一种元素类型的集合。
     * 如果这个函数被忽略掉的话，会尝试强制将当前集合的元素类型转换为目标元素类型的数组集合。
    */
    public Unlist<U>(project: (obj: T) => U[] = (obj: T) => <U[]><any>obj): IEnumerator<U> {
        var list: U[] = [];

        for (let block of this.sequence) {
            for (let x of project(block)) {
                list.push(x);
            }
        }

        return new IEnumerator<U>(list);
    }

    //#region "conversion"

    /**
     * This function returns a clone copy of the source sequence.
     * 
     * @param clone If this parameter is false, then this function will 
     * returns the origin array sequence directly.
    */
    public ToArray(clone: boolean = true): T[] {
        if (clone) {
            return [...this.sequence];
        } else {
            return this.sequence;
        }
    }

    /**
     * 将当前的这个不可变的只读序列对象转换为可动态增添删除元素的列表对象
    */
    public ToList(): List<T> {
        return new List<T>(this.sequence);
    }

    /**
     * 将当前的这个数据序列对象转换为键值对字典对象，方便用于数据的查找操作
    */
    public ToDictionary<V>(
        keySelector: (x: T) => string,
        elementSelector: (x: T) => V = (X: T) => {
            return <V>(<any>X);
        }): Dictionary<V> {

        let maps = {};
        let key: string;
        let value: V;

        for (let x of this.sequence) {
            // 2018-08-11 键名只能够是字符串类型的
            key = keySelector(x);
            value = elementSelector(x);

            maps[key] = value;
        }

        return new Dictionary<V>(maps);
    }

    /**
     * 将当前的这个数据序列转换为包含有内部位置指针数据的指针对象
    */
    public ToPointer(): Pointer<T> {
        return new Pointer<T>(this);
    }

    /**
     * 将当前的这个序列转换为一个滑窗数据的集合
    */
    public SlideWindows(winSize: number, step: number = 1): IEnumerator<SlideWindow<T>> {
        return SlideWindow.Split(this, winSize, step);
    }

    //#endregion
}