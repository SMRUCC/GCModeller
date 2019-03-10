// namespace Linq.DOM {

// 2018-10-15
// 为了方便书写代码，在其他脚本之中添加变量类型申明，在这里就不进行命名空间的包裹了

class HTMLTsElement {

    private node: any;

    /**
     * 可以从这里获取得到原生的``HTMLElement``对象用于操作
    */
    public get HTMLElement(): HTMLElement {
        return <HTMLElement>this.node;
    }

    public constructor(node: HTMLElement | HTMLTsElement) {
        this.node = node instanceof HTMLElement ?
            (<HTMLElement>node) :
            (<HTMLTsElement>node).node;
    }

    /**
     * 这个拓展函数总是会将节点中的原来的内容清空，然后显示html函数参数
     * 所给定的内容
    */
    public display(html: string | HTMLElement | HTMLTsElement): HTMLTsElement {
        if (!html) {
            this.HTMLElement.innerHTML = "";
        } else if (typeof html == "string") {
            this.HTMLElement.innerHTML = html;
        } else {
            var node: HTMLElement = html instanceof HTMLTsElement ?
                (<HTMLTsElement>html).HTMLElement :
                (<HTMLElement>html);
            var parent: HTMLElement = this.HTMLElement;

            parent.innerHTML = "";
            parent.appendChild(node);
        }

        return this;
    }

    public addClass(className: string): HTMLTsElement {
        var node = this.HTMLElement

        if (!node.classList.contains(className)) {
            node.classList.add(className);
        }
        return this;
    }

    public removeClass(className: string): HTMLTsElement {
        var node = this.HTMLElement;

        if (node.classList.contains(className)) {
            node.classList.remove(className);
        }
        return this;
    }

    public append(node: HTMLElement | HTMLTsElement): HTMLTsElement {
        if (node instanceof HTMLTsElement) {
            this.HTMLElement.appendChild(node.HTMLElement);
        } else {
            this.HTMLElement.appendChild(node);
        }

        return this;
    }

    /**
     * 将css的display属性值设置为block用来显示当前的节点
    */
    public show(): HTMLTsElement {
        this.HTMLElement.style.display = "block";
        return this;
    }

    /**
     * 将css的display属性值设置为none来隐藏当前的节点
    */
    public hide(): HTMLTsElement {
        this.HTMLElement.style.display = "none";
        return this;
    }
}