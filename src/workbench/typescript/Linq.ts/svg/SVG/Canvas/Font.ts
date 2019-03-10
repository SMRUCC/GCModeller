namespace Canvas {

    export class Font implements ICSSStyle {

        public size: string;
        public family: string;
        public bold: boolean;
        public italic: boolean;

        constructor(family: string,
            size: any = "12px",
            bold: boolean = false,
            italic: boolean = false) {

            this.size = size;
            this.family = family;
            this.bold = bold;
            this.italic = italic;
        }

        public Styling(node: SVGElement): SVGElement {
            var styles = [];

            if (this.bold) styles.push("bold");
            if (this.italic) styles.push("italic");

            node.style.fontFamily = this.family;
            node.style.fontSize = this.size;
            node.style.fontStyle = styles.join(" ");

            return node;
        }

        public CSSStyle(): string {
            var styles = [];

            if (this.bold) styles.push("bold");
            if (this.italic) styles.push("italic");

            return `font: ${styles.join(" ")} ${this.size} "${this.family}"`;
        }
    }
}