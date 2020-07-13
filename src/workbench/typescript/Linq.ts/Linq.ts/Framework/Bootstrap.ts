/// <reference path="./Router.ts" />

/**
 * 实现这个类需要重写下面的方法实现：
 * 
 * + ``protected abstract init(): void;``
 * + ``public abstract get appName(): string``
 * 
 * > ``appName``默认规则是php.net的路由规则，也可以将appName写在
 * > 页面的meta标签的content中，meta标签的name名称应该为``app``
 * 
 * 可以选择性的重写下面的事件处理器
 * 
 * + ``protected OnDocumentReady(): void``
 * + ``protected OnWindowLoad(): void``
 * + ``protected OnWindowUnload(): string``
 * + ``protected OnHashChanged(hash: string): void``
 * 
 * 也可以重写下面的事件来获取当前的app的名称
 * 
 * + ``protected getCurrentAppPage(): string``
*/
abstract class Bootstrap {

    protected status: "Sleep" | "Running";
    /**
     * 是否阻止用户关闭当前页面
    */
    protected hookUnload: string;

    public abstract get appName(): string;

    /**
     * 这个函数默认是取出url query之中的app参数字符串作为应用名称
     * 
     * @returns 如果没有定义app参数，则默认是返回``/``作为名称
    */
    protected get currentAppPage(): string {
        return getAllUrlParams().Item("app") || Router.appName || "/";
    }

    public get appStatus(): "Sleep" | "Running" {
        return this.status;
    }

    public get appHookMsg(): string {
        return this.hookUnload;
    }

    public constructor() {
        this.status = "Sleep";
        this.hookUnload = null;
    }

    private isCurrentAppPath(): boolean {
        let path = $ts.location.path;

        if (TypeScript.logging.outputEverything) {
            console.log(`Current url path is '${path}'`);
        }

        return path == this.appName;
    }

    private isCurrentApp(): boolean {
        var currentAppName: string = this.currentAppPage;
        var awake: boolean;

        if (this.appName.charAt(0) === "/") {
            // 是一个绝对路径，则按照路径进行判断
            return this.isCurrentAppPath();
        }

        if (Router.isCaseSensitive()) {
            awake = currentAppName == this.appName;
        } else {
            awake = currentAppName.toLowerCase() == this.appName.toLowerCase()
        }

        // 必须要当前的App名称和当前的页面app一致的时候这个App的运行才会被触发
        if (!awake) {
            return false;
        } else if (TypeScript.logging.outputEverything) {
            console.log(`%c[${$ts.typeof(this).class}] App(name:=${this.appName}) Init...`, "color:blue;");
        }

        return awake;
    }

    public Init(): void {
        var vm = this;

        if (!this.isCurrentApp()) {
            return;
        } else {
            // attach event handlers
            $ts(() => this.OnDocumentReady());
        }

        // 2019-1-7 因为js是解释执行的，所以OnWindowLoad函数里面的代码之中的this，
        // 可能会被解释为window对象
        // 从而导致出现bug，所以在这里需要使用一个函数的封装来避免这个问题
        window.onload = vm.OnWindowLoad;
        window.onbeforeunload = vm.OnWindowUnload;
        window.onhashchange = function () {
            var hash = window.location.hash;
            var val = hash.substr(1);

            vm.OnHashChanged(val);
        };

        Internal.EventHandles.hookEventHandles(this);

        this.init();
        this.status = "Running";
    }

    /**
     * 初始化代码
    */
    protected abstract init(): void;

    /**
     * Event handler on document is ready
    */
    protected OnDocumentReady(): void {
        // do nothing
    }
    /**
     * Event handler on Window loaded
    */
    protected OnWindowLoad(): void {
        // do nothing
    }

    protected OnWindowUnload(): string {
        if (!Strings.Empty(this.hookUnload, true)) {
            return this.hookUnload;
        }
    }

    public unhook() {
        this.hookUnload = null;
    }

    /**
     * Event handler on url hash link changed
    */
    protected OnHashChanged(hash: string): void {
        // do nothing
    }

    public toString(): string {
        return `[${this.status}] ${this.appName}`;
    }
}