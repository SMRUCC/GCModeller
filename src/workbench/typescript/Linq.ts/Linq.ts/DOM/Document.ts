/**
 * HTML文档操作帮助函数
*/
namespace DOM {

    /**
     * 判断当前的页面是否显示在一个iframe之中
     * 
     * https://stackoverflow.com/questions/326069/how-to-identify-if-a-webpage-is-being-loaded-inside-an-iframe-or-directly-into-t
    */
    export function inIframe(): boolean {
        try {
            return window.self !== window.top;
        } catch (e) {
            return true;
        }
    }

    /**
     * File download helper
     * 
     * @param name The file save name for download operation
     * @param uri The file object to download
    */
    export function download(name: string, uri: string | DataURI, isUrl: boolean = false): void {
        if (!isUrl && navigator.msSaveOrOpenBlob) {
            navigator.msSaveOrOpenBlob(DataExtensions.uriToBlob(uri), name);
        } else {
            downloadImpl(name, uri, isUrl);
        }
    }

    function downloadImpl(name: string, uri: string | DataURI, isUrl: boolean): void {
        var saveLink: HTMLAnchorElement = <any>$ts('<a>');
        var downloadSupported = 'download' in saveLink;

        if (downloadSupported) {
            saveLink.download = name;
            saveLink.style.display = 'none';
            document.body.appendChild(saveLink);

            if (isUrl && typeof uri == "string") {
                saveLink.href = <string>uri;
            } else {
                try {
                    let blob = DataExtensions.uriToBlob(uri);
                    let url = URL.createObjectURL(blob);

                    saveLink.href = url;
                    saveLink.onclick = function () {
                        requestAnimationFrame(function () {
                            URL.revokeObjectURL(url);
                        })
                    };
                } catch (e) {
                    if (TypeScript.logging.outputWarning) {
                        console.warn('This browser does not support object URLs. Falling back to string URL.');
                    }

                    if (typeof uri !== "string") {
                        uri = DataExtensions.toUri(uri);
                    }

                    saveLink.href = uri;
                }
            }

            saveLink.click();
            document.body.removeChild(saveLink);
        } else {
            if (typeof uri !== "string") {
                uri = DataExtensions.toUri(uri);
            }

            window.open(uri, '_temp', 'menubar=no,toolbar=no,status=no');
        }
    }

    /**
     * 尝试获取当前的浏览器的大小
    */
    export function clientSize(): number[] {
        var w = window,
            d = document,
            e = d.documentElement,
            g = d.getElementsByTagName('body')[0],
            x = w.innerWidth || e.clientWidth || g.clientWidth,
            y = w.innerHeight || e.clientHeight || g.clientHeight;

        return [x, y];
    }

    /**
     * 向指定id编号的div添加select标签的组件
     * 
     * @param containerID 这个编号不带有``#``前缀，这个容器可以是一个空白的div或者目标``<select>``标签对象的编号，
     *                    如果目标容器是一个``<select>``标签的时候，则要求selectName和className都必须要是空值
     * @param items 这个数组应该是一个``[title => value]``的键值对列表
    */
    export function AddSelectOptions(
        items: MapTuple<string, string>[],
        containerID: string,
        selectName: string = null,
        className: string = null) {

        var options = $from(items)
            .Select(item => `<option value="${item.value}">${item.key}</option>`)
            .JoinBy("\n");
        var html: string;

        if (isNullOrUndefined(selectName) && isNullOrUndefined(className)) {
            // 是一个<select>标签
            html = options;
        } else {
            html = `
                <select class="${className}" multiple name="${selectName}">
                    ${options}
                </select>`;
        }

        (<HTMLElement>$ts(`#${containerID}`)).innerHTML = html;
    }

    /**
     * @param headers 表格之中所显示的表头列表，也可以通过这个参数来对表格之中
     *   所需要进行显示的列进行筛选以及显示控制：
     *    + 如果这个参数为默认的空值，则说明显示所有的列数据
     *    + 如果这个参数不为空值，则会显示这个参数所指定的列出来
     *    + 可以通过``map [propertyName => display title]``来控制表头的标题输出
    */
    export function CreateHTMLTableNode<T extends {}>(
        rows: T[] | IEnumerator<T>,
        headers: string[] | IEnumerator<string> | IEnumerator<MapTuple<string, string>> | MapTuple<string, string>[] = null,
        attrs: Internal.TypeScriptArgument = null): HTMLTableElement {

        var thead: HTMLElement = $ts("<tr>");
        var tbody: HTMLElement = $ts("<tbody>");
        var fields: MapTuple<string, string>[];

        if (Array.isArray(rows)) {
            fields = headerMaps(headers || $ts(Object.keys(rows[0])));
        } else {
            fields = headerMaps(headers || $ts(Object.keys(rows.First)));
        }

        var rowHTML = function (r: object) {
            var tr: HTMLElement = $ts("<tr>");
            // 在这里将会控制列的显示
            fields.forEach(m => tr.appendChild($ts("<td>").display(r[m.key])));
            return tr;
        }

        if (Array.isArray(rows)) {
            rows.forEach(r => tbody.appendChild(rowHTML(r)));
        } else {
            rows.ForEach(r => tbody.appendChild(rowHTML(r)));
        }

        fields.forEach(r => thead.appendChild($ts("<th>", { id: r.value }).display(r.value)));

        return <HTMLTableElement>$ts("<table>", attrs)
            .asExtends
            .append($ts("<thead>").display(thead))
            .append(tbody)
            .HTMLElement;
    }

    /**
     * 向给定编号的div对象之中添加一个表格对象
     * 
     * @param headers 表头
     * @param div 新生成的table将会被添加在这个div之中，应该是一个带有``#``符号的节点id查询表达式
     * @param attrs ``<table>``的属性值，包括id，class等
    */
    export function AddHTMLTable<T extends {}>(
        rows: T[] | IEnumerator<T>,
        div: string,
        headers: string[] | IEnumerator<string> | IEnumerator<MapTuple<string, string>> | MapTuple<string, string>[] = null,
        attrs: Internal.TypeScriptArgument = null) {

        var id = `${Strings.Trim(div, "#")}-table`;

        if (attrs) {
            if (!attrs.id) { attrs.id = id; }
        } else {
            attrs = { id: id };
        }

        $ts(div).appendChild(CreateHTMLTableNode(rows, headers, attrs));
    }

    /**
     * @param headers ``[propertyName => displayTitle]``
    */
    function headerMaps(headers: string[] | IEnumerator<string> | IEnumerator<MapTuple<string, string>> | MapTuple<string, string>[]): MapTuple<string, string>[] {
        var type = $ts.typeof(headers);

        if (type.isArrayOf("string")) {
            return $from(<string[]>headers)
                .Select(h => new MapTuple<string, string>(h, h))
                .ToArray();
        } else if (type.isArrayOf(TypeExtensions.DictionaryMap)) {
            return <MapTuple<string, string>[]>headers;
        } else if (type.isEnumerator && typeof (<IEnumerator<any>>headers).First == "string") {
            return (<IEnumerator<string>>headers)
                .Select(h => new MapTuple<string, string>(h, h))
                .ToArray();
        } else if (type.isEnumerator && TypeScript.Reflection.getClass((<IEnumerator<any>>headers).First) == TypeExtensions.DictionaryMap) {
            return (<IEnumerator<MapTuple<string, string>>>headers).ToArray();
        } else {
            throw `Invalid sequence type: ${type.class}`;
        }
    }
}