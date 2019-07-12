/// <reference path="./Abstracts/TS.ts" />
/// <reference path="../../Data/StringHelpers/URL.ts" />
/// <reference path="../../Data/StringHelpers/PathHelper.ts" />
/// <reference path="../Modes.ts" />
/// <reference path="../../DOM/Document.ts" />
/// <reference path="../../DOM/InputValueGetter.ts" />
/// <reference path="../../Data/Range.ts" />

/**
 * The internal implementation of the ``$ts`` object.
*/
namespace Internal {

    export const StringEval = new Handlers.stringEval();

    /**
     * 对``$ts``对象的内部实现过程在这里
    */
    export function Static<T>(): TypeScript {
        var handle = Internal.Handlers.Shared;
        var ins: any = (any: ((() => void) | T | T[]), args: object) => queryFunction(handle, any, args);

        const stringEval = handle.string();

        ins.mode = Modes.production;

        ins = extendsUtils(ins, stringEval);
        ins = extendsLINQ(ins);
        ins = extendsHttpHelpers(ins);
        ins = extendsSelector(ins);

        return <TypeScript>ins;
    }

    function extendsHttpHelpers(ts: any): any {
        ts.post = function (url: string, data: object | FormData,
            callback?: ((response: IMsg<{}>) => void),
            options?: {
                sendContentType?: boolean
            }) {

            var contentType: string = HttpHelpers.measureContentType(data);
            var post = <HttpHelpers.PostData>{
                type: contentType,
                data: data,
                sendContentType: (options || {}).sendContentType || true
            };

            HttpHelpers.POST(urlSolver(url), post, function (response) {
                if (callback) {
                    callback(handleJSON(response));
                }
            });
        };
        ts.getText = function (url: string, callback: (text: string) => void, options = {
            nullForNotFound: false
        }) {
            HttpHelpers.GetAsyn(urlSolver(url), function (text: string, code: number) {
                if (code != 200) {
                    if (options.nullForNotFound) {
                        callback("");
                    } else {
                        callback(text);
                    }
                } else {
                    callback(text);
                }
            });
        }
        ts.get = function (url: string, callback?: ((response: IMsg<{}>) => void)) {
            HttpHelpers.GetAsyn(urlSolver(url), function (response) {
                if (callback) {
                    callback(handleJSON(response));
                }
            });
        };
        ts.upload = function (url: string, file: File, callback?: ((response: IMsg<{}>) => void)) {
            HttpHelpers.UploadFile(urlSolver(url), file, null, function (response) {
                if (callback) {
                    callback(handleJSON(response));
                }
            });
        };

        ts.location = buildURLHelper();
        ts.parseURL = (url => new TypeScript.URL(url));
        ts.goto = function (url: string, opt: GotoOptions = { currentFrame: false, lambda: false }) {
            if (url.charAt(0) == "#") {
                // url是一个文档节点id表达式，则执行文档内跳转
                TypeScript.URL.JumpToHash(url);
            } else if (opt.lambda) {
                return function () {
                    Goto(url, opt.currentFrame);
                }
            } else {
                Goto(url, opt.currentFrame);
            }
        }

        return ts;
    }

    function buildURLHelper() {
        var url = TypeScript.URL.WindowLocation();
        var location: any = function (
            arg: string,
            caseSensitive: boolean = true,
            Default: string = "") {

            return url.getArgument(arg, caseSensitive, Default);
        }

        location.path = url.path || "/";
        location.fileName = url.fileName;
        location.hash = function (arg: hashArgument | boolean = { trimprefix: true, doJump: false }, urlhash: string = null) {
            if (!isNullOrUndefined(urlhash)) {
                if (((typeof arg == "boolean") && (arg === true)) || (<hashArgument>arg).doJump) {
                    window.location.hash = urlhash;
                } else {
                    TypeScript.URL.SetHash(urlhash);
                }
            } else {
                // 获取当前url字符串之中hash标签值
                var tag = window.location.hash;
                var trimprefix: boolean;

                if (typeof arg == "boolean") {
                    trimprefix = arg;
                } else {
                    trimprefix = arg.trimprefix;
                }

                if (tag && trimprefix && (tag.length > 1)) {
                    return tag.substr(1);
                } else {
                    return isNullOrUndefined(tag) ? "" : tag;
                }
            }
        }

        return location;
    }

    const querySymbols: string[] = [":", "_"];

    function isValidSymbol(c: string): boolean {
        if (querySymbols.indexOf(c) > -1) {
            return true;
        } else {
            return Strings.isNumber(c) || Strings.isAlphabet(c);
        }
    }

    /**
     * 支持对meta标签解析内容的还原
     * 
     * @param url 对于meta标签，只会转义字符串最开始的url部分
    */
    export function urlSolver(url: string, currentFrame: boolean = false): string {
        // var url = "@view:task/user/xyz";
        // 在这里指定标签规则：
        // 1. 以@符号起始，能够包含的符号为冒号:，下划线_，字母和数字，其他的符号都会被看作为结束符号
        // 2. meta查询标签必须位于url字符串的起始位置，否则不进行解析

        if (url.charAt(0) == "@") {
            // 可能是对meta标签的查询
            // 去除第一个@标记符号之后进行查询
            // 因为url可能会带有@，所以可能会出现误查询的情况，所以在这里默认值设置为url
            // 当误查询的时候就会查询不到结果的时候，就可以返回当前的url值了
            var tag: string[] = [];
            var c: string;
            var metaQuery: string;

            // 第一个符号是@符号，跳过
            for (var i: number = 1; i < url.length; i++) {
                if (isValidSymbol(c = url.charAt(i))) {
                    tag.push(c);
                } else {
                    break;
                }
            }

            metaQuery = tag.join("");
            url = DOM.InputValueGetter.metaValue(metaQuery, metaQuery, !currentFrame) + url.substr(tag.length + 1);
        }

        return url;
    }

    function handleJSON(response: any): any {
        if (typeof response == "string") {

            /*
            if (TsLinq.URL.IsWellFormedUriString(response)) {
                // 是一个uri字符串，则不做解析
                return response;
            }*/

            // 尝试以json的格式进行结果的解析
            try {
                return JSON.parse(response);
            } catch (ex) {
                console.error("Invalid json text: ");
                console.error(response);

                throw ex;
            }
        } else {
            return response;
        }
    }

    function extendsUtils(ts: any, stringEval: Handlers.stringEval): any {
        ts.imports = function (
            jsURL: string | string[],
            callback: () => void = DoNothing,
            onErrorResumeNext: boolean = false,
            echo: boolean = false) {

            return new HttpHelpers.Imports(jsURL, onErrorResumeNext, echo).doLoad(callback);
        };
        ts.eval = function (script: string, lzw: boolean = false, callback?: () => void) {
            if (lzw) {
                script = LZW.decode(script);
            }
            HttpHelpers.Imports.doEval(script, callback);
        }
        ts.value = DOM.InputValueGetter.getValue;
        ts.inject = function (iframe: string, fun: (Delegate.Func<any> | string)[] | string | Delegate.Func<any>) {
            var frame: HTMLIFrameElement = <any>$ts(iframe);
            var envir: {
                eval: Delegate.Func<any>
            } = <any>frame.contentWindow;

            if (TypeScript.logging.outputEverything) {
                console.log(fun);
            }

            if (Array.isArray(fun)) {
                for (let p of fun) {
                    envir.eval(p.toString());
                }
            } else if (typeof fun == "string") {
                envir.eval(fun);
            } else {
                envir.eval(fun.toString());
            }
        };
        ts.text = function (id: string, htmlText: boolean = false) {
            var nodeID: string = Handlers.EnsureNodeId(id);
            var node: IHTMLElement = stringEval.doEval(nodeID, null, null);

            return htmlText ? node.innerHTML : node.innerText;
        };
        ts.loadJSON = function (id: string) {
            return JSON.parse(ts.text(id));
        };

        // file path helpers
        ts.parseFileName = TsLinq.PathHelper.fileName;

        /**
         * 得到不带有拓展名的文件名部分的字符串
         * 
         * @param path Full name
        */
        ts.baseName = TsLinq.PathHelper.basename;
        /**
         * 得到不带小数点的文件拓展名字符串
         * 
         * @param path Full name
        */
        ts.extensionName = TsLinq.PathHelper.extensionName;
        ts.withExtensionName = function (path: string, ext: string) {
            var fileExt: string = $ts.extensionName(path);
            var equals: boolean = fileExt.toLowerCase() == ext.toLowerCase();

            return equals;
        };

        ts.doubleRange = data.NumericRange.Create;

        return ts;
    }

    function extendsLINQ(ts: any): any {
        ts.isNullOrEmpty = function (obj: any) {
            return IsNullOrEmpty(obj);
        }
        ts.from = From;
        ts.csv = {
            toObjects: (data: string) => csv.dataframe.Parse(data).Objects(),
            toText: data => csv.toDataFrame(data).buildDoc()
        };
        ts.evalHTML = {
            table: DOM.CreateHTMLTableNode,
            selectOptions: DOM.AddSelectOptions
        };
        ts.appendTable = DOM.AddHTMLTable;

        return ts;
    }

    function extendsSelector(ts: any): any {
        ts.select = function (query: string, context: Window = window) {
            return Handlers.stringEval.select(query, context);
        }
        ts.select.getSelectedOptions = function (query: string, context: Window = window) {
            var sel: HTMLElement = $ts(query, {
                context: context
            });
            var options = DOM.InputValueGetter.getSelectedOptions(<any>sel);

            return new DOMEnumerator<HTMLOptionElement>(options);
        };
        ts.select.getOption = function (query: string, context: Window = window) {
            var sel: HTMLElement = $ts(query, {
                context: context
            });
            var options = DOM.InputValueGetter.getSelectedOptions(<any>sel);

            if (options.length == 0) {
                return null;
            } else {
                return options[0].value;
            }
        };

        return ts;
    }

    export function queryFunction<T>(handle: object, any: ((() => void) | T | T[]), args: object): any {
        var type: TypeInfo = TypeInfo.typeof(any);
        var typeOf: string = type.typeOf;
        // symbol renames due to problem in js compress tool
        //
        // ERROR - "eval" cannot be redeclared in strict mode
        //
        var queryEval: any = typeOf in handle ? handle[typeOf]() : null;
        var isHtmlCollection = (typeOf == "object") && (type.class == "HTMLCollection" || type.class == "NodeListOf");

        if (isHtmlCollection) {
            return Internal.Handlers.Shared.HTMLCollection().doEval(<any>any, type, args);
        } else if (type.IsArray) {
            // 转化为序列集合对象，相当于from函数                
            return (<Handlers.arrayEval<T>>queryEval).doEval(<T[]>any, type, args);
        } else if (type.typeOf == "function") {
            // 当html文档加载完毕之后就会执行传递进来的这个
            // 函数进行初始化
            DOM.Events.ready(<() => void>any);
        } else if (!isNullOrUndefined(eval)) {
            // 对html文档之中的节点元素进行查询操作
            // 或者创建新的节点
            return (<Handlers.IEval<T>>queryEval).doEval(<T>any, type, args);
        } else {
            // Fix for js compress tool error:
            //
            // ERROR - the "eval" object cannot be reassigned in strict mode
            let unsureEval = handle[type.class];

            if (!isNullOrUndefined(unsureEval)) {
                return (<Handlers.IEval<T>>unsureEval()).doEval(<T>any, type, args);
            } else {
                throw `Unsupported data type: ${type.toString()}`;
            }
        }
    }
}