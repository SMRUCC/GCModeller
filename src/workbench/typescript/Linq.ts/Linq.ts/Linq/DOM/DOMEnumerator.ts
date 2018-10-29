namespace Linq.DOM {

    // /**
    //  * Creates an instance of the element for the specified tag.
    //  * @param tagName The name of an element.
    // */
    // createElement<K extends keyof HTMLElementTagNameMap>(tagName: K, options ?: ElementCreationOptions): HTMLElementTagNameMap[K];

    export class DOMEnumerator<T extends HTMLElement> extends IEnumerator<T> {

        public constructor(elements: T[] | IEnumerator<T> | NodeListOf<T>) {
            super(DOMEnumerator.ensureElements(elements));
        }

        /**
         * 这个函数确保所传递进来的集合总是输出一个数组，方便当前的集合对象向其基类型传递数据源
        */
        private static ensureElements<T extends HTMLElement>(elements: T[] | IEnumerator<T> | NodeListOf<T>): T[] {
            var type = TypeInfo.typeof(elements);
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
            } else {
                list = Linq.EnsureArray(<T[]>elements);
            }

            return list;
        }

        /**
         * 使用这个函数进行节点值的设置或者获取
         * 
         * @param value 如果需要批量清除节点之中的值的话，需要传递一个空字符串，而非空值
        */
        public val(value: string | string[] | IEnumerator<string> = null): IEnumerator<string> {
            if (!(value == null && value == undefined)) {
                if (typeof value == "string") {
                    // 所有元素都设置同一个值
                    this.ForEach(element => {
                        element.nodeValue = value;
                    });
                } else if (Array.isArray(value)) {
                    this.ForEach((element, i) => {
                        element.nodeValue = value[i];
                    });
                } else {
                    this.ForEach((element, i) => {
                        element.nodeValue = value.ElementAt(i);
                    });
                }
            }

            return this.Select(element => element.nodeValue);
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
                    var array: string[] = Linq.EnsureArray(val, this.Count);

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

        public AddClass(className: string): DOMEnumerator<T> {
            this.ForEach(x => {
                if (!x.classList.contains(className)) {
                    x.classList.add(className);
                }
            })
            return this;
        }

        public AddEvent(eventName: string, handler: (sender: T, event: Event) => void) {
            this.ForEach(element => {
                var event = function (Event: Event) {
                    handler(element, Event);
                }
                Linq.DOM.addEvent(element, eventName, event);
            })
        }

        public onChange(handler: (sender: T, event: Event) => void) {
            this.AddEvent("onchange", handler);
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

        public RemoveClass(className: string): DOMEnumerator<T> {
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
        public Delete() {
            this.ForEach(x => x.parentNode.removeChild(x));
        }
    }
}