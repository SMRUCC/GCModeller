namespace DOM.Events {

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
        } else if (TypeScript.logging.outputEverything) {
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