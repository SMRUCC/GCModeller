/// <reference path="../DOM/DOMEnumerator.ts" />
/// <reference path="../DOM/InputValueGetter.ts" />

/**
 * Web应用程序路由器模块
 * 
 * 通过这个路由器模块管理制定的Web应用程序模块的运行或者休眠
*/
module Router {

    let hashLinks: Dictionary<string>;
    let webApp: Dictionary<Bootstrap>[] = <any>{};
    let caseSensitive: boolean = true;

    /**
     * meta标签中的app值
    */
    export const appName: string = <any>DOM.InputValueGetter.metaValue("app");

    export function isCaseSensitive(): boolean {
        return caseSensitive;
    }

    export function currentAppPage(): Bootstrap {
        for (let index in webApp) {
            for (let app of (<Dictionary<Bootstrap>>webApp[index]).Values.ToArray()) {
                if (app.appStatus == "Running") {
                    return app;
                }
            }
        }

        return null;
    }

    /**
     * 设置路由器对URL的解析是否是大小写不敏感模式，也可以在这里函数中设置参数为false，来切换为大小写敏感模式
     * 
     * @param option 通过这个参数来设置是否为大小写不敏感模式？
     * 
    */
    export function CaseInsensitive(option: boolean = true): void {
        caseSensitive = !option;
    }

    /**
     * @param module 默认的模块是``/``，即如果服务器为php服务器的话，则默认为index.php
    */
    export function AddAppHandler(app: Bootstrap, module = "/") {
        if (!(module in webApp)) {
            webApp[module] = new Dictionary<Bootstrap>({});
        }

        doModule(module, apps => apps.Add(app.appName, app));
    }

    function doModule(module: string, action: (apps: Dictionary<Bootstrap>) => void) {
        action(webApp[module]);
    }

    /**
     * fix for index.php, VBServerScript etc.
    */
    const indexModule = {
        "/": "general",
        "index.php": "php server",
        "index.perl": "perl server",
        "index.do": "",
        "index.jsp": "java server",
        "index.vbs": "VB server script",
        "index.vb": "VB server",
        "index.asp": "VB6 server",
        "index.aspx": "VB.NET server"
    };

    export interface IAppInfo {
        module: string;
        appName: string;
        className: string;
        status: string;
        hookUnload: string
    }

    export function getAppSummary(app: Bootstrap, module: string = "/"): IAppInfo {
        let type = $ts.typeof(app);
        let info = <IAppInfo>{
            module: module,
            appName: app.appName,
            className: type.class,
            status: app.appStatus,
            hookUnload: app.appHookMsg
        }

        return info;
    }

    /**
     * 从这个函数开始执行整个Web应用程序
    */
    export function RunApp(module = "/") {
        TypeScript.logging.log(TypeScript.URL.WindowLocation());

        if (module in webApp) {
            doModule(module, apps => apps.Select(app => app.value.Init()));
        } else if (module == "index" || module in indexModule) {
            let runInit: boolean = false;

            for (let index of Object.keys(indexModule)) {
                if (index in webApp) {
                    doModule(index, apps => apps.Select(app => app.value.Init()));
                    runInit = true;
                    break;
                }
            }

            if (!runInit) {
                throw "Default module is not found!";
            }

        } else {
            throw `Module "${module}" is not exists in your web app.`;
        }

        if (TypeScript.logging.outputEverything) {
            // 在console中显示table
            let summary: IAppInfo[] = [];

            Object.keys(webApp).forEach(module => {
                doModule(module, apps => {
                    apps.ForEach(app => summary.push(getAppSummary(app.value, module)));
                });
            });

            console.table(summary);
        }
    }

    const routerLink: string = "router-link";

    export function queryKey(argName: string): (link: string) => string {
        return link => getAllUrlParams(link).Item(argName);
    }

    export function moduleName(): (link: string) => string {
        return link => (new TypeScript.URL(link)).fileName;
    }

    /**
     * 父容器页面注册视图容器对象
    */
    export function register(appId: string = "app",
        hashKey: string | ((link: string) => string) = null,
        frameRegister: boolean = true) {

        var aLink: DOMEnumerator<HTMLAnchorElement>;
        var gethashKey: (link: string) => string;

        if (!hashLinks) {
            hashLinks = new Dictionary<string>({
                "/": "/"
            });
        }
        if (!hashKey) {
            gethashKey = link => (new TypeScript.URL(link)).fileName;
        } else if (typeof hashKey == "string") {
            gethashKey = Router.queryKey(hashKey);
        } else {
            gethashKey = <(link: string) => string>hashKey;
        }

        aLink = <any>$ts(".router");
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
        var app: HTMLDivElement = <any>$ts("#" + appId);
        var frame: HTMLIFrameElement = <any>$ts(`#${appId}-frame`);
        var size: number[] = DOM.clientSize();

        if (!app) {
            if (TypeScript.logging.outputWarning) {
                console.warn(`[#${appId}] not found!`);
            }
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
        var hash: string = TypeScript.URL.WindowLocation().hash;
        var url: string = hashLinks.Item(hash);

        if (url) {
            if (url == "/") {
                // 跳转到主页，重新刷新页面？
                window.location.hash = "";
                window.location.reload(true);
            } else {
                $ts("#" + appId).innerHTML = HttpHelpers.GET(url);
            }
        }
    }

    function navigate(link: string,
        stack: Window,
        appId: string,
        hashKey: (link: string) => string) {

        var frame: IHTMLElement = $ts("#" + appId);

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