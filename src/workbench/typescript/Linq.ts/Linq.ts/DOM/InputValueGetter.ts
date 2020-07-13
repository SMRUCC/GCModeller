/// <reference path="../Framework/Log4TypeScript.ts" />

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

        /**
         * @param strict 这个参数主要是针对非输入类型的控件的值获取而言的。
         * 如果目标id标记的控件不是输入类型的，则如果处于非严格模式下，
         * 即这个参数为``false``的时候会直接强制读取value属性值
        */
        export function getValue(resource: string, strict: boolean = true): any {
            return unifyGetValue($ts(resource), strict);
        }

        export function unifyGetValue(input: HTMLElement, strict: boolean = true): any {
            switch (input.tagName.toLowerCase()) {
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
            let inputType: string = input.type.toLowerCase();

            if (inputType == "checkbox") {
                return checkboxInput(input, true);
            } else if (inputType == "radio") {

                if (input instanceof DOMEnumerator) {
                    return (<DOMEnumerator<HTMLInputElement>>input)
                        .Where(radio => radio.checked)
                        .FirstOrDefault()
                        .value;
                } else {
                    return input.value;
                }

            } else {
                return input.value;
            }
        }

        /**
         * 这个函数所返回来的值是和checkbox的数量相关的，
         * 1. 如果有多个checkbox，则会返回一个数组
         * 2. 反之如果只有一个checkbox，则只会返回一个逻辑值，用来表示是否选中该选项
        */
        export function checkboxInput(input: HTMLInputElement | DOMEnumerator<HTMLInputElement>, singleAsLogical: boolean = false) {
            let inputs: DOMEnumerator<HTMLInputElement>;

            if (input instanceof DOMEnumerator) {
                inputs = <any>input
            } else {
                inputs = <any>new DOMEnumerator<HTMLInputElement>(<any>document.getElementsByName(input.name));
            }

            if (inputs.Count == 1) {
                let single = inputs.ElementAt(0);

                // check or unchecked
                // true or false
                if (singleAsLogical) {
                    return single.checked;
                } else if (single.checked) {
                    return single.value;
                } else {
                    return null;
                }

            } else {
                let values: string[] = inputs
                    .Where(c => c.checked)
                    .Select(box => box.value)
                    .ToArray(false);

                return values;
            }
        };

        /**
         * 获取被选中的选项的值的列表
        */
        export function selectOptionValues(input: HTMLSelectElement): any {
            let selects: HTMLOptionElement[] = <any>getSelectedOptions(input);
            let values = [];

            for (let sel of selects) {
                var value = (<HTMLOptionElement>sel).value;

                if (!value) {
                    value = (<HTMLOptionElement>sel).innerText;
                }

                values.push(value);
            }

            return values;
        }

        /**
         * return array containing references to selected option elements
        */
        export function getSelectedOptions(sel: HTMLSelectElement | DOMEnumerator<HTMLInputElement>) {
            var opts: HTMLOptionElement[] = []
            var opt: HTMLOptionElement;

            if (sel instanceof HTMLSelectElement) {
                // loop through options in select list
                for (var i = 0, len = sel.options.length; i < len; i++) {
                    opt = sel.options[i];

                    // check if selected
                    if (opt.selected) {
                        // add to array of option elements to return from this function
                        opts.push(opt);
                    }
                }
            } else if (sel instanceof HTMLInputElement) {
                if (sel.checked) {
                    return sel.value;
                } else {
                    return false;
                }
            } else {
                return sel
                    .Where(i => i.checked)
                    .Select(i => i.value)
                    .ToArray(false);
            }

            return opts;
        }

        export function largeText(text: HTMLTextAreaElement): any {
            return text.value;
        }
    }
}