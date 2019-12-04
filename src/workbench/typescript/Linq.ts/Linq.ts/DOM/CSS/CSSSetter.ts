namespace DOM.CSS.Setter {

    export function css(node: HTMLElement, style: string) {
        setStyle(node, parseStylesheet(style));
    }

    export function setStyle(node: HTMLElement, style: NamedValue<string>[]) {
        let css: CSSStyleDeclaration = node.style;

        TypeScript.logging.log(style);

        for (let declare of style) {
            applyStyle(css, declare.name, declare.value);
        }
    }

    function applyStyle(style: CSSStyleDeclaration, name: string, value: string) {
        switch (name.toLowerCase()) {
            case "color":
                style.color = value; break;
            case "background-color":
                style.backgroundColor = value; break;
            case "font-size":
                style.fontSize = value; break;
            default:
                style[name] = value;
                TypeScript.logging.warning(`Set style '${name}' is not implements yet...`);
        }
    }
}