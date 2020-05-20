namespace DOM.CSS.Setter {

    export function css(node: HTMLElement, style: string) {
        setStyle(node, parseStylesheet(style));
    }

    export function setStyle(node: HTMLElement, style: NamedValue<string>[]) {
        for (let declare of style) {
            applyStyle(node, declare.name, declare.value);
        }
    }

    /**
     * 20200427
     * 似乎直接应用于style属性上并不会起作用，所以在这里修改为应用于节点元素的style上
    */
    function applyStyle(styledNode: HTMLElement, name: string, value: string) {
        switch (name.toLowerCase()) {
            case "color":
                styledNode.style.color = value;
                break;
            case "background-color":
                styledNode.style.backgroundColor = value;
                break;
            case "font-size":
                styledNode.style.fontSize = value;
                break;
            default:
                styledNode.style[name] = value;
                TypeScript.logging.warning(`Set style '${name}' is not implements yet...`);
        }
    }
}