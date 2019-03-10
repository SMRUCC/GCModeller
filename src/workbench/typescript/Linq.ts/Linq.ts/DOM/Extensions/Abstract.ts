
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
     * 将当前的html文档节点元素之中的显示内容替换为参数所给定的html内容
    */
    display(html: string | HTMLElement | HTMLTsElement | (() => HTMLElement)): IHTMLElement;
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