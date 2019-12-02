namespace Internal {

    export class Arguments {

        /**
         * 发生查询的上下文，默认是当前文档
        */
        public context: Window | HTMLElement;

        //#region "meta tag value query"

        public caseInSensitive: boolean;
        /**
         * 进行meta节点查询失败时候所返回来的默认值
        */
        public defaultValue: string;

        //#endregion

        //#region "node query && create"

        /**
         * 对于节点查询和创建，是否采用原生的节点返回值？默认是返回原生的节点，否则会返回``HTMLTsElement``对象
         * 
         * + 假若采用原生的节点返回值，则会在该节点对象的prototype之中添加拓展函数
         * + 假若采用``HTMLTsElement``模型，则会返回一个经过包裹的``HTMLElement``节点对象
        */
        public nativeModel: boolean;

        //#endregion

        private static readonly ArgumentNames: string[] = Object.keys(Arguments.Default());

        /**
         * 在创建新的节点的时候，会有一个属性值的赋值过程，
         * 该赋值过程会需要使用这个函数来过滤Arguments的属性值，否则该赋值过程会将Arguments
         * 里面的属性名也进行赋值，可能会造成bug
        */
        public static nameFilter(args: object): string[] {
            return $from(Object.keys(args))
                .Where(name => this.ArgumentNames.indexOf(name) == -1)
                .ToArray();
        }

        public static Default(): Arguments {
            return <Arguments>{
                caseInSensitive: false,
                nativeModel: true,
                defaultValue: "",
                context: window
            }
        }
    }
}