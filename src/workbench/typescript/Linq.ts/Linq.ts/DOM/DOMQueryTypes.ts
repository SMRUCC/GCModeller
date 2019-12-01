namespace DOM {

    /**
     * HTML文档节点的查询类型
    */
    export enum QueryTypes {
        NoQuery = 0,
        /**
         * 表达式为 #xxx
         * 按照节点的id编号进行查询
         * 
         * ``<tag id="xxx">``
        */
        id = 1,
        /**
         * 表达式为 .xxx
         * 按照节点的class名称进行查询
         * 
         * ``<tag class="xxx">``
        */
        class = 10,
        name = 100,
        /**
         * 表达式为 xxx
         * 按照节点的名称进行查询
         * 
         * ``<xxx ...>``
        */
        tagName = -100,

        /**
         * query meta tag content value by name
         * 
         * ``@xxxx``
         * 
         * ```html
         * <meta name="user-login" content="xieguigang" />
         * ```
        */
        QueryMeta = 200
    }
}