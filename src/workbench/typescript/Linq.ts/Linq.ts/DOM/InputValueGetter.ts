namespace DOM {

    export module InputValueGetter {

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
                    if (TypeScript.logging.outputWarning) {
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

        export function getValue(id: string, strict: boolean = true): any {
            let input = $ts(Internal.Handlers.EnsureNodeId(id));

            switch (input.tagName) {
                case "input": return inputValue(<any>input);
                case "select": return selectOptionValues(<any>input);
                case "textarea": return largeText(<any>input);

                default:
                    if (strict) {
                        throw `Get value of <${input.tagName}> is not supported!`;
                    } else {
                        // 强制读取目标节点的value属性值
                        return input.getAttribute("value");
                    }
            }
        }

        export function inputValue(input: HTMLInputElement): any {
            if (input.type == "checkbox") {
                return checkboxInput(input);
            } else {
                return input.value;
            }
        }

        /**
         * 这个函数所返回来的值是和checkbox的数量相关的，
         * 1. 如果有多个checkbox，则会返回一个数组
         * 2. 反之如果只有一个checkbox，则只会返回一个逻辑值，用来表示是否选中该选项
        */
        export function checkboxInput(input: HTMLInputElement) {
            var inputs = document.getElementsByName(input.name);
            var values = [];

            if (inputs.length == 1) {
                return input.checked;
            } else {
                inputs.forEach(function (box: HTMLInputElement) {
                    var value = box.value;

                    if (box.checked) {
                        values.push(value);
                    }
                });

                return values;
            }
        };

        /**
         * 获取被选中的选项的值的列表
        */
        export function selectOptionValues(input: HTMLSelectElement): any {
            let selects = getSelectedOptions(input);
            let values = [];

            for (let sel of selects) {
                var value = sel.value;

                if (!value) {
                    value = sel.innerText;
                }

                values.push(value);
            }

            return values;
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

        export function largeText(text: HTMLTextAreaElement): any {

        }
    }
}