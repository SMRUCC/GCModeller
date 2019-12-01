/// <reference path="./sprintf.ts" />
/// <reference path="./URLPatterns.ts" />
/// <reference path="../../Collections/DictionaryMaps.ts" />

namespace TypeScript {      

    /**
     * URL组成字符串解析模块
    */
    export class URL {

        /**
         * 域名
        */
        public origin: string;
        public port: number;

        /**
         * 页面的路径
         * 
         * 这是一个绝对路径来的
        */
        public path: string;
        /**
         * URL查询参数
        */
        public get query(): NamedValue<string>[] {
            return this.queryArguments.ToArray(false);
        };

        /**
         * 未经过解析的查询参数的原始字符串
        */
        public queryRawString: string;

        /**
         * 不带拓展名的文件名称
        */
        public fileName: string;
        /**
         * 在URL字符串之中``#``符号后面的所有字符串都是hash值
        */
        public hash: string;
        /**
         * 网络协议名称
        */
        public protocol: string;

        private queryArguments: IEnumerator<NamedValue<string>>;

        /**
         * 在这里解析一个URL字符串
        */
        public constructor(url: string) {
            // http://localhost/router.html#http://localhost/b.html
            var token = Strings.GetTagValue(url, "://");

            this.protocol = token.name; token = Strings.GetTagValue(token.value, "/");
            this.origin = token.name; token = Strings.GetTagValue(token.value, "?");
            this.path = token.name;
            this.fileName = Strings.Empty(this.path) ? "" : TypeScript.PathHelper.basename(this.path);
            this.hash = $from(url.split("#")).Last;

            if (url.indexOf("#") < 0) {
                this.hash = "";
            }

            if (!isNullOrUndefined(this.path)) {
                // 将页面的路径标准化
                // 应该是一个从wwwroot起始的绝对路径
                if (this.path.charAt(0) !== "/") {
                    this.path = `/${this.path}`;
                }
            } else {
                this.path = "/";
            }

            var args: object = URL.UrlQuery(token.value);

            this.queryRawString = token.value;
            this.queryArguments = Dictionary
                .MapSequence<string>(args)
                .Select(m => new NamedValue<string>(m.key, m.value));

            token = Strings.GetTagValue(this.origin, ":");

            this.origin = token.name;
            this.port = Strings.Val(token.value);

            if (this.port == 0) {
                this.port = this.protocol == "https" ? 443 : 80;
            }
        }

        public getArgument(queryName: string, caseSensitive: boolean = true, Default: string = ""): string {
            if (Strings.Empty(queryName, false)) {
                return "";
            } else if (!caseSensitive) {
                queryName = queryName.toLowerCase();
            }

            return this.queryArguments
                .Where(map => caseSensitive ? map.name == queryName : map.name.toLowerCase() == queryName)
                .FirstOrDefault(<any>{ value: Default })
                .value;
        }

        /**
         * 将URL之中的query部分解析为字典对象
        */
        public static UrlQuery(args: string): object {
            if (args) {
                return URLPatterns.parseQueryString(args, false);
            } else {
                return {};
            }
        }

        /**
         * 跳转到url之中的hash编号的文档位置处
         * 
         * @param hash ``#xxx``文档节点编号表达式
        */
        public static JumpToHash(hash: string) {
            // Getting Y of target element
            // Go there directly or some transition
            window.scrollTo(0, $ts(hash).offsetTop);
        }

        /**
         * Set url hash without url jump in document
        */
        public static SetHash(hash: string) {
            if (history.pushState) {
                history.pushState(null, null, hash);
            } else {
                location.hash = hash;
            }
        }

        /**
         * 获取得到当前的url
        */
        public static WindowLocation(): URL {
            return new URL(window.location.href);
        }

        public toString(): string {
            var query = $from(this.query)
                .Select(q => `${q.name}=${encodeURIComponent(q.value)}`)
                .JoinBy("&");
            var url = `${this.protocol}://${this.origin}/${this.path}`;

            if (query) {
                url = url + "?" + query;
            }
            if (this.hash) {
                url = url + "#" + this.hash;
            }

            return url;
        }

        public static Refresh(url: string): string {
            return `${url}&refresh=${Math.random() * 10000}`;
        }

        /**
         * 获取所给定的URL之中的host名称字符串，如果解析失败会返回空值
        */
        public static getHostName(url: string): string {
            var match: RegExpMatchArray = url.match(URLPatterns.hostNamePattern);

            if (match != null && match.length > 2 && typeof match[2] === 'string' && match[2].length > 0) {
                return match[2];
            } else {
                return null;
            }
        }

        /** 
         * 将目标文本之中的所有的url字符串匹配出来
        */
        public static ParseAllUrlStrings(text: string): string[] {
            let urls: string[] = [];

            for (let url of Strings.getAllMatches(text, URLPatterns.urlPattern)) {
                urls.push(url[0]);
            }

            return urls;
        }

        /**
         * 判断所给定的目标字符串是否是一个base64编码的data uri字符串
        */
        public static IsWellFormedUriString(uri: string): boolean {
            return URLPatterns.isAPossibleUrlPattern(uri, URLPatterns.uriPattern);
        }
    }
}