
/**
 * 拓展的html文档节点元素对象
*/
interface IHTMLElement extends HTMLElement, HTMLExtensions {
}

interface HTMLExtensions {

    /**
     * 将当前的这个节点元素转换为拓展封装对象类型
    */
    asExtends: HTMLTsElement;
    /**
     * 任然是当前的这个文档节点对象，只不过是更加方便转换为any类型
    */
    any: any;

    /**
     * 将当前的html文档节点元素之中的显示内容替换为参数所给定的html内容
    */
    display(html: string | HTMLElement | HTMLTsElement | (() => HTMLElement)): IHTMLElement;
    append(html: string | HTMLElement | HTMLTsElement | (() => HTMLElement)): IHTMLElement;

    /**
     * @param reset If this parameter is true, then it means all of the style that this node have will be clear up.
    */
    css(stylesheet: string, reset?: boolean): IHTMLElement;

    /**
     * @param enable set this parameter to false to make user can not interact with current element.
    */
    interactive(enable: boolean);

    /**
     * 显示当前的节点元素
    */
    show(): IHTMLElement;
    /**
     * 将当前的节点元素从当前的文档之中隐藏掉
    */
    hide(): IHTMLElement;
    addClass(name: string): IHTMLElement;
    removeClass(name: string): IHTMLElement;
    /**
     * 当class列表中指定的class名称出现或者消失的时候将会触发给定的action调用
    */
    onClassChanged(className: string, action: Delegate.Sub, includesRemoves?: boolean): void;

    /**
     * Set/Delete attribute value of current html element node.
     * 
     * @param name HTMLelement attribute name
     * @param value The attribute value to be set, if this parameter is value nothing, 
     *     then it means delete the target attribute from the given html node element.
     *  
     * @returns This function will returns the source html element node after 
     *          the node attribute operation.
    */
    attr(name: string, value: string): IHTMLElement;

    /**
     * 清除当前的这个html文档节点元素之中的所有内容
    */
    clear(): IHTMLElement;

    /**
     * type casting from this base type
    */
    CType<T extends HTMLElement>(): T;

    asImage: IHTMLImageElement;
    asInput: IHTMLInputElement;

    selects<T extends HTMLElement>(cssSelector: string): DOMEnumerator<T>;
}

/**
 * 带有拓展元素的图像标签对象
*/
interface IHTMLImageElement extends HTMLImageElement, HTMLExtensions { }

/**
 * 带有拓展元素的输入框标签对象
*/
interface IHTMLInputElement extends HTMLInputElement, HTMLExtensions { }

/**
 * 带有拓展元素的链接标签对象
*/
interface IHTMLLinkElement extends HTMLAnchorElement, HTMLExtensions { }