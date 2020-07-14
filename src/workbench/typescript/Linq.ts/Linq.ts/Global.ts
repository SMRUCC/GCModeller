/// <reference path="Data/StringHelpers/sprintf.ts" />
/// <reference path="Collections/Abstract/Enumerator.ts" />
/// <reference path="Framework/Define/Handlers/Handlers.ts" />
/// <reference path="Helpers/Extensions.ts" />
/// <reference path="Helpers/Strings.ts" />
/// <reference path="Framework/Reflection/Type.ts" />
/// <reference path="Data/Encoder/MD5.ts" />
/// <reference path="Framework/Define/Internal.ts" />

// note: 2018-12-25
// this module just working on browser, some of the DOM api
// related function may not works as expected on server side 
// ``nodejs`` Environment.

if (typeof String.prototype['startsWith'] != 'function') {
    String.prototype['startsWith'] = function (str) {
        return this.slice(0, str.length) == str;
    };
}

/**
 * 动态加载脚本文件，然后在完成脚本文件的加载操作之后，执行一个指定的函数操作
 * 
 * @param callback 如果这个函数之中存在有HTML文档的操作，则可能会需要将代码放在``$ts(() => {...})``之中，
 *     等待整个html文档加载完毕之后再做程序的执行，才可能会得到正确的执行结果
*/
function $imports(jsURL: string | string[],
    callback: () => void = DoNothing,
    onErrorResumeNext: boolean = false,
    echo: boolean = false): void {

    return new HttpHelpers
        .Imports(jsURL, onErrorResumeNext, echo)
        .doLoad(callback);
}

/**
 * 使用script标签进行脚本文件的加载
 * 因为需要向body添加script标签，所以这个函数会需要等到文档加载完成之后才会被执行
*/
function $include(jsURL: string | string[]) {
    if (typeof jsURL == "string") {
        jsURL = [jsURL];
    }

    $ts(() => (<string[]>jsURL).forEach(js => {
        var script: HTMLElement = <HTMLElement>$ts("<script>", {
            type: "text/javascript",
            src: js
        });

        script.onload = function () {
            document.body.removeChild(script);
        }
        document.body.appendChild(script);
    }));
}

/**
 * 计算字符串的MD5值字符串
*/
function md5(string: string, key: string = null, raw: string = null): string {
    return MD5.calculate(string, key, raw);
}

/**
 * Linq数据流程管线的起始函数
 * 
 * ``$ts``函数也可以达到与这个函数相同的效果，但是这个函数更快一些
 * 
 * @param source 需要进行数据加工的集合对象
*/
function $from<T>(source: T[] | IEnumerator<T>): IEnumerator<T> {
    return new IEnumerator<T>(source);
}

/**
 * 将一个给定的字符串转换为组成该字符串的所有字符的枚举器
*/
function CharEnumerator(str: string): IEnumerator<string> {
    return new IEnumerator<string>(<string[]>Strings.ToCharArray(str));
}

/**
 * 判断目标对象集合是否是空的？
 * 
 * 这个函数也包含有``isNullOrUndefined``函数的判断功能
 * 
 * @param array 如果这个数组对象是空值或者未定义，都会被判定为空，如果长度为零，则同样也会被判定为空值
*/
function isNullOrEmpty<T>(array: T[] | IEnumerator<T>): boolean {
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
 * 
 * @param id 节点的id值，不带有``#``符号前缀的
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
        var args = TypeScript.URLPatterns.parseQueryString(queryString);

        return new Dictionary<string>(args);
    } else {
        return new Dictionary<string>({});
    }
}

/**
 * 调用这个函数会从当前的页面跳转到指定URL的页面
 * 
 * 如果当前的这个页面是一个iframe页面，则会通过父页面进行跳转
 * 
 * @param url 这个参数支持对meta标签数据的查询操作
 * @param currentFrame 如果这个参数为true，则不会进行父页面的跳转操作
*/
function $goto(url: string, currentFrame: boolean = false): void {
    var win: Window = window;

    if (!currentFrame) {
        // 从最顶层的文档页面进行跳转
        // https://developer.mozilla.org/en-US/docs/Web/API/Window/top
        win = window.top;
    }

    win.location.href = Internal.urlSolver(url, currentFrame);
}

function $download(url: string, rename: string = null) {
    DOM.download(rename, url, true);
}

/**
 * 这个函数会自动处理多行的情况
*/
function base64_decode(stream: string): string {
    var data: string[] = Strings.lineTokens(stream);
    var base64Str: string = $from(data)
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

/**
 * 将指定的SVG节点保存为png图片
 * 
 * @param svg 需要进行保存为图片的svg节点的对象实例或者对象的节点id值
 * @param name 所保存的文件名
 * @param options 配置参数，直接留空使用默认值就好了
*/
function saveSvgAsPng(
    svg: string | SVGElement,
    name: string,
    options: CanvasHelper.saveSvgAsPng.Options = CanvasHelper.saveSvgAsPng.Options.Default()) {

    return CanvasHelper.saveSvgAsPng.Encoder.saveSvgAsPng(svg, name, options);
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
const executeJavaScript: string = "javascript:void(0);";

/**
 * 对于这个函数的返回值还需要做类型转换
 * 
 * 如果是节点查询或者创建的话，可以使用``asExtends``属性来获取``HTMLTsElememnt``拓展对象
*/
const $ts: Internal.TypeScript = Internal.Static();

/**
 * 从文档之中查询或者创建一个新的图像标签元素
*/
const $image = function (query: string | HTMLElement, args?: Internal.TypeScriptArgument): IHTMLImageElement {
    return Internal.typeGenericElement<IHTMLImageElement>(query, args);
};

/**
 * 从文档之中查询或者创建一个新的输入标签元素
*/
const $input = function (query: string | HTMLElement, args?: Internal.TypeScriptArgument): IHTMLInputElement {
    return Internal.typeGenericElement<IHTMLInputElement>(query, args)
};

const $link = function (query: string | HTMLElement, args?: Internal.TypeScriptArgument): IHTMLLinkElement {
    return Internal.typeGenericElement<IHTMLLinkElement>(query, args);
};

const $iframe = function (query: string | HTMLElement, args?: Internal.TypeScriptArgument): HTMLIFrameElement {
    return Internal.typeGenericElement<HTMLIFrameElement>(query, args);
};

/**
 * 进行对象的浅克隆
*/
function $clone<T extends {}>(obj: T): T {
    let copy: {} = {};

    if (!isNullOrUndefined(obj)) {
        for (let name in obj) {
            copy[<any>name] = obj[name];
        }
    } else {
        TypeScript.logging.warning("target object is nothing!");
    }

    return <any>copy;
}