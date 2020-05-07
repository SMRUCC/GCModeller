/**
 * object creator helper module
*/
declare module Activator {
    /**
     * MetaReader对象和字典相似，只不过是没有类型约束，并且为只读集合
    */
    function CreateMetaReader<V>(nameValues: NamedValue<V>[] | IEnumerator<NamedValue<V>>): TypeScript.Data.MetaReader;
    /**
     * @param properties 如果这个属性定义集合是一个object，则应该是一个IProperty接口的字典对象
    */
    function CreateInstance(properties: object | NamedValue<IProperty>[]): any;
    /**
    * 利用一个名称字符串集合创建一个js对象
    *
    * @param names object的属性名称列表
    * @param init 使用这个函数为该属性指定一个初始值
   */
    function EmptyObject<V>(names: string[] | IEnumerator<string>, init: (() => V) | V): object;
    /**
     * 从键值对集合创建object对象，键名或者名称属性会作为object对象的属性名称
    */
    function CreateObject<V>(nameValues: NamedValue<V>[] | IEnumerator<NamedValue<V>> | MapTuple<string, V>[] | IEnumerator<MapTuple<string, V>>): object;
}
declare namespace data.sprintf {
    /**
     * 对占位符的匹配结果
    */
    class match {
        match: string;
        left: boolean;
        sign: string;
        pad: string;
        min: string;
        precision: string;
        code: string;
        negative: boolean;
        argument: string;
        toString(): string;
    }
    /**
     * 格式化占位符
     *
     * Possible format values:
     *
     * + ``%%`` – Returns a percent sign
     * + ``%b`` – Binary number
     * + ``%c`` – The character according to the ASCII value
     * + ``%d`` – Signed decimal number
     * + ``%f`` – Floating-point number
     * + ``%o`` – Octal number
     * + ``%s`` – String
     * + ``%x`` – Hexadecimal number (lowercase letters)
     * + ``%X`` – Hexadecimal number (uppercase letters)
     *
     * Additional format values. These are placed between the % and the letter (example %.2f):
     *
     * + ``+``      (Forces both + and – in front of numbers. By default, only negative numbers are marked)
     * + ``–``      (Left-justifies the variable value)
     * + ``0``      zero will be used for padding the results to the right string size
     * + ``[0-9]``  (Specifies the minimum width held of to the variable value)
     * + ``.[0-9]`` (Specifies the number of decimal digits or maximum string length)
     *
    */
    const placeholder: RegExp;
    /**
     * @param argumentList ERROR - "arguments" cannot be redeclared in strict mode
    */
    function parseFormat(string: string, argumentList: any[]): {
        matches: match[];
        convCount: number;
        strings: string[];
    };
    /**
     * ### Javascript sprintf
     *
     * > http://www.webtoolkit.info/javascript-sprintf.html#.W5sf9FozaM8
     *
     * Several programming languages implement a sprintf function, to output a
     * formatted string. It originated from the C programming language, printf
     * function. Its a string manipulation function.
     *
     * This is limited sprintf Javascript implementation. Function returns a
     * string formatted by the usual printf conventions. See below for more details.
     * You must specify the string and how to format the variables in it.
    */
    function doFormat(format: string, ...argv: any[]): string;
    /**
     * 进行格式化占位符对格式化参数的字符串替换操作
    */
    function doSubstitute(matches: sprintf.match[], strings: string[]): string;
    function convert(match: sprintf.match, nosign?: boolean): string;
}
/**
 * 可用于ES6的for循环的泛型迭代器对象，这个也是这个框架之中的所有序列模型的基础
 *
 * ```js
 * var it = makeIterator(['a', 'b']);
 *
 * it.next() // { value: "a", done: false }
 * it.next() // { value: "b", done: false }
 * it.next() // { value: undefined, done: true }
 *
 * function makeIterator(array) {
 *     var nextIndex = 0;
 *
 *     return {
 *         next: function() {
 *             return nextIndex < array.length ?
 *                 {value: array[nextIndex++], done: false} :
 *                 {value: undefined, done: true};
 *         }
 *     };
 * }
 * ```
*/
declare class LINQIterator<T> {
    /**
     * The data sequence with specific generic type.
    */
    protected sequence: T[];
    private i;
    /**
     * 实现迭代器的关键元素之1
    */
    [Symbol.iterator](): this;
    /**
     * The number of elements in the data sequence.
    */
    readonly Count: number;
    constructor(array: T[]);
    reset(): LINQIterator<T>;
    /**
     * 实现迭代器的关键元素之2
    */
    next(): IPopulated<T>;
}
/**
 * 迭代器对象所产生的一个当前的index状态值
*/
interface IPopulated<T> {
    value: T;
    done: boolean;
}
/**
 * The linq pipline implements at here. (在这个模块之中实现具体的数据序列算法)
*/
declare module Enumerable {
    function Range(from: number, to: number, steps?: number): number[];
    function Min(...v: number[]): number;
    /**
     * 进行数据序列的投影操作
     *
    */
    function Select<T, TOut>(source: T[], project: (e: T, i: number) => TOut): IEnumerator<TOut>;
    /**
     * 进行数据序列的排序操作
     *
    */
    function OrderBy<T>(source: T[], key: (e: T) => number): IEnumerator<T>;
    function OrderByDescending<T>(source: T[], key: (e: T) => number): IEnumerator<T>;
    function Take<T>(source: T[], n: number): IEnumerator<T>;
    function Skip<T>(source: T[], n: number): IEnumerator<T>;
    function TakeWhile<T>(source: T[], predicate: (e: T) => boolean): IEnumerator<T>;
    function Where<T>(source: T[], predicate: (e: T) => boolean): IEnumerator<T>;
    function SkipWhile<T>(source: T[], predicate: (e: T) => boolean): IEnumerator<T>;
    function All<T>(source: T[], predicate: (e: T) => boolean): boolean;
    function Any<T>(source: T[], predicate: (e: T) => boolean): boolean;
    /**
     * Implements a ``group by`` operation by binary tree data structure.
    */
    function GroupBy<T, TKey>(source: T[], getKey: (e: T) => TKey, compares: (a: TKey, b: TKey) => number): IEnumerator<Group<TKey, T>>;
    function AllKeys<T>(sequence: T[]): string[];
}
/**
 * Provides a set of static (Shared in Visual Basic) methods for querying
 * objects that implement ``System.Collections.Generic.IEnumerable<T>``.
 *
 * (这个枚举器类型是构建出一个Linq查询表达式所必须的基础类型，这是一个静态的集合，不会发生元素的动态添加或者删除)
*/
declare class IEnumerator<T> extends LINQIterator<T> {
    /**
     * 获取序列的元素类型
    */
    readonly ElementType: TypeScript.Reflection.TypeInfo;
    /**
     * Get the element value at a given index position
     * of this data sequence.
     *
     * @param index index value should be an integer value.
    */
    ElementAt(index?: string | number): T;
    /**
     * 可以从一个数组或者枚举器构建出一个Linq序列
     *
     * @param source The enumerator data source, this constructor will perform
     *       a sequence copy action on this given data source sequence at here.
    */
    constructor(source: T[] | IEnumerator<T>);
    /**
     * 在明确类型信息的情况下进行强制类型转换
    */
    ctype<U>(): IEnumerator<U>;
    private static getArray;
    indexOf(x: T): number;
    /**
     * Get the first element in this sequence
    */
    readonly First: T;
    /**
     * Get the last element in this sequence
    */
    readonly Last: T;
    /**
     * If the sequence length is zero, then returns the default value.
    */
    FirstOrDefault(Default?: T): T;
    /**
     * 两个序列求总和
    */
    Union<U, K, V>(another: IEnumerator<U> | U[], tKey: (x: T) => K, uKey: (x: U) => K, compare: (a: K, b: K) => number, project?: (x: T, y: U) => V): IEnumerator<V>;
    /**
     * 如果在another序列之中找不到对应的对象，则当前序列会和一个空对象合并
     * 如果another序列之中有多余的元素，即该元素在当前序列之中找不到的元素，会被扔弃
     *
     * @param project 如果这个参数被忽略掉了的话，将会直接进行属性的合并
    */
    Join<U, K, V>(another: IEnumerator<U> | U[], tKey: (x: T) => K, uKey: (x: U) => K, compare: (a: K, b: K) => number, project?: (x: T, y: U) => V): IEnumerator<V>;
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
    Select<TOut>(selector: (o: T, i: number) => TOut): IEnumerator<TOut>;
    /**
     * Groups the elements of a sequence according to a key selector function.
     * The keys are compared by using a comparer and each group's elements
     * are projected by using a specified function.
     *
     * @param compares 注意，javascript在进行中文字符串的比较的时候存在bug，如果当key的类型是字符串的时候，
     *                 在这里需要将key转换为数值进行比较，遇到中文字符串可能会出现bug
    */
    GroupBy<TKey>(keySelector: (o: T) => TKey, compares?: (a: TKey, b: TKey) => number): IEnumerator<Group<TKey, T>>;
    /**
     * Filters a sequence of values based on a predicate.
     *
     * @param predicate A test condition function.
     *
     * @returns Sub sequence of the current sequence with all
     *     element test pass by the ``predicate`` function.
    */
    Where(predicate: (e: T) => boolean): IEnumerator<T>;
    Which(predicate: (e: T) => boolean, first?: boolean): number | IEnumerator<number>;
    /**
     * Get the min value in current sequence.
     * (求取这个序列集合的最小元素，使用这个函数要求序列之中的元素都必须能够被转换为数值)
    */
    Min(project?: (e: T) => number): T;
    /**
     * Get the max value in current sequence.
     * (求取这个序列集合的最大元素，使用这个函数要求序列之中的元素都必须能够被转换为数值)
    */
    Max(project?: (e: T) => number): T;
    /**
     * 求取这个序列集合的平均值，使用这个函数要求序列之中的元素都必须能够被转换为数值
    */
    Average(project?: (e: T) => number): number;
    /**
     * 求取这个序列集合的和，使用这个函数要求序列之中的元素都必须能够被转换为数值
    */
    Sum(project?: (e: T) => number): number;
    /**
     * Sorts the elements of a sequence in ascending order according to a key.
     *
     * @param key A function to extract a key from an element.
     *
     * @returns An ``System.Linq.IOrderedEnumerable<T>`` whose elements are
     *          sorted according to a key.
    */
    OrderBy(key: (e: T) => number): IEnumerator<T>;
    /**
     * Sorts the elements of a sequence in descending order according to a key.
     *
     * @param key A function to extract a key from an element.
     *
     * @returns An ``System.Linq.IOrderedEnumerable<T>`` whose elements are
     *          sorted in descending order according to a key.
    */
    OrderByDescending(key: (e: T) => number): IEnumerator<T>;
    /**
     * Split a sequence by elements count
    */
    Split(size: number): IEnumerator<T[]>;
    /**
     * 取出序列之中的前n个元素
    */
    Take(n: number): IEnumerator<T>;
    /**
     * 跳过序列的前n个元素之后返回序列之中的所有剩余元素
    */
    Skip(n: number): IEnumerator<T>;
    /**
     * 序列元素的位置反转
    */
    Reverse(): IEnumerator<T>;
    /**
     * Returns elements from a sequence as long as a specified condition is true.
     * (与Where类似，只不过这个函数只要遇到第一个不符合条件的，就会立刻终止迭代)
    */
    TakeWhile(predicate: (e: T) => boolean): IEnumerator<T>;
    /**
     * Bypasses elements in a sequence as long as a specified condition is true
     * and then returns the remaining elements.
    */
    SkipWhile(predicate: (e: T) => boolean): IEnumerator<T>;
    /**
     * 判断这个序列之中的所有元素是否都满足特定条件
    */
    All(predicate: (e: T) => boolean): boolean;
    /**
     * 判断这个序列之中的任意一个元素是否满足特定的条件
    */
    Any(predicate?: (e: T) => boolean): boolean;
    /**
     * 对序列中的元素进行去重
    */
    Distinct(key?: (o: T) => string): IEnumerator<T>;
    /**
     * 将序列按照符合条件的元素分成区块
     *
     * @param isDelimiter 一个用于判断当前的元素是否是分割元素的函数
     * @param reserve 是否保留下这个分割对象？默认不保留
    */
    ChunkWith(isDelimiter: (x: T) => boolean, reserve?: boolean): IEnumerator<T[]>;
    /**
     * Performs the specified action for each element in an array.
     *
     * @param callbackfn  A function that accepts up to three arguments. forEach
     * calls the callbackfn function one time for each element in the array.
     *
    */
    ForEach(callbackfn: (x: T, index: number) => void): void;
    /**
     * Contract the data sequence to string
     *
     * @param deli Delimiter string that using for the string.join function
     * @param toString A lambda that describ how to convert the generic type object to string token
     *
     * @returns A contract string.
    */
    JoinBy(deli: string, toString?: (x: T) => string): string;
    /**
     * 如果当前的这个数据序列之中的元素的类型是某一种元素类型的集合，或者该元素
     * 可以描述为另一种类型的元素的集合，则可以通过这个函数来进行降维操作处理。
     *
     * @param project 这个投影函数描述了如何将某一种类型的元素降维至另外一种元素类型的集合。
     * 如果这个函数被忽略掉的话，会尝试强制将当前集合的元素类型转换为目标元素类型的数组集合。
    */
    Unlist<U>(project?: (obj: T) => U[]): IEnumerator<U>;
    /**
     * This function returns a clone copy of the source sequence.
     *
     * @param clone If this parameter is false, then this function will
     * returns the origin array sequence directly.
    */
    ToArray(clone?: boolean): T[];
    /**
     * 将当前的这个不可变的只读序列对象转换为可动态增添删除元素的列表对象
    */
    ToList(): List<T>;
    /**
     * 将当前的这个数据序列对象转换为键值对字典对象，方便用于数据的查找操作
    */
    ToDictionary<V>(keySelector: (x: T) => string, elementSelector?: (x: T) => V): Dictionary<V>;
    /**
     * 将当前的这个数据序列转换为包含有内部位置指针数据的指针对象
    */
    ToPointer(): Pointer<T>;
    /**
     * 将当前的这个序列转换为一个滑窗数据的集合
    */
    SlideWindows(winSize: number, step?: number): IEnumerator<SlideWindow<T>>;
}
/**
 * A collection of html elements with same tag, name or class
*/
declare class DOMEnumerator<T extends HTMLElement> extends IEnumerator<T> {
    /**
     * 这个只读属性只返回第一个元素的tagName
     *
     * @summary 这个属性名与html的节点元素对象的tagName属性名称保持一致
     * 方便进行代码的编写操作
    */
    readonly tagName: string;
    /**
     * 这个只读属性主要是针对于input输入控件组而言的
     *
     * 在假设控件组都是相同类型的情况下, 这个属性直接返回第一个元素的type值
    */
    readonly type: string;
    /**
     * 1. IEnumerator
     * 2. NodeListOf
     * 3. HTMLCollection
    */
    constructor(elements: T[] | IEnumerator<T> | NodeListOf<T> | HTMLCollection);
    /**
     * 这个函数确保所传递进来的集合总是输出一个数组，方便当前的集合对象向其基类型传递数据源
    */
    private static ensureElements;
    /**
     * 使用这个函数进行节点值的设置或者获取
     *
     * 这个函数不传递任何参数则表示获取值
     *
     * @param value 如果需要批量清除节点之中的值的话，需要传递一个空字符串，而非空值
    */
    val(value?: number | string | string[] | IEnumerator<string>): IEnumerator<string>;
    private static setVal;
    private static getVal;
    /**
     * 使用这个函数设置或者获取属性值
     *
     * @param attrName 属性名称
     * @param val + 如果为字符串值，则当前的结合之中的所有的节点的指定属性都将设置为相同的属性值
     *            + 如果为字符串集合或者字符串数组，则会分别设置对应的index的属性值
     *            + 如果是一个函数，则会设置根据该节点所生成的字符串为属性值
     *
     * @returns 函数总是会返回所设置的或者读取得到的属性值的字符串集合
    */
    attr(attrName: string, val?: string | IEnumerator<string> | string[] | ((x: T) => string)): IEnumerator<string>;
    addClass(className: string): DOMEnumerator<T>;
    addEvent(eventName: string, handler: (sender: T, event: Event) => void): void;
    onChange(handler: (sender: T, event: Event) => void): void;
    /**
     * 为当前的html节点集合添加鼠标点击事件处理函数
    */
    onClick(handler: (sender: T, event: MouseEvent) => void): void;
    removeClass(className: string): DOMEnumerator<T>;
    /**
     * 通过设置css之中的display值来将集合之中的所有的节点元素都隐藏掉
    */
    hide(): DOMEnumerator<T>;
    /**
     * 通过设置css之中的display值来将集合之中的所有的节点元素都显示出来
    */
    show(): DOMEnumerator<T>;
    /**
     * 将所选定的节点批量删除
    */
    delete(): void;
}
declare namespace Internal.Handlers.Selector {
    function getElementByIdUnderContext(id: string, context: Window | HTMLElement): any;
    function selectElementsUnderContext(query: DOM.Query, context: Window | HTMLElement): Element;
}
declare namespace Internal.Handlers {
    function hasKey(object: object, key: string): boolean;
    /**
     * 这个函数确保给定的id字符串总是以符号``#``开始的
    */
    function makesureElementIdSelector(str: string): string;
    /**
     * 字符串格式的值意味着对html文档节点的查询
    */
    class stringEval implements IEval<string> {
        private static ensureArguments;
        /**
         * Node selection by css selector
         *
         * @param query 函数会在这里自动的处理转义问题
         * @param context 默认为当前的窗口文档
        */
        static select<T extends HTMLElement>(query: string, context?: Window | HTMLElement): DOMEnumerator<T>;
        doEval(expr: string, type: TypeScript.Reflection.TypeInfo, args: object): any;
        /**
         * 创建新的HTML节点元素
        */
        static createNew(expr: string, args: Arguments, context?: Window): HTMLElement | HTMLTsElement;
        static setAttributes(node: HTMLElement, attrs: object): void;
        /**
         * 添加事件
        */
        private static hookEvt;
    }
}
declare namespace Internal.Handlers {
    /**
     * 在这个字典之中的键名称主要有两大类型:
     *
     * + typeof 类型判断结果
     * + TypeInfo.class 类型名称
    */
    const Shared: {
        /**
         * HTML document query handler
        */
        string: () => stringEval;
        /**
         * Create a linq object
        */
        array: () => arrayEval<{}>;
        NodeListOf: () => DOMCollection<HTMLElement>;
        HTMLCollection: () => DOMCollection<HTMLElement>;
    };
    interface IEval<T> {
        doEval(expr: T, type: TypeScript.Reflection.TypeInfo, args: object): any;
    }
    /**
     * Create a Linq Enumerator
    */
    class arrayEval<V> implements IEval<V[]> {
        doEval(expr: V[], type: TypeScript.Reflection.TypeInfo, args: object): any;
    }
    class DOMCollection<V extends HTMLElement> implements IEval<V[]> {
        doEval(expr: V[], type: TypeScript.Reflection.TypeInfo, args: object): DOMEnumerator<V>;
    }
}
/**
 * 通用数据拓展函数集合
*/
declare module DataExtensions {
    function merge(obj: {}, ...args: {}[]): {};
    function arrayBufferToBase64(buffer: Array<number> | ArrayBuffer): string;
    function toUri(data: DataURI): string;
    /**
     * 将uri之中的base64字符串数据转换为一个byte数据流
    */
    function uriToBlob(uri: string | DataURI): Blob;
    function base64ToBlob(base64: string): ArrayBuffer;
    /**
     * @param fill 进行向量填充的初始值，可能不适用于引用类型，推荐应用于初始的基元类型
    */
    function Dim<T>(len: number, fill?: T): T[];
}
/**
 * 描述了一个键值对集合
*/
declare class MapTuple<K, V> {
    key: K;
    value: V;
    /**
     * 创建一个新的键值对集合
     *
     * @param key 键名称，一般是字符串
     * @param value 目标键名所映射的值
    */
    constructor(key?: K, value?: V);
    valueOf(): V;
    ToArray(): any[];
    toString(): string;
}
/**
 * 描述了一个带有名字属性的变量值
*/
declare class NamedValue<T> {
    name: string;
    value: T;
    /**
     * 获取得到变量值的类型定义信息
    */
    readonly TypeOfValue: TypeScript.Reflection.TypeInfo;
    /**
     * 这个之对象是否是空的？
    */
    readonly IsEmpty: boolean;
    /**
     * @param name 变量值的名字属性
     * @param value 这个变量值
    */
    constructor(name?: string, value?: T);
    valueOf(): T;
    ToArray(): any[];
    toString(): string;
}
/**
 * TypeScript string helpers.
 * (这个模块之中的大部分的字符串处理函数的行为是和VisualBasic之中的字符串函数的行为是相似的)
*/
declare module Strings {
    const x0: number;
    const x9: number;
    const asterisk: number;
    const cr: number;
    const lf: number;
    const a: number;
    const z: number;
    const A: number;
    const Z: number;
    const numericPattern: RegExp;
    /**
     * 判断所给定的字符串文本是否是任意实数的正则表达式模式
    */
    function isNumericPattern(text: string): boolean;
    /**
     * 尝试将任意类型的目标对象转换为数值类型
     *
     * @returns 一个数值
    */
    function as_numeric(obj: any): number;
    /**
     * 因为在js之中没有类型信息，所以如果要取得类型信息必须要有一个目标对象实例
     * 所以在这里，函数会需要一个实例对象来取得类型值
    */
    function AsNumeric<T>(obj: T): (x: T) => number;
    /**
     * 对bytes数值进行格式自动优化显示
     *
     * @param bytes
     *
     * @return 经过自动格式优化过后的大小显示字符串
    */
    function Lanudry(bytes: number): string;
    /**
     * how to escape xml entities in javascript?
     *
     * > https://stackoverflow.com/questions/7918868/how-to-escape-xml-entities-in-javascript
    */
    function escapeXml(unsafe: string): string;
    /**
     * 这个函数会将字符串起始的数字给匹配出来
     * 如果匹配失败会返回零
     *
     * 与VB之中的val函数的行为相似，但是这个函数返回整形数
     *
     * @param text 这个函数并没有执行trim操作，所以如果字符串的起始为空白符的话
     *     会导致解析结果为零
    */
    function parseInt(text: string): number;
    /**
     * Create new string value by repeats a given char n times.
     *
     * @param c A single char
     * @param n n chars
    */
    function New(c: string, n: number): string;
    /**
     * Round the number value or number text in given decimals.
     *
     * @param decimals 默认是保留3位有效数字的
    */
    function round(x: number | string, decimals?: number): number | false;
    /**
     * 判断当前的这个字符是否是一个数字？
     *
     * @param c A single character, length = 1
    */
    function isNumber(c: string): boolean;
    /**
     * 判断当前的这个字符是否是一个字母？
     *
     * @param c A single character, length = 1
    */
    function isAlphabet(c: string): boolean;
    /**
     * 将字符串转换为一个实数
     * 这个函数是直接使用parseFloat函数来工作的，如果不是符合格式的字符串，则可能会返回NaN
    */
    function Val(str: string): number;
    /**
     * 将文本字符串按照newline进行分割
    */
    function lineTokens(text: string): string[];
    /**
     * 如果不存在``tag``分隔符，则返回来的``tuple``里面，``name``是输入的字符串，``value``则是空字符串
     *
     * @param tag 分割name和value的分隔符，默认是一个空白符号
    */
    function GetTagValue(str: string, tag?: string): NamedValue<string>;
    /**
     * 取出大文本之中指定的前n行文本
    */
    function PeekLines(text: string, n: number): string[];
    function LCase(str: string): string;
    function UCase(str: string): string;
    /**
     * Get all regex pattern matches in target text value.
    */
    function getAllMatches(text: string, pattern: string | RegExp): RegExpExecArray[];
    /**
     * Removes the given chars from the begining of the given
     * string and the end of the given string.
     *
     * @param chars A collection of characters that will be trimmed.
     *    (如果这个参数为空值，则会直接使用字符串对象自带的trim函数来完成工作)
     *
     * @returns 这个函数总是会确保返回来的值不是空值，如果输入的字符串参数为空值，则会直接返回零长度的空字符串
    */
    function Trim(str: string, chars?: string | number[]): string;
    function LTrim(str: string, chars?: string | number[]): string;
    function RTrim(str: string, chars?: string | number[]): string;
    /**
     * Determine that the given string is empty string or not?
     * (判断给定的字符串是否是空值？)
     *
     * @param stringAsFactor 假若这个参数为真的话，那么字符串``undefined``或者``NULL``以及``null``也将会被当作为空值处理
    */
    function Empty(str: string, stringAsFactor?: boolean): boolean;
    /**
     * 测试字符串是否是空白集合
     *
     * @param stringAsFactor 如果这个参数为真，则``\t``和``\s``等也会被当作为空白
    */
    function Blank(str: string, stringAsFactor?: boolean): boolean;
    /**
     * Determine that the whole given string is match a given regex pattern.
    */
    function IsPattern(str: string, pattern: RegExp | string): boolean;
    /**
     * Remove duplicate string values from JS array
     *
     * https://stackoverflow.com/questions/9229645/remove-duplicate-values-from-js-array
    */
    function Unique(a: string[]): string[];
    /**
     * Count char numbers appears in the given string value
    */
    function Count(str: string, c: string): number;
    /**
     * 将字符串转换为字符数组
     *
     * @description > https://jsperf.com/convert-string-to-char-code-array/9
     *    经过测试，使用数组push的效率最高
     *
     * @param charCode 返回来的数组是否应该是一组字符的ASCII值而非字符本身？默认是返回字符数组的。
     *
     * @returns A character array, all of the string element in the array
     *      is one character length.
    */
    function ToCharArray(str: string, charCode?: boolean): string[] | number[];
    /**
     * Measure the string length, a null string value or ``undefined``
     * variable will be measured as ZERO length.
    */
    function Len(s: string): number;
    /**
     * 比较两个字符串的大小，可以同于字符串的分组操作
    */
    function CompareTo(s1: string, s2: string): number;
    const sprintf: typeof data.sprintf.doFormat;
    /**
     * @param charsPerLine 每一行文本之中的字符数量的最大值
    */
    function WrappingLines(text: string, charsPerLine?: number, lineTrim?: boolean): string;
}
declare namespace TypeScript.Reflection {
    /**
     * 类似于反射类型
    */
    class TypeInfo {
        /**
         * 直接使用系统内置的``typeof``运算符得到的结果
         *
         * This property have one of the values in these strings:
         * ``string object|string|number|boolean|symbol|undefined|function|array``
        */
        typeOf: string;
        /**
         * 类型class的名称，例如TypeInfo, IEnumerator等。
         * 如果这个属性是空的，则说明是js之中的基础类型
        */
        class: string;
        namespace: string;
        /**
         * class之中的字段域列表
        */
        property: string[];
        /**
         * 函数方法名称列表
        */
        methods: string[];
        /**
         * 是否是js之中的基础类型？
        */
        readonly isPrimitive: boolean;
        /**
         * 是否是一个数组集合对象？
        */
        readonly isArray: boolean;
        /**
         * 是否是一个枚举器集合对象？
        */
        readonly isEnumerator: boolean;
        /**
         * 当前的对象是某种类型的数组集合对象
        */
        isArrayOf(genericType: string): boolean;
        toString(): string;
    }
}
/**
 * JavaScript MD5 1.0.1
 * https://github.com/blueimp/JavaScript-MD5
 *
 * Copyright 2011, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 *
 * Based on
 * A JavaScript implementation of the RSA Data Security, Inc. MD5 Message
 * Digest Algorithm, as defined in RFC 1321.
 * Version 2.2 Copyright (C) Paul Johnston 1999 - 2009
 * Other contributors: Greg Holt, Andrew Kepert, Ydnar, Lostinet
 * Distributed under the BSD License
 * See http://pajhome.org.uk/crypt/md5 for more info.
*/
declare module MD5 {
    /**
     * Add integers, wrapping at 2^32. This uses 16-bit operations internally
     * to work around bugs in some JS interpreters.
    */
    function safe_add(x: number, y: number): number;
    /**
     * Bitwise rotate a 32-bit number to the left.
    */
    function bit_rol(num: number, cnt: number): number;
    function md5_cmn(q: number, a: number, b: number, x: number, s: number, t: number): number;
    function md5_ff(a: number, b: number, c: number, d: number, x: number, s: number, t: number): number;
    function md5_gg(a: number, b: number, c: number, d: number, x: number, s: number, t: number): number;
    function md5_hh(a: number, b: number, c: number, d: number, x: number, s: number, t: number): number;
    function md5_ii(a: number, b: number, c: number, d: number, x: number, s: number, t: number): number;
    /**
     * Calculate the MD5 of an array of little-endian words, and a bit length.
    */
    function binl_md5(x: number[], len: number): number[];
    /**
     * Convert an array of little-endian words to a string
    */
    function binl2rstr(input: number[]): string;
    /**
     * Convert a raw string to an array of little-endian words
     * Characters >255 have their high-byte silently ignored.
    */
    function rstr2binl(input: string): number[];
    /**
     * Calculate the MD5 of a raw string
    */
    function rstr_md5(s: string): string;
    /**
     * Calculate the HMAC-MD5, of a key and some data (raw strings)
    */
    function rstr_hmac_md5(key: string, data: string): string;
    /**
     * Convert a raw string to a hex string
    */
    function rstr2hex(input: string): string;
    /**
     * Encode a string as utf-8
    */
    function str2rstr_utf8(input: string): string;
    /**
     * Take string arguments and return either raw or hex encoded strings
    */
    function raw_md5(s: string): string;
    function hex_md5(s: string): string;
    function raw_hmac_md5(k: string, d: string): string;
    function hex_hmac_md5(k: string, d: string): string;
    /**
     * 利用这个函数来进行字符串的MD5值的计算操作
    */
    function calculate(string: string, key?: string, raw?: string): string;
}
declare namespace Internal {
    /**
     * The internal typescript symbol
    */
    interface TypeScript {
        /**
         * 这个属性控制着这个框架的调试器的输出行为
         *
         * + 如果这个参数为``debug``，则会在浏览器的console上面输出各种和调试相关的信息
         * + 如果这个参数为``production``，则不会在浏览器的console上面输出调试相关的信息，你会得到一个比较干净的console输出窗口
        */
        mode: Modes;
        /**
         * 将一个通过类名称或者标签名称进行选择的节点列表转换为一个节点枚举器
         *
         * ##### 20191030 在这里为了重载的兼容性，nodes参数就从原来的T泛型变更为现在Element基本类型
        */
        <T extends HTMLElement>(nodes: NodeListOf<Element>): DOMEnumerator<T>;
        <T extends HTMLElement & Node & ChildNode>(nodes: NodeListOf<T>): DOMEnumerator<T>;
        /**
         * Extends the properties and methods of the given original html element node.
         *
         * @param element A given html element object.
        */
        <T extends HTMLElement>(element: T): IHTMLElement;
        /**
         * Create a new node or query a node by its id.
         * (创建或者查询节点)
         *
         * @param query + ``#xxxx`` query a node element by id
         *              + ``<xxx>`` create a new node element by a given tag name
         *              + ``<svg:xx>`` create a svg node.
        */
        <T extends HTMLElement>(query: string, args?: TypeScriptArgument): IHTMLElement;
        <T>(array: T[]): IEnumerator<T>;
        /**
         * query meta tag by name attribute value for its content.
         *
         * @param meta The meta tag name, it should be start with a ``@`` symbol.
        */
        (meta: string): string;
        /**
         * Handles event on document load ready.
         *
         * @param ready The handler of the target event.
        */
        (ready: () => void): void;
        /**
         * Query by class name or tag name
         *
         * @param query A selector expression
        */
        select: IquerySelector;
        hook(trigger: Delegate.Func<boolean> | DOM.Events.StatusChanged, handler: Delegate.Sub, tag?: string): any;
        /**
         * 向目标html标签中添加一个表格对象
         *
         * @param div 应该是带有``#``的id查询表达式
        */
        appendTable<T extends {}>(rows: T[] | IEnumerator<T>, div: string, headers?: string[] | IEnumerator<string> | IEnumerator<MapTuple<string, string>> | MapTuple<string, string>[], attrs?: Internal.TypeScriptArgument): void;
        /**
         * 将目标序列转换为一个HTML节点元素
        */
        evalHTML: HtmlDocumentDeserializer;
        /**
         * 动态的导入脚本
         *
         * @param jsURL 需要进行动态导入的脚本的文件链接路径
         * @param onErrorResumeNext 当加载出错的时候，是否继续执行下一个脚本？如果为false，则出错之后会抛出错误停止执行
        */
        imports(jsURL: string | string[], callback?: () => void, onErrorResumeNext?: boolean, echo?: boolean): void;
        /**
         * 将函数注入给定id编号的iframe之中
         *
         * @param iframe ``#xxx``编号查询表达式
         * @param fun 目标函数，请注意，这个函数应该是尽量不引用依赖其他对象的
        */
        inject(iframe: string, fun: (Delegate.Func<any> | string)[] | string | Delegate.Func<any>): void;
        /**
         * 动态加载脚本
         *
         * @param script 脚本的文本内容
         * @param lzw_decompress 目标脚本内容是否是lzw压缩过后的内容，如果是的话，则这个函数会进行lzw解压缩
        */
        eval(script: string, lzw_decompress?: boolean, callback?: () => void): void;
        /**
         * 从当前的html页面之中选择一个指定的节点，然后将节点内的文本以json格式进行解析
         *
         * @param id HTML元素的id，可以同时兼容编号和带``#``的编号
        */
        loadJSON(id: string): any;
        /**
         * @param id HTML元素的id，可以同时兼容编号和带``#``的编号
         * @param htmlText 主要是针对``<pre>``标签之中的VB.NET代码
        */
        text(id: string, htmlText?: boolean): string;
        /**
         * 这个函数主要是应用于``<input>``, ``<textarea>``以及``<select>``标签对象
         * 的value属性值的读取操作
         *
         * 但是如果set_value参数不是空的，则会设置参数到目标控件之上
         *
         * @param id 目标``<input>``标签对象的``id``编号
         *
         * @returns 对于checkbox类型的input而言，逻辑值是以字符串的形式返回
        */
        value(id: string, set_value?: string, strict?: boolean): any;
        typeof<T extends object>(any: T): TypeScript.Reflection.TypeInfo;
        clone<T>(obj: T): T;
        /**
         * Get unix timestamp of current time
        */
        unixtimestamp(): number;
        /**
         * isNullOrUndefined
        */
        isNullOrEmpty(obj: any): boolean;
        /**
         * 判断目标集合是否为空
        */
        isNullOrEmpty<T>(list: T[] | IEnumerator<T>): boolean;
        /**
         * Linq函数链的起始
        */
        from<T>(seq: T[]): IEnumerator<T>;
        /**
         * 请注意：这个函数只会接受来自后端的json返回，如果不是json格式，则可能会解析出错
         *
         * 请尽量使用upload方法进行文件的上传
         *
         * @param url 目标数据源，这个参数也支持meta标签的查询语法
        */
        post<T>(url: string, data: object | FormData, callback?: ((response: IMsg<T>) => void), options?: {
            sendContentType?: boolean;
        }): void;
        /**
         * 请注意：这个函数只会接受来自后端的json返回，如果不是json格式，则可能会解析出错
         *
         * @param url 目标数据源，这个参数也支持meta标签查询语法
        */
        get<T>(url: string, callback?: ((response: IMsg<T>) => void)): void;
        /**
         * GET a text file on your web server.
        */
        getText(url: string, callback: (text: string) => void, options?: {
            nullForNotFound: boolean;
        }): void;
        /**
         * File upload helper
         *
         * @param url 目标数据源，这个参数也支持meta标签查询语法
        */
        upload<T>(url: string, file: File, callback?: ((response: IMsg<T>) => void)): void;
        /**
         * Get the url location of current window page.
         * (获取当前的页面的URL字符串解析模型，这个只读属性可以接受一个变量名参数来获取得到对应的GET参数值)
        */
        readonly location: IURL;
        /**
         * 解析一个给定的URL字符串
        */
        parseURL(url: string): TypeScript.URL;
        /**
         * Url solver of the meta reference value.
        */
        url(reference: string, currentFrame?: boolean): string;
        /**
         * 从当前页面跳转到给定的链接页面
         *
         * @param url 链接，也支持meta查询表达式，如果是以``#``起始的文档节点id表达式，则会在文档内跳转到目标节点位置
         * @param currentFrame 如果当前页面为iframe的话，则只跳转iframe的显示，当这个参数为真的话；
         *      如果这个参数为false，则从父页面进行跳转
        */
        goto(url: string, opt?: GotoOptions): Delegate.Sub;
        /**
         * 针对csv数据序列的操作帮助对象
        */
        csv: IcsvHelperApi;
        /**
         * 将目标字符串解释为一个逻辑值
        */
        parseBool(text: string): boolean;
        /**
         * 解析的结果为``filename.ext``的完整文件名格式
         *
         * @param path Full name
        */
        parseFileName(path: string): string;
        /**
         * 得到不带有拓展名的文件名部分的字符串
         *
         * @param path Full name
        */
        baseName(path: string): string;
        /**
         * 得到不带小数点的文件拓展名字符串
         *
         * @param path Full name
        */
        extensionName(path: string): string;
        /**
         * 注意：这个函数是大小写无关的
         *
         * @param path 文件路径字符串
         * @param ext 不带有小数点的文件拓展名字符串
        */
        withExtensionName(path: string, ext: string): boolean;
        doubleRange(x: number[] | IEnumerator<number>): data.NumericRange;
    }
}
declare namespace TypeScript.URLPatterns {
    const hostNamePattern: RegExp;
    /**
     * Regexp pattern for data uri string
    */
    const uriPattern: RegExp;
    /**
     * Regexp pattern for web browser url string
    */
    const urlPattern: RegExp;
    function isFromSameOrigin(url: string): boolean;
    /**
     * 判断目标文本是否可能是一个url字符串
    */
    function isAPossibleUrlPattern(text: string, pattern?: RegExp): boolean;
    /**
     * 将URL查询字符串解析为字典对象，所传递的查询字符串应该是查询参数部分，即问号之后的部分，而非完整的url
     *
     * @param queryString URL查询参数
     * @param lowerName 是否将所有的参数名称转换为小写形式？
     *
     * @returns 键值对形式的字典对象
    */
    function parseQueryString(queryString: string, lowerName?: boolean): object;
}
declare namespace Internal {
    /**
     * 程序的堆栈追踪信息
     *
     * 这个对象是调用堆栈``StackFrame``片段对象的序列集合
    */
    class StackTrace extends IEnumerator<StackFrame> {
        constructor(frames: IEnumerator<StackFrame> | StackFrame[]);
        /**
         * 导出当前的程序运行位置的调用堆栈信息
        */
        static Dump(): StackTrace;
        /**
         * 获取函数调用者的名称的帮助函数
        */
        static GetCallerMember(): StackFrame;
        toString(): string;
    }
}
declare namespace TypeScript.Reflection {
    /**
     * 获取某一个对象的类型信息
    */
    function $typeof<T>(obj: T): TypeInfo;
    /**
     * 获取object对象上所定义的所有的函数
    */
    function GetObjectMethods<T>(obj: T): string[];
    /**
     * 获取得到类型名称
    */
    function getClass(obj: any): string;
    function getClassInternal(obj: any, isArray: boolean, isObject: boolean, isNull: boolean): string;
    function getObjectClassName(obj: object, isnull: boolean): string;
    function getElementType(array: Array<any>): string;
}
/**
 * 键值对映射哈希表
 *
 * ```
 * IEnumerator<MapTuple<string, V>>
 * ```
*/
declare class Dictionary<V> extends IEnumerator<MapTuple<string, V>> {
    private maps;
    /**
     * 返回一个被复制的当前的map对象
    */
    readonly Object: object;
    /**
     * 如果键名称是空值的话，那么这个函数会自动使用caller的函数名称作为键名进行值的获取
     *
     * https://stackoverflow.com/questions/280389/how-do-you-find-out-the-caller-function-in-javascript
     *
     * @param key 键名或者序列的索引号
    */
    Item(key?: string | number): V;
    /**
     * 获取这个字典对象之中的所有的键名
    */
    readonly Keys: IEnumerator<string>;
    /**
     * 获取这个字典对象之中的所有的键值
    */
    readonly Values: IEnumerator<V>;
    /**
     * 将目标对象转换为一个类型约束的映射序列集合
    */
    constructor(maps?: object | MapTuple<string, V>[] | IEnumerator<MapTuple<string, V>>);
    static FromMaps<V>(maps: MapTuple<string, V>[] | IEnumerator<MapTuple<string, V>>): Dictionary<V>;
    static FromNamedValues<V>(values: NamedValue<V>[] | IEnumerator<NamedValue<V>>): Dictionary<V>;
    static MapSequence<V>(maps: object | MapTuple<string, V>[] | IEnumerator<MapTuple<string, V>>): IEnumerator<MapTuple<string, V>>;
    /**
     * 将目标对象转换为一个类型约束的映射序列集合
    */
    static ObjectMaps<V>(maps: object | MapTuple<string, V>[] | IEnumerator<MapTuple<string, V>>): MapTuple<string, V>[];
    /**
     * 查看这个字典集合之中是否存在所给定的键名
    */
    ContainsKey(key: string): boolean;
    /**
     * 向这个字典对象之中添加一个键值对，请注意，如果key已经存在这个字典对象中了，这个函数会自动覆盖掉key所对应的原来的值
    */
    Add(key: string, value: V): Dictionary<V>;
    /**
     * 删除一个给定键名所指定的键值对
    */
    Delete(key: string): Dictionary<V>;
}
declare namespace TypeScript {
    /**
     * URL组成字符串解析模块
    */
    class URL {
        /**
         * 域名
        */
        origin: string;
        port: number;
        /**
         * 页面的路径
         *
         * 这是一个绝对路径来的
        */
        path: string;
        /**
         * URL查询参数
        */
        readonly query: NamedValue<string>[];
        /**
         * 未经过解析的查询参数的原始字符串
        */
        queryRawString: string;
        /**
         * 不带拓展名的文件名称
        */
        fileName: string;
        /**
         * 在URL字符串之中``#``符号后面的所有字符串都是hash值
        */
        hash: string;
        /**
         * 网络协议名称
        */
        protocol: string;
        private queryArguments;
        /**
         * 在这里解析一个URL字符串
        */
        constructor(url: string);
        getArgument(queryName: string, caseSensitive?: boolean, Default?: string): string;
        /**
         * 将URL之中的query部分解析为字典对象
        */
        static UrlQuery(args: string): object;
        /**
         * 跳转到url之中的hash编号的文档位置处
         *
         * @param hash ``#xxx``文档节点编号表达式
        */
        static JumpToHash(hash: string): void;
        /**
         * Set url hash without url jump in document
        */
        static SetHash(hash: string): void;
        /**
         * 获取得到当前的url
        */
        static WindowLocation(): URL;
        toString(): string;
        static Refresh(url: string): string;
        /**
         * 获取所给定的URL之中的host名称字符串，如果解析失败会返回空值
        */
        static getHostName(url: string): string;
        /**
         * 将目标文本之中的所有的url字符串匹配出来
        */
        static ParseAllUrlStrings(text: string): string[];
        /**
         * 判断所给定的目标字符串是否是一个base64编码的data uri字符串
        */
        static IsWellFormedUriString(uri: string): boolean;
    }
}
declare namespace TypeScript {
    /**
     * String helpers for the file path string.
    */
    module PathHelper {
        /**
         * 只保留文件名（已经去除了文件夹路径以及文件名最后的拓展名部分）
        */
        function basename(fileName: string): string;
        function extensionName(fileName: string): string;
        /**
         * 函数返回文件名或者文件夹的名称
        */
        function fileName(path: string): string;
    }
}
/**
 * 这个枚举选项的值会影响框架之中的调试器的终端输出行为
*/
declare enum Modes {
    /**
     * Framework debug level
     * (这个等级下会输出所有信息)
    */
    debug = 0,
    /**
     * development level
     * (这个等级下会输出警告信息)
    */
    development = 10,
    /**
     * production level
     * (只会输出错误信息，默认等级)
    */
    production = 200
}
/**
 * HTML文档操作帮助函数
*/
declare namespace DOM {
    /**
     * 判断当前的页面是否显示在一个iframe之中
     *
     * https://stackoverflow.com/questions/326069/how-to-identify-if-a-webpage-is-being-loaded-inside-an-iframe-or-directly-into-t
    */
    function inIframe(): boolean;
    /**
     * File download helper
     *
     * @param name The file save name for download operation
     * @param uri The file object to download
    */
    function download(name: string, uri: string | DataURI, isUrl?: boolean): void;
    /**
     * 尝试获取当前的浏览器的大小
    */
    function clientSize(): number[];
    /**
     * 向指定id编号的div添加select标签的组件
     *
     * @param containerID 这个编号不带有``#``前缀，这个容器可以是一个空白的div或者目标``<select>``标签对象的编号，
     *                    如果目标容器是一个``<select>``标签的时候，则要求selectName和className都必须要是空值
     * @param items 这个数组应该是一个``[title => value]``的键值对列表
    */
    function AddSelectOptions(items: MapTuple<string, string>[], containerID: string, selectName?: string, className?: string): void;
    /**
     * @param headers 表格之中所显示的表头列表，也可以通过这个参数来对表格之中
     *   所需要进行显示的列进行筛选以及显示控制：
     *    + 如果这个参数为默认的空值，则说明显示所有的列数据
     *    + 如果这个参数不为空值，则会显示这个参数所指定的列出来
     *    + 可以通过``map [propertyName => display title]``来控制表头的标题输出
    */
    function CreateHTMLTableNode<T extends {}>(rows: T[] | IEnumerator<T>, headers?: string[] | IEnumerator<string> | IEnumerator<MapTuple<string, string>> | MapTuple<string, string>[], attrs?: Internal.TypeScriptArgument): HTMLTableElement;
    /**
     * 向给定编号的div对象之中添加一个表格对象
     *
     * @param headers 表头
     * @param div 新生成的table将会被添加在这个div之中，应该是一个带有``#``符号的节点id查询表达式
     * @param attrs ``<table>``的属性值，包括id，class等
    */
    function AddHTMLTable<T extends {}>(rows: T[] | IEnumerator<T>, div: string, headers?: string[] | IEnumerator<string> | IEnumerator<MapTuple<string, string>> | MapTuple<string, string>[], attrs?: Internal.TypeScriptArgument): void;
}
declare namespace TypeScript {
    /**
     * Console logging helper
    */
    abstract class logging {
        private constructor();
        /**
         * 应用程序的开发模式：只会输出框架的警告信息
        */
        static readonly outputWarning: boolean;
        /**
         * 框架开发调试模式：会输出所有的调试信息到终端之上
        */
        static readonly outputEverything: boolean;
        /**
         * 生产模式：只会输出错误信息
        */
        static readonly outputError: boolean;
        static warning(msg: any): void;
        /**
         * 使用这个函数显示object的时候，将不会发生样式的变化
        */
        static log(obj: any, color?: string | ConsoleColors): void;
        static table<T extends {}>(objects: T[] | string | IEnumerator<T>): void;
        static runGroup(title: string, program: Delegate.Action): void;
    }
    enum ConsoleColors {
        /**
         * do not set the colors
        */
        NA = -1,
        /**
         * The color black.
        */
        Black = 0,
        /**
         * The color blue.
        */
        Blue = 9,
        /**
         * The color cyan (blue - green).
        */
        Cyan = 11,
        /**
         * The color dark blue.
        */
        DarkBlue = 1,
        /**
         * The color dark cyan(dark blue - green).
        */
        DarkCyan = 3,
        /**
         * The color dark gray.
        */
        DarkGray = 8,
        /**
         * The color dark green.
        */
        DarkGreen = 2,
        /**
         * The color dark magenta(dark purplish - red).
        */
        DarkMagenta = 5,
        /**
         * The color dark red.
        */
        DarkRed = 4,
        /**
         * The color dark yellow(ochre).
        */
        DarkYellow = 6,
        /**
         * The color gray.
        */
        Gray = 7,
        /**
         * The color green.
        */
        Green = 10,
        /**
         * The color magenta(purplish - red).
        */
        Magenta = 13,
        /**
         * The color red.
        */
        Red = 12,
        /**
         * The color white.
        */
        White = 15,
        /**
         * The color yellow.
        */
        Yellow = 14
    }
}
declare namespace DOM {
    module InputValueGetter {
        /**
         * Query meta tag content value by name
         *
         * @param allowQueryParent 当当前的文档之中不存在目标meta标签的时候，
         *    如果当前文档为iframe文档，则是否允许继续往父节点的文档做查询？
         *    默认为False，即只在当前文档环境之中进行查询操作
         * @param Default 查询失败的时候所返回来的默认值
        */
        function metaValue(name: string, Default?: string, allowQueryParent?: boolean): string;
        /**
         * @param strict 这个参数主要是针对非输入类型的控件的值获取而言的。
         * 如果目标id标记的控件不是输入类型的，则如果处于非严格模式下，
         * 即这个参数为``false``的时候会直接强制读取value属性值
        */
        function getValue(resource: string, strict?: boolean): any;
        function inputValue(input: HTMLInputElement): any;
        /**
         * 这个函数所返回来的值是和checkbox的数量相关的，
         * 1. 如果有多个checkbox，则会返回一个数组
         * 2. 反之如果只有一个checkbox，则只会返回一个逻辑值，用来表示是否选中该选项
        */
        function checkboxInput(input: HTMLInputElement | DOMEnumerator<HTMLInputElement>, singleAsLogical?: boolean): string | boolean | string[];
        /**
         * 获取被选中的选项的值的列表
        */
        function selectOptionValues(input: HTMLSelectElement): any;
        /**
         * return array containing references to selected option elements
        */
        function getSelectedOptions(sel: HTMLSelectElement | DOMEnumerator<HTMLInputElement>): string | false | string[] | HTMLOptionElement[];
        function largeText(text: HTMLTextAreaElement): any;
    }
}
declare namespace DOM.Events {
    /**
     * Add custom user event.
     *
     * @param trigger This lambda function detects that custom event is triggered or not.
     * @param handler This lambda function contains the processor code of your custom event.
    */
    function Add(trigger: Delegate.Func<boolean> | StatusChanged, handler: Delegate.Action, tag?: string): void;
}
declare namespace data {
    /**
     * A numeric range model.
     * (一个数值范围)
    */
    class NumericRange implements DoubleRange {
        /**
         * 这个数值范围的最大值
        */
        max: number;
        /**
         * 这个数值范围的最小值
        */
        min: number;
        /**
         * ``[min, max]``
        */
        readonly range: number[];
        /**
         * Create a new numeric range object
        */
        constructor(min: number | DoubleRange, max?: number);
        /**
         * The delta length between the max and the min value.
        */
        readonly Length: number;
        /**
         * 从一个数值序列之中创建改数值序列的值范围
         *
         * @param numbers A given numeric data sequence.
        */
        static Create(numbers: number[] | IEnumerator<number>): NumericRange;
        /**
         * 判断目标数值是否在当前的这个数值范围之内
        */
        IsInside(x: number): boolean;
        /**
         * 将一个位于此区间内的实数映射到另外一个区间之中
        */
        ScaleMapping(x: number, range: DoubleRange): number;
        /**
         * Get a numeric sequence within current range with a given step
         *
         * @param step The delta value of the step forward,
         *      by default is 10% of the range length.
        */
        PopulateNumbers(step?: number): number[];
        /**
         * Display the range in format ``[min, max]``
        */
        toString(): string;
    }
}
/**
 * The internal implementation of the ``$ts`` object.
*/
declare namespace Internal {
    const StringEval: Handlers.stringEval;
    function typeGenericElement<T extends HTMLElement>(query: string | HTMLElement, args?: Internal.TypeScriptArgument): T;
    /**
     * 对``$ts``对象的内部实现过程在这里
    */
    function Static<T>(): TypeScript;
    /**
     * 支持对meta标签解析内容的还原
     *
     * @param url 对于meta标签，只会转义字符串最开始的url部分
    */
    function urlSolver(url: string, currentFrame?: boolean): string;
    function queryFunction<T>(handle: object, any: ((() => void) | T | T[]), args: object): any;
}
/**
 * 动态加载脚本文件，然后在完成脚本文件的加载操作之后，执行一个指定的函数操作
 *
 * @param callback 如果这个函数之中存在有HTML文档的操作，则可能会需要将代码放在``$ts(() => {...})``之中，
 *     等待整个html文档加载完毕之后再做程序的执行，才可能会得到正确的执行结果
*/
declare function $imports(jsURL: string | string[], callback?: () => void, onErrorResumeNext?: boolean, echo?: boolean): void;
/**
 * 使用script标签进行脚本文件的加载
 * 因为需要向body添加script标签，所以这个函数会需要等到文档加载完成之后才会被执行
*/
declare function $include(jsURL: string | string[]): void;
/**
 * 计算字符串的MD5值字符串
*/
declare function md5(string: string, key?: string, raw?: string): string;
/**
 * Linq数据流程管线的起始函数
 *
 * ``$ts``函数也可以达到与这个函数相同的效果，但是这个函数更快一些
 *
 * @param source 需要进行数据加工的集合对象
*/
declare function $from<T>(source: T[] | IEnumerator<T>): IEnumerator<T>;
/**
 * 将一个给定的字符串转换为组成该字符串的所有字符的枚举器
*/
declare function CharEnumerator(str: string): IEnumerator<string>;
/**
 * 判断目标对象集合是否是空的？
 *
 * 这个函数也包含有``isNullOrUndefined``函数的判断功能
 *
 * @param array 如果这个数组对象是空值或者未定义，都会被判定为空，如果长度为零，则同样也会被判定为空值
*/
declare function isNullOrEmpty<T>(array: T[] | IEnumerator<T>): boolean;
/**
 * 查看目标变量的对象值是否是空值或者未定义
*/
declare function isNullOrUndefined(obj: any): boolean;
/**
 * HTML/Javascript: how to access JSON data loaded in a script tag.
 *
 * @param id 节点的id值，不带有``#``符号前缀的
*/
declare function LoadJson(id: string): any;
declare function LoadText(id: string): string;
/**
 * Quick Tip: Get URL Parameters with JavaScript
 *
 * > https://www.sitepoint.com/get-url-parameters-with-javascript/
 *
 * @param url get query string from url (optional) or window
*/
declare function getAllUrlParams(url?: string): Dictionary<string>;
/**
 * 调用这个函数会从当前的页面跳转到指定URL的页面
 *
 * 如果当前的这个页面是一个iframe页面，则会通过父页面进行跳转
 *
 * @param url 这个参数支持对meta标签数据的查询操作
 * @param currentFrame 如果这个参数为true，则不会进行父页面的跳转操作
*/
declare function $goto(url: string, currentFrame?: boolean): void;
declare function $download(url: string, rename?: string): void;
/**
 * 这个函数会自动处理多行的情况
*/
declare function base64_decode(stream: string): string;
/**
 * 这个函数什么也不做，主要是用于默认的参数值
*/
declare function DoNothing(): any;
/**
 * 将指定的SVG节点保存为png图片
 *
 * @param svg 需要进行保存为图片的svg节点的对象实例或者对象的节点id值
 * @param name 所保存的文件名
 * @param options 配置参数，直接留空使用默认值就好了
*/
declare function saveSvgAsPng(svg: string | SVGElement, name: string, options?: CanvasHelper.saveSvgAsPng.Options): void;
/**
 * ### Javascript sprintf
 *
 * > http://www.webtoolkit.info/javascript-sprintf.html#.W5sf9FozaM8
 *
 * Several programming languages implement a sprintf function, to output a
 * formatted string. It originated from the C programming language, printf
 * function. Its a string manipulation function.
 *
 * This is limited sprintf Javascript implementation. Function returns a
 * string formatted by the usual printf conventions. See below for more details.
 * You must specify the string and how to format the variables in it.
*/
declare const sprintf: typeof data.sprintf.doFormat;
declare const executeJavaScript: string;
/**
 * 对于这个函数的返回值还需要做类型转换
 *
 * 如果是节点查询或者创建的话，可以使用``asExtends``属性来获取``HTMLTsElememnt``拓展对象
*/
declare const $ts: Internal.TypeScript;
/**
 * 从文档之中查询或者创建一个新的图像标签元素
*/
declare const $image: (query: string | HTMLElement, args?: Internal.TypeScriptArgument) => IHTMLImageElement;
/**
 * 从文档之中查询或者创建一个新的输入标签元素
*/
declare const $input: (query: string | HTMLElement, args?: Internal.TypeScriptArgument) => IHTMLInputElement;
declare const $link: (query: string | HTMLElement, args?: Internal.TypeScriptArgument) => IHTMLLinkElement;
declare const $iframe: (query: string | HTMLElement, args?: Internal.TypeScriptArgument) => HTMLIFrameElement;
interface IProperty {
    get: () => any;
    set: (value: any) => void;
}
declare namespace RequireGlobal {
    /**
     * Linq???????????
     *
     * ``$ts``????????????????????????????
     *
     * @param source ?????????????
    */
    function $from<T>(source: T[] | IEnumerator<T>): IEnumerator<T>;
}
declare namespace TypeExtensions {
    /**
     * Warning message of Nothing
    */
    const objectIsNothing: string;
    /**
     * 字典类型的元素类型名称字符串
    */
    const DictionaryMap: string;
    /**
     * Make sure target input is a number
    */
    function ensureNumeric(x: number | string): number;
    /**
     * 判断目标是否为可以直接转换为字符串的数据类型
    */
    function isPrimitive(any: any): boolean;
    function isElement(obj: any): boolean;
    function isMessageObject(obj: any): boolean;
}
/**
 * 按照某一个键值进行分组的集合对象
*/
declare class Group<TKey, T> extends IEnumerator<T> {
    /**
     * 当前的分组之中的值所都共有的键值对象
    */
    Key: TKey;
    /**
     * Group members, readonly property.
    */
    readonly Group: T[];
    constructor(key: TKey, group: T[]);
    /**
     * 创建一个键值对映射序列，这些映射都具有相同的键名
    */
    ToMaps(): MapTuple<TKey, T>[];
}
/**
 * 表示一个动态列表对象
*/
declare class List<T> extends IEnumerator<T> {
    constructor(src?: T[] | IEnumerator<T>);
    /**
     * 可以使用这个方法进行静态代码的链式添加
    */
    Add(x: T): List<T>;
    /**
     * 批量的添加
    */
    AddRange(x: T[] | IEnumerator<T>): List<T>;
    /**
     * 查找给定的元素在当前的这个列表之中的位置，不存在则返回-1
    */
    IndexOf(x: T): number;
    /**
     * 返回列表之中的第一个元素，然后删除第一个元素，剩余元素整体向前平移一个单位
    */
    Pop(): T;
}
declare class Matrix<T> extends IEnumerator<T[]> {
    readonly rows: number;
    readonly columns: number;
    /**
     * [m, n], m列n行
    */
    constructor(m: number, n: number, fill?: T);
    private static emptyMatrix;
    /**
     * Get or set matrix element value
    */
    M(i: number, j: number, val?: T): T;
    column(i: number, set?: T[] | IEnumerator<T>): T[];
    row(i: number, set?: T[] | IEnumerator<T>): T[];
    toString(): string;
}
/**
 * A data sequence object with a internal index pointer.
*/
declare class Pointer<T> extends IEnumerator<T> {
    /**
     * The index pointer of the current data sequence.
    */
    p: number;
    /**
     * The index pointer is at the end of the data sequence?
    */
    readonly EndRead: boolean;
    /**
     * Get the element value in current location i;
    */
    readonly Current: T;
    /**
     * Get current index element value and then move the pointer
     * to next position.
    */
    readonly Next: T;
    constructor(src: T[] | IEnumerator<T>);
    /**
     * Just move the pointer to the next position and then
     * returns current pointer object.
    */
    MoveNext(): Pointer<T>;
    /**
     * 以当前的位置为基础，得到偏移后的位置的值，但不会改变现有的指针的位置值
    */
    Peek(offset: number): T;
}
/**
 * 序列之中的对某一个区域的滑窗操作结果对象
*/
declare class SlideWindow<T> extends IEnumerator<T> {
    /**
     * 这个滑窗对象在原始的数据序列之中的最左端的位置
    */
    index: number;
    constructor(index: number, src: T[] | IEnumerator<T>);
    /**
     * 创建指定片段长度的滑窗对象
     *
     * @param winSize 滑窗片段的长度
     * @param step 滑窗的步进长度，默认是一个步进
    */
    static Split<T>(src: T[] | IEnumerator<T>, winSize: number, step?: number): IEnumerator<SlideWindow<T>>;
}
/**
 * 序列之中的元素下标的操作方法集合
*/
declare namespace Which {
    /**
     * 查找出所给定的逻辑值集合之中的所有true的下标值
    */
    function Is(booleans: boolean[] | IEnumerator<boolean>): IEnumerator<number>;
    /**
     * 默认的通用类型的比较器对象
    */
    class DefaultCompares<T> {
        /**
         * 一个用于比较通用类型的数值转换器对象
        */
        private as_numeric;
        compares(a: T, b: T): number;
        static default<T>(): (a: T, b: T) => number;
    }
    /**
     * 查找出序列之中最大的元素的序列下标编号
     *
     * @param x 所给定的数据序列
     * @param compare 默认是将x序列之中的元素转换为数值进行大小的比较的
    */
    function Max<T>(x: IEnumerator<T>, compare?: (a: T, b: T) => number): number;
    /**
     * 查找出序列之中最小的元素的序列下标编号
     *
     * @param x 所给定的数据序列
     * @param compare 默认是将x序列之中的元素转换为数值进行大小的比较的
    */
    function Min<T>(x: IEnumerator<T>, compare?: (a: T, b: T) => number): number;
}
declare namespace Enumerable {
    class JoinHelper<T, U> {
        private xset;
        private yset;
        private keysT;
        private keysU;
        constructor(x: T[], y: U[]);
        JoinProject<V>(x: T, y: U): V;
        Union<K, V>(tKey: (x: T) => K, uKey: (x: U) => K, compare: (a: K, b: K) => number, project?: (x: T, y: U) => V): IEnumerator<V>;
        private buildUtree;
        LeftJoin<K, V>(tKey: (x: T) => K, uKey: (x: U) => K, compare: (a: K, b: K) => number, project?: (x: T, y: U) => V): IEnumerator<V>;
    }
}
declare namespace DOM {
    class Query {
        type: QueryTypes;
        singleNode: boolean;
        /**
         * Name of the return value is the trimmed expression
        */
        expression: string;
        /**
         * + ``#`` by id
         * + ``.`` by class
         * + ``!`` by name
         * + ``&`` SINGLE NODE
         * + ``@`` read meta tag
         * + ``&lt;>`` create new tag
        */
        static parseQuery(expr: string): Query;
        /**
         * by node id
        */
        private static getById;
        /**
         * by class name
        */
        private static getByClass;
        /**
         * by name attribute
        */
        private static getByName;
        /**
         * by tag name
        */
        private static getByTag;
        /**
         * create new node
        */
        private static createElement;
        private static queryMeta;
        private static isSelectorQuery;
        private static parseExpression;
    }
}
declare namespace DOM {
    /**
     * HTML文档节点的查询类型
    */
    enum QueryTypes {
        NoQuery = 0,
        /**
         * 表达式为 #xxx
         * 按照节点的id编号进行查询
         *
         * ``<tag id="xxx">``
        */
        id = 1,
        /**
         * 表达式为 .xxx
         * 按照节点的class名称进行查询
         *
         * ``<tag class="xxx">``
        */
        class = 10,
        name = 100,
        /**
         * 表达式为 xxx
         * 按照节点的名称进行查询
         *
         * ``<xxx ...>``
        */
        tagName = -100,
        /**
         * query meta tag content value by name
         *
         * ``@xxxx``
         *
         * ```html
         * <meta name="user-login" content="xieguigang" />
         * ```
        */
        QueryMeta = 200
    }
}
declare namespace DOM {
    module InputValueSetter {
        /**
         * 设置控件的输入值
         *
         * @param resource name or id
         *
         * @param strict 这个参数主要是针对非输入类型的控件的值获取而言的。
         *   如果目标id标记的控件不是输入类型的，则如果处于非严格模式下，
         *   即这个参数为``false``的时候会直接强制读取value属性值
        */
        function setValue(resource: string, value: string, strict?: boolean): void;
    }
}
declare namespace DOM {
    class node {
        tagName: string;
        id: string;
        classList: string[];
        attrs: NamedValue<string>[];
        static FromNode(htmlNode: HTMLElement): node;
        static tokenList(tokens: DOMTokenList): string[];
        static nameValueMaps(attrs: NamedNodeMap): NamedValue<string>[];
    }
}
declare namespace DOM {
    /**
     * 用于解析XML节点之中的属性值的正则表达式
    */
    const attrs: RegExp;
    /**
     * 将表达式之中的节点名称，以及该节点上面的属性值都解析出来
    */
    function ParseNodeDeclare(expr: string): {
        tag: string;
        attrs: NamedValue<string>[];
    };
}
declare namespace DOM.Animation {
    /**
     * 查看在当前的浏览器中，是否支持css3动画特性
    */
    function isSupportsCSSAnimation(): boolean;
}
declare namespace DOM.CSS {
    interface ICSS {
        selector: string;
        styles: NamedValue<string>[];
    }
    /**
     * 解析选择器所对应的样式配置信息
    */
    function parseStylesheet(content: string): NamedValue<string>[];
}
declare namespace DOM.CSS.Setter {
    function css(node: HTMLElement, style: string): void;
    function setStyle(node: HTMLElement, style: NamedValue<string>[]): void;
}
declare namespace DOM.Events {
    /**
     * Execute a given function when the document is ready.
     * It is called when the DOM is ready which can be prior to images and other external content is loaded.
     *
     * 可以处理多个函数作为事件，也可以通过loadComplete函数参数来指定准备完毕的状态
     * 默认的状态是interactive即只需要加载完DOM既可以开始立即执行函数
     *
     * @param fn A function that without any parameters
     * @param loadComplete + ``interactive``: The document has finished loading. We can now access the DOM elements.
     *                     + ``complete``: The page is fully loaded.
     * @param iframe Event execute on document from target iframe.
     *
    */
    function ready(fn: () => void, loadComplete?: string[], iframe?: HTMLIFrameElement): void;
    /**
     * 向一个给定的HTML元素或者HTML元素的集合之中的对象添加给定的事件
     *
     * @param el HTML节点元素或者节点元素的集合
     * @param type 事件的名称字符串
     * @param fn 对事件名称所指定的事件进行处理的工作函数，这个工作函数应该具备有一个事件对象作为函数参数
    */
    function addEvent(el: any, type: string, fn: (event: Event) => void): void;
}
declare namespace DOM.Events {
    class StatusChanged {
        private predicate;
        private triggerNo;
        private agree;
        readonly changed: boolean;
        constructor(predicate: Delegate.Func<boolean>, triggerNo?: boolean);
    }
}
/**
 * 拓展的html文档节点元素对象
*/
interface IHTMLElement extends HTMLElement, HTMLExtensions {
}
interface HTMLExtensions {
    /**
     * 将当前的这个节点元素转换为拓展封装对象类型
    */
    asExtends: HTMLTsElement;
    /**
     * 任然是当前的这个文档节点对象，只不过是更加方便转换为any类型
    */
    any: any;
    /**
     * 将当前的html文档节点元素之中的显示内容替换为参数所给定的html内容
     *
     * @param html 可以为文档文本字符串内容，也可以是一个节点对象的实例，或者不需要参数的生成器函数
    */
    display(html: string | HTMLElement | HTMLTsElement | (() => HTMLElement)): IHTMLElement;
    /**
     * 将一个或者多个文档节点对象添加至当前的节点之中
     *
     * @returns 这个函数返回当前的文档节点对象实例
    */
    appendElement(...html: (string | HTMLElement | HTMLTsElement | (() => HTMLElement))[]): IHTMLElement;
    /**
     * @param reset If this parameter is true, then it means all of the style that this node have will be clear up.
    */
    css(stylesheet: string, reset?: boolean): IHTMLElement;
    /**
     * @param enable set this parameter to false to make user can not interact with current element.
    */
    interactive(enable: boolean): any;
    /**
     * 显示当前的节点元素
    */
    show(): IHTMLElement;
    /**
     * 将当前的节点元素从当前的文档之中隐藏掉
    */
    hide(): IHTMLElement;
    addClass(name: string): IHTMLElement;
    removeClass(name: string): IHTMLElement;
    /**
     * 当class列表中指定的class名称出现或者消失的时候将会触发给定的action调用
    */
    onClassChanged(className: string, action: Delegate.Sub, includesRemoves?: boolean): void;
    /**
     * Set/Delete attribute value of current html element node.
     *
     * @param name HTMLelement attribute name
     * @param value The attribute value to be set, if this parameter is value nothing,
     *     then it means delete the target attribute from the given html node element.
     *
     * @returns This function will returns the source html element node after
     *          the node attribute operation.
    */
    attr(name: string, value: string): IHTMLElement;
    /**
     * 清除当前的这个html文档节点元素之中的所有内容
    */
    clear(): IHTMLElement;
    /**
     * type casting from this base type
    */
    CType<T extends HTMLElement>(): T;
    asImage: IHTMLImageElement;
    asInput: IHTMLInputElement;
    selects<T extends HTMLElement>(cssSelector: string): DOMEnumerator<T>;
}
/**
 * 带有拓展元素的图像标签对象
*/
interface IHTMLImageElement extends HTMLImageElement, HTMLExtensions {
}
/**
 * 带有拓展元素的输入框标签对象
*/
interface IHTMLInputElement extends HTMLInputElement, HTMLExtensions {
}
/**
 * 带有拓展元素的链接标签对象
*/
interface IHTMLLinkElement extends HTMLAnchorElement, HTMLExtensions {
}
/**
 * TypeScript脚本之中的HTML节点元素的类型代理接口
*/
declare class HTMLTsElement {
    private node;
    /**
     * 可以从这里获取得到原生的``HTMLElement``对象用于操作
    */
    readonly HTMLElement: HTMLElement;
    constructor(node: HTMLElement | HTMLTsElement);
    /**
     * 这个拓展函数总是会将节点中的原来的内容清空，然后显示html函数参数
     * 所给定的内容
     *
     * @param html 当这个参数为一个无参数的函数的时候，主要是用于生成一个比较复杂的文档节点而使用的;
     *    如果为字符串文本类型，则是直接将文本当作为HTML代码赋值给当前的这个节点对象的innerHTML属性;
    */
    display(html: string | number | boolean | HTMLElement | HTMLTsElement | (() => HTMLElement)): HTMLTsElement;
    /**
     * Clear all of the contents in current html element node.
    */
    clear(): HTMLTsElement;
    text(innerText: string): HTMLTsElement;
    addClass(className: string): HTMLTsElement;
    removeClass(className: string): HTMLTsElement;
    /**
     * 在当前的HTML文档节点之中添加一个新的文档节点
    */
    append(node: HTMLElement | HTMLTsElement | (() => HTMLElement)): HTMLTsElement;
    /**
     * 将css的display属性值设置为block用来显示当前的节点
    */
    show(): HTMLTsElement;
    /**
     * 将css的display属性值设置为none来隐藏当前的节点
    */
    hide(): HTMLTsElement;
}
/**
 * 在这里对原生的html节点进行拓展
*/
declare namespace TypeExtensions {
    /**
     * 在原生节点模式之下对输入的给定的节点对象添加拓展方法
     *
     * 向HTML节点对象的原型定义之中拓展新的方法和成员属性
     * 这个函数的输出在ts之中可能用不到，主要是应用于js脚本
     * 编程之中
     *
     * @param node 当查询失败的时候是空值
    */
    function Extends(node: HTMLElement): HTMLElement;
}
declare namespace TypeScript.Data {
    /**
     * 这个对象可以自动的将调用者的函数名称作为键名进行对应的键值的读取操作
    */
    class MetaReader {
        /**
         * 字典对象
         *
         * > 在这里不使用Dictionary对象是因为该对象为一个强类型约束对象
        */
        private readonly meta;
        constructor(meta: object);
        /**
         * Read meta object value by call name
         *
         * > https://stackoverflow.com/questions/280389/how-do-you-find-out-the-caller-function-in-javascript
        */
        GetValue(key?: string): any;
    }
}
declare namespace TsLinq {
    class PriorityQueue<T> extends IEnumerator<QueueItem<T>> {
        /**
         * 队列元素
        */
        readonly Q: QueueItem<T>[];
        constructor();
        /**
         *
        */
        enqueue(obj: T): void;
        extract(i: number): QueueItem<T>;
        dequeue(): QueueItem<T>;
    }
    class QueueItem<T> {
        value: T;
        below: QueueItem<T>;
        above: QueueItem<T>;
        constructor(x: T);
        toString(): string;
    }
}
/**
 * Binary tree implements
*/
declare namespace algorithm.BTree {
    /**
     * 用于进行数据分组所需要的最基础的二叉树数据结构
     *
     * ``{key => value}``
    */
    class binaryTree<T, V> {
        /**
         * 根节点，根节点的key值可能会对二叉树的构建造成很大的影响
        */
        root: node<T, V>;
        /**
         * 这个函数指针描述了如何对两个``key``之间进行比较
         *
         * 返回结果值：
         *
         * + ``等于0`` 表示二者相等
         * + ``大于0`` 表示a大于b
         * + ``小于0`` 表示a小于b
        */
        compares: (a: T, b: T) => number;
        /**
         * 构建一个二叉树对象
         *
         * @param comparer 这个函数指针描述了如何进行两个对象之间的比较操作，如果这个函数参数使用默认值的话
         *                 则只能够针对最基本的数值，逻辑变量进行操作
        */
        constructor(comparer?: (a: T, b: T) => number);
        /**
         * 向这个二叉树对象之中添加一个子节点
        */
        add(term: T, value?: V): void;
        /**
         * 根据key值查找一个节点，然后获取该节点之中与key所对应的值
         *
         * @returns 如果这个函数返回空值，则表示可能未找到目标子节点
        */
        find(term: T): V;
        /**
         * 将这个二叉树对象转换为一个节点的数组
        */
        ToArray(): node<T, V>[];
        /**
         * 将这个二叉树对象转换为一个Linq查询表达式所需要的枚举器类型
        */
        AsEnumerable(): IEnumerator<node<T, V>>;
    }
}
declare namespace algorithm.BTree {
    /**
     * data extension module for binary tree nodes data sequence
    */
    module binaryTreeExtensions {
        /**
         * Convert a binary tree object as a node array.
        */
        function populateNodes<T, V>(tree: node<T, V>): node<T, V>[];
    }
}
declare namespace algorithm.BTree {
    /**
     * A binary tree node.
    */
    class node<T, V> {
        key: T;
        value: V;
        left: node<T, V>;
        right: node<T, V>;
        constructor(key: T, value?: V, left?: node<T, V>, right?: node<T, V>);
        toString(): string;
    }
}
declare namespace TypeScript.ColorManager {
    function toColorObject(c: string): IW3color;
    function colorObject(rgb: any, a: any, h: any, s: any): any;
    function getColorArr(x: "names" | "hexs"): string[];
    function w3SetColorsByAttribute(): void;
}
declare namespace TypeScript.ColorManager {
    interface Irgb {
        r: number;
        g: number;
        b: number;
    }
    interface Icmyk {
        c: number;
        m: number;
        y: number;
        k: number;
    }
    interface Ihsl {
        h: number;
        s: number;
        l: number;
    }
    class IW3color {
        red: number;
        blue: number;
        green: number;
        hue: number;
        sat: number;
        opacity: number;
        whiteness: number;
        lightness: number;
        blackness: number;
        cyan: number;
        magenta: number;
        yellow: number;
        black: number;
        ncol: string;
        valid: boolean;
    }
}
declare namespace TypeScript.ColorManager {
    function hslToRgb(hue: any, sat: any, light: any): {
        r: any;
        g: any;
        b: any;
    };
    function hueToRgb(t1: any, t2: any, hue: any): any;
    function hwbToRgb(hue: any, white: any, black: any): {
        r: any;
        g: any;
        b: any;
    };
    function cmykToRgb(c: any, m: any, y: any, k: any): {
        r: any;
        g: any;
        b: any;
    };
    function rgbToHsl(r: any, g: any, b: any): {
        h: any;
        s: any;
        l: any;
    };
    function rgbToHwb(r: number, g: number, b: number): {
        h: any;
        w: any;
        b: any;
    };
    function rgbToCmyk(r: number, g: number, b: number): {
        c: any;
        m: any;
        y: any;
        k: any;
    };
}
declare namespace TypeScript.ColorManager {
    function toHex(n: any): any;
    function cl(x: any): void;
    function w3trim(x: any): any;
    function isHex(x: any): boolean;
}
declare namespace TypeScript.ColorManager {
    /**
     * w3color.js ver.1.18 by w3schools.com (Do not remove this line)
    */
    class w3color implements IW3color {
        red: number;
        blue: number;
        green: number;
        hue: number;
        sat: number;
        opacity: number;
        whiteness: number;
        lightness: number;
        blackness: number;
        cyan: number;
        magenta: number;
        yellow: number;
        black: number;
        ncol: string;
        valid: boolean;
        static readonly emptyObject: IW3color;
        constructor(color?: string | IW3color, elmnt?: HTMLElement);
        toRgbString(): string;
        toRgbaString(): string;
        toHwbString(): string;
        toHwbStringDecimal(): string;
        toHwbaString(): string;
        toHslString(): string;
        toHslStringDecimal(): string;
        toHslaString(): string;
        toCmykString(): string;
        toCmykStringDecimal(): string;
        toNcolString(): string;
        toNcolStringDecimal(): string;
        toNcolaString(): string;
        toName(): string;
        toHexString(): string;
        toRgb(): {
            r: number;
            g: number;
            b: number;
            a: number;
        };
        toHsl(): {
            h: number;
            s: number;
            l: number;
            a: number;
        };
        toHwb(): {
            h: number;
            w: number;
            b: number;
            a: number;
        };
        toCmyk(): {
            c: number;
            m: number;
            y: number;
            k: number;
            a: number;
        };
        toNcol(): {
            ncol: string;
            w: number;
            b: number;
            a: number;
        };
        isDark(n: any): boolean;
        saturate(n: any): void;
        desaturate(n: any): void;
        lighter(n: any): void;
        darker(n: any): void;
        private attachValues;
    }
}
/**
 * How to Encode and Decode Strings with Base64 in JavaScript
 *
 * https://gist.github.com/ncerminara/11257943
 *
 * In base64 encoding, the character set is ``[A-Z, a-z, 0-9, and + /]``.
 * If the rest length is less than 4, the string is padded with ``=``
 * characters.
 *
 * (符号``=``只是用来进行字符串的长度填充使用的，因为base64字符串的长度应该总是4的倍数)
*/
declare module Base64 {
    /**
     * 简单的检测一下所给定的字符串是否是有效的base64字符串
    */
    function isValidBase64String(text: string): boolean;
    /**
     * 将任意文本编码为base64字符串
    */
    function encode(text: string): string;
    /**
     * 将base64字符串解码为普通的文本字符串
    */
    function decode(base64: string): string;
    /**
     * 将文本转换为utf8编码的文本字符串
    */
    function utf8_encode(text: string): string;
    /**
     * 将utf8编码的文本转换为原来的文本
    */
    function utf8_decode(text: string): string;
}
/**
 * 可能对unicode的支持不是很好，推荐只用来压缩ASCII字符串
*/
declare module LZW {
    /**
     * LZW-compress a string
    */
    function encode(s: string): string;
    /**
     * Decompress an LZW-encoded string
    */
    function decode(s: string): string;
}
declare namespace Levenshtein {
    interface IScoreFunc {
        insert(c: string | number): number;
        delete(c: string | number): number;
        substitute(s: string | number, t: string | number): number;
    }
    function DistanceMatrix(source: string, target: string, score?: IScoreFunc): number[][];
    function ComputeDistance(source: string, target: string, score?: IScoreFunc): number;
}
declare class StringBuilder {
    private buffer;
    private newLine;
    /**
     * 返回得到当前的缓冲区的字符串数据长度大小
    */
    readonly Length: number;
    /**
     * @param newLine 换行符的文本，默认为纯文本格式，也可以指定为html格式的换行符``<br />``
    */
    constructor(str?: string | StringBuilder, newLine?: string);
    /**
     * 向当前的缓冲之中添加目标文本
    */
    Append(text: string): StringBuilder;
    /**
     * 向当前的缓冲之中添加目标文本病在最末尾添加一个指定的换行符
    */
    AppendLine(text?: string): StringBuilder;
    toString(): string;
}
declare namespace Internal {
    class BackgroundWorker {
        static $workers: {};
        static readonly hasWorkerFeature: boolean;
        /**
         * 加载所给定的脚本，并创建一个后台线程
         *
         * 可以在脚本中使用格式为``{$name}``的占位符进行标注
         * 然后通过args参数来对这些占位符进行替换，从而达到参数传递的目的
         *
         * @param script 脚本的url或者文本内容，这个主要是为了解决跨域创建后台线程的问题
         * @param args 向脚本模板之中进行赋值操作的变量列表，这个参数应该是一个键值对对象
        */
        static RunWorker(script: string, onMessage: Delegate.Sub, args?: {}): void;
        private static registerWorker;
        private static buildWorker;
        /**
         * How to create a Web Worker from a string
         *
         * > https://stackoverflow.com/questions/10343913/how-to-create-a-web-worker-from-a-string/10372280#10372280
        */
        private static fetchWorker;
        static Stop(script: string): void;
    }
}
/**
 * Web应用程序路由器模块
 *
 * 通过这个路由器模块管理制定的Web应用程序模块的运行或者休眠
*/
declare module Router {
    /**
     * meta标签中的app值
    */
    const appName: string;
    function isCaseSensitive(): boolean;
    /**
     * 设置路由器对URL的解析是否是大小写不敏感模式，也可以在这里函数中设置参数为false，来切换为大小写敏感模式
     *
     * @param option 通过这个参数来设置是否为大小写不敏感模式？
     *
    */
    function CaseInsensitive(option?: boolean): void;
    /**
     * @param module 默认的模块是``/``，即如果服务器为php服务器的话，则默认为index.php
    */
    function AddAppHandler(app: Bootstrap, module?: string): void;
    interface IAppInfo {
        module: string;
        appName: string;
        className: string;
        status: string;
        hookUnload: string;
    }
    function getAppSummary(app: Bootstrap, module?: string): IAppInfo;
    /**
     * 从这个函数开始执行整个Web应用程序
    */
    function RunApp(module?: string): void;
    function queryKey(argName: string): (link: string) => string;
    function moduleName(): (link: string) => string;
    /**
     * 父容器页面注册视图容器对象
    */
    function register(appId?: string, hashKey?: string | ((link: string) => string), frameRegister?: boolean): void;
    /**
     * 当前的堆栈环境是否是最顶层的堆栈？
    */
    function IsTopWindowStack(): boolean;
    /**
     * 因为link之中可能存在查询参数，所以必须要在web服务器上面测试
    */
    function goto(link: string, appId: string, hashKey: (link: string) => string, stack?: Window): void;
}
/**
 * 实现这个类需要重写下面的方法实现：
 *
 * + ``protected abstract init(): void;``
 * + ``public abstract get appName(): string``
 *
 * > ``appName``默认规则是php.net的路由规则，也可以将appName写在
 * > 页面的meta标签的content中，meta标签的name名称应该为``app``
 *
 * 可以选择性的重写下面的事件处理器
 *
 * + ``protected OnDocumentReady(): void``
 * + ``protected OnWindowLoad(): void``
 * + ``protected OnWindowUnload(): string``
 * + ``protected OnHashChanged(hash: string): void``
 *
 * 也可以重写下面的事件来获取当前的app的名称
 *
 * + ``protected getCurrentAppPage(): string``
*/
declare abstract class Bootstrap {
    protected status: string;
    /**
     * 是否阻止用户关闭当前页面
    */
    protected hookUnload: string;
    abstract readonly appName: string;
    /**
     * 这个函数默认是取出url query之中的app参数字符串作为应用名称
     *
     * @returns 如果没有定义app参数，则默认是返回``/``作为名称
    */
    protected readonly currentAppPage: string;
    readonly appStatus: string;
    readonly appHookMsg: string;
    constructor();
    private isCurrentAppPath;
    private isCurrentApp;
    Init(): void;
    /**
     * 初始化代码
    */
    protected abstract init(): void;
    /**
     * Event handler on document is ready
    */
    protected OnDocumentReady(): void;
    /**
     * Event handler on Window loaded
    */
    protected OnWindowLoad(): void;
    protected OnWindowUnload(): string;
    unhook(): void;
    /**
     * Event handler on url hash link changed
    */
    protected OnHashChanged(hash: string): void;
    toString(): string;
}
/**
 * 通用的JavaScript闭包函数指针抽象接口集合
 * 这些接口之间可以相互重载
*/
declare namespace Delegate {
    /**
     * 带有一个参数的子程序
    */
    interface Sub {
        (arg: any): void;
    }
    /**
     * 带有两个参数的子程序
    */
    interface Sub {
        (arg1: any, arg2: any): void;
    }
    interface Sub {
        (arg1: any, arg2: any, arg3: any): void;
    }
    interface Sub {
        (arg1: any, arg2: any, arg3: any, arg4: any): void;
    }
    interface Sub {
        (arg1: any, arg2: any, arg3: any, arg4: any, arg5: any): void;
    }
    /**
     * 不带任何参数的子程序
    */
    interface Action {
        (): void;
    }
    /**
     * 不带参数的函数指针
    */
    interface Func<V> {
        <V>(): V;
    }
    /**
     * 带有一个函数参数的函数指针
    */
    interface Func<V> {
        <T, V>(arg: T): V;
    }
    interface Func<V> {
        <T1, T2, V>(arg1: T1, arg2: T2): V;
    }
    interface Func<V> {
        <T1, T2, T3, V>(arg1: T1, arg2: T2, arg3: T3): V;
    }
    interface Func<V> {
        <T1, T2, T3, T4, V>(arg1: T1, arg2: T2, arg3: T3, arg4: T4): V;
    }
    interface Func<V> {
        <T1, T2, T3, T4, T5, V>(arg1: T1, arg2: T2, arg3: T3, arg4: T4, arg5: T5): V;
    }
}
declare namespace Framework.Extensions {
    /**
     * 确保所传递进来的参数输出的是一个序列集合对象
    */
    function EnsureCollection<T>(data: T | T[] | IEnumerator<T>, n?: number): IEnumerator<T>;
    /**
     * 确保随传递进来的参数所输出的是一个数组对象
     *
     * @param data 如果这个参数是一个数组，则在这个函数之中会执行复制操作
     * @param n 如果data数据序列长度不足，则会使用null进行补充，n为任何小于data长度的正实数都不会进行补充操作，
     *     相反只会返回前n个元素，如果n是负数，则不进行任何操作
    */
    function EnsureArray<T>(data: T | T[] | IEnumerator<T>, n?: number): T[];
    /**
     * Extends `from` object with members from `to`.
     *
     * > https://stackoverflow.com/questions/122102/what-is-the-most-efficient-way-to-deep-clone-an-object-in-javascript
     *
     * @param to If `to` is null, a deep clone of `from` is returned
    */
    function extend<V>(from: V, to?: V): V;
}
declare namespace TypeScript {
    function gc(): {};
}
declare namespace TypeScript {
    /**
     * https://github.com/natewatson999/js-gc
    */
    module garbageCollect {
        /**
         * try to do garbageCollect by invoke this function
        */
        const handler: Delegate.Func<any>;
    }
}
declare namespace Internal {
    class Arguments {
        /**
         * 发生查询的上下文，默认是当前文档
        */
        context: Window | HTMLElement;
        caseInSensitive: boolean;
        /**
         * 进行meta节点查询失败时候所返回来的默认值
        */
        defaultValue: string;
        /**
         * 对于节点查询和创建，是否采用原生的节点返回值？默认是返回原生的节点，否则会返回``HTMLTsElement``对象
         *
         * + 假若采用原生的节点返回值，则会在该节点对象的prototype之中添加拓展函数
         * + 假若采用``HTMLTsElement``模型，则会返回一个经过包裹的``HTMLElement``节点对象
        */
        nativeModel: boolean;
        private static readonly ArgumentNames;
        /**
         * 在创建新的节点的时候，会有一个属性值的赋值过程，
         * 该赋值过程会需要使用这个函数来过滤Arguments的属性值，否则该赋值过程会将Arguments
         * 里面的属性名也进行赋值，可能会造成bug
        */
        static nameFilter(args: object): string[];
        static Default(): Arguments;
    }
}
declare namespace Internal {
    /**
        在这里主要包含有对$ts静态函数符号的实现的具体代码
     
     
     */
}
declare namespace Internal {
    interface IURL {
        /**
         * 获取得到GET参数
        */
        (arg: string, caseSensitive?: boolean, Default?: string): string;
        /**
         * 在``?``查询前面之前出现的，包含有页面文件名，但是不包含有网址的域名，协议名之类的剩余的字符串构成了页面的路径
        */
        readonly path: string;
        readonly fileName: string;
        /**
         * 在当前的url之中是否包含有查询参数？
        */
        readonly hasQueryArguments: boolean;
        readonly url: TypeScript.URL;
        /**
         * 获取当前的url之中的hash值，这个返回来的哈希标签是默认不带``#``符号前缀的
         *
         * @param arg 如果参数urlHash不为空，则这个参数表示是否进行文档内跳转？
         *    如果为空的话，则表示解析hash字符串的时候是否应该去掉前缀的``#``符号
         *
         * @returns 这个函数不会返回空值或者undefined，只会返回空字符串或者hash标签值
        */
        hash(arg?: hashArgument | boolean, urlhash?: string): string;
    }
    interface HtmlDocumentDeserializer {
        /**
         * 将目标序列转换为一个表格HTML节点元素
        */
        table: <T extends {}>(rows: T[] | IEnumerator<T>, headers?: string[] | IEnumerator<string> | IEnumerator<MapTuple<string, string>> | MapTuple<string, string>[], attrs?: Internal.TypeScriptArgument) => HTMLTableElement;
        /**
         * 向指定id编号的div添加select标签的组件
         *
         * @param containerID 这个编号不带有``#``前缀，这个容器可以是一个空白的div或者目标``<select>``标签对象的编号，
         *                    如果目标容器是一个``<select>``标签的时候，则要求selectName和className都必须要是空值
         * @param items 这个数组应该是一个``[title => value]``的键值对列表
        */
        selectOptions: (items: MapTuple<string, string>[], containerID: string, selectName?: string, className?: string) => void;
    }
    interface hashArgument {
        doJump?: boolean;
        trimprefix?: boolean;
    }
    interface GotoOptions {
        currentFrame?: boolean;
        lambda?: boolean;
    }
    interface IquerySelector {
        <T extends HTMLElement>(query: string, context?: Window): DOMEnumerator<T>;
        /**
         * query参数应该是节点id查询表达式
         * 主要是应用于获取checkbox或者select的结果值获取
        */
        getSelectedOptions(query: string, context?: Window): DOMEnumerator<HTMLOptionElement>;
        /**
         * 获取得到select控件的选中的选项值，没做选择则返回null
         *
         * @param query id查询表达式，这个函数只支持单选模式的结果，例如select控件以及radio控件
         * @returns 返回被选中的项目的value属性值
        */
        getOption(query: string, context?: Window): string;
        getSelects(id: string): HTMLSelectElement;
    }
    interface IcsvHelperApi {
        (data: string, isTsv?: boolean | ((data: string) => boolean)): csv.dataframe;
        /**
         * 将csv文档文本进行解析，然后反序列化为js对象的集合
        */
        toObjects<T>(data: string): IEnumerator<T>;
        /**
         * 将js的对象序列进行序列化，构建出csv格式的文本文档字符串数据
        */
        toText<T>(data: IEnumerator<T> | T[], outTsv?: boolean): string;
        toUri<T>(data: IEnumerator<T> | T[], outTsv?: boolean): DataURI;
    }
}
declare namespace Internal {
}
declare namespace Internal {
    /**
     * 这个参数对象模型主要是针对创建HTML对象的
    */
    interface TypeScriptArgument {
        /**
         * HTML节点对象的编号（通用属性）
        */
        id?: string;
        /**
         * HTML节点对象的CSS样式字符串（通用属性）
        */
        style?: string | CSSStyleDeclaration;
        /**
         * HTML节点对象的class类型（通用属性）
        */
        class?: string | string[];
        type?: string;
        href?: string;
        text?: string;
        visible?: boolean;
        alt?: string;
        checked?: boolean;
        selected?: boolean;
        /**
         * 应用于``<a>``标签进行文件下载重命名文件所使用的
        */
        download?: string;
        target?: string;
        src?: string;
        width?: string | number;
        height?: string | number;
        /**
         * 进行查询操作的上下文环境，这个主要是针对iframe环境之中的操作的
        */
        context?: Window | HTMLElement | IHTMLElement;
        title?: string;
        name?: string;
        /**
         * HTML的输入控件的预设值
        */
        value?: string | number | boolean;
        for?: string;
        /**
         * 处理HTML节点对象的点击事件，这个属性值应该是一个无参数的函数来的
        */
        onclick?: Delegate.Sub | string;
        onmouseover?: Delegate.Sub | string;
        "data-toggle"?: string;
        "data-target"?: string;
        "aria-hidden"?: boolean;
        usemap?: string;
        shape?: string;
        coords?: string;
    }
}
declare namespace Internal {
    /**
     * 调用堆栈之中的某一个栈片段信息
    */
    class StackFrame {
        caller: string;
        file: string;
        memberName: string;
        line: number;
        column: number;
        toString(): string;
        static Parse(line: string): StackFrame;
        private static getFileName;
    }
}
declare namespace TypeScript {
    /**
     * 性能计数器
    */
    class Benchmark {
        readonly start: number;
        private lastCheck;
        constructor();
        Tick(): CheckPoint;
    }
    /**
     * 单位都是毫秒
    */
    class CheckPoint {
        /**
         * 性能计数器起始的时间，这个时间点在所有的由同一个性能计数器对象所产生的checkpoint之间都是一样的
        */
        start: number;
        /**
         * 创建当前的这个checkpoint的时候的时间戳
        */
        time: number;
        /**
         * 创建这个checkpoint时间点的时候与上一次创建checkpoint的时间点之间的长度
         * 性能计数主要是查看这个属性值
        */
        sinceLastCheck: number;
        sinceFromStart: number;
        /**
         * 获取从``time``到当前时间所流逝的毫秒计数
        */
        readonly elapsedMilisecond: number;
    }
}
declare module Cookies {
    function setCookie(name: string, value: string, exdays?: number): void;
    /**
     * Cookie 不存在，函数会返回空字符串
    */
    function getCookie(cookiename: string): string;
    /**
     * 将cookie设置为过期，进行cookie的删除操作
    */
    function delCookie(name: string): void;
}
declare namespace TypeScript.LocalDb {
    interface IUseDBDatabase {
        (db: IDBDatabase): void;
    }
    class Open {
        dbName: string;
        private using;
        version: number;
        private db;
        constructor(dbName: string, using: IUseDBDatabase, version?: number);
        private processDbRequest;
    }
}
declare namespace CanvasHelper {
    /**
     * Uses canvas.measureText to compute and return the width of the given text of given font in pixels.
     *
     * @param {String} text The text to be rendered.
     * @param {String} font The css font descriptor that text is to be rendered with (e.g. "bold 14px verdana").
     *
     * @see https://stackoverflow.com/questions/118241/calculate-text-width-with-javascript/21015393#21015393
     *
     */
    function getTextWidth(text: string, font: string): number;
    /**
     * found this trick at http://talideon.com/weblog/2005/02/detecting-broken-images-js.cfm
    */
    function imageOk(img: HTMLImageElement): boolean;
    /**
     * @param size [width, height]
    */
    function createCanvas(size: [number, number], id: string, title: string, display?: string): HTMLCanvasElement;
    function supportsText(ctx: CanvasRenderingContext2D): boolean;
    class fontSize {
        point?: number;
        pixel?: number;
        em?: number;
        percent?: number;
        readonly sizes: fontSize[];
        toString(): string;
        static css(size: fontSize): string;
    }
    class CSSFont {
        fontName: string;
        size: fontSize;
        apply(node: HTMLElement): void;
        static applyCSS(node: HTMLElement, font: CSSFont): void;
    }
}
declare namespace CanvasHelper.saveSvgAsPng {
    const xlink: string;
    function isElement(obj: any): boolean;
    function requireDomNode(el: any): any;
    /**
     * 判断所给定的url指向的资源是否是来自于外部域的资源？
    */
    function isExternal(url: string): boolean;
    function inlineImages(el: SVGSVGElement, callback: Delegate.Action): void;
    /**
     * 获取得到width或者height的值
    */
    function getDimension(el: SVGSVGElement, clone: SVGSVGElement, dim: string): number;
    function reEncode(data: string): string;
}
declare namespace CanvasHelper.saveSvgAsPng {
    const xmlns: string;
    /**
     * ##### 2018-10-12 XMl标签必须要一开始就出现，否则会出现错误
     *
     * error on line 2 at column 14: XML declaration allowed only at the start of the document
    */
    const doctype: string;
    /**
     * https://github.com/exupero/saveSvgAsPng
    */
    class Encoder {
        static prepareSvg(el: SVGSVGElement, options?: Options, cb?: (html: string | HTMLImageElement, width: number, height: number) => void): void;
        private static doInlineImages;
        static svgAsDataUri(el: any, options: any, cb?: (uri: string) => void): void;
        /**
         * 将svg转换为base64 data uri
        */
        private static convertToPng;
        static svgAsPngUri(el: any, options: Options, cb: (uri: string) => void): void;
        static saveSvg(el: any, name: any, options: any): void;
        /**
         * 将指定的SVG节点保存为png图片
         *
         * @param svg 需要进行保存为图片的svg节点的对象实例或者对象的节点id值
         * @param name 所保存的文件名
         * @param options 配置参数，直接留空使用默认值就好了
        */
        static saveSvgAsPng(svg: string | SVGElement, name: string, options?: Options): void;
    }
}
declare namespace CanvasHelper.saveSvgAsPng {
    class Options {
        selectorRemap: (selectorText: string) => string;
        modifyStyle: (cssText: string) => string;
        encoderType: string;
        encoderOptions: number;
        backgroundColor: string;
        canvg: (canvas: HTMLCanvasElement, src: HTMLImageElement) => void;
        scale: number;
        responsive: boolean;
        width: number;
        height: number;
        left: number;
        top: number;
        static Default(): Options;
    }
    class styles {
        static doStyles(el: SVGSVGElement, options: Options, cssLoadedCallback: (css: string) => void): void;
        private static processCssRules;
        private static processFontQueue;
        private static getFontMimeTypeFromUrl;
        private static warnFontNotSupport;
    }
}
/**
 * 包含有一个表示类型的mime_type属性以及存储数据的data属性
 * 用于生成一个data uri字符串
*/
interface DataURI {
    mime_type: string;
    /**
     * base64 string or array buffer blob
    */
    data: string | ArrayBuffer;
}
/**
 * 前端与后台之间的get/post的消息通信格式的简单接口抽象
*/
interface IMsg<T> {
    /**
     * 错误代码，一般使用零表示没有错误
    */
    code: number;
    /**
     * 消息的内容
     * 当code不等于零的时候，表示发生错误，则这个时候的错误消息将会以字符串的形式返回
    */
    info: string | T;
    url: string;
}
declare namespace HttpHelpers {
    /**
     * Javascript动态加载帮助函数
    */
    class Imports {
        /**
         * 发生加载错误的脚本，例如404，脚本文件不存在等错误
        */
        private errors;
        private jsURL;
        private i;
        /**
         * 当脚本执行的时候抛出异常的时候是否继续执行下去？
        */
        private onErrorResumeNext;
        private echo;
        /**
         * @param modules javascript脚本文件的路径集合
         * @param onErrorResumeNext On Error Resume Next Or Just Break
        */
        constructor(modules: string | string[], onErrorResumeNext?: boolean, echo?: boolean);
        private nextScript;
        /**
         * 开始进行异步的脚本文件加载操作
         *
         * @param callback 在所有指定的脚本文件都完成了加载操作之后所调用的异步回调函数
        */
        doLoad(callback?: () => void): void;
        /**
         * 完成向服务器的数据请求操作之后
         * 加载代码文本
        */
        private doExec;
        /**
         * @param script 这个函数可以支持base64字符串格式的脚本的动态加载
         * @param context 默认是添加在当前文档窗口环境之中
        */
        static doEval(script: string, callback?: () => void, context?: Window): void;
        /**
         * 得到相对于当前路径而言的目标脚本全路径
        */
        static getFullPath(url: string): string;
    }
}
declare namespace HttpHelpers {
    const contentTypes: {
        form: string;
        /**
         * 请注意：如果是php服务器，则$_POST很有可能不会自动解析json数据，导致$_POST变量为空数组
         * 则这个时候会需要你在php文件之中手动处理一下$_POST变量：
         *
         * ```php
         * $json  = file_get_contents("php://input");
         * $_POST = json_decode($json, true);
         * ```
        */
        json: string;
        text: string;
        /**
         * 传统的表单post格式
        */
        www: string;
    };
    interface httpCallback {
        (response: string, code: number, contentType: string): void;
    }
    function measureContentType(obj: any): string;
    /**
     * 这个函数只会返回200成功代码的响应内容，对于其他的状态代码都会返回null
     * (这个函数是同步方式的)
    */
    function GET(url: string): string;
    /**
     * 使用异步调用的方式进行数据的下载操作
     *
     * @param callback ``callback(http.responseText, http.status)``
    */
    function GetAsyn(url: string, callback: httpCallback): void;
    function POST(url: string, postData: PostData, callback: (response: string, code: number) => void): void;
    /**
     * 使用multipart form类型的数据进行文件数据的上传操作
     *
     * @param url 函数会通过POST方式将文件数据上传到这个url所指定的服务器资源位置
     *
    */
    function UploadFile(url: string, postData: File | Blob | string, fileName: string, callback: (response: string, code: number) => void): void;
    /**
     * @param a 如果这个是一个无参数的函数, 则会求值之后再进行序列化
    */
    function serialize<T extends {}>(a: T | Delegate.Func<T>, nullAsStringFactor?: boolean): string;
    /**
     * 在这个数据包对象之中应该包含有
     *
     * + ``type``属性，用来设置``Content-type``
     * + ``data``属性，可以是``formData``或者一个``object``
    */
    class PostData {
        /**
         * content type
        */
        type: string;
        /**
         * 将要进行POST上传的数据包
        */
        data: FormData | object | string | Blob | File;
        sendContentType: boolean;
        toString(): string;
    }
}
/**
 * http://www.rfc-editor.org/rfc/rfc4180.txt
*/
declare namespace csv {
    /**
     * Common Format and MIME Type for Comma-Separated Values (CSV) Files
    */
    const contentType: string;
    /**
     * ``csv``文件模型
    */
    class dataframe extends IEnumerator<csv.row> {
        /**
         * Csv文件的第一行作为header
        */
        readonly headers: IEnumerator<string>;
        /**
         * 获取除了第一行作为``header``数据的剩余的所有的行数据
        */
        readonly contents: IEnumerator<row>;
        /**
         * 从行序列之中构建出一个csv对象模型
        */
        constructor(rows: row[] | IEnumerator<row>);
        /**
         * 获取指定列名称的所有的行的列数据
         *
         * @param name csv文件的列名称，第一行之中的文本数据的内容
         *
         * @returns 该使用名称所指定的列的所有的内容字符串的枚举序列对象
        */
        Column(name: string): IEnumerator<string>;
        /**
         * 向当前的数据框对象之中添加一行数据
        */
        AppendLine(line: row): dataframe;
        /**
         * 向当前的数据框对象之中添加多行数据
        */
        AppendRows(data: IEnumerator<row> | row[]): dataframe;
        /**
         * 将当前的这个数据框对象转换为csv文本内容
        */
        buildDoc(tsvFormat?: boolean): string;
        /**
         * 使用反射操作将csv文档转换为特定类型的对象数据序列
         *
         * @param fieldMaps 这个参数是一个对象，其描述了如何将csv文档之中在js之中
         *     的非法标识符转换为合法的标识符的映射
         * @param activator 这个函数指针描述了如何创建一个新的指定类型的对象的过程，
         *     这个函数指针不可以有参数的传递。
         *
         * @returns 这个函数返回类型约束的对象Linq序列集合
        */
        Objects<T>(fieldMaps?: object, activator?: () => T): IEnumerator<T>;
        private static ensureMapsAll;
        /**
         * 使用ajax将csv文件保存到服务器
         *
         * @param url csv文件数据将会被通过post方法保存到这个url所指定的网络资源上面
         * @param callback ajax异步回调，默认是打印返回结果到终端之上
         *
        */
        save(url: string, fileName?: string, callback?: (response: string) => void): void;
        /**
         * 使用ajax GET加载csv文件数据，不推荐使用这个方法处理大型的csv文件数据
         *
         * @param callback 当这个异步回调为空值的时候，函数使用同步的方式工作，返回csv对象
         *                 如果这个参数不是空值，则以异步的方式工作，此时函数会返回空值
         * @param parseText 如果url返回来的数据之中还包含有其他的信息，则会需要这个参数来进行csv文本数据的解析
        */
        static Load(url: string, callback?: (csv: dataframe) => void, parseText?: (response: string, contentType?: string) => content): dataframe;
        private static isTsv;
        /**
         * 默认是直接加个csv标签将格式设为默认的csv文件
        */
        private static defaultContent;
        /**
         * 将所给定的文本文档内容解析为数据框对象
         *
         * @param tsv 所需要进行解析的文本内容是否为使用``<TAB>``作为分割符的tsv文本文件？
         *   默认不是，即默认使用逗号``,``作为分隔符的csv文本文件。
        */
        static Parse(text: string, tsv?: boolean): dataframe;
        static head(allTextLines: IEnumerator<string>, parse: (line: string) => row): string[][];
    }
    interface content {
        /**
         * 文档的类型为``csv``还是``tsv``
        */
        type: string;
        content: string;
    }
}
declare namespace csv.HTML {
    /**
     * 将数据框对象转换为HTMl格式的表格对象的html代码
     *
     * @param tblClass 所返回来的html表格代码之中的table对象的类型默认是bootstrap类型的，
     * 所以默认可以直接应用bootstrap的样式在这个表格之上
     *
     * @returns 表格的HTML代码
    */
    function toHTMLTable(data: dataframe, tblClass?: string[]): string;
    function createHTMLTable<T extends object>(data: IEnumerator<T>, tblClass?: string[]): string;
}
declare namespace csv {
    /**
     * csv文件之中的一行数据，相当于当前行的列数据的集合
    */
    class row extends IEnumerator<string> {
        /**
         * 当前的这一个行对象的列数据集合
         *
         * 注意，你无法通过直接修改这个数组之中的元素来达到修改这个行之中的值的目的
         * 因为这个属性会返回这个行的数组值的复制对象
        */
        readonly columns: string[];
        /**
         * 这个只读属性仅用于生成csv文件
        */
        readonly rowLine: string;
        constructor(cells: string[] | IEnumerator<string>);
        /**
         * Returns the index of the first occurrence of a value in an array.
         *
         * 函数得到指定的值在本行对象之中的列的编号
         *
         * @param value The value to locate in the array.
         * @param fromIndex The array index at which to begin the search. If ``fromIndex`` is omitted,
         *      the search starts at index 0.
         *
         * @returns 如果这个函数返回-1则表示找不到
        */
        indexOf(value: string, fromIndex?: number): number;
        ProjectObject(headers: string[] | IEnumerator<string>): object;
        private static autoEscape;
        static Parse(line: string): row;
        static ParseTsv(line: string): row;
    }
}
declare namespace csv {
    /**
     * 通过Chars枚举来解析域，分隔符默认为逗号
     * > https://github.com/xieguigang/sciBASIC/blame/701f9d0e6307a779bb4149c57a22a71572f1e40b/Data/DataFrame/IO/csv/Tokenizer.vb#L97
     *
    */
    function CharsParser(s: string, delimiter?: string, quot?: string): string[];
}
declare namespace csv {
    /**
     * 将对象序列转换为``dataframe``对象
     *
     * 这个函数只能够转换object类型的数据，对于基础类型将不保证能够正常工作
     *
     * @param data 因为这个对象序列对象是具有类型约束的，所以可以直接从第一个
     *    元素对象之中得到所有的属性名称作为csv文件头的数据
    */
    function toDataFrame<T extends object>(data: IEnumerator<T> | T[]): dataframe;
    function isTsvFile(content: string): boolean;
}
