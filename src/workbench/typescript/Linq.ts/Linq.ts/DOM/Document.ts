/**
 * HTML文档操作帮助函数
*/
namespace DOM {

    /**
     * Query meta tag content value by name
     * 
     * @param allowQueryParent 当当前的文档之中不存在目标meta标签的时候，
     *    如果当前文档为iframe文档，则是否允许继续往父节点的文档做查询？
     *    默认为False，即只在当前文档环境之中进行查询操作
     * @param Default 查询失败的时候所返回来的默认值
    */
    export function metaValue(name: string, Default: string = null, allowQueryParent: boolean = false): string {
        var selector: string = `meta[name~="${name}"]`;
        var meta: Element = document.querySelector(selector);
        var getContent = function () {
            if (meta) {
                var content: string = meta.getAttribute("content");
                return content ? content : Default;
            } else {
                if (Internal.outputWarning()) {
                    console.warn(`${selector} not found in current context!`);
                }

                return Default;
            }
        };

        if (!meta && allowQueryParent) {
            meta = parent.window
                .document
                .querySelector(selector);
        }

        return getContent();
    }

    /**
     * File download helper
     * 
     * @param name The file save name for download operation
     * @param uri The file object to download
    */
    export function download(name: string, uri: string): void {
        if (navigator.msSaveOrOpenBlob) {
            navigator.msSaveOrOpenBlob(DataExtensions.uriToBlob(uri), name);
        } else {
            downloadImpl(name, uri);
        }
    }

    function downloadImpl(name: string, uri: string): void {
        var saveLink: HTMLAnchorElement = <any>$ts('<a>');
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
                    })
                };
            } catch (e) {
                if (Internal.outputWarning()) {
                    console.warn('This browser does not support object URLs. Falling back to string URL.');
                }

                saveLink.href = uri;
            }

            saveLink.click();
            document.body.removeChild(saveLink);
        } else {
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
     * return array containing references to selected option elements
    */
    export function getSelectedOptions(sel: HTMLSelectElement) {
        var opts: HTMLOptionElement[] = []
        var opt: HTMLOptionElement;

        // loop through options in select list
        for (var i = 0, len = sel.options.length; i < len; i++) {
            opt = sel.options[i];

            // check if selected
            if (opt.selected) {
                // add to array of option elements to return from this function
                opts.push(opt);
            }
        }

        return opts;
    }

    /**
     * 向指定id编号的div添加select标签的组件
    */
    export function AddSelectOptions(
        items: MapTuple<string, string>[],
        div: string,
        selectName: string,
        className: string = "") {

        var options = From(items)
            .Select(item => `<option value="${item.value}">${item.key}</option>`)
            .JoinBy("\n");
        var html: string = `
            <select class="${className}" multiple name="${selectName}">
                ${options}
            </select>`;

        (<HTMLElement>$ts(`#${div}`)).innerHTML = html;
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

        var thead: HTMLElement = $ts("<thead>");
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

        fields.forEach(r => thead.appendChild($ts("<th>").display(r.value)));

        return <HTMLTableElement>$ts("<table>", attrs)
            .asExtends
            .append(thead)
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

        var id = `${div}-table`;

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
        var type = TypeInfo.typeof(headers);

        if (type.IsArrayOf("string")) {
            return From(<string[]>headers)
                .Select(h => new MapTuple<string, string>(h, h))
                .ToArray();
        } else if (type.IsArrayOf(TypeExtensions.DictionaryMap)) {
            return <MapTuple<string, string>[]>headers;
        } else if (type.IsEnumerator && typeof (<IEnumerator<any>>headers).First == "string") {
            return (<IEnumerator<string>>headers)
                .Select(h => new MapTuple<string, string>(h, h))
                .ToArray();
        } else if (type.IsEnumerator && TypeInfo.getClass((<IEnumerator<any>>headers).First) == TypeExtensions.DictionaryMap) {
            return (<IEnumerator<MapTuple<string, string>>>headers).ToArray();
        } else {
            throw `Invalid sequence type: ${type.class}`;
        }
    }

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
    */
    export function ready(fn: () => void, loadComplete: string[] = ["interactive", "complete"]) {
        if (typeof fn !== 'function') {
            // Sanity check
            return;
        } else if (Internal.outputEverything()) {
            console.log("Add Document.ready event handler.");
            console.log(`document.readyState = ${document.readyState}`)
        }

        // 2018-12-25 "interactive", "complete" 这两种状态都可以算作是DOM已经准备好了
        if (loadComplete.indexOf(document.readyState) > -1) {
            // If document is already loaded, run method
            return fn();
        } else {
            // Otherwise, wait until document is loaded
            document.addEventListener('DOMContentLoaded', fn, false);
        }
    }

    /**
     * 向一个给定的HTML元素或者HTML元素的集合之中的对象添加给定的事件
     * 
     * @param el HTML节点元素或者节点元素的集合
     * @param type 事件的名称字符串
     * @param fn 对事件名称所指定的事件进行处理的工作函数，这个工作函数应该具备有一个事件对象作为函数参数
    */
    export function addEvent(el: any, type: string, fn: (event: Event) => void): void {
        if (document.addEventListener) {
            if (el && (el.nodeName) || el === window) {
                (<HTMLElement>el).addEventListener(type, fn, false);
            } else if (el && el.length) {
                for (var i = 0; i < el.length; i++) {
                    addEvent(el[i], type, fn);
                }
            }
        } else {
            if (el && el.nodeName || el === window) {
                el.attachEvent('on' + type, () => {
                    return fn.call(el, window.event);
                });
            } else if (el && el.length) {
                for (var i = 0; i < el.length; i++) {
                    addEvent(el[i], type, fn);
                }
            }
        }
    }
}