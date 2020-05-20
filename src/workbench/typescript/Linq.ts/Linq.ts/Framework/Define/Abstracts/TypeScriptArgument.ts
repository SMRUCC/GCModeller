namespace Internal {

    /**
     * 这个参数对象模型主要是针对创建HTML对象的
    */
    export interface TypeScriptArgument {
        /**
         * HTML节点对象的编号（通用属性）
        */
        id?: string;
        /**
         * HTML节点对象的CSS样式字符串（通用属性）
        */
        style?: string | CSSStyleDeclaration;
        /**
         * HTML节点对象的class类型（通用属性）
        */
        class?: string | string[];
        type?: string;
        href?: string;
        text?: string;
        visible?: boolean;
        alt?: string;
        checked?: boolean;
        selected?: boolean;
        autofocus?: boolean;
        placeholder?: string;

        /**
         * 应用于``<a>``标签进行文件下载重命名文件所使用的
        */
        download?: string;
        target?: string;
        src?: string;
        width?: string | number;
        height?: string | number;
        /**
         * 进行查询操作的上下文环境，这个主要是针对iframe环境之中的操作的
        */
        context?: Window | HTMLElement | IHTMLElement;
        title?: string;
        name?: string;
        /**
         * HTML的输入控件的预设值
        */
        value?: string | number | boolean;
        for?: string;
        role?: string;
        tabindex?: number;

        "max-width"?: string;
        frameborder?: string;
        border?: string;

        marginwidth?: string;
        marginheight?: string;
        scrolling?: string;
        allowtransparency?: string;

        /**
         * 处理HTML节点对象的点击事件，这个属性值应该是一个无参数的函数来的
        */
        onclick?: Delegate.Sub | string;
        onmouseover?: Delegate.Sub | string;
        /**
         * 主要是应用于输入控件
        */
        onchange?: Delegate.Sub | string;

        "data-toggle"?: string;
        "data-target"?: string;

        "aria-hidden"?: boolean;
        "aria-label"?: string;
        "aria-live"?: string;
        "aria-haspopup"?: boolean;
        "aria-owns"?: string;
        "aria-expanded"?: boolean;

        "aria-labelledby"?: string;

        usemap?: string;
        shape?: string;
        coords?: string;
    }
}