namespace DOM.Animation {

    /**
     * 查看在当前的浏览器中，是否支持css3动画特性
    */
    export function isSupportsCSSAnimation(): boolean {
        var bodyStyle = document.getElementsByTagName("body")[0].style;

        if (typeof bodyStyle.animation != "undefined" || typeof (<any>bodyStyle).WebkitAnimation != "undefined") {
            return true;
        } else {
            return false;
        }
    }
}