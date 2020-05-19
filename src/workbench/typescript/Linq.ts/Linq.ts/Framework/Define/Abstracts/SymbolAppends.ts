namespace Internal {

    export interface IURL {

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
        hash(arg?: hashArgument | boolean, urlhash?: string): string
    }

    export interface HtmlDocumentDeserializer {

        /**
         * 将目标序列转换为一个表格HTML节点元素
        */
        table: <T extends {}>(
            rows: T[] | IEnumerator<T>,
            headers?: string[] | IEnumerator<string> | IEnumerator<MapTuple<string, string>> | MapTuple<string, string>[],
            attrs?: Internal.TypeScriptArgument) => HTMLTableElement;

        /**
         * 向指定id编号的div添加select标签的组件
         * 
         * @param containerID 这个编号不带有``#``前缀，这个容器可以是一个空白的div或者目标``<select>``标签对象的编号，
         *                    如果目标容器是一个``<select>``标签的时候，则要求selectName和className都必须要是空值
         * @param items 这个数组应该是一个``[title => value]``的键值对列表
        */
        selectOptions: (
            items: MapTuple<string, string>[],
            containerID: string,
            selectName?: string,
            className?: string) => void;
    }

    export interface hashArgument {
        doJump?: boolean;
        trimprefix?: boolean;
    }

    export interface GotoOptions {
        currentFrame?: boolean;
        lambda?: boolean;
    }

    export interface IquerySelector {
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

    export interface IcsvHelperApi {

        /**
         * parse the given text into dataframe object.
        */
        (data: string, isTsv?: boolean | ((data: string) => boolean)): csv.dataframe;

        isTsvFile(data: string): boolean;

        /**
         * 将csv文档文本进行解析，然后反序列化为js对象的集合
        */
        toObjects<T>(data: string): IEnumerator<T>;
        /**
         * 将js的对象序列进行序列化，构建出csv格式的文本文档字符串数据
        */
        toText<T>(data: IEnumerator<T> | T[], outTsv?: boolean): string;

        /**
         * build csv or tsv from target object sequence and then encode as base64 uri
        */
        toUri<T>(data: IEnumerator<T> | T[], outTsv?: boolean): DataURI;
    }
}