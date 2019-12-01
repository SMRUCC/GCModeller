namespace DOM {

    export class Query {

        public type: QueryTypes;
        public singleNode: boolean;

        /**
         * Name of the return value is the trimmed expression
        */
        public expression: string;

        /**
         * + ``#`` by id
         * + ``.`` by class
         * + ``!`` by name
         * + ``&`` SINGLE NODE
         * + ``@`` read meta tag
         * + ``&lt;>`` create new tag
        */
        public static parseQuery(expr: string): Query {
            var isSingle: boolean = false;

            if (expr.charAt(0) == "&") {
                isSingle = true;
                expr = expr.substr(1);
            } else {
                isSingle = false;
            }

            return Query.parseExpression(expr, isSingle);
        }

        /**
         * by node id
        */
        private static getById(id: string): Query {
            return <Query>{
                type: QueryTypes.id,
                singleNode: true,
                expression: id
            };
        }

        /**
         * by class name
        */
        private static getByClass(className: string, isSingle: boolean): Query {
            return <Query>{
                type: QueryTypes.class,
                singleNode: isSingle,
                expression: className
            };
        }

        /**
         * by name attribute
        */
        private static getByName(nameVal: string, isSingle: boolean): Query {
            return <Query>{
                type: QueryTypes.name,
                singleNode: isSingle,
                expression: `[name='${nameVal}']`
            };
        }

        /**
         * by tag name
        */
        private static getByTag(tag: string, isSingle: boolean): Query {
            return <Query>{
                type: QueryTypes.tagName,
                singleNode: isSingle,
                expression: tag
            };
        }

        /**
         * create new node
        */
        private static createElement(expr: string): Query {
            return <Query>{
                type: QueryTypes.NoQuery,
                singleNode: true,
                expression: expr
            };
        }

        private static queryMeta(expr: string): Query {
            return <Query>{
                type: QueryTypes.QueryMeta,
                singleNode: true,
                expression: expr
            }
        }

        private static isSelectorQuery(expr: string): boolean {
            var hasMultiple: boolean = expr.indexOf(" ") > -1;
            var isNodeCreate: boolean = expr.charAt(0) == "<" && expr.charAt(expr.length - 1) == ">";

            return hasMultiple && !isNodeCreate;
        }

        private static parseExpression(expr: string, isSingle: boolean): Query {
            var prefix: string = expr.charAt(0);

            if (Query.isSelectorQuery(expr)) {
                // 可能是复杂查询表达式
                return <Query>{
                    type: QueryTypes.tagName,
                    singleNode: isSingle,
                    expression: expr
                };
            }

            switch (prefix) {
                case "#": return this.getById(expr.substr(1));
                case ".": return this.getByClass(expr, isSingle);
                case "!": return this.getByName(expr.substr(1), isSingle);
                case "<": return this.createElement(expr);
                case "@": return this.queryMeta(expr.substr(1));

                default: return this.getByTag(expr, isSingle);
            }
        }
    }
}