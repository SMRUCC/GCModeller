/**
 * 路由器模块
*/
module Router {

    var hashLinks: Dictionary<string>;

    const routerLink: string = "router-link";

    export function queryKey(argName: string): (link: string) => string {
        return link => getAllUrlParams(link).Item(argName);
    }

    export function moduleName(): (link: string) => string {
        return link => (new TsLinq.URL(link)).fileName;
    }

    /**
     * 父容器页面注册视图容器对象
    */
    export function register(appId: string = "app",
        hashKey: string | ((link: string) => string) = null,
        frameRegister: boolean = true) {

        var aLink: Linq.DOM.DOMEnumerator<HTMLAnchorElement>;
        var gethashKey: (link: string) => string;

        if (!hashLinks) {
            hashLinks = new Dictionary<string>({
                "/": "/"
            });
        }
        if (!hashKey) {
            gethashKey = link => (new TsLinq.URL(link)).fileName;
        } else if (typeof hashKey == "string") {
            gethashKey = Router.queryKey(hashKey);
        } else {
            gethashKey = <(link: string) => string>hashKey;
        }

        aLink = $ts(".router");
        aLink.attr("router-link", link => link.href);
        aLink.attr("href", "javascript:void(0);");
        aLink.onClick((link, click) => {
            Router.goto(link.getAttribute("router-link"), appId, gethashKey);
        });
        aLink.attr(routerLink)
            .ForEach(link => {
                hashLinks.Add(gethashKey(link), link);
            });

        // 假设当前的url之中有hash的话，还需要根据注册的路由配置进行跳转显示
        hashChanged(appId);
        // clientResize(appId);
    }

    function clientResize(appId: string) {
        var app: HTMLDivElement = $ts("#" + appId);
        var frame: HTMLIFrameElement = $ts(`#${appId}-frame`);
        var size: number[] = Linq.DOM.clientSize();

        if (!app) {
            console.warn(`[#${appId}] not found!`);
        } else {
            app.style.width = size[0].toString();
            app.style.height = size[1].toString();
            frame.width = size[0].toString();
            frame.height = size[1].toString();
        }
    }

    /**
     * 根据当前url之中的hash进行相应的页面的显示操作
    */
    function hashChanged(appId: string) {
        var hash: string = TsLinq.URL.WindowLocation().hash;
        var url: string = hashLinks.Item(hash);

        if (url) {
            if (url == "/") {
                // 跳转到主页，重新刷新页面？
                window.location.hash = "";
                window.location.reload(true);
            } else {
                (<HTMLDivElement>$ts("#" + appId)).innerHTML =
                    HttpHelpers.GET(url);
            }
        }
    }

    function navigate(link: string,
        stack: Window,
        appId: string,
        hashKey: (link: string) => string) {

        var frame: HTMLDivElement = $ts("#" + appId);

        frame.innerHTML = HttpHelpers.GET(link);
        Router.register(appId, hashKey, false);
        window.location.hash = hashKey(link);
    }

    /**
     * 当前的堆栈环境是否是最顶层的堆栈？
    */
    export function IsTopWindowStack(): boolean {
        return parent && (parent.location.pathname == window.location.pathname);
    }

    /**
     * 因为link之中可能存在查询参数，所以必须要在web服务器上面测试
    */
    export function goto(link: string, appId: string, hashKey: (link: string) => string, stack: Window = null) {
        if (!Router.IsTopWindowStack()) {
            (<any>parent).Router.goto(link, appId, hashKey, parent);
        } else if (stack) {
            // 没有parent了，已经到达最顶端了
            navigate(link, stack, appId, hashKey);
        } else {
            navigate(link, window, appId, hashKey);
        }
    }
}