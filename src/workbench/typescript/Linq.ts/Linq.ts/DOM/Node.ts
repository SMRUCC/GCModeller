namespace DOM {

    export class node {

        public tagName: string;
        public id: string;
        public classList: string[];
        public attrs: NamedValue<string>[];

        public static FromNode(htmlNode: HTMLElement): node {
            var n: node = new node();
            n.tagName = htmlNode.tagName;
            n.id = htmlNode.id;
            n.classList = this.tokenList(htmlNode.classList);
            n.attrs = this.nameValueMaps(htmlNode.attributes);

            return n;
        }

        public static tokenList(tokens: DOMTokenList): string[] {
            var list: string[] = [];

            for (var i: number = 0; i < tokens.length; i++) {
                list.push(tokens.item(i));
            }

            return list;
        }

        public static nameValueMaps(attrs: NamedNodeMap): NamedValue<string>[] {
            var list: NamedValue<string>[] = [];
            var attr: Attr;
            var map: NamedValue<string>;

            for (var i: number = 0; i < attrs.length; i++) {
                attr = attrs.item(i);
                map = new NamedValue<string>(attr.name, attr.value);
                list.push(map);
            }

            return list;
        }
    }
}