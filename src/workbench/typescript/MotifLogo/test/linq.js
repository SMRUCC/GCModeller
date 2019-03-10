var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var data;
(function (data) {
    var sprintf;
    (function (sprintf) {
        /**
         * 对占位符的匹配结果
        */
        var match = /** @class */ (function () {
            function match() {
            }
            match.prototype.toString = function () {
                return JSON.stringify(this);
            };
            return match;
        }());
        sprintf.match = match;
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
        sprintf.placeholder = new RegExp(/(%([%]|(\-)?(\+|\x20)?(0)?(\d+)?(\.(\d)?)?([bcdfosxX])))/g);
        function parseFormat(string, arguments) {
            var stringPosStart = 0;
            var stringPosEnd = 0;
            var matchPosEnd = 0;
            var convCount = 0;
            var match = null;
            var matches = [];
            var strings = [];
            while (match = sprintf.placeholder.exec(string)) {
                stringPosStart = matchPosEnd;
                stringPosEnd = sprintf.placeholder.lastIndex - match[0].length;
                strings[strings.length] = string.substring(stringPosStart, stringPosEnd);
                matchPosEnd = sprintf.placeholder.lastIndex;
                matches[matches.length] = {
                    match: match[0],
                    left: match[3] ? true : false,
                    sign: match[4] || '',
                    pad: match[5] || ' ',
                    min: match[6] || 0,
                    precision: match[8],
                    code: match[9] || '%',
                    negative: parseInt(arguments[convCount]) < 0 ? true : false,
                    argument: String(arguments[convCount])
                };
                if (match[9]) {
                    convCount += 1;
                }
            }
            strings[strings.length] = string.substring(matchPosEnd);
            return {
                matches: matches,
                convCount: convCount,
                strings: strings
            };
        }
        sprintf.parseFormat = parseFormat;
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
        function doFormat(format) {
            var argv = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                argv[_i - 1] = arguments[_i];
            }
            if (typeof arguments == "undefined") {
                return null;
            }
            if (arguments.length < 1) {
                return null;
            }
            if (typeof arguments[0] != "string") {
                return null;
            }
            if (typeof RegExp == "undefined") {
                return null;
            }
            var parsed = sprintf.parseFormat(format, argv);
            var convCount = parsed.convCount;
            if (parsed.matches.length == 0) {
                // 没有格式化参数的占位符，则直接输出原本的字符串
                return format;
            }
            else {
                // console.log(parsed);
            }
            if (argv.length < convCount) {
                // 格式化参数的数量少于占位符的数量，则抛出错误
                throw "Mismatch format argument numbers (" + argv.length + " !== " + convCount + ")!";
            }
            else {
                return sprintf.doSubstitute(parsed.matches, parsed.strings);
            }
        }
        sprintf.doFormat = doFormat;
        /**
         * 进行格式化占位符对格式化参数的字符串替换操作
        */
        function doSubstitute(matches, strings) {
            var i = null;
            var substitution = null;
            var numVal = 0;
            var newString = '';
            for (i = 0; i < matches.length; i++) {
                if (matches[i].code == '%') {
                    substitution = '%';
                }
                else if (matches[i].code == 'b') {
                    matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(2));
                    substitution = sprintf.convert(matches[i], true);
                }
                else if (matches[i].code == 'c') {
                    numVal = Math.abs(parseInt(matches[i].argument));
                    matches[i].argument = String(String.fromCharCode(parseInt(String(numVal))));
                    substitution = sprintf.convert(matches[i], true);
                }
                else if (matches[i].code == 'd') {
                    matches[i].argument = String(Math.abs(parseInt(matches[i].argument)));
                    substitution = sprintf.convert(matches[i]);
                }
                else if (matches[i].code == 'f') {
                    numVal = matches[i].precision ? parseFloat(matches[i].precision) : 6;
                    matches[i].argument = String(Math.abs(parseFloat(matches[i].argument)).toFixed(numVal));
                    substitution = sprintf.convert(matches[i]);
                }
                else if (matches[i].code == 'o') {
                    matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(8));
                    substitution = sprintf.convert(matches[i]);
                }
                else if (matches[i].code == 's') {
                    numVal = matches[i].precision ?
                        parseFloat(matches[i].precision) :
                        matches[i].argument.length;
                    matches[i].argument = matches[i].argument.substring(0, numVal);
                    substitution = sprintf.convert(matches[i], true);
                }
                else if (matches[i].code == 'x') {
                    matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(16));
                    substitution = sprintf.convert(matches[i]);
                }
                else if (matches[i].code == 'X') {
                    matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(16));
                    substitution = sprintf.convert(matches[i]).toUpperCase();
                }
                else {
                    substitution = matches[i].match;
                }
                newString += strings[i];
                newString += substitution;
            }
            return newString + strings[i];
        }
        sprintf.doSubstitute = doSubstitute;
        function convert(match, nosign) {
            if (nosign === void 0) { nosign = false; }
            if (nosign) {
                match.sign = '';
            }
            else {
                match.sign = match.negative ? '-' : match.sign;
            }
            var l = parseFloat(match.min) -
                match.argument.length + 1 -
                match.sign.length;
            var pad = new Array(l < 0 ? 0 : l).join(match.pad);
            if (!match.left) {
                if (match.pad == "0" || nosign) {
                    return match.sign + pad + match.argument;
                }
                else {
                    return pad + match.sign + match.argument;
                }
            }
            else {
                if (match.pad == "0" || nosign) {
                    return match.sign + match.argument + pad.replace(/0/g, ' ');
                }
                else {
                    return match.sign + match.argument + pad;
                }
            }
        }
        sprintf.convert = convert;
    })(sprintf = data.sprintf || (data.sprintf = {}));
})(data || (data = {}));
/**
 * Provides a set of static (Shared in Visual Basic) methods for querying
 * objects that implement ``System.Collections.Generic.IEnumerable<T>``.
 *
 * (这个枚举器类型是构建出一个Linq查询表达式所必须的基础类型，这是一个静态的集合，不会发生元素的动态添加或者删除)
*/
var IEnumerator = /** @class */ (function () {
    //#endregion
    /**
     * 可以从一个数组或者枚举器构建出一个Linq序列
     *
     * @param source The enumerator data source, this constructor will perform
     *       a sequence copy action on this given data source sequence at here.
    */
    function IEnumerator(source) {
        if (!source) {
            this.sequence = [];
        }
        else if (Array.isArray(source)) {
            // 2018-07-31 为了防止外部修改source导致sequence数组被修改
            // 在这里进行数组复制，防止出现这种情况
            this.sequence = source.slice();
        }
        else {
            this.sequence = source.sequence.slice();
        }
    }
    Object.defineProperty(IEnumerator.prototype, "ElementType", {
        //#region "readonly property"
        /**
         * 获取序列的元素类型
        */
        get: function () {
            return TypeInfo.typeof(this.First);
        },
        enumerable: true,
        configurable: true
    });
    ;
    Object.defineProperty(IEnumerator.prototype, "Count", {
        /**
         * The number of elements in the data sequence.
        */
        get: function () {
            return this.sequence.length;
        },
        enumerable: true,
        configurable: true
    });
    ;
    /**
     * Get the element value at a given index position
     * of this data sequence.
     *
     * @param index index value should be an integer value.
    */
    IEnumerator.prototype.ElementAt = function (index) {
        if (index === void 0) { index = null; }
        if (!index) {
            index = 0;
        }
        else if (typeof index == "string") {
            throw "Item index='" + index + "' must be an integer!";
        }
        return this.sequence[index];
    };
    IEnumerator.prototype.indexOf = function (x) {
        return this.sequence.indexOf(x);
    };
    Object.defineProperty(IEnumerator.prototype, "First", {
        /**
         * Get the first element in this sequence
        */
        get: function () {
            return this.sequence[0];
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(IEnumerator.prototype, "Last", {
        /**
         * Get the last element in this sequence
        */
        get: function () {
            return this.sequence[this.Count - 1];
        },
        enumerable: true,
        configurable: true
    });
    /**
     * 两个序列求总和
    */
    IEnumerator.prototype.Union = function (another, tKey, uKey, compare, project) {
        if (project === void 0) { project = null; }
        if (!Array.isArray(another)) {
            another = another.ToArray();
        }
        var join = new Enumerable.JoinHelper(this.sequence, another);
        return join.Union(tKey, uKey, compare, project);
    };
    /**
     * 如果在another序列之中找不到对应的对象，则当前序列会和一个空对象合并
     * 如果another序列之中有多余的元素，即该元素在当前序列之中找不到的元素，会被扔弃
     *
     * @param project 如果这个参数被忽略掉了的话，将会直接进行属性的合并
    */
    IEnumerator.prototype.Join = function (another, tKey, uKey, compare, project) {
        if (project === void 0) { project = null; }
        if (!Array.isArray(another)) {
            another = another.ToArray();
        }
        var join = new Enumerable.JoinHelper(this.sequence, another);
        return join.LeftJoin(tKey, uKey, compare, project);
    };
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
    IEnumerator.prototype.Select = function (selector) {
        return Enumerable.Select(this.sequence, selector);
    };
    /**
     * Groups the elements of a sequence according to a key selector function.
     * The keys are compared by using a comparer and each group's elements
     * are projected by using a specified function.
     *
     * @param compares 注意，javascript在进行中文字符串的比较的时候存在bug，如果当key的类型是字符串的时候，
     *                 在这里需要将key转换为数值进行比较，遇到中文字符串可能会出现bug
    */
    IEnumerator.prototype.GroupBy = function (keySelector, compares) {
        return Enumerable.GroupBy(this.sequence, keySelector, compares);
    };
    /**
     * Filters a sequence of values based on a predicate.
     *
     * @param predicate A test condition function.
     *
     * @returns Sub sequence of the current sequence with all
     *     element test pass by the ``predicate`` function.
    */
    IEnumerator.prototype.Where = function (predicate) {
        return Enumerable.Where(this.sequence, predicate);
    };
    /**
     * Get the min value in current sequence.
     * (求取这个序列集合的最小元素，使用这个函数要求序列之中的元素都必须能够被转换为数值)
    */
    IEnumerator.prototype.Min = function (project) {
        if (project === void 0) { project = function (e) { return DataExtensions.as_numeric(e); }; }
        return Enumerable.OrderBy(this.sequence, project).First;
    };
    /**
     * Get the max value in current sequence.
     * (求取这个序列集合的最大元素，使用这个函数要求序列之中的元素都必须能够被转换为数值)
    */
    IEnumerator.prototype.Max = function (project) {
        if (project === void 0) { project = function (e) { return DataExtensions.as_numeric(e); }; }
        return Enumerable.OrderByDescending(this.sequence, project).First;
    };
    /**
     * 求取这个序列集合的平均值，使用这个函数要求序列之中的元素都必须能够被转换为数值
    */
    IEnumerator.prototype.Average = function (project) {
        if (project === void 0) { project = null; }
        if (this.Count == 0) {
            return 0;
        }
        else {
            return this.Sum(project) / this.sequence.length;
        }
    };
    /**
     * 求取这个序列集合的和，使用这个函数要求序列之中的元素都必须能够被转换为数值
    */
    IEnumerator.prototype.Sum = function (project) {
        if (project === void 0) { project = null; }
        var x = 0;
        if (!project)
            project = function (e) {
                return Number(e);
            };
        for (var i = 0; i < this.sequence.length; i++) {
            x += project(this.sequence[i]);
        }
        return x;
    };
    /**
     * Sorts the elements of a sequence in ascending order according to a key.
     *
     * @param key A function to extract a key from an element.
     *
     * @returns An ``System.Linq.IOrderedEnumerable<T>`` whose elements are
     *          sorted according to a key.
    */
    IEnumerator.prototype.OrderBy = function (key) {
        return Enumerable.OrderBy(this.sequence, key);
    };
    /**
     * Sorts the elements of a sequence in descending order according to a key.
     *
     * @param key A function to extract a key from an element.
     *
     * @returns An ``System.Linq.IOrderedEnumerable<T>`` whose elements are
     *          sorted in descending order according to a key.
    */
    IEnumerator.prototype.OrderByDescending = function (key) {
        return Enumerable.OrderByDescending(this.sequence, key);
    };
    /**
     * 取出序列之中的前n个元素
    */
    IEnumerator.prototype.Take = function (n) {
        return Enumerable.Take(this.sequence, n);
    };
    /**
     * 跳过序列的前n个元素之后返回序列之中的所有剩余元素
    */
    IEnumerator.prototype.Skip = function (n) {
        return Enumerable.Skip(this.sequence, n);
    };
    /**
     * 序列元素的位置反转
    */
    IEnumerator.prototype.Reverse = function () {
        var rseq = this.ToArray().reverse();
        return new IEnumerator(rseq);
    };
    /**
     * Returns elements from a sequence as long as a specified condition is true.
     * (与Where类似，只不过这个函数只要遇到第一个不符合条件的，就会立刻终止迭代)
    */
    IEnumerator.prototype.TakeWhile = function (predicate) {
        return Enumerable.TakeWhile(this.sequence, predicate);
    };
    /**
     * Bypasses elements in a sequence as long as a specified condition is true
     * and then returns the remaining elements.
    */
    IEnumerator.prototype.SkipWhile = function (predicate) {
        return Enumerable.SkipWhile(this.sequence, predicate);
    };
    /**
     * 判断这个序列之中的所有元素是否都满足特定条件
    */
    IEnumerator.prototype.All = function (predicate) {
        return Enumerable.All(this.sequence, predicate);
    };
    /**
     * 判断这个序列之中的任意一个元素是否满足特定的条件
    */
    IEnumerator.prototype.Any = function (predicate) {
        if (predicate === void 0) { predicate = null; }
        if (predicate) {
            return Enumerable.Any(this.sequence, predicate);
        }
        else {
            if (!this.sequence || this.sequence.length == 0) {
                return false;
            }
            else {
                return true;
            }
        }
    };
    /**
     * 对序列中的元素进行去重
    */
    IEnumerator.prototype.Distinct = function (key) {
        if (key === void 0) { key = function (o) { return o.toString(); }; }
        return this
            .GroupBy(key, Strings.CompareTo)
            .Select(function (group) { return group.First; });
    };
    /**
     * 将序列按照符合条件的元素分成区块
     *
     * @param isDelimiter 一个用于判断当前的元素是否是分割元素的函数
     * @param reserve 是否保留下这个分割对象？默认不保留
    */
    IEnumerator.prototype.ChunkWith = function (isDelimiter, reserve) {
        if (reserve === void 0) { reserve = false; }
        var chunks = new List();
        var buffer = [];
        this.sequence.forEach(function (x) {
            if (isDelimiter(x)) {
                chunks.Add(buffer);
                if (reserve) {
                    buffer = [x];
                }
                else {
                    buffer = [];
                }
            }
            else {
                buffer.push(x);
            }
        });
        if (buffer.length > 0) {
            chunks.Add(buffer);
        }
        return chunks;
    };
    /**
     * Performs the specified action for each element in an array.
     *
     * @param callbackfn  A function that accepts up to three arguments. forEach
     * calls the callbackfn function one time for each element in the array.
     *
    */
    IEnumerator.prototype.ForEach = function (callbackfn) {
        this.sequence.forEach(callbackfn);
    };
    /**
     * Contract the data sequence to string
     *
     * @param deli Delimiter string that using for the string.join function
     * @param toString A lambda that describ how to convert the generic type object to string token
     *
     * @returns A contract string.
    */
    IEnumerator.prototype.JoinBy = function (deli, toString) {
        if (toString === void 0) { toString = function (x) {
            if (typeof x === "string") {
                return x;
            }
            else {
                return x.toString();
            }
        }; }
        return this.Select(function (x) { return toString(x); })
            .ToArray()
            .join(deli);
    };
    /**
     * 如果当前的这个数据序列之中的元素的类型是某一种元素类型的集合，或者该元素
     * 可以描述为另一种类型的元素的集合，则可以通过这个函数来进行降维操作处理。
     *
     * @param project 这个投影函数描述了如何将某一种类型的元素降维至另外一种元素类型的集合。
     * 如果这个函数被忽略掉的话，会尝试强制将当前集合的元素类型转换为目标元素类型的数组集合。
    */
    IEnumerator.prototype.Unlist = function (project) {
        if (project === void 0) { project = function (obj) { return obj; }; }
        var list = [];
        this.ForEach(function (a) {
            project(a).forEach(function (x) { return list.push(x); });
        });
        return new IEnumerator(list);
    };
    //#region "conversion"
    /**
     * This function returns a clone copy of the source sequence.
    */
    IEnumerator.prototype.ToArray = function () {
        return this.sequence.slice();
    };
    /**
     * 将当前的这个不可变的只读序列对象转换为可动态增添删除元素的列表对象
    */
    IEnumerator.prototype.ToList = function () {
        return new List(this.sequence);
    };
    /**
     * 将当前的这个数据序列对象转换为键值对字典对象，方便用于数据的查找操作
    */
    IEnumerator.prototype.ToDictionary = function (keySelector, elementSelector) {
        if (elementSelector === void 0) { elementSelector = function (X) {
            return X;
        }; }
        var maps = {};
        this.sequence.forEach(function (x) {
            // 2018-08-11 键名只能够是字符串类型的
            var key = keySelector(x);
            var value = elementSelector(x);
            maps[key] = value;
        });
        return new Dictionary(maps);
    };
    /**
     * 将当前的这个数据序列转换为包含有内部位置指针数据的指针对象
    */
    IEnumerator.prototype.ToPointer = function () {
        return new Pointer(this);
    };
    /**
     * 将当前的这个序列转换为一个滑窗数据的集合
    */
    IEnumerator.prototype.SlideWindows = function (winSize, step) {
        if (step === void 0) { step = 1; }
        return data.SlideWindow.Split(this, winSize, step);
    };
    return IEnumerator;
}());
var Linq;
(function (Linq) {
    var TsQuery;
    (function (TsQuery) {
        TsQuery.handler = {
            /**
             * HTML document query handler
            */
            string: function () { return new TsQuery.stringEval(); },
            /**
             * Create a linq object
            */
            array: function () { return new arrayEval(); }
        };
        /**
         * Create a Linq Enumerator
        */
        var arrayEval = /** @class */ (function () {
            function arrayEval() {
            }
            arrayEval.prototype.doEval = function (expr, type, args) {
                return From(expr);
            };
            return arrayEval;
        }());
        TsQuery.arrayEval = arrayEval;
    })(TsQuery = Linq.TsQuery || (Linq.TsQuery = {}));
})(Linq || (Linq = {}));
/**
 * 通用数据拓展函数集合
*/
var DataExtensions;
(function (DataExtensions) {
    function arrayBufferToBase64(buffer) {
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
    DataExtensions.arrayBufferToBase64 = arrayBufferToBase64;
    function uriToBlob(uri) {
        var byteString = window.atob(uri.split(',')[1]);
        var mimeString = uri.split(',')[0].split(':')[1].split(';')[0];
        var buffer = new ArrayBuffer(byteString.length);
        var intArray = new Uint8Array(buffer);
        for (var i = 0; i < byteString.length; i++) {
            intArray[i] = byteString.charCodeAt(i);
        }
        return new Blob([buffer], { type: mimeString });
    }
    DataExtensions.uriToBlob = uriToBlob;
    function getCook(cookiename) {
        // Get name followed by anything except a semicolon
        var cookie = document.cookie;
        var cookiestring = RegExp("" + cookiename + "[^;]+").exec(cookie);
        var value;
        // Return everything after the equal sign, 
        // or an empty string if the cookie name not found
        if (!!cookiestring) {
            value = cookiestring.toString().replace(/^[^=]+./, "");
        }
        else {
            value = "";
        }
        return decodeURIComponent(value);
    }
    DataExtensions.getCook = getCook;
    /**
     * 将URL查询字符串解析为字典对象，所传递的查询字符串应该是查询参数部分，即问号之后的部分，而非完整的url
     *
     * @param queryString URL查询参数
     * @param lowerName 是否将所有的参数名称转换为小写形式？
     *
     * @returns 键值对形式的字典对象
    */
    function parseQueryString(queryString, lowerName) {
        if (lowerName === void 0) { lowerName = false; }
        // stuff after # is not part of query string, so get rid of it
        // split our query string into its component parts
        var arr = queryString.split('#')[0].split('&');
        // we'll store the parameters here
        var obj = {};
        for (var i = 0; i < arr.length; i++) {
            // separate the keys and the values
            var a = arr[i].split('=');
            // in case params look like: list[]=thing1&list[]=thing2
            var paramNum = undefined;
            var paramName = a[0].replace(/\[\d*\]/, function (v) {
                paramNum = v.slice(1, -1);
                return '';
            });
            // set parameter value (use 'true' if empty)
            var paramValue = typeof (a[1]) === 'undefined' ? "true" : a[1];
            if (lowerName) {
                paramName = paramName.toLowerCase();
            }
            // if parameter name already exists
            if (obj[paramName]) {
                // convert value to array (if still string)
                if (typeof obj[paramName] === 'string') {
                    obj[paramName] = [obj[paramName]];
                }
                if (typeof paramNum === 'undefined') {
                    // if no array index number specified...
                    // put the value on the end of the array
                    obj[paramName].push(paramValue);
                }
                else {
                    // if array index number specified...
                    // put the value at that index number
                    obj[paramName][paramNum] = paramValue;
                }
            }
            else {
                // if param name doesn't exist yet, set it
                obj[paramName] = paramValue;
            }
        }
        return obj;
    }
    DataExtensions.parseQueryString = parseQueryString;
    /**
     * 尝试将任意类型的目标对象转换为数值类型
     *
     * @returns 一个数值
    */
    function as_numeric(obj) {
        return AsNumeric(obj)(obj);
    }
    DataExtensions.as_numeric = as_numeric;
    /**
     * 因为在js之中没有类型信息，所以如果要取得类型信息必须要有一个目标对象实例
     * 所以在这里，函数会需要一个实例对象来取得类型值
    */
    function AsNumeric(obj) {
        if (obj == null || obj == undefined) {
            return null;
        }
        if (typeof obj === 'number') {
            return function (x) { return x; };
        }
        else if (typeof obj === 'boolean') {
            return function (x) {
                if (x == true) {
                    return 1;
                }
                else {
                    return -1;
                }
            };
        }
        else if (typeof obj == 'undefined') {
            return function (x) { return 0; };
        }
        else if (typeof obj == 'string') {
            return function (x) {
                return Strings.Val(x);
            };
        }
        else {
            // 其他的所有情况都转换为零
            return function (x) { return 0; };
        }
    }
    DataExtensions.AsNumeric = AsNumeric;
})(DataExtensions || (DataExtensions = {}));
/**
 * TypeScript string helpers
*/
var Strings;
(function (Strings) {
    Strings.x0 = "0".charCodeAt(0);
    Strings.x9 = "9".charCodeAt(0);
    Strings.numericPattern = /[-]?\d+(\.\d+)?/g;
    /**
     * 判断所给定的字符串文本是否是任意实数的正则表达式模式
    */
    function isNumericPattern(text) {
        return IsPattern(text, Strings.numericPattern);
    }
    Strings.isNumericPattern = isNumericPattern;
    /**
     * @param text A single character
    */
    function isNumber(text) {
        var code = text.charCodeAt(0);
        return code >= Strings.x0 && code <= Strings.x9;
    }
    Strings.isNumber = isNumber;
    /**
     * 将字符串转换为一个实数
    */
    function Val(str) {
        if (str == null || str == '' || str == undefined || str == "undefined") {
            // 将空字符串转换为零
            return 0;
        }
        else if (str == "NA" || str == "NaN") {
            return Number.NaN;
        }
        else if (str == "Inf") {
            return Number.POSITIVE_INFINITY;
        }
        else if (str == "-Inf") {
            return Number.NEGATIVE_INFINITY;
        }
        else {
            return parseFloat(str);
        }
    }
    Strings.Val = Val;
    /**
     * 将文本字符串按照newline进行分割
    */
    function lineTokens(text) {
        return (!text) ? [] : text.trim().split("\n");
    }
    Strings.lineTokens = lineTokens;
    /**
     * 如果不存在``tag``分隔符，则返回来的``tuple``里面，``name``是输入的字符串，``value``则是空字符串
     *
     * @param tag 分割name和value的分隔符，默认是一个空白符号
    */
    function GetTagValue(str, tag) {
        if (tag === void 0) { tag = " "; }
        if (!str) {
            return new NamedValue();
        }
        else {
            return tagValueImpl(str, tag);
        }
    }
    Strings.GetTagValue = GetTagValue;
    function tagValueImpl(str, tag) {
        var i = str.indexOf(tag);
        var tagLen = Len(tag);
        if (i > -1) {
            var name = str.substr(0, i);
            var value = str.substr(i + tagLen);
            return new NamedValue(name, value);
        }
        else {
            return new NamedValue(str, "");
        }
    }
    /**
     * Removes the given chars from the begining of the given
     * string and the end of the given string.
     *
     * @param chars A collection of characters that will be trimmed.
    */
    function Trim(str, chars) {
        if (typeof chars == "string") {
            chars = From(Strings.ToCharArray(chars))
                .Select(function (c) { return c.charCodeAt(0); })
                .ToArray();
        }
        return function (chars) {
            return From(Strings.ToCharArray(str))
                .SkipWhile(function (c) { return chars.indexOf(c.charCodeAt(0)) > -1; })
                .Reverse()
                .SkipWhile(function (c) { return chars.indexOf(c.charCodeAt(0)) > -1; })
                .Reverse()
                .JoinBy("");
        }(chars);
    }
    Strings.Trim = Trim;
    /**
     * Determine that the given string is empty string or not?
     * (判断给定的字符串是否是空值？)
     *
     * @param stringAsFactor 假若这个参数为真的话，那么字符串``undefined``也将会被当作为空值处理
    */
    function Empty(str, stringAsFactor) {
        if (stringAsFactor === void 0) { stringAsFactor = false; }
        if (!str) {
            return true;
        }
        else if (str == undefined) {
            return true;
        }
        else if (str.length == 0) {
            return true;
        }
        else if (stringAsFactor && str.toString() == "undefined") {
            return true;
        }
        else {
            return false;
        }
    }
    Strings.Empty = Empty;
    /**
     * Determine that the whole given string is match a given regex pattern.
    */
    function IsPattern(str, pattern) {
        if (!str) {
            // 字符串是空的，则肯定不满足
            return false;
        }
        var matches = str.match(ensureRegexp(pattern));
        if (isNullOrUndefined(matches)) {
            return false;
        }
        else {
            var match = matches[0];
            var test = match == str;
            return test;
        }
    }
    Strings.IsPattern = IsPattern;
    function ensureRegexp(pattern) {
        if (typeof pattern == "string") {
            return new RegExp(pattern);
        }
        else {
            return pattern;
        }
    }
    /**
     * Remove duplicate string values from JS array
     *
     * https://stackoverflow.com/questions/9229645/remove-duplicate-values-from-js-array
    */
    function uniq(a) {
        var seen = {};
        return a.filter(function (item) {
            return seen.hasOwnProperty(item) ? false : (seen[item] = true);
        });
    }
    Strings.uniq = uniq;
    /**
     * 将字符串转换为字符数组
     *
     * @description > https://jsperf.com/convert-string-to-char-code-array/9
     *    经过测试，使用数组push的效率最高
     *
     * @returns A character array, all of the string element in the array
     *      is one character length.
    */
    function ToCharArray(str) {
        var cc = [];
        var strLen = str.length;
        for (var i = 0; i < strLen; ++i) {
            cc.push(str.charAt(i));
        }
        return cc;
    }
    Strings.ToCharArray = ToCharArray;
    /**
     * Measure the string length, a null string value or ``undefined``
     * variable will be measured as ZERO length.
    */
    function Len(s) {
        if (!s || s == undefined) {
            return 0;
        }
        else {
            return s.length;
        }
    }
    Strings.Len = Len;
    function CompareTo(s1, s2) {
        var l1 = Strings.Len(s1);
        var l2 = Strings.Len(s2);
        var minl = Math.min(l1, l2);
        for (var i = 0; i < minl; i++) {
            var x = s1.charCodeAt(i);
            var y = s2.charCodeAt(i);
            if (x > y) {
                return 1;
            }
            else if (x < y) {
                return -1;
            }
        }
        if (l1 > l2) {
            return 1;
        }
        else if (l1 < l2) {
            return -1;
        }
        else {
            return 0;
        }
    }
    Strings.CompareTo = CompareTo;
    Strings.sprintf = data.sprintf.doFormat;
})(Strings || (Strings = {}));
/**
 * 类似于反射类型
*/
var TypeInfo = /** @class */ (function () {
    function TypeInfo() {
    }
    Object.defineProperty(TypeInfo.prototype, "IsPrimitive", {
        /**
         * 是否是js之中的基础类型？
        */
        get: function () {
            return !this.class;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(TypeInfo.prototype, "IsArray", {
        /**
         * 是否是一个数组集合对象？
        */
        get: function () {
            return this.typeOf == "array";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(TypeInfo.prototype, "IsEnumerator", {
        /**
         * 是否是一个枚举器集合对象？
        */
        get: function () {
            return this.typeOf == "object" && this.class == "IEnumerator";
        },
        enumerable: true,
        configurable: true
    });
    /**
     * 当前的对象是某种类型的数组集合对象
    */
    TypeInfo.prototype.IsArrayOf = function (genericType) {
        return this.IsArray && this.class == genericType;
    };
    /**
     * 获取某一个对象的类型信息
    */
    TypeInfo.typeof = function (obj) {
        var type = typeof obj;
        var isObject = type == "object";
        var isArray = Array.isArray(obj);
        var className = "";
        var isNull = isNullOrUndefined(obj);
        if (isArray) {
            var x = obj[0];
            if ((className = typeof x) == "object") {
                className = x.constructor.name;
            }
            else {
                // do nothing
            }
        }
        else if (isObject) {
            if (isNull) {
                console.warn("Object is nothing! [https://docs.microsoft.com/en-us/dotnet/visual-basic/language-reference/nothing]");
                className = "null";
            }
            else {
                className = obj.constructor.name;
            }
        }
        else {
            className = "";
        }
        var typeInfo = new TypeInfo;
        typeInfo.typeOf = isArray ? "array" : type;
        typeInfo.class = className;
        if (isNull) {
            typeInfo.property = [];
            typeInfo.methods = [];
        }
        else {
            typeInfo.property = isObject ? Object.keys(obj) : [];
            typeInfo.methods = TypeInfo.GetObjectMethods(obj);
        }
        return typeInfo;
    };
    /**
     * 获取object对象上所定义的所有的函数
    */
    TypeInfo.GetObjectMethods = function (obj) {
        var res = [];
        for (var m in obj) {
            if (typeof obj[m] == "function") {
                res.push(m);
            }
        }
        return res;
    };
    TypeInfo.prototype.toString = function () {
        if (this.typeOf == "object") {
            return "<" + this.typeOf + "> " + this.class;
        }
        else {
            return this.typeOf;
        }
    };
    /**
     * 利用一个名称字符串集合创建一个js对象
     *
     * @param names object的属性名称列表
     * @param init 使用这个函数为该属性指定一个初始值
    */
    TypeInfo.EmptyObject = function (names, init) {
        var obj = {};
        if (Array.isArray(names)) {
            names.forEach(function (name) { return obj[name] = init(); });
        }
        else {
            names.ForEach(function (name) { return obj[name] = init(); });
        }
        return obj;
    };
    /**
     * 从键值对集合创建object对象，键名或者名称属性会作为object对象的属性名称
    */
    TypeInfo.CreateObject = function (nameValues) {
        var obj = {};
        var type = TypeInfo.typeof(nameValues);
        if (type.IsArray && type.class == "Map") {
            nameValues.forEach(function (map) { return obj[map.key] = map.value; });
        }
        else if (type.IsArray && type.class == "NamedValue") {
            nameValues.forEach(function (nv) { return obj[nv.name] = nv.value; });
        }
        else if (type.class == "IEnumerator") {
            var seq = nameValues;
            type = seq.ElementType;
            if (type.class == "Map") {
                nameValues
                    .ForEach(function (map) {
                    obj[map.key] = map.value;
                });
            }
            else if (type.class == "NamedValue") {
                nameValues
                    .ForEach(function (nv) {
                    obj[nv.name] = nv.value;
                });
            }
            else {
                console.error(type);
                throw "Unsupport data type: " + type.class;
            }
        }
        else {
            throw "Unsupport data type: " + JSON.stringify(type);
        }
        return obj;
    };
    /**
     * MetaReader对象和字典相似，只不过是没有类型约束，并且为只读集合
    */
    TypeInfo.CreateMetaReader = function (nameValues) {
        return new TsLinq.MetaReader(TypeInfo.CreateObject(nameValues));
    };
    return TypeInfo;
}());
/// <reference path="Data/sprintf.ts" />
/// <reference path="Linq/Collections/Enumerator.ts" />
/// <reference path="Linq/TsQuery/TsQuery.ts" />
/// <reference path="Helpers/Extensions.ts" />
/// <reference path="Helpers/Strings.ts" />
/// <reference path="Type.ts" />
if (typeof String.prototype['startsWith'] != 'function') {
    String.prototype['startsWith'] = function (str) {
        return this.slice(0, str.length) == str;
    };
}
/**
 * 对于这个函数的返回值还需要做类型转换
 *
 * 如果是节点查询或者创建的话，可以使用``asExtends``属性来获取``HTMLTsElememnt``拓展对象
*/
function $ts(any, args) {
    if (args === void 0) { args = null; }
    var type = TypeInfo.typeof(any);
    var typeOf = type.typeOf;
    var handle = Linq.TsQuery.handler;
    var eval = typeOf in handle ? handle[typeOf]() : null;
    if (type.IsArray) {
        // 转化为序列集合对象，相当于from函数
        var creator = eval;
        return creator.doEval(any, type, args);
    }
    else if (type.typeOf == "function") {
        // 当html文档加载完毕之后就会执行传递进来的这个
        // 函数进行初始化
        Linq.DOM.ready(any);
    }
    else {
        // 对html文档之中的节点元素进行查询操作
        // 或者创建新的节点
        return eval.doEval(any, type, args);
    }
}
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
var sprintf = data.sprintf.doFormat;
/**
 * Linq数据流程管线的起始函数
 *
 * @param source 需要进行数据加工的集合对象
*/
function From(source) {
    return new IEnumerator(source);
}
/**
 * 将一个给定的字符串转换为组成该字符串的所有字符的枚举器
*/
function CharEnumerator(str) {
    return new IEnumerator(Strings.ToCharArray(str));
}
/**
 * Query meta tag content value by name
*/
function metaValue(name, Default) {
    if (Default === void 0) { Default = null; }
    var meta = document.querySelector("meta[name~=\"" + name + "\"]");
    var content;
    if (meta) {
        content = meta.getAttribute("content");
        return content ? content : Default;
    }
    else {
        return Default;
    }
}
/**
 * 判断目标对象集合是否是空的？
 *
 * @param array 如果这个数组对象是空值或者未定义，都会被判定为空，如果长度为零，则同样也会被判定为空值
*/
function IsNullOrEmpty(array) {
    if (array == null || array == undefined) {
        return true;
    }
    else if (Array.isArray(array) && array.length == 0) {
        return true;
    }
    else if (array.Count == 0) {
        return true;
    }
    else {
        return false;
    }
}
/**
 * 查看目标变量的对象值是否是空值或者未定义
*/
function isNullOrUndefined(obj) {
    if (obj == null || obj == undefined) {
        return true;
    }
    else {
        return false;
    }
}
/**
 * HTML/Javascript: how to access JSON data loaded in a script tag.
*/
function LoadJson(id) {
    return JSON.parse(LoadText(id));
}
function LoadText(id) {
    return document.getElementById(id).textContent;
}
/**
 * Quick Tip: Get URL Parameters with JavaScript
 *
 * > https://www.sitepoint.com/get-url-parameters-with-javascript/
 *
 * @param url get query string from url (optional) or window
*/
function getAllUrlParams(url) {
    if (url === void 0) { url = window.location.href; }
    if (url.indexOf("?") > -1) {
        // if query string exists
        var queryString = Strings.GetTagValue(url, '?').value;
        var args = DataExtensions.parseQueryString(queryString);
        return new Dictionary(args);
    }
    else {
        return new Dictionary({});
    }
}
/**
 * 调用这个函数会从当前的页面跳转到指定URL的页面
*/
function Goto(url) {
    window.location.href = url;
}
/**
 * 这个函数会自动处理多行的情况
*/
function base64_decode(stream) {
    var data = Strings.lineTokens(stream);
    var base64Str = From(data)
        .Where(function (s) { return s && s.length > 0; })
        .Select(function (s) { return s.trim(); })
        .JoinBy("");
    var text = Base64.decode(base64Str);
    return text;
}
/**
 * 这个函数什么也不做，主要是用于默认的参数值
*/
function DoNothing() {
    return null;
}
var TypeExtensions;
(function (TypeExtensions) {
    function ensureNumeric(x) {
        if (typeof x == "number") {
            return x;
        }
        else {
            return parseFloat(x);
        }
    }
    TypeExtensions.ensureNumeric = ensureNumeric;
})(TypeExtensions || (TypeExtensions = {}));
/**
 * http://www.rfc-editor.org/rfc/rfc4180.txt
*/
var csv;
(function (csv_1) {
    /**
     * Common Format and MIME Type for Comma-Separated Values (CSV) Files
    */
    var contentType = "text/csv";
    /**
     * ``csv``文件模型
    */
    var dataframe = /** @class */ (function (_super) {
        __extends(dataframe, _super);
        function dataframe(rows) {
            return _super.call(this, rows) || this;
        }
        Object.defineProperty(dataframe.prototype, "headers", {
            /**
             * Csv文件的第一行作为header
            */
            get: function () {
                return new IEnumerator(this.sequence[0]);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(dataframe.prototype, "contents", {
            /**
             * 获取除了第一行作为``header``数据的剩余的所有的行数据
            */
            get: function () {
                return this.Skip(1);
            },
            enumerable: true,
            configurable: true
        });
        /**
         * 获取指定列名称的所有的行的列数据
         *
         * @param name csv文件的列名称，第一行之中的文本数据的内容
         *
         * @returns 该使用名称所指定的列的所有的内容字符串的枚举序列对象
        */
        dataframe.prototype.Column = function (name) {
            var index = this.sequence[0].indexOf(name);
            if (index == -1) {
                return new IEnumerator([]);
            }
            else {
                return this.Select(function (r) { return r.ElementAt(index); });
            }
        };
        /**
         * 向当前的数据框对象之中添加一行数据
        */
        dataframe.prototype.AppendLine = function (line) {
            this.sequence.push(line);
            return this;
        };
        /**
         * 向当前的数据框对象之中添加多行数据
        */
        dataframe.prototype.AppendRows = function (data) {
            var _this = this;
            if (Array.isArray(data)) {
                data.forEach(function (r) { return _this.sequence.push(r); });
            }
            else {
                data.ForEach(function (r) { return _this.sequence.push(r); });
            }
            return this;
        };
        /**
         * 将当前的这个数据框对象转换为csv文本内容
        */
        dataframe.prototype.buildDoc = function () {
            return this.Select(function (r) { return r.rowLine; }).JoinBy("\n");
        };
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
        dataframe.prototype.Objects = function (fieldMaps, activator) {
            if (fieldMaps === void 0) { fieldMaps = {}; }
            if (activator === void 0) { activator = function () {
                return {};
            }; }
            var header = dataframe.ensureMapsAll(fieldMaps, this.headers.ToArray());
            var objs = this
                .Skip(1)
                .Select(function (r) {
                var o = activator();
                r.ForEach(function (c, i) {
                    o[header(i)] = c;
                });
                return o;
            });
            return objs;
        };
        dataframe.ensureMapsAll = function (fieldMaps, headers) {
            for (var i = 0; i < headers.length - 1; i++) {
                var column = headers[i];
                if (column in fieldMaps) {
                    // do nothing
                }
                else {
                    // fill gaps
                    fieldMaps[column] = column;
                }
            }
            return function (i) {
                return fieldMaps[headers[i]];
            };
        };
        /**
         * 使用ajax将csv文件保存到服务器
         *
         * @param url csv文件数据将会被通过post方法保存到这个url所指定的网络资源上面
         * @param callback ajax异步回调，默认是打印返回结果到终端之上
         *
        */
        dataframe.prototype.save = function (url, callback) {
            if (callback === void 0) { callback = function (response) {
                console.log(response);
            }; }
            var file = this.buildDoc();
            var data = {
                type: contentType,
                data: file
            };
            HttpHelpers.UploadFile(url, data, callback);
        };
        /**
         * 使用ajax GET加载csv文件数据，不推荐使用这个方法处理大型的csv文件数据
         *
         * @param callback 当这个异步回调为空值的时候，函数使用同步的方式工作，返回csv对象
         *                 如果这个参数不是空值，则以异步的方式工作，此时函数会返回空值
         * @param parseText 如果url返回来的数据之中还包含有其他的信息，则会需要这个参数来进行csv文本数据的解析
        */
        dataframe.Load = function (url, tsv, callback, parseText) {
            if (tsv === void 0) { tsv = false; }
            if (callback === void 0) { callback = null; }
            if (parseText === void 0) { parseText = function (str) { return str; }; }
            if (callback == null || callback == undefined) {
                // 同步
                return dataframe.Parse(parseText(HttpHelpers.GET(url)), tsv);
            }
            else {
                // 异步
                HttpHelpers.GetAsyn(url, function (text, code) {
                    if (code == 200) {
                        callback(dataframe.Parse(parseText(text), tsv));
                    }
                    else {
                        throw "Error while load csv data source, http " + code + ": " + text;
                    }
                });
            }
            return null;
        };
        /**
         * 将所给定的文本文档内容解析为数据框对象
         *
         * @param tsv 所需要进行解析的文本内容是否为使用``<TAB>``作为分割符的tsv文本文件？
         *   默认不是，即默认使用逗号``,``作为分隔符的csv文本文件。
        */
        dataframe.Parse = function (text, tsv) {
            if (tsv === void 0) { tsv = false; }
            var parse = tsv ? csv_1.row.ParseTsv : csv_1.row.Parse;
            var rows = From(text.split(/\n/)).Select(parse);
            return new dataframe(rows);
        };
        return dataframe;
    }(IEnumerator));
    csv_1.dataframe = dataframe;
})(csv || (csv = {}));
var csv;
(function (csv) {
    /**
     * 将对象序列转换为``dataframe``对象
     *
     * 这个函数只能够转换object类型的数据，对于基础类型将不保证能够正常工作
     *
     * @param data 因为这个对象序列对象是具有类型约束的，所以可以直接从第一个
     *    元素对象之中得到所有的属性名称作为csv文件头的数据
    */
    function toDataFrame(data) {
        var header = $ts(Object.keys(data.First));
        var rows = data
            .Select(function (obj) {
            var columns = header
                .Select(function (ref, i) {
                return toString(obj[ref]);
            });
            return new csv.row(columns);
        });
        return new csv.dataframe([new csv.row(header)]).AppendRows(rows);
    }
    csv.toDataFrame = toDataFrame;
    function toString(obj) {
        if (isNullOrUndefined(obj)) {
            // 这个对象值是空的，所以在csv文件之中是空字符串
            return "";
        }
        else {
            return "" + obj;
        }
    }
})(csv || (csv = {}));
var csv;
(function (csv) {
    var HTML;
    (function (HTML) {
        var bootstrap = ["table", "table-hover"];
        /**
         * 将数据框对象转换为HTMl格式的表格对象的html代码
         *
         * @param tblClass 所返回来的html表格代码之中的table对象的类型默认是bootstrap类型的，
         * 所以默认可以直接应用bootstrap的样式在这个表格之上
         *
         * @returns 表格的HTML代码
        */
        function toHTMLTable(data, tblClass) {
            if (tblClass === void 0) { tblClass = bootstrap; }
            var th = data.headers
                .Select(function (h) { return "<th>" + h + "</th>"; })
                .JoinBy("\n");
            var tr = data.contents
                .Select(function (r) { return r.Select(function (c) { return "<td>" + c + "</td>"; }).JoinBy(""); })
                .Select(function (r) { return "<tr>" + r + "</tr>"; })
                .JoinBy("\n");
            return "\n            <table class=\"" + tblClass + "\">\n                <thead>\n                    <tr>" + th + "</tr>\n                </thead>\n                <tbody>\n                    " + tr + "\n                </tbody>\n            </table>";
        }
        HTML.toHTMLTable = toHTMLTable;
        function createHTMLTable(data, tblClass) {
            if (tblClass === void 0) { tblClass = bootstrap; }
            return toHTMLTable(csv.toDataFrame(data), tblClass);
        }
        HTML.createHTMLTable = createHTMLTable;
    })(HTML = csv.HTML || (csv.HTML = {}));
})(csv || (csv = {}));
var csv;
(function (csv) {
    /**
     * 一行数据
    */
    var row = /** @class */ (function (_super) {
        __extends(row, _super);
        function row(cells) {
            return _super.call(this, cells) || this;
        }
        Object.defineProperty(row.prototype, "columns", {
            /**
             * 当前的这一个行对象的列数据集合
             *
             * 注意，你无法通过直接修改这个数组之中的元素来达到修改这个行之中的值的目的
             * 因为这个属性会返回这个行的数组值的复制对象
            */
            get: function () {
                return this.sequence.slice();
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(row.prototype, "rowLine", {
            /**
             * 这个只读属性仅用于生成csv文件
            */
            get: function () {
                return From(this.columns)
                    .Select(row.autoEscape)
                    .JoinBy(",");
            },
            enumerable: true,
            configurable: true
        });
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
        row.prototype.indexOf = function (value, fromIndex) {
            if (fromIndex === void 0) { fromIndex = 0; }
            return this.sequence.indexOf(value);
        };
        row.prototype.ProjectObject = function (headers) {
            var obj = {};
            var data = this.columns;
            if (Array.isArray(headers)) {
                headers.forEach(function (h, i) {
                    obj[h] = data[i];
                });
            }
            else {
                headers.ForEach(function (h, i) {
                    obj[h] = data[i];
                });
            }
            return obj;
        };
        row.autoEscape = function (c) {
            if (c.indexOf(",") > -1) {
                return "\"" + c + "\"";
            }
            else {
                return c;
            }
        };
        row.Parse = function (line) {
            return new row(csv.CharsParser(line));
        };
        row.ParseTsv = function (line) {
            return new row(csv.CharsParser(line, "\t"));
        };
        return row;
    }(IEnumerator));
    csv.row = row;
})(csv || (csv = {}));
/// <reference path="Enumerator.ts" />
/**
 * A data sequence object with a internal index pointer.
*/
var Pointer = /** @class */ (function (_super) {
    __extends(Pointer, _super);
    function Pointer(src) {
        var _this = _super.call(this, src) || this;
        // 2018-09-02 在js里面，数值必须要进行初始化
        // 否则会出现NA初始值，导致使用EndRead属性判断失败
        // 可能会导致死循环的问题出现
        _this.i = 0;
        return _this;
    }
    Object.defineProperty(Pointer.prototype, "EndRead", {
        /**
         * The index pointer is at the end of the data sequence?
        */
        get: function () {
            return this.i >= this.Count;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Pointer.prototype, "Current", {
        /**
         * Get the element value in current location i;
        */
        get: function () {
            return this.sequence[this.i];
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Pointer.prototype, "Next", {
        /**
         * Get current index element value and then move the pointer
         * to next position.
        */
        get: function () {
            var x = this.Current;
            this.i = this.i + 1;
            return x;
        },
        enumerable: true,
        configurable: true
    });
    /**
     * Just move the pointer to the next position and then
     * returns current pointer object.
    */
    Pointer.prototype.MoveNext = function () {
        this.i = this.i + 1;
        return this;
    };
    return Pointer;
}(IEnumerator));
/// <reference path="../Linq/Collections/Pointer.ts" />
var csv;
(function (csv) {
    /**
     * 通过Chars枚举来解析域，分隔符默认为逗号
     * > https://github.com/xieguigang/sciBASIC/blame/701f9d0e6307a779bb4149c57a22a71572f1e40b/Data/DataFrame/IO/csv/Tokenizer.vb#L97
     *
    */
    function CharsParser(s, delimiter, quot) {
        if (delimiter === void 0) { delimiter = ","; }
        if (quot === void 0) { quot = '"'; }
        var tokens = [];
        var temp = [];
        var openStack = false;
        var buffer = From(Strings.ToCharArray(s)).ToPointer();
        var dblQuot = new RegExp("[" + quot + "]{2}", 'g');
        var cellStr = function () {
            // https://stackoverflow.com/questions/1144783/how-to-replace-all-occurrences-of-a-string-in-javascript
            // 2018-09-02
            // 如果join函数的参数是空的话，则js之中默认是使用逗号作为连接符的 
            return temp.join("").replace(dblQuot, quot);
        };
        var procEscape = function (c) {
            if (!StartEscaping(temp)) {
                // 查看下一个字符是否为分隔符
                // 因为前面的 Dim c As Char = +buffer 已经位移了，所以在这里直接取当前的字符
                var peek = buffer.Current;
                // 也有可能是 "" 转义 为单个 "
                var lastQuot = (temp.length > 0 && temp[temp.length - 1] != quot);
                if (temp.length == 0 && peek == delimiter) {
                    // openStack意味着前面已经出现一个 " 了
                    // 这里又出现了一个 " 并且下一个字符为分隔符
                    // 则说明是 "", 当前的cell内容是一个空字符串
                    tokens.push("");
                    temp = [];
                    buffer.MoveNext();
                    openStack = false;
                }
                else if ((peek == delimiter || buffer.EndRead) && lastQuot) {
                    // 下一个字符为分隔符，则结束这个token
                    tokens.push(cellStr());
                    temp = [];
                    // 跳过下一个分隔符，因为已经在这里判断过了
                    buffer.MoveNext();
                    openStack = false;
                }
                else {
                    // 不是，则继续添加
                    temp.push(c);
                }
            }
            else {
                // \" 会被转义为单个字符 "
                temp[temp.length - 1] = c;
            }
        };
        while (!buffer.EndRead) {
            var c = buffer.Next;
            if (openStack) {
                if (c == quot) {
                    procEscape(c);
                }
                else {
                    // 由于双引号而产生的转义          
                    temp.push(c);
                }
            }
            else {
                if (temp.length == 0 && c == quot) {
                    // token的第一个字符串为双引号，则开始转义
                    openStack = true;
                }
                else {
                    if (c == delimiter) {
                        tokens.push(cellStr());
                        temp = [];
                    }
                    else {
                        temp.push(c);
                    }
                }
            }
        }
        if (temp.length > 0) {
            tokens.push(cellStr());
        }
        return tokens;
    }
    csv.CharsParser = CharsParser;
    /**
     * 当前的token对象之中是否是转义的起始，即当前的token之中的最后一个符号
     * 是否是转义符<paramref name="escape"/>?
    */
    function StartEscaping(buffer, escape) {
        if (escape === void 0) { escape = "\\"; }
        if (IsNullOrEmpty(buffer)) {
            return false;
        }
        else {
            return buffer[buffer.length - 1] == escape;
        }
    }
})(csv || (csv = {}));
/**
 * How to Encode and Decode Strings with Base64 in JavaScript
 *
 * https://gist.github.com/ncerminara/11257943
*/
var Base64 = /** @class */ (function () {
    function Base64() {
    }
    /**
     * 将任意文本编码为base64字符串
    */
    Base64.encode = function (text) {
        var base64 = [];
        var n, r, i, s, o, u, a;
        var f = 0;
        text = Base64.utf8_encode(text);
        while (f < text.length) {
            n = text.charCodeAt(f++);
            r = text.charCodeAt(f++);
            i = text.charCodeAt(f++);
            s = n >> 2;
            o = (n & 3) << 4 | r >> 4;
            u = (r & 15) << 2 | i >> 6;
            a = i & 63;
            if (isNaN(r)) {
                u = a = 64;
            }
            else if (isNaN(i)) {
                a = 64;
            }
            base64.push(this.keyStr.charAt(s));
            base64.push(this.keyStr.charAt(o));
            base64.push(this.keyStr.charAt(u));
            base64.push(this.keyStr.charAt(a));
        }
        return base64.join("");
    };
    /**
     * 将base64字符串解码为普通的文本字符串
    */
    Base64.decode = function (base64) {
        var text = "";
        var n, r, i;
        var s, o, u, a;
        var f = 0;
        base64 = base64.replace(/[^A-Za-z0-9+/=]/g, "");
        while (f < base64.length) {
            s = this.keyStr.indexOf(base64.charAt(f++));
            o = this.keyStr.indexOf(base64.charAt(f++));
            u = this.keyStr.indexOf(base64.charAt(f++));
            a = this.keyStr.indexOf(base64.charAt(f++));
            n = s << 2 | o >> 4;
            r = (o & 15) << 4 | u >> 2;
            i = (u & 3) << 6 | a;
            text = text + String.fromCharCode(n);
            if (u != 64) {
                text = text + String.fromCharCode(r);
            }
            if (a != 64) {
                text = text + String.fromCharCode(i);
            }
        }
        text = Base64.utf8_decode(text);
        return text;
    };
    /**
     * 将文本转换为utf8编码的文本字符串
    */
    Base64.utf8_encode = function (text) {
        var chars = [];
        text = text.replace(/rn/g, "n");
        for (var n = 0; n < text.length; n++) {
            var r = text.charCodeAt(n);
            if (r < 128) {
                chars.push(String.fromCharCode(r));
            }
            else if (r > 127 && r < 2048) {
                chars.push(String.fromCharCode(r >> 6 | 192));
                chars.push(String.fromCharCode(r & 63 | 128));
            }
            else {
                chars.push(String.fromCharCode(r >> 12 | 224));
                chars.push(String.fromCharCode(r >> 6 & 63 | 128));
                chars.push(String.fromCharCode(r & 63 | 128));
            }
        }
        return chars.join("");
    };
    /**
     * 将utf8编码的文本转换为原来的文本
    */
    Base64.utf8_decode = function (text) {
        var t = [];
        var n = 0;
        var r = 0;
        var c2 = 0;
        var c3 = 0;
        while (n < text.length) {
            r = text.charCodeAt(n);
            if (r < 128) {
                t.push(String.fromCharCode(r));
                n++;
            }
            else if (r > 191 && r < 224) {
                c2 = text.charCodeAt(n + 1);
                t.push(String.fromCharCode((r & 31) << 6 | c2 & 63));
                n += 2;
            }
            else {
                c2 = text.charCodeAt(n + 1);
                c3 = text.charCodeAt(n + 2);
                t.push(String.fromCharCode((r & 15) << 12 | (c2 & 63) << 6 | c3 & 63));
                n += 3;
            }
        }
        return t.join("");
    };
    Base64.keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    return Base64;
}());
var TsLinq;
(function (TsLinq) {
    /**
     * �����������Զ��Ľ������ߵĺ���������Ϊ�������ж�Ӧ�ļ�ֵ�Ķ�ȡ����
    */
    var MetaReader = /** @class */ (function () {
        function MetaReader(meta) {
            this.meta = meta;
        }
        /**
         * Read meta object value by call name
         *
         * > https://stackoverflow.com/questions/280389/how-do-you-find-out-the-caller-function-in-javascript
        */
        MetaReader.prototype.GetValue = function (key) {
            if (key === void 0) { key = null; }
            if (!key) {
                key = TsLinq.StackTrace.GetCallerMember().memberName;
            }
            if (key in this.meta) {
                return this.meta[key];
            }
            else {
                return null;
            }
        };
        return MetaReader;
    }());
    TsLinq.MetaReader = MetaReader;
})(TsLinq || (TsLinq = {}));
var TsLinq;
(function (TsLinq) {
    var PriorityQueue = /** @class */ (function (_super) {
        __extends(PriorityQueue, _super);
        function PriorityQueue(events) {
            var _this = _super.call(this, []) || this;
            _this.events = events;
            return _this;
        }
        Object.defineProperty(PriorityQueue.prototype, "Q", {
            /**
             * 队列元素
            */
            get: function () {
                return this.sequence;
            },
            enumerable: true,
            configurable: true
        });
        PriorityQueue.prototype.enqueue = function (obj) {
            var last = this.Last;
            var q = this.Q;
            var x = new QueueItem(obj);
            q.push(x);
            if (last) {
                last.below = x;
                x.above = last;
            }
        };
        PriorityQueue.prototype.extract = function (i) {
            var q = this.Q;
            var x_above = q[i - 1];
            var x_below = q[i + 1];
            var x = q.splice(i, 1)[0];
            if (x_above) {
                x_above.below = x_below;
            }
            if (x_below) {
                x_below.above = x_above;
            }
            return x;
        };
        PriorityQueue.prototype.dequeue = function () {
            return this.extract(0);
        };
        return PriorityQueue;
    }(IEnumerator));
    TsLinq.PriorityQueue = PriorityQueue;
    var QueueItem = /** @class */ (function () {
        function QueueItem(x) {
            this.value = x;
        }
        QueueItem.prototype.toString = function () {
            return this.value.toString();
        };
        return QueueItem;
    }());
    TsLinq.QueueItem = QueueItem;
})(TsLinq || (TsLinq = {}));
var data;
(function (data_1) {
    /**
     * 一个数值范围
    */
    var NumericRange = /** @class */ (function () {
        // #endregion
        // #region Constructors (1)
        function NumericRange(min, max) {
            this.min = min;
            this.max = max;
        }
        Object.defineProperty(NumericRange.prototype, "range", {
            /**
             * ``[min, max]``
            */
            get: function () {
                return [this.min, this.max];
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(NumericRange.prototype, "Length", {
            // #endregion
            // #region Public Accessors (1)
            get: function () {
                return this.max - this.min;
            },
            enumerable: true,
            configurable: true
        });
        // #endregion
        // #region Public Static Methods (1)
        /**
         * 从一个数值序列之中创建改数值序列的值范围
        */
        NumericRange.Create = function (numbers) {
            var seq = Array.isArray(numbers) ?
                $ts(numbers) :
                numbers;
            var min = seq.Min();
            var max = seq.Max();
            return new NumericRange(min, max);
        };
        // #endregion
        // #region Public Methods (3)
        /**
         * 判断目标数值是否在当前的这个数值范围之内
        */
        NumericRange.prototype.IsInside = function (x) {
            return x >= this.min && x <= this.max;
        };
        /**
         * Get a numeric sequence within current range with a given step
         *
         * @param step The delta value of the step forward,
         *      by default is 10% of the range length.
        */
        NumericRange.prototype.PopulateNumbers = function (step) {
            if (step === void 0) { step = (this.Length / 10); }
            var data = [];
            for (var x = this.min; x < this.max; x += step) {
                data.push(x);
            }
            return data;
        };
        /**
         * Display the range in format ``[min, max]``
        */
        NumericRange.prototype.toString = function () {
            return "[" + this.min + ", " + this.max + "]";
        };
        return NumericRange;
    }());
    data_1.NumericRange = NumericRange;
})(data || (data = {}));
var data;
(function (data) {
    /**
     * 序列之中的对某一个区域的滑窗操作结果对象
    */
    var SlideWindow = /** @class */ (function (_super) {
        __extends(SlideWindow, _super);
        function SlideWindow(index, src) {
            var _this = _super.call(this, src) || this;
            _this.index = index;
            return _this;
        }
        /**
         * 创建指定片段长度的滑窗对象
         *
         * @param winSize 滑窗片段的长度
         * @param step 滑窗的步进长度，默认是一个步进
        */
        SlideWindow.Split = function (src, winSize, step) {
            if (step === void 0) { step = 1; }
            if (!Array.isArray(src)) {
                src = src.ToArray();
            }
            var len = src.length - winSize;
            var windows = [];
            for (var i = 0; i < len; i += step) {
                var chunk = new Array(winSize);
                for (var j = 0; j < winSize; j++) {
                    chunk[j] = src[i + j];
                }
                windows.push(new SlideWindow(i, chunk));
            }
            return new IEnumerator(windows);
        };
        return SlideWindow;
    }(IEnumerator));
    data.SlideWindow = SlideWindow;
})(data || (data = {}));
var StringBuilder = /** @class */ (function () {
    function StringBuilder(str, newLine) {
        if (str === void 0) { str = null; }
        if (newLine === void 0) { newLine = "\n"; }
        if (!str) {
            this.buffer = "";
        }
        else if (typeof str == "string") {
            this.buffer = "" + str;
        }
        else {
            this.buffer = "" + str.buffer;
        }
        this.newLine = newLine;
    }
    Object.defineProperty(StringBuilder.prototype, "Length", {
        get: function () {
            return this.buffer.length;
        },
        enumerable: true,
        configurable: true
    });
    StringBuilder.prototype.Append = function (text) {
        this.buffer = this.buffer + text;
        return this;
    };
    StringBuilder.prototype.AppendLine = function (text) {
        return this.Append(text + this.newLine);
    };
    StringBuilder.prototype.toString = function () {
        return this.buffer + "";
    };
    return StringBuilder;
}());
/// <reference path="./sprintf.ts" />
var TsLinq;
(function (TsLinq) {
    /**
     * URL组成字符串解析模块
    */
    var URL = /** @class */ (function () {
        function URL(url) {
            // http://localhost/router.html#http://localhost/b.html
            var token = Strings.GetTagValue(url, "://");
            this.protocol = token.name;
            token = Strings.GetTagValue(token.value, "/");
            this.origin = token.name;
            token = Strings.GetTagValue(token.value, "?");
            this.path = token.name;
            this.fileName = Strings.Empty(this.path) ? "" : URL.basename(this.path);
            this.hash = From(url.split("#")).Last;
            if (url.indexOf("#") < 0) {
                this.hash = "";
            }
            var args = URL.UrlQuery(token.value);
            this.query = new Dictionary(args)
                .Select(function (m) { return new NamedValue(m.key, m.value); })
                .ToArray();
        }
        /**
         * 将URL之中的query部分解析为字典对象
        */
        URL.UrlQuery = function (args) {
            if (args) {
                return DataExtensions.parseQueryString(args, false);
            }
            else {
                return {};
            }
        };
        /**
         * 只保留文件名（已经去除了文件夹路径以及文件名最后的拓展名部分）
        */
        URL.basename = function (fileName) {
            var nameTokens = From(fileName.split("/")).Last.split(".");
            var name = From(nameTokens)
                .Take(nameTokens.length - 1)
                .JoinBy(".");
            return name;
        };
        /**
         * 获取得到当前的url
        */
        URL.WindowLocation = function () {
            return new URL(window.location.href);
        };
        /**
         * 对bytes数值进行格式自动优化显示
         *
         * @param bytes
         *
         * @return 经过自动格式优化过后的大小显示字符串
        */
        URL.Lanudry = function (bytes) {
            var symbols = ["B", "KB", "MB", "GB", "TB"];
            var exp = Math.floor(Math.log(bytes) / Math.log(1000));
            var symbol = symbols[exp];
            var val = (bytes / Math.pow(1000, Math.floor(exp)));
            return sprintf("%.2f " + symbol, val);
        };
        URL.prototype.toString = function () {
            var query = From(this.query)
                .Select(function (q) { return q.name + "=" + encodeURIComponent(q.value); })
                .JoinBy("&");
            var url = this.protocol + "://" + this.origin + "/" + this.path;
            if (query) {
                url = url + "?" + query;
            }
            if (this.hash) {
                url = url + "#" + this.hash;
            }
            return url;
        };
        URL.Refresh = function (url) {
            return url + "&refresh=" + Math.random() * 10000;
        };
        return URL;
    }());
    TsLinq.URL = URL;
})(TsLinq || (TsLinq = {}));
var TsLinq;
(function (TsLinq) {
    /**
     * 性能计数器
    */
    var Benchmark = /** @class */ (function () {
        function Benchmark() {
            this.start = (new Date).getTime();
            this.lastCheck = this.start;
        }
        Benchmark.prototype.Tick = function () {
            var now = (new Date).getTime();
            var checkpoint = new CheckPoint();
            checkpoint.start = this.start;
            checkpoint.time = now;
            checkpoint.sinceFromStart = now - this.start;
            checkpoint.sinceLastCheck = now - this.lastCheck;
            this.lastCheck = now;
            return checkpoint;
        };
        return Benchmark;
    }());
    TsLinq.Benchmark = Benchmark;
    /**
     * 单位都是毫秒
    */
    var CheckPoint = /** @class */ (function () {
        function CheckPoint() {
        }
        Object.defineProperty(CheckPoint.prototype, "elapsedMilisecond", {
            /**
             * 获取从``time``到当前时间所流逝的毫秒计数
            */
            get: function () {
                return (new Date).getTime() - this.time;
            },
            enumerable: true,
            configurable: true
        });
        return CheckPoint;
    }());
    TsLinq.CheckPoint = CheckPoint;
})(TsLinq || (TsLinq = {}));
var HttpHelpers;
(function (HttpHelpers) {
    /**
     * 这个函数只会返回200成功代码的响应内容，对于其他的状态代码都会返回null
     * (这个函数是同步方式的)
    */
    function GET(url) {
        var request = new XMLHttpRequest();
        // `false` makes the request synchronous
        request.open('GET', url, false);
        request.send(null);
        if (request.status === 200) {
            return request.responseText;
        }
        else {
            return null;
        }
    }
    HttpHelpers.GET = GET;
    /**
     * 使用异步调用的方式进行数据的下载操作
    */
    function GetAsyn(url, callback) {
        var http = new XMLHttpRequest();
        http.open("GET", url, true);
        http.onreadystatechange = function () {
            if (http.readyState == 4) {
                callback(http.responseText, http.status);
            }
        };
        http.send(null);
    }
    HttpHelpers.GetAsyn = GetAsyn;
    function POST(url, postData, callback) {
        var http = new XMLHttpRequest();
        var data = postData.data;
        http.open('POST', url, true);
        // Send the proper header information along with the request
        http.setRequestHeader('Content-type', postData.type);
        // Call a function when the state changes.
        http.onreadystatechange = function () {
            if (http.readyState == 4) {
                callback(http.responseText, http.status);
            }
        };
        http.send(data);
    }
    HttpHelpers.POST = POST;
    /**
     * 使用multipart form类型的数据进行文件数据的上传操作
     *
     * @param url 函数会通过POST方式将文件数据上传到这个url所指定的服务器资源位置
     *
    */
    function UploadFile(url, postData, callback) {
        var data = new FormData();
        data.append("File", postData.data);
        HttpHelpers.POST(url, {
            type: postData.type,
            data: data
        }, callback);
    }
    HttpHelpers.UploadFile = UploadFile;
    var PostData = /** @class */ (function () {
        function PostData() {
        }
        PostData.prototype.toString = function () {
            return this.type;
        };
        return PostData;
    }());
    HttpHelpers.PostData = PostData;
})(HttpHelpers || (HttpHelpers = {}));
var Linq;
(function (Linq) {
    /**
     * 确保所传递进来的参数输出的是一个序列集合对象
    */
    function EnsureCollection(data, n) {
        if (n === void 0) { n = -1; }
        return new IEnumerator(Linq.EnsureArray(data, n));
    }
    Linq.EnsureCollection = EnsureCollection;
    /**
     * 确保随传递进来的参数所输出的是一个数组对象
     *
     * @param data 如果这个参数是一个数组，则在这个函数之中会执行复制操作
     * @param n 如果data数据序列长度不足，则会使用null进行补充，n为任何小于data长度的正实数都不会进行补充操作，
     *     相反只会返回前n个元素，如果n是负数，则不进行任何操作
    */
    function EnsureArray(data, n) {
        if (n === void 0) { n = -1; }
        var type = TypeInfo.typeof(data);
        var array;
        if (type.IsEnumerator) {
            array = data.ToArray();
        }
        else if (type.IsArray) {
            array = data.slice();
        }
        else {
            var x = data;
            if (n <= 0) {
                array = [x];
            }
            else {
                array = [];
                for (var i = 0; i < n; i++) {
                    array.push(x);
                }
            }
        }
        if (1 <= n) {
            if (n < array.length) {
                array = array.slice(0, n);
            }
            else if (n > array.length) {
                var len = array.length;
                for (var i = len; i < n; i++) {
                    array.push(null);
                }
            }
            else {
                // n 和 array 等长，不做任何事
            }
        }
        return array;
    }
    Linq.EnsureArray = EnsureArray;
    /**
     * extends 'from' object with members from 'to'. If 'to' is null, a deep clone of 'from' is returned
     *
     * > https://stackoverflow.com/questions/122102/what-is-the-most-efficient-way-to-deep-clone-an-object-in-javascript
    */
    function extend(from, to) {
        if (to === void 0) { to = null; }
        if (from == null || typeof from != "object")
            return from;
        if (from.constructor != Object && from.constructor != Array)
            return from;
        if (from.constructor == Date ||
            from.constructor == RegExp ||
            from.constructor == Function ||
            from.constructor == String ||
            from.constructor == Number ||
            from.constructor == Boolean)
            return new from.constructor(from);
        to = to || new from.constructor();
        for (var name in from) {
            to[name] = typeof to[name] == "undefined" ? extend(from[name], null) : to[name];
        }
        return to;
    }
    Linq.extend = extend;
})(Linq || (Linq = {}));
/**
 * 路由器模块
*/
var Router;
(function (Router) {
    var hashLinks;
    var routerLink = "router-link";
    function queryKey(argName) {
        return function (link) { return getAllUrlParams(link).Item(argName); };
    }
    Router.queryKey = queryKey;
    function moduleName() {
        return function (link) { return (new TsLinq.URL(link)).fileName; };
    }
    Router.moduleName = moduleName;
    /**
     * 父容器页面注册视图容器对象
    */
    function register(appId, hashKey, frameRegister) {
        if (appId === void 0) { appId = "app"; }
        if (hashKey === void 0) { hashKey = null; }
        if (frameRegister === void 0) { frameRegister = true; }
        var aLink;
        var gethashKey;
        if (!hashLinks) {
            hashLinks = new Dictionary({
                "/": "/"
            });
        }
        if (!hashKey) {
            gethashKey = function (link) { return (new TsLinq.URL(link)).fileName; };
        }
        else if (typeof hashKey == "string") {
            gethashKey = Router.queryKey(hashKey);
        }
        else {
            gethashKey = hashKey;
        }
        aLink = $ts(".router");
        aLink.attr("router-link", function (link) { return link.href; });
        aLink.attr("href", "javascript:void(0);");
        aLink.onClick(function (link, click) {
            Router.goto(link.getAttribute("router-link"), appId, gethashKey);
        });
        aLink.attr(routerLink)
            .ForEach(function (link) {
            hashLinks.Add(gethashKey(link), link);
        });
        // 假设当前的url之中有hash的话，还需要根据注册的路由配置进行跳转显示
        hashChanged(appId);
        // clientResize(appId);
    }
    Router.register = register;
    function clientResize(appId) {
        var app = $ts("#" + appId);
        var frame = $ts("#" + appId + "-frame");
        var size = Linq.DOM.clientSize();
        if (!app) {
            console.warn("[#" + appId + "] not found!");
        }
        else {
            app.style.width = size[0].toString();
            app.style.height = size[1].toString();
            frame.width = size[0].toString();
            frame.height = size[1].toString();
        }
    }
    /**
     * 根据当前url之中的hash进行相应的页面的显示操作
    */
    function hashChanged(appId) {
        var hash = TsLinq.URL.WindowLocation().hash;
        var url = hashLinks.Item(hash);
        if (url) {
            if (url == "/") {
                // 跳转到主页，重新刷新页面？
                window.location.hash = "";
                window.location.reload(true);
            }
            else {
                $ts("#" + appId).innerHTML =
                    HttpHelpers.GET(url);
            }
        }
    }
    function navigate(link, stack, appId, hashKey) {
        var frame = $ts("#" + appId);
        frame.innerHTML = HttpHelpers.GET(link);
        Router.register(appId, hashKey, false);
        window.location.hash = hashKey(link);
    }
    /**
     * 当前的堆栈环境是否是最顶层的堆栈？
    */
    function IsTopWindowStack() {
        return parent && (parent.location.pathname == window.location.pathname);
    }
    Router.IsTopWindowStack = IsTopWindowStack;
    /**
     * 因为link之中可能存在查询参数，所以必须要在web服务器上面测试
    */
    function goto(link, appId, hashKey, stack) {
        if (stack === void 0) { stack = null; }
        if (!Router.IsTopWindowStack()) {
            parent.Router.goto(link, appId, hashKey, parent);
        }
        else if (stack) {
            // 没有parent了，已经到达最顶端了
            navigate(link, stack, appId, hashKey);
        }
        else {
            navigate(link, window, appId, hashKey);
        }
    }
    Router.goto = goto;
})(Router || (Router = {}));
/**
 * 序列之中的元素下标的操作方法集合
*/
var Which;
(function (Which) {
    /**
     * 查找出所给定的逻辑值集合之中的所有true的下标值
    */
    function Is(booleans) {
        if (Array.isArray(booleans)) {
            booleans = new IEnumerator(booleans);
        }
        return booleans
            .Select(function (flag, i) {
            return {
                flag: flag, index: i
            };
        })
            .Where(function (t) { return t.flag; })
            .Select(function (t) { return t.index; });
    }
    Which.Is = Is;
    /**
     * 默认的通用类型的比较器对象
    */
    var DefaultCompares = /** @class */ (function () {
        function DefaultCompares() {
            /**
             * 一个用于比较通用类型的数值转换器对象
            */
            this.as_numeric = null;
        }
        DefaultCompares.prototype.compares = function (a, b) {
            if (!this.as_numeric) {
                this.as_numeric = DataExtensions.AsNumeric(a);
                if (!this.as_numeric) {
                    this.as_numeric = DataExtensions.AsNumeric(b);
                }
            }
            if (!this.as_numeric) {
                // a 和 b 都是null或者undefined
                // 认为这两个空值是相等的
                // 则this.as_numeric会在下一个循环之中被赋值
                return 0;
            }
            else {
                return this.as_numeric(a) - this.as_numeric(b);
            }
        };
        DefaultCompares.default = function () {
            return new DefaultCompares().compares;
        };
        return DefaultCompares;
    }());
    Which.DefaultCompares = DefaultCompares;
    /**
     * 查找出序列之中最大的元素的序列下标编号
     *
     * @param x 所给定的数据序列
     * @param compare 默认是将x序列之中的元素转换为数值进行大小的比较的
    */
    function Max(x, compare) {
        if (compare === void 0) { compare = DefaultCompares.default(); }
        var xMax = null;
        var iMax = 0;
        for (var i = 0; i < x.Count; i++) {
            if (compare(x.ElementAt(i), xMax) > 0) {
                // x > xMax
                xMax = x.ElementAt(i);
                iMax = i;
            }
        }
        return iMax;
    }
    Which.Max = Max;
    /**
     * 查找出序列之中最小的元素的序列下标编号
     *
     * @param x 所给定的数据序列
     * @param compare 默认是将x序列之中的元素转换为数值进行大小的比较的
    */
    function Min(x, compare) {
        if (compare === void 0) { compare = DefaultCompares.default(); }
        return Max(x, function (a, b) { return -compare(a, b); });
    }
    Which.Min = Min;
})(Which || (Which = {}));
/**
 * Binary tree implements
*/
var algorithm;
(function (algorithm) {
    var BTree;
    (function (BTree) {
        /**
         * 用于进行数据分组所需要的最基础的二叉树数据结构
         *
         * ``{key => value}``
        */
        var binaryTree = /** @class */ (function () {
            /**
             * 构建一个二叉树对象
             *
             * @param comparer 这个函数指针描述了如何进行两个对象之间的比较操作，如果这个函数参数使用默认值的话
             *                 则只能够针对最基本的数值，逻辑变量进行操作
            */
            function binaryTree(comparer) {
                if (comparer === void 0) { comparer = function (a, b) {
                    var x = DataExtensions.as_numeric(a);
                    var y = DataExtensions.as_numeric(b);
                    return x - y;
                }; }
                this.compares = comparer;
            }
            /**
             * 向这个二叉树对象之中添加一个子节点
            */
            binaryTree.prototype.add = function (term, value) {
                if (value === void 0) { value = null; }
                var np = this.root;
                var cmp = 0;
                if (!np) {
                    // 根节点是空的，则将当前的term作为根节点
                    this.root = new BTree.node(term, value);
                    return;
                }
                while (np) {
                    cmp = this.compares(term, np.key);
                    if (cmp == 0) {
                        // this node is existed
                        // value replace??
                        np.value = value;
                        break;
                    }
                    else if (cmp < 0) {
                        if (np.left) {
                            np = np.left;
                        }
                        else {
                            // np is a leaf node?
                            // add at here
                            np.left = new BTree.node(term, value);
                            break;
                        }
                    }
                    else {
                        if (np.right) {
                            np = np.right;
                        }
                        else {
                            np.right = new BTree.node(term, value);
                            break;
                        }
                    }
                }
            };
            /**
             * 根据key值查找一个节点，然后获取该节点之中与key所对应的值
             *
             * @returns 如果这个函数返回空值，则表示可能未找到目标子节点
            */
            binaryTree.prototype.find = function (term) {
                var np = this.root;
                var cmp = 0;
                while (np) {
                    cmp = this.compares(term, np.key);
                    if (cmp == 0) {
                        return np.value;
                    }
                    else if (cmp < 0) {
                        np = np.left;
                    }
                    else {
                        np = np.right;
                    }
                }
                // not exists
                return null;
            };
            /**
             * 将这个二叉树对象转换为一个节点的数组
            */
            binaryTree.prototype.ToArray = function () {
                return BTree.binaryTreeExtensions.populateNodes(this.root);
            };
            /**
             * 将这个二叉树对象转换为一个Linq查询表达式所需要的枚举器类型
            */
            binaryTree.prototype.AsEnumerable = function () {
                return new IEnumerator(this.ToArray());
            };
            return binaryTree;
        }());
        BTree.binaryTree = binaryTree;
    })(BTree = algorithm.BTree || (algorithm.BTree = {}));
})(algorithm || (algorithm = {}));
var algorithm;
(function (algorithm) {
    var BTree;
    (function (BTree) {
        /**
         * data extension module for binary tree nodes data sequence
        */
        var binaryTreeExtensions;
        (function (binaryTreeExtensions) {
            /**
             * Convert a binary tree object as a node array.
            */
            function populateNodes(tree) {
                var out = [];
                visitInternal(tree, out);
                return out;
            }
            binaryTreeExtensions.populateNodes = populateNodes;
            function visitInternal(tree, out) {
                // 20180929 为什么会存在undefined的节点呢？
                if (isNullOrUndefined(tree)) {
                    console.warn(tree);
                    return;
                }
                else {
                    out.push(tree);
                }
                if (tree.left) {
                    visitInternal(tree.left, out);
                }
                if (tree.right) {
                    visitInternal(tree.right, out);
                }
            }
        })(binaryTreeExtensions = BTree.binaryTreeExtensions || (BTree.binaryTreeExtensions = {}));
    })(BTree = algorithm.BTree || (algorithm.BTree = {}));
})(algorithm || (algorithm = {}));
var algorithm;
(function (algorithm) {
    var BTree;
    (function (BTree) {
        /**
         * A binary tree node.
        */
        var node = /** @class */ (function () {
            function node(key, value, left, right) {
                if (value === void 0) { value = null; }
                if (left === void 0) { left = null; }
                if (right === void 0) { right = null; }
                this.key = key;
                this.left = left;
                this.right = right;
                this.value = value;
            }
            node.prototype.toString = function () {
                return this.key.toString();
            };
            return node;
        }());
        BTree.node = node;
    })(BTree = algorithm.BTree || (algorithm.BTree = {}));
})(algorithm || (algorithm = {}));
var TsLinq;
(function (TsLinq) {
    /**
     * 调用堆栈之中的某一个栈片段信息
    */
    var StackFrame = /** @class */ (function () {
        function StackFrame() {
        }
        StackFrame.prototype.toString = function () {
            return this.caller + " [as " + this.memberName + "](" + this.file + ":" + this.line + ":" + this.column + ")";
        };
        StackFrame.Parse = function (line) {
            var frame = new StackFrame();
            var file = StackFrame.getFileName(line);
            var caller = line.replace(file, "").trim().substr(3);
            file = file.substr(1, file.length - 2);
            if (caller.indexOf("/") > -1 || caller.indexOf(":") > -1) {
                // 没有替换成功，任然是一个文件路径，则可能
                // 是html文档之中的一个最开始的函数调用
                // 是没有caller的
                caller = "<HTML\\Document>";
            }
            var position = $ts(file.match(/([:]\d+){2}$/m)[0].split(":"));
            var posStrLen = (position.Select(function (s) { return s.length; }).Sum() + 2);
            var location = From(position)
                .Where(function (s) { return s.length > 0; })
                .Select(function (x) { return Strings.Val(x); })
                .ToArray();
            frame.file = file.substr(0, file.length - posStrLen);
            var alias = caller.match(/\[.+\]/);
            var memberName = (!alias || alias.length == 0) ? null : alias[0];
            if (memberName) {
                caller = caller
                    .substr(0, caller.length - memberName.length)
                    .trim();
                frame.memberName = memberName
                    .substr(3, memberName.length - 4)
                    .trim();
            }
            else {
                var t = caller.split(".");
                frame.memberName = t[t.length - 1];
            }
            frame.caller = caller;
            frame.line = location[0];
            frame.column = location[1];
            return frame;
        };
        StackFrame.getFileName = function (line) {
            var matches = line.match(/\(.+\)/);
            if (!matches || matches.length == 0) {
                // 2018-09-14 可能是html文件之中
                return "(" + line.substr(6).trim() + ")";
            }
            else {
                return matches[0];
            }
        };
        return StackFrame;
    }());
    TsLinq.StackFrame = StackFrame;
})(TsLinq || (TsLinq = {}));
var TsLinq;
(function (TsLinq) {
    /**
     * 程序的堆栈追踪信息
     *
     * 这个对象是调用堆栈``StackFrame``片段对象的序列集合
    */
    var StackTrace = /** @class */ (function (_super) {
        __extends(StackTrace, _super);
        function StackTrace(frames) {
            return _super.call(this, frames) || this;
        }
        /**
         * 导出当前的程序运行位置的调用堆栈信息
        */
        StackTrace.Dump = function () {
            var err = new Error().stack.split("\n");
            var trace = From(err)
                //   1 是第一行 err 字符串, 
                // + 1 是跳过当前的这个Dump函数的栈信息
                .Skip(1 + 1)
                .Select(TsLinq.StackFrame.Parse);
            return new StackTrace(trace);
        };
        /**
         * 获取函数调用者的名称的帮助函数
        */
        StackTrace.GetCallerMember = function () {
            var trace = StackTrace.Dump().ToArray();
            // index = 1 是GetCallerMemberName这个函数的caller的栈片段
            // index = 2 就是caller的caller的栈片段，即该caller的CallerMemberName
            var caller = trace[1 + 1];
            return caller;
        };
        StackTrace.prototype.toString = function () {
            var sb = new StringBuilder();
            this.ForEach(function (frame) {
                sb.AppendLine("  at " + frame.toString());
            });
            return sb.toString();
        };
        return StackTrace;
    }(IEnumerator));
    TsLinq.StackTrace = StackTrace;
})(TsLinq || (TsLinq = {}));
var CanvasHelper;
(function (CanvasHelper) {
    var innerCanvas;
    /**
     * Uses canvas.measureText to compute and return the width of the given text of given font in pixels.
     *
     * @param {String} text The text to be rendered.
     * @param {String} font The css font descriptor that text is to be rendered with (e.g. "bold 14px verdana").
     *
     * @see https://stackoverflow.com/questions/118241/calculate-text-width-with-javascript/21015393#21015393
     *
     */
    function getTextWidth(text, font) {
        // re-use canvas object for better performance
        var canvas = innerCanvas || (innerCanvas = $ts("<canvas>"));
        var context = canvas.getContext("2d");
        var metrics;
        context.font = font;
        metrics = context.measureText(text);
        return metrics.width;
    }
    CanvasHelper.getTextWidth = getTextWidth;
    /**
     * found this trick at http://talideon.com/weblog/2005/02/detecting-broken-images-js.cfm
    */
    function imageOk(img) {
        "use strict";
        // During the onload event, IE correctly identifies any images that
        // weren't downloaded as not complete. Others should too. Gecko-based
        // browsers act like NS4 in that they report this incorrectly.
        if (!img.complete) {
            return false;
        }
        // However, they do have two very useful properties: naturalWidth and
        // naturalHeight. These give the true size of the image. If it failed
        // to load, either of these should be zero.
        if (typeof img.naturalWidth !== "undefined" && img.naturalWidth === 0) {
            return false;
        }
        // No other way of checking: assume it's ok.
        return true;
    }
    CanvasHelper.imageOk = imageOk;
    /**
     * @param size [width, height]
    */
    function createCanvas(size, id, title, display) {
        "use strict";
        if (display === void 0) { display = "block"; }
        var canvas = document.createElement("canvas");
        //check for canvas support before attempting anything
        if (!canvas.getContext) {
            return null;
        }
        var ctx = canvas.getContext('2d');
        //check for html5 text drawing support
        if (!supportsText(ctx)) {
            return null;
        }
        //size the canvas
        canvas.width = size[0];
        canvas.height = size[1];
        canvas.id = id;
        canvas.title = title;
        canvas.style.display = display;
        return canvas;
    }
    CanvasHelper.createCanvas = createCanvas;
    function supportsText(ctx) {
        if (!ctx.fillText) {
            return false;
        }
        if (!ctx.measureText) {
            return false;
        }
        return true;
    }
    CanvasHelper.supportsText = supportsText;
    var fontSize = /** @class */ (function () {
        function fontSize() {
            this.sizes = [];
        }
        return fontSize;
    }());
    CanvasHelper.fontSize = fontSize;
})(CanvasHelper || (CanvasHelper = {}));
var CanvasHelper;
(function (CanvasHelper) {
    var saveSvgAsPng;
    (function (saveSvgAsPng) {
        saveSvgAsPng.xlink = "http://www.w3.org/1999/xlink";
        function isElement(obj) {
            return obj instanceof HTMLElement || obj instanceof SVGElement;
        }
        saveSvgAsPng.isElement = isElement;
        function requireDomNode(el) {
            if (!isElement(el)) {
                throw new Error('an HTMLElement or SVGElement is required; got ' + el);
            }
            else {
                return el;
            }
        }
        saveSvgAsPng.requireDomNode = requireDomNode;
        /**
         * 判断所给定的url指向的资源是否是来自于外部域的资源？
        */
        function isExternal(url) {
            return url && url.lastIndexOf('http', 0) == 0 && url.lastIndexOf(window.location.host) == -1;
        }
        saveSvgAsPng.isExternal = isExternal;
        function inlineImages(el, callback) {
            requireDomNode(el);
            var images = el.querySelectorAll('image');
            var left = images.length;
            var checkDone = function (count) {
                if (count === 0) {
                    callback();
                }
            };
            checkDone(left);
            for (var i = 0; i < images.length; i++) {
                left = renderInlineImage(images[i], left, checkDone);
            }
        }
        saveSvgAsPng.inlineImages = inlineImages;
        function renderInlineImage(image, left, checkDone) {
            var href = image.getAttributeNS(saveSvgAsPng.xlink, "href");
            if (href) {
                if (typeof href != "string") {
                    href = href.value;
                }
                if (isExternal(href)) {
                    console.warn("Cannot render embedded images linking to external hosts: " + href);
                    return;
                }
            }
            var canvas = document.createElement('canvas');
            var ctx = canvas.getContext('2d');
            var img = new Image();
            img.crossOrigin = "anonymous";
            href = href || image.getAttribute('href');
            if (href) {
                img.src = href;
                img.onload = function () {
                    canvas.width = img.width;
                    canvas.height = img.height;
                    ctx.drawImage(img, 0, 0);
                    image.setAttributeNS(saveSvgAsPng.xlink, "href", canvas.toDataURL('image/png'));
                    left--;
                    checkDone(left);
                };
                img.onerror = function () {
                    console.error("Could not load " + href);
                    left--;
                    checkDone(left);
                };
            }
            else {
                left--;
                checkDone(left);
            }
            return left;
        }
        /**
         * 获取得到width或者height的值
        */
        function getDimension(el, clone, dim) {
            var v = (el.viewBox && el.viewBox.baseVal && el.viewBox.baseVal[dim]) ||
                (clone.getAttribute(dim) !== null && !clone.getAttribute(dim).match(/%$/) && parseInt(clone.getAttribute(dim))) ||
                el.getBoundingClientRect()[dim] ||
                parseInt(clone.style[dim]) ||
                parseInt(window.getComputedStyle(el).getPropertyValue(dim));
            if (typeof v === 'undefined' || v === null) {
                return 0;
            }
            else {
                var val = parseFloat(v);
                return isNaN(val) ? 0 : val;
            }
        }
        saveSvgAsPng.getDimension = getDimension;
        function reEncode(data) {
            data = encodeURIComponent(data);
            data = data.replace(/%([0-9A-F]{2})/g, function (match, p1) {
                var c = String.fromCharCode(('0x' + p1));
                return c === '%' ? '%25' : c;
            });
            return decodeURIComponent(data);
        }
        saveSvgAsPng.reEncode = reEncode;
    })(saveSvgAsPng = CanvasHelper.saveSvgAsPng || (CanvasHelper.saveSvgAsPng = {}));
})(CanvasHelper || (CanvasHelper = {}));
var CanvasHelper;
(function (CanvasHelper) {
    var saveSvgAsPng;
    (function (saveSvgAsPng) {
        saveSvgAsPng.xmlns = "http://www.w3.org/2000/xmlns/";
        /**
         * ##### 2018-10-12 XMl标签必须要一开始就出现，否则会出现错误
         *
         * error on line 2 at column 14: XML declaration allowed only at the start of the document
        */
        saveSvgAsPng.doctype = "<?xml version=\"1.0\" standalone=\"no\"?>\n            <!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\" [<!ENTITY nbsp \"&#160;\">]>";
        /**
         * https://github.com/exupero/saveSvgAsPng
        */
        var Encoder = /** @class */ (function () {
            function Encoder() {
            }
            Encoder.prepareSvg = function (el, options, cb) {
                if (options === void 0) { options = new saveSvgAsPng.Options(); }
                if (cb === void 0) { cb = null; }
                saveSvgAsPng.requireDomNode(el);
                options.scale = options.scale || 1;
                options.responsive = options.responsive || false;
                saveSvgAsPng.inlineImages(el, function () {
                    var outer = document.createElement("div");
                    var clone = el.cloneNode(true);
                    var width, height;
                    if (el.tagName == 'svg') {
                        width = options.width || saveSvgAsPng.getDimension(el, clone, 'width');
                        height = options.height || saveSvgAsPng.getDimension(el, clone, 'height');
                    }
                    else if (el.getBBox) {
                        var box = el.getBBox();
                        width = box.x + box.width;
                        height = box.y + box.height;
                        clone.setAttribute('transform', clone.getAttribute('transform').replace(/translate\(.*?\)/, ''));
                        var svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
                        svg.appendChild(clone);
                        clone = svg;
                    }
                    else {
                        console.error('Attempted to render non-SVG element', el);
                        return;
                    }
                    clone.setAttribute("version", "1.1");
                    if (!clone.getAttribute('xmlns')) {
                        clone.setAttributeNS(saveSvgAsPng.xmlns, "xmlns", "http://www.w3.org/2000/svg");
                    }
                    if (!clone.getAttribute('xmlns:xlink')) {
                        clone.setAttributeNS(saveSvgAsPng.xmlns, "xmlns:xlink", "http://www.w3.org/1999/xlink");
                    }
                    if (options.responsive) {
                        clone.removeAttribute('width');
                        clone.removeAttribute('height');
                        clone.setAttribute('preserveAspectRatio', 'xMinYMin meet');
                    }
                    else {
                        clone.setAttribute("width", (width * options.scale).toString());
                        clone.setAttribute("height", (height * options.scale).toString());
                    }
                    clone.setAttribute("viewBox", [
                        options.left || 0,
                        options.top || 0,
                        width,
                        height
                    ].join(" "));
                    var fos = clone.querySelectorAll('foreignObject > *');
                    for (var i = 0; i < fos.length; i++) {
                        if (!fos[i].getAttribute('xmlns')) {
                            fos[i].setAttributeNS(saveSvgAsPng.xmlns, "xmlns", "http://www.w3.org/1999/xhtml");
                        }
                    }
                    outer.appendChild(clone);
                    // In case of custom fonts we need to fetch font first, and then inline
                    // its url into data-uri format (encode as base64). That's why style
                    // processing is done asynchonously. Once all inlining is finshed
                    // cssLoadedCallback() is called.
                    saveSvgAsPng.styles.doStyles(el, options, cssLoadedCallback);
                    function cssLoadedCallback(css) {
                        // here all fonts are inlined, so that we can render them properly.
                        var s = document.createElement('style');
                        s.setAttribute('type', 'text/css');
                        s.innerHTML = "<![CDATA[\n" + css + "\n]]>";
                        var defs = document.createElement('defs');
                        defs.appendChild(s);
                        clone.insertBefore(defs, clone.firstChild);
                        if (cb) {
                            var outHtml = outer.innerHTML;
                            outHtml = outHtml.replace(/NS\d+:href/gi, 'xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href');
                            cb(outHtml, width, height);
                        }
                    }
                });
            };
            Encoder.svgAsDataUri = function (el, options, cb) {
                if (cb === void 0) { cb = null; }
                this.prepareSvg(el, options, function (svg) {
                    var uri = 'data:image/svg+xml;base64,' + window.btoa(saveSvgAsPng.reEncode(saveSvgAsPng.doctype + svg));
                    if (cb) {
                        cb(uri);
                    }
                });
            };
            Encoder.svgAsPngUri = function (el, options, cb) {
                if (options === void 0) { options = new saveSvgAsPng.Options(); }
                saveSvgAsPng.requireDomNode(el);
                options.encoderType = options.encoderType || 'image/png';
                options.encoderOptions = options.encoderOptions || 0.8;
                var convertToPng = function (src, w, h) {
                    var canvas = $ts('<canvas>', {
                        width: w,
                        height: h
                    });
                    var context = canvas.getContext('2d');
                    if (options.canvg) {
                        options.canvg(canvas, src);
                    }
                    else {
                        context.drawImage(src, 0, 0);
                    }
                    if (options.backgroundColor) {
                        context.globalCompositeOperation = 'destination-over';
                        context.fillStyle = options.backgroundColor;
                        context.fillRect(0, 0, canvas.width, canvas.height);
                    }
                    var png;
                    try {
                        png = canvas.toDataURL(options.encoderType, options.encoderOptions);
                    }
                    catch (e) {
                        // 20181013 在typescript之中还不支持SecurityError??
                        // (typeof SecurityError !== 'undefined' && e instanceof SecurityError) || 
                        if (e.name == "SecurityError") {
                            console.error("Rendered SVG images cannot be downloaded in this browser.");
                            return;
                        }
                        else {
                            throw e;
                        }
                    }
                    cb(png);
                };
                if (options.canvg) {
                    this.prepareSvg(el, options, convertToPng);
                }
                else {
                    this.svgAsDataUri(el, options, function (uri) {
                        var image = new Image();
                        image.onload = function () {
                            convertToPng(image, image.width, image.height);
                        };
                        image.onerror = function () {
                            console.error('There was an error loading the data URI as an image on the following SVG\n', window.atob(uri.slice(26)), '\n', "Open the following link to see browser's diagnosis\n", uri);
                        };
                        image.src = uri;
                    });
                }
            };
            Encoder.saveSvg = function (el, name, options) {
                saveSvgAsPng.requireDomNode(el);
                options = options || {};
                this.svgAsDataUri(el, options, function (uri) {
                    Linq.DOM.download(name, uri);
                });
            };
            /**
             * 将指定的SVG节点保存为png图片
             *
             * @param svg 需要进行保存为图片的svg节点的对象实例或者对象的节点id值
             * @param name 所保存的文件名
             * @param options 配置参数，直接留空使用默认值就好了
            */
            Encoder.saveSvgAsPng = function (svg, name, options) {
                if (options === void 0) { options = saveSvgAsPng.Options.Default(); }
                if (typeof svg == "string") {
                    svg = $ts(svg);
                    saveSvgAsPng.requireDomNode(svg);
                }
                else {
                    saveSvgAsPng.requireDomNode(svg);
                }
                this.svgAsPngUri(svg, options, function (uri) {
                    Linq.DOM.download(name, uri);
                });
            };
            return Encoder;
        }());
        saveSvgAsPng.Encoder = Encoder;
    })(saveSvgAsPng = CanvasHelper.saveSvgAsPng || (CanvasHelper.saveSvgAsPng = {}));
})(CanvasHelper || (CanvasHelper = {}));
var CanvasHelper;
(function (CanvasHelper) {
    var saveSvgAsPng;
    (function (saveSvgAsPng) {
        var Options = /** @class */ (function () {
            function Options() {
            }
            Options.Default = function () {
                return {
                    encoderType: "image/png",
                    encoderOptions: 0.8,
                    scale: 1,
                    responsive: false,
                    left: 0,
                    top: 0
                };
            };
            return Options;
        }());
        saveSvgAsPng.Options = Options;
        var styles = /** @class */ (function () {
            function styles() {
            }
            styles.doStyles = function (el, options, cssLoadedCallback) {
                var css = "";
                // each font that has extranl link is saved into queue, and processed
                // asynchronously
                var fontsQueue = [];
                var sheets = document.styleSheets;
                for (var i = 0; i < sheets.length; i++) {
                    var rules;
                    try {
                        rules = sheets[i].cssRules;
                    }
                    catch (e) {
                        console.warn("Stylesheet could not be loaded: " + sheets[i].href);
                        continue;
                    }
                    if (rules != null) {
                        css += this.processCssRules(el, rules, options, sheets[i].href, fontsQueue);
                    }
                }
                // Now all css is processed, it's time to handle scheduled fonts
                this.processFontQueue(fontsQueue, css, cssLoadedCallback);
            };
            styles.processCssRules = function (el, rules, options, sheetHref, fontsQueue) {
                var css = "";
                for (var j = 0, match; j < rules.length; j++, match = null) {
                    var rule = rules[j];
                    if (typeof (rule.style) == "undefined") {
                        continue;
                    }
                    var selectorText;
                    try {
                        selectorText = rule.selectorText;
                    }
                    catch (err) {
                        console.warn("The following CSS rule has an invalid selector: \"" + rule + "\"", err);
                    }
                    try {
                        if (selectorText) {
                            match = el.querySelector(selectorText) || el.parentNode.querySelector(selectorText);
                        }
                    }
                    catch (err) {
                        console.warn("Invalid CSS selector \"" + selectorText + "\"", err);
                    }
                    if (match) {
                        var selector = options.selectorRemap ? options.selectorRemap(rule.selectorText) : rule.selectorText;
                        var cssText = options.modifyStyle ? options.modifyStyle(rule.style.cssText) : rule.style.cssText;
                        css += selector + " { " + cssText + " }\n";
                    }
                    else if (rule.cssText.match(/^@font-face/)) {
                        // below we are trying to find matches to external link. E.g.
                        // @font-face {
                        //   // ...
                        //   src: local('Abel'), url(https://fonts.gstatic.com/s/abel/v6/UzN-iejR1VoXU2Oc-7LsbvesZW2xOQ-xsNqO47m55DA.woff2);
                        // }
                        //
                        // This regex will save extrnal link into first capture group
                        var fontUrlRegexp = /url\(["']?(.+?)["']?\)/;
                        // TODO: This needs to be changed to support multiple url declarations per font.
                        var fontUrlMatch = rule.cssText.match(fontUrlRegexp);
                        var externalFontUrl = (fontUrlMatch && fontUrlMatch[1]) || '';
                        var fontUrlIsDataURI = externalFontUrl.match(/^data:/);
                        if (fontUrlIsDataURI) {
                            // We should ignore data uri - they are already embedded
                            externalFontUrl = '';
                        }
                        if (externalFontUrl === 'about:blank') {
                            // no point trying to load this
                            externalFontUrl = '';
                        }
                        if (externalFontUrl) {
                            // okay, we are lucky. We can fetch this font later
                            //handle url if relative
                            if (externalFontUrl["startsWith"]('../')) {
                                externalFontUrl = sheetHref + '/../' + externalFontUrl;
                            }
                            else if (externalFontUrl["startsWith"]('./')) {
                                externalFontUrl = sheetHref + '/.' + externalFontUrl;
                            }
                            fontsQueue.push({
                                text: rule.cssText,
                                // Pass url regex, so that once font is downladed, we can run `replace()` on it
                                fontUrlRegexp: fontUrlRegexp,
                                format: styles.getFontMimeTypeFromUrl(externalFontUrl),
                                url: externalFontUrl
                            });
                        }
                        else {
                            // otherwise, use previous logic
                            css += rule.cssText + '\n';
                        }
                    }
                }
                return css;
            };
            styles.processFontQueue = function (queue, css, cssLoadedCallback) {
                var style = this;
                if (queue.length > 0) {
                    // load fonts one by one until we have anything in the queue:
                    var font = queue.pop();
                    processNext(font);
                }
                else {
                    // no more fonts to load.
                    cssLoadedCallback(css);
                }
                /**
                 * 在这里通过网络下载字体文件，然后序列化为base64字符串，最后以URI的形式写入到SVG之中
                */
                function processNext(font) {
                    // TODO: This could benefit from caching.
                    var oReq = new XMLHttpRequest();
                    oReq.addEventListener('load', fontLoaded);
                    oReq.addEventListener('error', transferFailed);
                    oReq.addEventListener('abort', transferFailed);
                    oReq.open('GET', font.url);
                    oReq.responseType = 'arraybuffer';
                    oReq.send();
                    function fontLoaded() {
                        // TODO: it may be also worth to wait until fonts are fully loaded before
                        // attempting to rasterize them. (e.g. use https://developer.mozilla.org/en-US/docs/Web/API/FontFaceSet )
                        var fontBits = oReq.response;
                        var fontInBase64 = DataExtensions.arrayBufferToBase64(fontBits);
                        updateFontStyle(font, fontInBase64);
                    }
                    function transferFailed(e) {
                        console.warn('Failed to load font from: ' + font.url);
                        console.warn(e);
                        css += font.text + '\n';
                        style.processFontQueue(queue, css, cssLoadedCallback);
                    }
                    function updateFontStyle(font, fontInBase64) {
                        var dataUrl = "url(\"data:" + font.format + ";base64," + fontInBase64 + "\")";
                        css += font.text.replace(font.fontUrlRegexp, dataUrl) + '\n';
                        // schedule next font download on next tick.
                        setTimeout(function () {
                            style.processFontQueue(queue, css, cssLoadedCallback);
                        }, 0);
                    }
                }
            };
            styles.getFontMimeTypeFromUrl = function (fontUrl) {
                var extensions = Object.keys(supportedFormats);
                for (var i = 0; i < extensions.length; ++i) {
                    var extension = extensions[i];
                    // TODO: This is not bullet proof, it needs to handle edge cases...
                    if (fontUrl.indexOf('.' + extension) > 0) {
                        return supportedFormats[extension];
                    }
                }
                this.warnFontNotSupport(fontUrl);
                return 'application/octet-stream';
            };
            styles.warnFontNotSupport = function (fontUrl) {
                // If you see this error message, you probably need to update code above.
                console.warn("Unknown font format for " + fontUrl + "; Fonts may not be working correctly");
            };
            return styles;
        }());
        saveSvgAsPng.styles = styles;
        var supportedFormats = {
            'woff2': 'font/woff2',
            'woff': 'font/woff',
            'otf': 'application/x-font-opentype',
            'ttf': 'application/x-font-ttf',
            'eot': 'application/vnd.ms-fontobject',
            'sfnt': 'application/font-sfnt',
            'svg': 'image/svg+xml'
        };
        var font = /** @class */ (function () {
            function font() {
            }
            return font;
        }());
    })(saveSvgAsPng = CanvasHelper.saveSvgAsPng || (CanvasHelper.saveSvgAsPng = {}));
})(CanvasHelper || (CanvasHelper = {}));
/// <reference path="../../Data/StackTrace/StackTrace.ts" />
/**
 * 键值对映射哈希表
*/
var Dictionary = /** @class */ (function (_super) {
    __extends(Dictionary, _super);
    /**
     * 将目标对象转换为一个类型约束的映射序列集合
    */
    function Dictionary(maps) {
        if (maps === void 0) { maps = null; }
        var _this = _super.call(this, Dictionary.ObjectMaps(maps)) || this;
        if (isNullOrUndefined(maps)) {
            _this.maps = {};
        }
        else if (Array.isArray(maps)) {
            _this.maps = TypeInfo.CreateObject(maps);
        }
        else if (TypeInfo.typeof(maps).class == "IEnumerator") {
            _this.maps = TypeInfo.CreateObject(maps);
        }
        else {
            _this.maps = maps;
        }
        return _this;
    }
    Object.defineProperty(Dictionary.prototype, "Object", {
        get: function () {
            return Linq.extend(this.maps);
        },
        enumerable: true,
        configurable: true
    });
    /**
     * 如果键名称是空值的话，那么这个函数会自动使用caller的函数名称作为键名进行值的获取
     *
     * https://stackoverflow.com/questions/280389/how-do-you-find-out-the-caller-function-in-javascript
     *
     * @param key 键名或者序列的索引号
    */
    Dictionary.prototype.Item = function (key) {
        if (key === void 0) { key = null; }
        if (!key) {
            key = TsLinq.StackTrace.GetCallerMember().memberName;
        }
        if (typeof key == "string") {
            return (this.maps[key]);
        }
        else {
            return this.sequence[key].value;
        }
    };
    Object.defineProperty(Dictionary.prototype, "Keys", {
        /**
         * 获取这个字典对象之中的所有的键名
        */
        get: function () {
            return From(Object.keys(this.maps));
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Dictionary.prototype, "Values", {
        /**
         * 获取这个字典对象之中的所有的键值
        */
        get: function () {
            var _this = this;
            return this.Keys.Select(function (key) { return _this.Item(key); });
        },
        enumerable: true,
        configurable: true
    });
    Dictionary.FromMaps = function (maps) {
        return new Dictionary(maps);
    };
    Dictionary.FromNamedValues = function (values) {
        return new Dictionary(TypeInfo.CreateObject(values));
    };
    /**
     * 将目标对象转换为一个类型约束的映射序列集合
    */
    Dictionary.ObjectMaps = function (maps) {
        var type = TypeInfo.typeof(maps);
        if (isNullOrUndefined(maps)) {
            return [];
        }
        if (Array.isArray(maps)) {
            return maps;
        }
        else if (type.class == "IEnumerator") {
            return maps.ToArray();
        }
        else {
            return From(Object.keys(maps))
                .Select(function (key) { return new Map(key, maps[key]); })
                .ToArray();
        }
    };
    /**
     * 查看这个字典集合之中是否存在所给定的键名
    */
    Dictionary.prototype.ContainsKey = function (key) {
        return key in this.maps;
    };
    /**
     * 向这个字典对象之中添加一个键值对，请注意，如果key已经存在这个字典对象中了，这个函数会自动覆盖掉key所对应的原来的值
    */
    Dictionary.prototype.Add = function (key, value) {
        this.maps[key] = value;
        this.sequence = Dictionary.ObjectMaps(this.maps);
        return this;
    };
    /**
     * 删除一个给定键名所指定的键值对
    */
    Dictionary.prototype.Delete = function (key) {
        if (key in this.maps) {
            delete this.maps[key];
            this.sequence = Dictionary.ObjectMaps(this.maps);
        }
        return this;
    };
    return Dictionary;
}(IEnumerator));
/// <reference path="Enumerator.ts" />
/**
 * The linq pipline implements at here. (在这个模块之中实现具体的数据序列算法)
*/
var Enumerable;
(function (Enumerable) {
    /**
     * 进行数据序列的投影操作
     *
    */
    function Select(source, project) {
        var projections = [];
        source.forEach(function (o, i) {
            projections.push(project(o, i));
        });
        return new IEnumerator(projections);
    }
    Enumerable.Select = Select;
    /**
     * 进行数据序列的排序操作
     *
    */
    function OrderBy(source, key) {
        // array clone
        var clone = source.slice();
        clone.sort(function (a, b) {
            // a - b
            return key(a) - key(b);
        });
        // console.log("clone");
        // console.log(clone);
        return new IEnumerator(clone);
    }
    Enumerable.OrderBy = OrderBy;
    function OrderByDescending(source, key) {
        return Enumerable.OrderBy(source, function (e) {
            // b - a
            return -key(e);
        });
    }
    Enumerable.OrderByDescending = OrderByDescending;
    function Take(source, n) {
        var takes = [];
        var len = source.length;
        for (var i = 0; i < n; i++) {
            if (i > len) {
                break;
            }
            else {
                takes.push(source[i]);
            }
        }
        return new IEnumerator(takes);
    }
    Enumerable.Take = Take;
    function Skip(source, n) {
        var takes = [];
        if (n >= source.length) {
            return new IEnumerator([]);
        }
        for (var i = n; i < source.length; i++) {
            takes.push(source[i]);
        }
        return new IEnumerator(takes);
    }
    Enumerable.Skip = Skip;
    function TakeWhile(source, predicate) {
        var takes = [];
        for (var i = 0; i < source.length; i++) {
            if (predicate(source[i])) {
                takes.push(source[i]);
            }
            else {
                break;
            }
        }
        return new IEnumerator(takes);
    }
    Enumerable.TakeWhile = TakeWhile;
    function Where(source, predicate) {
        var takes = [];
        source.forEach(function (o) {
            if (predicate(o)) {
                takes.push(o);
            }
        });
        return new IEnumerator(takes);
    }
    Enumerable.Where = Where;
    function SkipWhile(source, predicate) {
        for (var i = 0; i < source.length; i++) {
            if (predicate(source[i])) {
                // skip
            }
            else {
                // end skip
                return Enumerable.Skip(source, i);
            }
        }
        // skip all
        return new IEnumerator([]);
    }
    Enumerable.SkipWhile = SkipWhile;
    function All(source, predicate) {
        for (var i = 0; i < source.length; i++) {
            if (!predicate(source[i])) {
                return false;
            }
        }
        return true;
    }
    Enumerable.All = All;
    function Any(source, predicate) {
        for (var i = 0; i < source.length; i++) {
            if (predicate(source[i])) {
                return true;
            }
        }
        return false;
    }
    Enumerable.Any = Any;
    /**
     * Implements a ``group by`` operation by binary tree data structure.
    */
    function GroupBy(source, getKey, compares) {
        var tree = new algorithm.BTree.binaryTree(compares);
        source.forEach(function (obj) {
            var key = getKey(obj);
            var list = tree.find(key);
            if (list) {
                list.push(obj);
            }
            else {
                tree.add(key, [obj]);
            }
        });
        console.log(tree);
        return tree.AsEnumerable().Select(function (node) {
            return new Group(node.key, node.value);
        });
    }
    Enumerable.GroupBy = GroupBy;
    function AllKeys(sequence) {
        return From(sequence)
            .Select(function (o) { return Object.keys(o); })
            .Unlist()
            .Distinct()
            .ToArray();
    }
    Enumerable.AllKeys = AllKeys;
    var JoinHelper = /** @class */ (function () {
        function JoinHelper(x, y) {
            this.xset = x;
            this.yset = y;
            this.keysT = AllKeys(x);
            this.keysU = AllKeys(y);
        }
        JoinHelper.prototype.JoinProject = function (x, y) {
            var out = {};
            this.keysT.forEach(function (k) { return out[k] = x[k]; });
            this.keysU.forEach(function (k) { return out[k] = y[k]; });
            return out;
        };
        JoinHelper.prototype.Union = function (tKey, uKey, compare, project) {
            if (project === void 0) { project = this.JoinProject; }
            var tree = this.buildUtree(uKey, compare);
            var output = [];
            var keyX = new algorithm.BTree.binaryTree(compare);
            this.xset.forEach(function (x) {
                var key = tKey(x);
                var list = tree.find(key);
                if (list) {
                    // 有交集，则进行叠加投影
                    list.forEach(function (y) { return output.push(project(x, y)); });
                    if (!keyX.find(key)) {
                        keyX.add(key);
                    }
                }
                else {
                    // 没有交集，则投影空对象
                    output.push(project(x, {}));
                }
            });
            this.yset.forEach(function (y) {
                var key = uKey(y);
                if (!keyX.find(key)) {
                    // 没有和X进行join，则需要union到最终的结果之中
                    // 这个y是找不到对应的x元素的
                    output.push(project({}, y));
                }
            });
            return new IEnumerator(output);
        };
        JoinHelper.prototype.buildUtree = function (uKey, compare) {
            var tree = new algorithm.BTree.binaryTree(compare);
            this.yset.forEach(function (obj) {
                var key = uKey(obj);
                var list = tree.find(key);
                if (list) {
                    list.push(obj);
                }
                else {
                    tree.add(key, [obj]);
                }
            });
            return tree;
        };
        JoinHelper.prototype.LeftJoin = function (tKey, uKey, compare, project) {
            if (project === void 0) { project = this.JoinProject; }
            var tree = this.buildUtree(uKey, compare);
            var output = [];
            this.xset.forEach(function (x) {
                var key = tKey(x);
                var list = tree.find(key);
                if (list) {
                    // 有交集，则进行叠加投影
                    list.forEach(function (y) { return output.push(project(x, y)); });
                }
                else {
                    // 没有交集，则投影空对象
                    output.push(project(x, {}));
                }
            });
            return new IEnumerator(output);
        };
        return JoinHelper;
    }());
    Enumerable.JoinHelper = JoinHelper;
})(Enumerable || (Enumerable = {}));
/**
 * 按照某一个键值进行分组的集合对象
*/
var Group = /** @class */ (function (_super) {
    __extends(Group, _super);
    function Group(key, group) {
        var _this = _super.call(this, group) || this;
        _this.Key = key;
        return _this;
    }
    Object.defineProperty(Group.prototype, "Group", {
        /**
         * Group members, readonly property.
        */
        get: function () {
            return this.sequence;
        },
        enumerable: true,
        configurable: true
    });
    /**
     * 创建一个键值对映射序列，这些映射都具有相同的键名
    */
    Group.prototype.ToMaps = function () {
        var _this = this;
        return From(this.sequence)
            .Select(function (x) { return new Map(_this.Key, x); })
            .ToArray();
    };
    return Group;
}(IEnumerator));
/**
 * 表示一个动态列表对象
*/
var List = /** @class */ (function (_super) {
    __extends(List, _super);
    function List(src) {
        if (src === void 0) { src = null; }
        return _super.call(this, src || []) || this;
    }
    /**
     * 可以使用这个方法进行静态代码的链式添加
    */
    List.prototype.Add = function (x) {
        this.sequence.push(x);
        return this;
    };
    /**
     * 批量的添加
    */
    List.prototype.AddRange = function (x) {
        var _this = this;
        if (Array.isArray(x)) {
            x.forEach(function (o) { return _this.sequence.push(o); });
        }
        else {
            x.ForEach(function (o) { return _this.sequence.push(o); });
        }
        return this;
    };
    /**
     * 查找给定的元素在当前的这个列表之中的位置，不存在则返回-1
    */
    List.prototype.IndexOf = function (x) {
        return this.sequence.indexOf(x);
    };
    /**
     * 返回列表之中的第一个元素，然后删除第一个元素，剩余元素整体向前平移一个单位
    */
    List.prototype.Pop = function () {
        var x1 = this.First;
        this.sequence = this.sequence.slice(1);
        return x1;
    };
    return List;
}(IEnumerator));
/**
 * 描述了一个键值对集合
*/
var Map = /** @class */ (function () {
    /**
     * 创建一个新的键值对集合
     *
    */
    function Map(key, value) {
        if (key === void 0) { key = null; }
        if (value === void 0) { value = null; }
        this.key = key;
        this.value = value;
    }
    Map.prototype.toString = function () {
        return "[" + this.key.toString() + ", " + this.value.toString() + "]";
    };
    return Map;
}());
/**
 * 描述了一个带有名字属性的变量值
*/
var NamedValue = /** @class */ (function () {
    function NamedValue(name, val) {
        if (name === void 0) { name = null; }
        if (val === void 0) { val = null; }
        this.name = name;
        this.value = val;
    }
    Object.defineProperty(NamedValue.prototype, "TypeOfValue", {
        /**
         * 获取得到变量值的类型定义信息
        */
        get: function () {
            return TypeInfo.typeof(this.value);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(NamedValue.prototype, "IsEmpty", {
        /**
         * 这个之对象是否是空的？
        */
        get: function () {
            return Strings.Empty(this.name) && (!this.value || this.value == undefined);
        },
        enumerable: true,
        configurable: true
    });
    NamedValue.prototype.toString = function () {
        return this.name;
    };
    return NamedValue;
}());
/**
 * HTML文档操作帮助函数
*/
var Linq;
(function (Linq) {
    var DOM;
    (function (DOM) {
        function download(name, uri) {
            if (navigator.msSaveOrOpenBlob) {
                navigator.msSaveOrOpenBlob(DataExtensions.uriToBlob(uri), name);
            }
            else {
                var saveLink = document.createElement('a');
                var downloadSupported = 'download' in saveLink;
                if (downloadSupported) {
                    saveLink.download = name;
                    saveLink.style.display = 'none';
                    document.body.appendChild(saveLink);
                    try {
                        var blob = DataExtensions.uriToBlob(uri);
                        var url = URL.createObjectURL(blob);
                        saveLink.href = url;
                        saveLink.onclick = function () {
                            requestAnimationFrame(function () {
                                URL.revokeObjectURL(url);
                            });
                        };
                    }
                    catch (e) {
                        console.warn('This browser does not support object URLs. Falling back to string URL.');
                        saveLink.href = uri;
                    }
                    saveLink.click();
                    document.body.removeChild(saveLink);
                }
                else {
                    window.open(uri, '_temp', 'menubar=no,toolbar=no,status=no');
                }
            }
        }
        DOM.download = download;
        function clientSize() {
            var w = window, d = document, e = d.documentElement, g = d.getElementsByTagName('body')[0], x = w.innerWidth || e.clientWidth || g.clientWidth, y = w.innerHeight || e.clientHeight || g.clientHeight;
            return [x, y];
        }
        DOM.clientSize = clientSize;
        /**
         * 向指定id编号的div添加select标签的组件
        */
        function AddSelectOptions(items, div, selectName, className) {
            if (className === void 0) { className = ""; }
            var options = From(items)
                .Select(function (item) { return "<option value=\"" + item.value + "\">" + item.key + "</option>"; })
                .JoinBy("\n");
            var html = "\n            <select class=\"" + className + "\" multiple name=\"" + selectName + "\">\n                " + options + "\n            </select>";
            $ts("#" + div).innerHTML = html;
        }
        DOM.AddSelectOptions = AddSelectOptions;
        /**
         * 向给定编号的div对象之中添加一个表格对象
         *
         * @param headers 表头
         * @param div 新生成的table将会被添加在这个div之中
         * @param attrs ``<table>``的属性值，包括id，class等
        */
        function AddHTMLTable(rows, headers, div, attrs) {
            if (attrs === void 0) { attrs = null; }
            var thead = $ts("<thead>");
            var tbody = $ts("<tbody>");
            var table = $ts("<table id=\"" + div + "-table\">");
            if (attrs) {
                if (attrs.id) {
                    table.id = attrs.id;
                }
                if (!IsNullOrEmpty(attrs.classList)) {
                    attrs.classList.forEach(function (c) { return table.classList.add(c); });
                }
                if (!IsNullOrEmpty(attrs.attrs)) {
                    From(attrs.attrs)
                        .Where(function (a) { return a.name != "id" && a.name != "class"; })
                        .ForEach(function (a) {
                        table.setAttribute(a.name, a.value);
                    });
                }
            }
            var fields = headerMaps(headers);
            rows.forEach(function (r) {
                var tr = $ts("<tr>");
                fields.forEach(function (m) {
                    var td = $ts("<td>");
                    td.innerHTML = r[m.key];
                    tr.appendChild(td);
                });
                tbody.appendChild(tr);
            });
            fields.forEach(function (r) {
                var th = $ts("th");
                th.innerHTML = r.value;
                thead.appendChild(th);
            });
            table.appendChild(thead);
            table.appendChild(tbody);
            $ts(div).appendChild(table);
        }
        DOM.AddHTMLTable = AddHTMLTable;
        function headerMaps(headers) {
            var type = TypeInfo.typeof(headers);
            if (type.IsArrayOf("string")) {
                return From(headers)
                    .Select(function (h) { return new Map(h, h); })
                    .ToArray();
            }
            else if (type.IsArrayOf("Map")) {
                return headers;
            }
            else if (type.IsEnumerator && typeof headers[0] == "string") {
                return headers
                    .Select(function (h) { return new Map(h, h); })
                    .ToArray();
            }
            else if (type.IsEnumerator && TypeInfo.typeof(headers[0]).class == "Map") {
                return headers.ToArray();
            }
            else {
                throw "Invalid sequence type: " + type.class;
            }
        }
        /**
         * Execute a given function when the document is ready.
         *
         * @param fn A function that without any parameters
        */
        function ready(fn) {
            if (typeof fn !== 'function') {
                // Sanity check
                return;
            }
            if (document.readyState === 'complete') {
                // If document is already loaded, run method
                return fn();
            }
            else {
                // Otherwise, wait until document is loaded
                document.addEventListener('DOMContentLoaded', fn, false);
            }
        }
        DOM.ready = ready;
        /**
         * 向一个给定的HTML元素或者HTML元素的集合之中的对象添加给定的事件
         *
         * @param el HTML节点元素或者节点元素的集合
         * @param type 事件的名称字符串
         * @param fn 对事件名称所指定的事件进行处理的工作函数，这个工作函数应该具备有一个事件对象作为函数参数
        */
        function addEvent(el, type, fn) {
            if (document.addEventListener) {
                if (el && (el.nodeName) || el === window) {
                    el.addEventListener(type, fn, false);
                }
                else if (el && el.length) {
                    for (var i = 0; i < el.length; i++) {
                        addEvent(el[i], type, fn);
                    }
                }
            }
            else {
                if (el && el.nodeName || el === window) {
                    el.attachEvent('on' + type, function () {
                        return fn.call(el, window.event);
                    });
                }
                else if (el && el.length) {
                    for (var i = 0; i < el.length; i++) {
                        addEvent(el[i], type, fn);
                    }
                }
            }
        }
        DOM.addEvent = addEvent;
    })(DOM = Linq.DOM || (Linq.DOM = {}));
})(Linq || (Linq = {}));
var Linq;
(function (Linq) {
    var DOM;
    (function (DOM) {
        // /**
        //  * Creates an instance of the element for the specified tag.
        //  * @param tagName The name of an element.
        // */
        // createElement<K extends keyof HTMLElementTagNameMap>(tagName: K, options ?: ElementCreationOptions): HTMLElementTagNameMap[K];
        var DOMEnumerator = /** @class */ (function (_super) {
            __extends(DOMEnumerator, _super);
            function DOMEnumerator(elements) {
                return _super.call(this, DOMEnumerator.ensureElements(elements)) || this;
            }
            /**
             * 这个函数确保所传递进来的集合总是输出一个数组，方便当前的集合对象向其基类型传递数据源
            */
            DOMEnumerator.ensureElements = function (elements) {
                var type = TypeInfo.typeof(elements);
                var list;
                /**
                 * TypeInfo {typeOf: "object", class: "NodeList", property: Array(2), methods: Array(5)}
                 * IsArray: false
                 * IsEnumerator: false
                 * IsPrimitive: false
                 * class: "NodeList"
                 * methods: (5) ["item", "entries", "forEach", "keys", "values"]
                 * property: (2) ["0", "1"]
                 * typeOf: "object"
                */
                if (type.class == "NodeList") {
                    list = [];
                    elements.forEach(function (x) { return list.push(x); });
                }
                else {
                    list = Linq.EnsureArray(elements);
                }
                return list;
            };
            /**
             * 使用这个函数进行节点值的设置或者获取
             *
             * @param value 如果需要批量清除节点之中的值的话，需要传递一个空字符串，而非空值
            */
            DOMEnumerator.prototype.val = function (value) {
                if (value === void 0) { value = null; }
                if (!(value == null && value == undefined)) {
                    if (typeof value == "string") {
                        // 所有元素都设置同一个值
                        this.ForEach(function (element) {
                            element.nodeValue = value;
                        });
                    }
                    else if (Array.isArray(value)) {
                        this.ForEach(function (element, i) {
                            element.nodeValue = value[i];
                        });
                    }
                    else {
                        this.ForEach(function (element, i) {
                            element.nodeValue = value.ElementAt(i);
                        });
                    }
                }
                return this.Select(function (element) { return element.nodeValue; });
            };
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
            DOMEnumerator.prototype.attr = function (attrName, val) {
                if (val === void 0) { val = null; }
                if (val) {
                    if (typeof val == "function") {
                        return this.Select(function (x) {
                            var value = val(x);
                            x.setAttribute(attrName, value);
                            return value;
                        });
                    }
                    else {
                        var array = Linq.EnsureArray(val, this.Count);
                        return this.Select(function (x, i) {
                            var value = array[i];
                            x.setAttribute(attrName, value);
                            return value;
                        });
                    }
                }
                else {
                    return this.Select(function (x) { return x.getAttribute(attrName); });
                }
            };
            DOMEnumerator.prototype.AddClass = function (className) {
                this.ForEach(function (x) {
                    if (!x.classList.contains(className)) {
                        x.classList.add(className);
                    }
                });
                return this;
            };
            DOMEnumerator.prototype.AddEvent = function (eventName, handler) {
                this.ForEach(function (element) {
                    var event = function (Event) {
                        handler(element, Event);
                    };
                    Linq.DOM.addEvent(element, eventName, event);
                });
            };
            DOMEnumerator.prototype.onChange = function (handler) {
                this.AddEvent("onchange", handler);
            };
            /**
             * 为当前的html节点集合添加鼠标点击事件处理函数
            */
            DOMEnumerator.prototype.onClick = function (handler) {
                this.ForEach(function (element) {
                    element.onclick = function (ev) {
                        handler(this, ev);
                        return false;
                    };
                });
            };
            DOMEnumerator.prototype.RemoveClass = function (className) {
                this.ForEach(function (x) {
                    if (x.classList.contains(className)) {
                        x.classList.remove(className);
                    }
                });
                return this;
            };
            /**
             * 通过设置css之中的display值来将集合之中的所有的节点元素都隐藏掉
            */
            DOMEnumerator.prototype.hide = function () {
                this.ForEach(function (x) { return x.style.display = "none"; });
                return this;
            };
            /**
             * 通过设置css之中的display值来将集合之中的所有的节点元素都显示出来
            */
            DOMEnumerator.prototype.show = function () {
                this.ForEach(function (x) { return x.style.display = "block"; });
                return this;
            };
            /**
             * 将所选定的节点批量删除
            */
            DOMEnumerator.prototype.Delete = function () {
                this.ForEach(function (x) { return x.parentNode.removeChild(x); });
            };
            return DOMEnumerator;
        }(IEnumerator));
        DOM.DOMEnumerator = DOMEnumerator;
    })(DOM = Linq.DOM || (Linq.DOM = {}));
})(Linq || (Linq = {}));
var Linq;
(function (Linq) {
    var DOM;
    (function (DOM) {
        /**
         * HTML文档节点的查询类型
        */
        var QueryTypes;
        (function (QueryTypes) {
            QueryTypes[QueryTypes["NoQuery"] = 0] = "NoQuery";
            /**
             * 表达式为 #xxx
             * 按照节点的id编号进行查询
             *
             * ``<tag id="xxx">``
            */
            QueryTypes[QueryTypes["id"] = 1] = "id";
            /**
             * 表达式为 .xxx
             * 按照节点的class名称进行查询
             *
             * ``<tag class="xxx">``
            */
            QueryTypes[QueryTypes["class"] = 10] = "class";
            /**
             * 表达式为 xxx
             * 按照节点的名称进行查询
             *
             * ``<xxx ...>``
            */
            QueryTypes[QueryTypes["tagName"] = -100] = "tagName";
            /**
             * query meta tag content value by name
             *
             * ``@xxxx``
             *
             * ```html
             * <meta name="user-login" content="xieguigang" />
             * ```
            */
            QueryTypes[QueryTypes["QueryMeta"] = 200] = "QueryMeta";
        })(QueryTypes = DOM.QueryTypes || (DOM.QueryTypes = {}));
        var Query = /** @class */ (function () {
            function Query() {
            }
            /**
             * + ``#`` by id
             * + ``.`` by claSS
             * + ``&`` SINGLE NODE
             * + ``@`` read meta tag
             * + ``&lt;>`` create new tag
            */
            Query.parseQuery = function (expr) {
                var isSingle = false;
                if (expr.charAt(0) == "&") {
                    isSingle = true;
                    expr = expr.substr(1);
                }
                else {
                    isSingle = false;
                }
                return Query.parseExpression(expr, isSingle);
            };
            /**
             * by node id
            */
            Query.getById = function (id) {
                return {
                    type: QueryTypes.id,
                    singleNode: true,
                    expression: id
                };
            };
            /**
             * by class name
            */
            Query.getByClass = function (className, isSingle) {
                return {
                    type: QueryTypes.class,
                    singleNode: isSingle,
                    expression: className
                };
            };
            /**
             * by tag name
            */
            Query.getByTag = function (tag, isSingle) {
                return {
                    type: QueryTypes.tagName,
                    singleNode: isSingle,
                    expression: tag
                };
            };
            /**
             * create new node
            */
            Query.createElement = function (expr) {
                return {
                    type: QueryTypes.NoQuery,
                    singleNode: true,
                    expression: expr
                };
            };
            Query.queryMeta = function (expr) {
                return {
                    type: QueryTypes.QueryMeta,
                    singleNode: true,
                    expression: expr
                };
            };
            Query.isSelectorQuery = function (expr) {
                var hasMultiple = expr.indexOf(" ") > -1;
                var isNodeCreate = expr.charAt(0) == "<" && expr.charAt(expr.length - 1) == ">";
                return hasMultiple && !isNodeCreate;
            };
            Query.parseExpression = function (expr, isSingle) {
                var prefix = expr.charAt(0);
                if (Query.isSelectorQuery(expr)) {
                    // 可能是复杂查询表达式
                    return {
                        type: QueryTypes.tagName,
                        singleNode: isSingle,
                        expression: expr
                    };
                }
                switch (prefix) {
                    case "#": return this.getById(expr.substr(1));
                    case ".": return this.getByClass(expr, isSingle);
                    case "<": return this.createElement(expr);
                    case "@": return this.queryMeta(expr.substr(1));
                    default: return this.getByTag(expr, isSingle);
                }
            };
            return Query;
        }());
        DOM.Query = Query;
    })(DOM = Linq.DOM || (Linq.DOM = {}));
})(Linq || (Linq = {}));
var Linq;
(function (Linq) {
    var DOM;
    (function (DOM) {
        var node = /** @class */ (function () {
            function node() {
            }
            node.FromNode = function (htmlNode) {
                var n = new node();
                n.tagName = htmlNode.tagName;
                n.id = htmlNode.id;
                n.classList = this.tokenList(htmlNode.classList);
                n.attrs = this.nameValueMaps(htmlNode.attributes);
                return n;
            };
            node.tokenList = function (tokens) {
                var list = [];
                for (var i = 0; i < tokens.length; i++) {
                    list.push(tokens.item(i));
                }
                return list;
            };
            node.nameValueMaps = function (attrs) {
                var list = [];
                var attr;
                var map;
                for (var i = 0; i < attrs.length; i++) {
                    attr = attrs.item(i);
                    map = new NamedValue(attr.name, attr.value);
                    list.push(map);
                }
                return list;
            };
            return node;
        }());
        DOM.node = node;
    })(DOM = Linq.DOM || (Linq.DOM = {}));
})(Linq || (Linq = {}));
var Linq;
(function (Linq) {
    var DOM;
    (function (DOM) {
        /**
         * 用于解析XML节点之中的属性值的正则表达式
        */
        DOM.attrs = /\S+\s*[=]\s*((["].*["])|(['].*[']))/g;
        /**
         * 将表达式之中的节点名称，以及该节点上面的属性值都解析出来
        */
        function ParseNodeDeclare(expr) {
            // <a href="..." onclick="...">
            var declare = expr
                .substr(1, expr.length - 2)
                .trim();
            var tagValue = Strings.GetTagValue(declare, " ");
            var tag = tagValue.name;
            var attrs = [];
            if (tagValue.value.length > 0) {
                // 使用正则表达式进行解析
                attrs = From(tagValue.value.match(DOM.attrs))
                    .Where(function (s) { return s.length > 0; })
                    .Select(function (s) {
                    var attr = Strings.GetTagValue(s, "=");
                    var val = attr.value.trim();
                    val = val.substr(1, val.length - 2);
                    return new NamedValue(attr.name, val);
                }).ToArray();
            }
            return {
                tag: tag, attrs: attrs
            };
        }
        DOM.ParseNodeDeclare = ParseNodeDeclare;
    })(DOM = Linq.DOM || (Linq.DOM = {}));
})(Linq || (Linq = {}));
// namespace Linq.DOM {
// 2018-10-15
// 为了方便书写代码，在其他脚本之中添加变量类型申明，在这里就不进行命名空间的包裹了
var HTMLTsElement = /** @class */ (function () {
    function HTMLTsElement(node) {
        this.node = node instanceof HTMLElement ?
            node :
            node.node;
    }
    Object.defineProperty(HTMLTsElement.prototype, "HTMLElement", {
        /**
         * 可以从这里获取得到原生的``HTMLElement``对象用于操作
        */
        get: function () {
            return this.node;
        },
        enumerable: true,
        configurable: true
    });
    /**
     * 这个拓展函数总是会将节点中的原来的内容清空，然后显示html函数参数
     * 所给定的内容
    */
    HTMLTsElement.prototype.display = function (html) {
        if (!html) {
            this.HTMLElement.innerHTML = "";
        }
        else if (typeof html == "string") {
            this.HTMLElement.innerHTML = html;
        }
        else {
            var node = html instanceof HTMLTsElement ?
                html.HTMLElement :
                html;
            var parent = this.HTMLElement;
            parent.innerHTML = "";
            parent.appendChild(node);
        }
        return this;
    };
    HTMLTsElement.prototype.addClass = function (className) {
        var node = this.HTMLElement;
        if (!node.classList.contains(className)) {
            node.classList.add(className);
        }
        return this;
    };
    HTMLTsElement.prototype.removeClass = function (className) {
        var node = this.HTMLElement;
        if (node.classList.contains(className)) {
            node.classList.remove(className);
        }
        return this;
    };
    HTMLTsElement.prototype.append = function (node) {
        if (node instanceof HTMLTsElement) {
            this.HTMLElement.appendChild(node.HTMLElement);
        }
        else {
            this.HTMLElement.appendChild(node);
        }
        return this;
    };
    /**
     * 将css的display属性值设置为block用来显示当前的节点
    */
    HTMLTsElement.prototype.show = function () {
        this.HTMLElement.style.display = "block";
        return this;
    };
    /**
     * 将css的display属性值设置为none来隐藏当前的节点
    */
    HTMLTsElement.prototype.hide = function () {
        this.HTMLElement.style.display = "none";
        return this;
    };
    return HTMLTsElement;
}());
var Linq;
(function (Linq) {
    var TsQuery;
    (function (TsQuery) {
        /**
         * 这个函数确保给定的id字符串总是以符号``#``开始的
        */
        function EnsureNodeId(str) {
            if (!str) {
                throw "The given node id value is nothing!";
            }
            else if (str[0] == "#") {
                return str;
            }
            else {
                return "#" + str;
            }
        }
        TsQuery.EnsureNodeId = EnsureNodeId;
        /**
         * 字符串格式的值意味着对html文档节点的查询
        */
        var stringEval = /** @class */ (function () {
            function stringEval() {
            }
            stringEval.ensureArguments = function (args) {
                if (isNullOrUndefined(args)) {
                    return TsQuery.Arguments.Default();
                }
                else {
                    var opts = args;
                    // 2018-10-16
                    // 如果不在这里进行判断赋值，则nativeModel属性的值为undefined
                    // 会导致总会判断为true的bug出现
                    if (isNullOrUndefined(opts.nativeModel)) {
                        // 为了兼容以前的代码，在这里总是默认为TRUE
                        opts.nativeModel = true;
                    }
                    return opts;
                }
            };
            stringEval.prototype.doEval = function (expr, type, args) {
                var query = Linq.DOM.Query.parseQuery(expr);
                var argument = stringEval.ensureArguments(args);
                if (query.type == Linq.DOM.QueryTypes.id) {
                    // 按照id查询
                    var node = document.getElementById(query.expression);
                    if (isNullOrUndefined(node)) {
                        console.warn("Unable to found a node which its ID='" + expr + "'!");
                        return null;
                    }
                    else {
                        if (argument.nativeModel) {
                            return stringEval.extends(node);
                        }
                        else {
                            return new HTMLTsElement(node);
                        }
                    }
                }
                else if (query.type == Linq.DOM.QueryTypes.NoQuery) {
                    return stringEval.createNew(expr, argument);
                }
                else if (!query.singleNode) {
                    // 返回节点集合
                    var nodes = document
                        .querySelectorAll(query.expression);
                    var it = new Linq.DOM.DOMEnumerator(nodes);
                    return it;
                }
                else if (query.type == Linq.DOM.QueryTypes.QueryMeta) {
                    return metaValue(query.expression, (args || {})["default"]);
                }
                else {
                    // 只返回第一个满足条件的节点
                    return document.querySelector(query.expression);
                }
            };
            /**
             * 在原生节点模式之下对输入的给定的节点对象添加拓展方法
             *
             * 向HTML节点对象的原型定义之中拓展新的方法和成员属性
             * 这个函数的输出在ts之中可能用不到，主要是应用于js脚本
             * 编程之中
             *
             * @param node 当查询失败的时候是空值
            */
            stringEval.extends = function (node) {
                var obj = node;
                if (isNullOrUndefined(node)) {
                    return null;
                }
                var extendsNode = new HTMLTsElement(node);
                /**
                 * 这个拓展函数总是会将节点中的原来的内容清空，然后显示html函数参数
                 * 所给定的内容
                */
                obj.display = function (html) {
                    extendsNode.display(html);
                    return node;
                };
                obj.show = function () {
                    extendsNode.show();
                    return node;
                };
                obj.hide = function () {
                    extendsNode.hide();
                    return node;
                };
                obj.addClass = function (name) {
                    extendsNode.addClass(name);
                    return node;
                };
                obj.removeClass = function (name) {
                    extendsNode.removeClass(name);
                    return node;
                };
                // 用这个方法可以很方便的从现有的节点进行转换
                // 也可以直接使用new进行构造
                obj.asExtends = extendsNode;
                return node;
            };
            /**
             * 创建新的HTML节点元素
            */
            stringEval.createNew = function (expr, args) {
                var declare = Linq.DOM.ParseNodeDeclare(expr);
                var node = document.createElement(declare.tag);
                declare.attrs.forEach(function (attr) {
                    node.setAttribute(attr.name, attr.value);
                });
                if (args) {
                    TsQuery.Arguments
                        .nameFilter(args)
                        .forEach(function (name) {
                        node.setAttribute(name, args[name]);
                    });
                }
                if (args.nativeModel) {
                    return stringEval.extends(node);
                }
                else {
                    return new HTMLTsElement(node);
                }
            };
            return stringEval;
        }());
        TsQuery.stringEval = stringEval;
    })(TsQuery = Linq.TsQuery || (Linq.TsQuery = {}));
})(Linq || (Linq = {}));
var Linq;
(function (Linq) {
    var TsQuery;
    (function (TsQuery) {
        var Arguments = /** @class */ (function () {
            function Arguments() {
            }
            /**
             * 在创建新的节点的时候，会有一个属性值的赋值过程，
             * 该赋值过程会需要使用这个函数来过滤Arguments的属性值，否则该赋值过程会将Arguments
             * 里面的属性名也进行赋值，可能会造成bug
            */
            Arguments.nameFilter = function (args) {
                var _this = this;
                return From(Object.keys(args))
                    .Where(function (name) { return _this.ArgumentNames.indexOf(name) == -1; })
                    .ToArray();
            };
            Arguments.Default = function () {
                return {
                    caseInSensitive: false,
                    nativeModel: true,
                    defaultValue: ""
                };
            };
            //#endregion
            Arguments.ArgumentNames = Object.keys(new Arguments());
            return Arguments;
        }());
        TsQuery.Arguments = Arguments;
    })(TsQuery = Linq.TsQuery || (Linq.TsQuery = {}));
})(Linq || (Linq = {}));
//# sourceMappingURL=linq.js.map