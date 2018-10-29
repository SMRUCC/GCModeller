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
function $ts<T>(any: (() => void) | T | T[], args: object = null): IEnumerator<T> | void | any {
    var type: TypeInfo = TypeInfo.typeof(any);
    var typeOf: string = type.typeOf;
    var handle = Linq.TsQuery.handler;
    var eval: any = typeOf in handle ? handle[typeOf]() : null;

    if (type.IsArray) {
        // 转化为序列集合对象，相当于from函数
        var creator = <Linq.TsQuery.arrayEval<T>>eval;
        return <IEnumerator<T>>creator.doEval(<T[]>any, type, args);
    } else if (type.typeOf == "function") {
        // 当html文档加载完毕之后就会执行传递进来的这个
        // 函数进行初始化
        Linq.DOM.ready(<() => void>any);
    } else {
        // 对html文档之中的节点元素进行查询操作
        // 或者创建新的节点
        return (<Linq.TsQuery.IEval<T>>eval).doEval(<T>any, type, args);
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
const sprintf = data.sprintf.doFormat;

/**
 * Linq数据流程管线的起始函数
 * 
 * @param source 需要进行数据加工的集合对象
*/
function From<T>(source: T[] | IEnumerator<T>): IEnumerator<T> {
    return new IEnumerator<T>(source);
}

/**
 * 将一个给定的字符串转换为组成该字符串的所有字符的枚举器
*/
function CharEnumerator(str: string): IEnumerator<string> {
    return new IEnumerator<string>(Strings.ToCharArray(str));
}

/**
 * Query meta tag content value by name
*/
function metaValue(name: string, Default: string = null): string {
    var meta = document.querySelector(`meta[name~="${name}"]`);
    var content: string;

    if (meta) {
        content = meta.getAttribute("content");
        return content ? content : Default;
    } else {
        return Default;
    }
}

/**
 * 判断目标对象集合是否是空的？
 * 
 * @param array 如果这个数组对象是空值或者未定义，都会被判定为空，如果长度为零，则同样也会被判定为空值
*/
function IsNullOrEmpty<T>(array: T[] | IEnumerator<T>): boolean {
    if (array == null || array == undefined) {
        return true;
    } else if (Array.isArray(array) && array.length == 0) {
        return true;
    } else if ((<IEnumerator<T>>(<any>array)).Count == 0) {
        return true;
    } else {
        return false;
    }
}

/**
 * 查看目标变量的对象值是否是空值或者未定义
*/
function isNullOrUndefined(obj: any): boolean {
    if (obj == null || obj == undefined) {
        return true;
    } else {
        return false;
    }
}

/**
 * HTML/Javascript: how to access JSON data loaded in a script tag.
*/
function LoadJson(id: string): any {
    return JSON.parse(LoadText(id));
}

function LoadText(id: string): string {
    return document.getElementById(id).textContent;
}

/**
 * Quick Tip: Get URL Parameters with JavaScript
 * 
 * > https://www.sitepoint.com/get-url-parameters-with-javascript/
 * 
 * @param url get query string from url (optional) or window
*/
function getAllUrlParams(url: string = window.location.href): Dictionary<string> {
    if (url.indexOf("?") > -1) {
        // if query string exists
        var queryString: string = Strings.GetTagValue(url, '?').value;
        var args = DataExtensions.parseQueryString(queryString)
        return new Dictionary<string>(args);
    } else {
        return new Dictionary<string>({});
    }
}

/**
 * 调用这个函数会从当前的页面跳转到指定URL的页面
*/
function Goto(url: string): void {
    window.location.href = url;
}

/**
 * 这个函数会自动处理多行的情况
*/
function base64_decode(stream: string): string {
    var data: string[] = Strings.lineTokens(stream);
    var base64Str: string = From(data)
        .Where(s => s && s.length > 0)
        .Select(s => s.trim())
        .JoinBy("");
    var text: string = Base64.decode(base64Str);

    return text;
}

/**
 * 这个函数什么也不做，主要是用于默认的参数值
*/
function DoNothing(): any {
    return null;
}