/**
 * 在这里对原生的html节点进行拓展
*/
namespace TypeExtensions {

    function appendElement(extendsNode: HTMLTsElement, html: any) {
        if (typeof html == "string") {
            html = $ts("<span>").display(html);
        } else if (typeof html == "function") {
            html = html()
        }

        extendsNode.append(html);
    }

    /**
     * 在原生节点模式之下对输入的给定的节点对象添加拓展方法
     * 
     * 向HTML节点对象的原型定义之中拓展新的方法和成员属性
     * 这个函数的输出在ts之中可能用不到，主要是应用于js脚本
     * 编程之中
     * 
     * @param node 当查询失败的时候是空值
    */
    export function Extends(node: HTMLElement): HTMLElement {
        let obj: any = node;

        if (isNullOrUndefined(node)) {
            return null;
        }

        let extendsNode: HTMLTsElement = new HTMLTsElement(node);

        obj.css = function (stylesheet: string, reset: boolean = false) {
            if (reset) {
                node.setAttribute("style", stylesheet);
            } else {
                DOM.CSS.Setter.css(node, stylesheet);
            }

            return node;
        };

        /**
         * 这个拓展函数总是会将节点中的原来的内容清空，然后显示html函数参数
         * 所给定的内容
        */
        obj.display = function (html: string | HTMLElement) {
            extendsNode.display(html);
            return node;
        };
        obj.appendElement = function (html: any) {

            // a(args[])
            if (Array.isArray(html)) {
                html.forEach(n => appendElement(extendsNode, n));
            } else {
                // a(a,b,c,d,...)
                for (let i: number = 0; i < arguments.length; i++) {
                    appendElement(extendsNode, arguments[i]);
                }
            }

            return node;
        }
        obj.show = function () {
            extendsNode.show();
            return node;
        };
        obj.hide = function () {
            extendsNode.hide();
            return node;
        }
        obj.addClass = function (name: string) {
            extendsNode.addClass(name);
            return node;
        }
        obj.removeClass = function (name: string) {
            extendsNode.removeClass(name);
            return node;
        }
        obj.onClassChanged = function (className: string, action: Delegate.Sub, includesRemoves?: boolean) {
            let predicate = new DOM.Events.StatusChanged(function () {
                return node.classList.contains(className);
            }, includesRemoves);

            $ts.hook(predicate, action);
        };

        obj.CType = function () {
            return node;
        };
        obj.clear = function () {
            node.innerHTML = "";
            return node;
        }
        obj.selects = cssSelector => Internal.Handlers.stringEval.select(cssSelector, node);
        obj.attr = function (name: string, value: string) {
            if ((name = name.toLowerCase()) == "src" || name == "href") {
                value = Internal.urlSolver(value, true);
                TypeScript.logging.log(`set_attr ${name}='${value}'`);
            }

            if (isNullOrUndefined(value)) {
                node.removeAttribute(name);
            } else {
                node.setAttribute(name, value);
            }

            return node;
        }

        obj.interactive = function (enable: boolean) {
            if (enable) {
                node.style.pointerEvents = "auto";
                node.style.opacity = "1";
                node.style.filter = null;
                node.style.webkitFilter = null;
            } else {
                node.style.pointerEvents = "none";
                node.style.opacity = "0.4";
                node.style.filter = "grayscale(100%)";
                node.style.webkitFilter = "grayscale(100%)";
            }
        };

        // 用这个方法可以很方便的从现有的节点进行转换
        // 也可以直接使用new进行构造
        obj.asExtends = extendsNode;
        obj.any = node;
        obj.asImage = node;
        obj.asInput = node;

        return node;
    }
}