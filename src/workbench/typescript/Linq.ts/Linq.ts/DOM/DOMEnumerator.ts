/// <reference path="../Collections/Abstract/Enumerator.ts" />

// 2018-12-06
// 为了方便书写代码，在其他脚本之中添加变量类型申明，在这里就不进行命名空间的包裹了

// /**
//  * Creates an instance of the element for the specified tag.
//  * @param tagName The name of an element.
// */
// createElement<K extends keyof HTMLElementTagNameMap>(tagName: K, options ?: ElementCreationOptions): HTMLElementTagNameMap[K];

/**
 * A collection of html elements with same tag, name or class
*/
class DOMEnumerator<T extends HTMLElement> extends IEnumerator<T> {

    /**
     * 这个只读属性只返回第一个元素的tagName
     * 
     * @summary 这个属性名与html的节点元素对象的tagName属性名称保持一致
     * 方便进行代码的编写操作
    */
    public get tagName(): string {
        return this.First.tagName;
    }

    /**
     * 这个只读属性主要是针对于input输入控件组而言的
     * 
     * 在假设控件组都是相同类型的情况下, 这个属性直接返回第一个元素的type值
    */
    public get type(): string {
        if (this.tagName.toLowerCase() == "input") {
            return (<any>this.First).type;
        } else {
            return this.tagName;
        }
    }

    /**
     * 1. IEnumerator
     * 2. NodeListOf
     * 3. HTMLCollection
    */
    public constructor(elements: T[] | IEnumerator<T> | NodeListOf<T> | HTMLCollection) {
        super(DOMEnumerator.ensureElements(elements));
    }

    /**
     * 这个函数确保所传递进来的集合总是输出一个数组，方便当前的集合对象向其基类型传递数据源
    */
    private static ensureElements<T extends HTMLElement>(elements: T[] | IEnumerator<T> | NodeListOf<T> | HTMLCollection): T[] {
        var type = $ts.typeof(elements);
        var list: T[];

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
            (<NodeListOf<T>>elements).forEach(x => list.push(x));
        } else if (type.class == "HTMLCollection") {
            var collection: HTMLCollection = <any>elements;
            list = [];
            for (var i: number = 0; i < collection.length; i++) {
                list.push(<any>collection.item(i));
            }
        } else {
            list = Framework.Extensions.EnsureArray(<T[]>elements);
        }

        // 在最后进行元素拓展
        for (var node of list) {
            TypeExtensions.Extends(node);
        }

        return list;
    }

    /**
     * 使用这个函数进行节点值的设置或者获取
     * 
     * 这个函数不传递任何参数则表示获取值
     * 
     * @param value 如果需要批量清除节点之中的值的话，需要传递一个空字符串，而非空值
    */
    public val(value: number | string | string[] | IEnumerator<string> = null): IEnumerator<string> {
        if (isNullOrUndefined(value)) {
            return this.Select(element => DOMEnumerator.getVal(element));
        } else {
            if (typeof value == "string" || typeof value == "number") {
                // 所有元素都设置同一个值
                this.ForEach(e => DOMEnumerator.setVal(e, <string>value));
            } else if (Array.isArray(value)) {
                this.ForEach((e, i) => DOMEnumerator.setVal(e, value[i]));
            } else {
                this.ForEach((e, i) => DOMEnumerator.setVal(e, (<IEnumerator<string>>value).ElementAt(i)));
            }
        }
    }

    private static setVal(element: HTMLElement, text: string) {
        if (element instanceof HTMLInputElement) {
            (<HTMLInputElement>element).value = text;
        } else {
            element.textContent = text;
        }
    }

    private static getVal(element: HTMLElement): string {
        if (element instanceof HTMLInputElement) {
            return (<HTMLInputElement>element).value;
        } else {
            return element.textContent;
        }
    }

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
    public attr(attrName: string, val: string | IEnumerator<string> | string[] | ((x: T) => string) = null): IEnumerator<string> {
        if (val) {
            if (typeof val == "function") {
                return this.Select(x => {
                    var value: string = val(x);
                    x.setAttribute(attrName, value);
                    return value;
                });
            } else {
                var array: string[] = Framework.Extensions.EnsureArray(val, this.Count);

                return this.Select((x, i) => {
                    var value: string = array[i];
                    x.setAttribute(attrName, value);
                    return value;
                });
            }
        } else {
            return this.Select(x => x.getAttribute(attrName));
        }
    }

    public addClass(className: string): DOMEnumerator<T> {
        this.ForEach(node => {
            if (!node.classList.contains(className)) {
                node.classList.add(className);
            }
        })
        return this;
    }

    public addEvent(eventName: string, handler: (sender: T, event: Event) => void) {
        this.ForEach(element => {
            var event = function (Event: Event) {
                handler(element, Event);
            }
            DOM.Events.addEvent(element, eventName, event);
        })
    }

    public onChange(handler: (sender: T, event: Event) => void) {
        this.addEvent("onchange", handler);
    }

    /**
     * 为当前的html节点集合添加鼠标点击事件处理函数
    */
    public onClick(handler: (sender: T, event: MouseEvent) => void) {
        this.ForEach(element => {
            element.onclick = function (this: HTMLElement, ev: MouseEvent) {
                handler(<T>this, ev);
                return <any>false;
            }
        });
    }

    public removeClass(className: string): DOMEnumerator<T> {
        this.ForEach(x => {
            if (x.classList.contains(className)) {
                x.classList.remove(className);
            }
        })
        return this;
    }

    /**
     * 通过设置css之中的display值来将集合之中的所有的节点元素都隐藏掉
    */
    public hide(): DOMEnumerator<T> {
        this.ForEach(x => x.style.display = "none");
        return this;
    }

    /**
     * 通过设置css之中的display值来将集合之中的所有的节点元素都显示出来
    */
    public show(): DOMEnumerator<T> {
        this.ForEach(x => x.style.display = "block");
        return this;
    }

    /**
     * 将所选定的节点批量删除
    */
    public delete() {
        this.ForEach(x => x.parentNode.removeChild(x));
    }
}