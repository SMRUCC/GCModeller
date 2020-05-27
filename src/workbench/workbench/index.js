/**
 * Module handles configurable splashscreen to show while app is loading.
 */
var workbench;
(function (workbench) {
    var view;
    (function (view) {
        /**
         * When splashscreen was shown.
         * @ignore
         */
        var splashScreenTimestamp = 0;
        /**
         * Splashscreen is loaded and ready to show.
         * @ignore
         */
        var splashScreenReady = false;
        /**
         * Main window has been loading for a min amount of time.
         * @ignore
         */
        var slowStartup = false;
        /**
         * True when expected work is complete and we've closed splashscreen, else user prematurely closed splashscreen.
         * @ignore
         */
        var done = false;
        /**
         * Show splashscreen if criteria are met.
         * @ignore
         */
        var showSplash = function () {
            if (splashScreen && splashScreenReady && slowStartup) {
                splashScreen.show();
                splashScreenTimestamp = Date.now();
            }
        };
        /**
         * Close splashscreen / show main screen. Ensure screen is visible for a min amount of time.
         * @ignore
         */
        var closeSplashScreen = function (main, min) {
            if (splashScreen) {
                var timeout = min - (Date.now() - splashScreenTimestamp);
                setTimeout(function () {
                    done = true;
                    if (splashScreen) {
                        splashScreen.isDestroyed() || splashScreen.close(); // Avoid `Error: Object has been destroyed` (#19)
                        splashScreen = null;
                    }
                    main.show();
                }, timeout);
            }
        };
        /**
         * The actual splashscreen browser window.
         * @ignore
         */
        var splashScreen;
        /**
         * Initializes a splashscreen that will show/hide smartly (and handle show/hiding of main window).
         * @param config - Configures splashscren
         * @returns {BrowserWindow} the main browser window ready for loading
         */
        view.initSplashScreen = function (config) {
            var xConfig = {
                windowOpts: config.windowOpts,
                templateUrl: config.templateUrl,
                splashScreenOpts: config.splashScreenOpts,
                delay: config.delay || 500,
                minVisible: config.minVisible || 500,
                closeWindow: config.closeWindow || true,
            };
            xConfig.splashScreenOpts.frame = false;
            xConfig.splashScreenOpts.center = true;
            xConfig.splashScreenOpts.show = false;
            xConfig.windowOpts.show = false;
            var window = new BrowserWindow(xConfig.windowOpts);
            splashScreen = new BrowserWindow(xConfig.splashScreenOpts);
            splashScreen.loadURL("file://" + xConfig.templateUrl);
            xConfig.closeWindow && splashScreen.on("close", function () {
                done || window.close();
            });
            // Splashscreen is fully loaded and ready to view.
            splashScreen.webContents.on("did-finish-load", function () {
                splashScreenReady = true;
                showSplash();
            });
            // Startup is taking enough time to show a splashscreen.
            setTimeout(function () {
                slowStartup = true;
                showSplash();
            }, xConfig.delay);
            window.webContents.on("did-finish-load", function () {
                closeSplashScreen(window, xConfig.minVisible);
            });
            return window;
        };
        /**
         * Initializes a splashscreen that will show/hide smartly (and handle show/hiding of main window).
         * Use this function if you need to send/receive info to the splashscreen (e.g., you want to send
         * IPC messages to the splashscreen to inform the user of the app's loading state).
         * @param config - Configures splashscren
         * @returns {DynamicSplashScreen} the main browser window and the created splashscreen
         */
        view.initDynamicSplashScreen = function (config) {
            return {
                main: view.initSplashScreen(config),
                // initSplashScreen initializes splashscreen so this is a safe cast.
                splashScreen: splashScreen,
            };
        };
    })(view = workbench.view || (workbench.view = {}));
})(workbench || (workbench = {}));
var workbench;
(function (workbench) {
    var view;
    (function (view) {
        function renderAppMenu(template) {
            var menu = Menu.buildFromTemplate(runRender(template));
            Menu.setApplicationMenu(menu);
            return menu;
        }
        view.renderAppMenu = renderAppMenu;
        function runRender(template) {
            for (var _i = 0, template_1 = template; _i < template_1.length; _i++) {
                var item = template_1[_i];
                renderMenuTemplate(item);
            }
            return template;
        }
        /**
         * replace all url as menu click
        */
        function renderMenuTemplate(templ) {
            if (!(templ.submenu instanceof Menu)) {
                templ.submenu
                    .filter(function (sm) { return "click" in sm; })
                    .forEach(function (sm) {
                    var url = sm.click + "";
                    sm.click = function () {
                        require('electron').shell.openExternal(url);
                    };
                });
                templ.submenu
                    .filter(function (sm) { return sm.label == "Quit"; })
                    .forEach(function (sm) {
                    sm.click = function () {
                        app.quit();
                    };
                });
            }
            return templ;
        }
    })(view = workbench.view || (workbench.view = {}));
})(workbench || (workbench = {}));
var workbench;
(function (workbench) {
    var view;
    (function (view_1) {
        // 保持对window对象的全局引用，如果不这么做的话，当JavaScript对象被
        // 垃圾回收的时候，window对象将会自动的关闭
        var windows = {};
        function getMainWindow() {
            if (mainView in windows) {
                return windows[mainView];
            }
            else {
                return null;
            }
        }
        view_1.getMainWindow = getMainWindow;
        function createWindow(view, size, callback, lambda, debug) {
            if (size === void 0) { size = [800, 600]; }
            if (callback === void 0) { callback = null; }
            if (lambda === void 0) { lambda = false; }
            if (debug === void 0) { debug = false; }
            var invoke = function () {
                // 创建浏览器窗口。
                var win = new BrowserWindow({ width: size[0], height: size[1] });
                // 然后加载应用的 index.html。
                win.loadFile(view);
                if (debug) {
                    // 打开开发者工具
                    win.webContents.openDevTools();
                }
                // 当 window 被关闭，这个事件会被触发。
                win.on('closed', function () {
                    // 取消引用 window 对象，如果你的应用支持多窗口的话，
                    // 通常会把多个 window 对象存放在一个数组里面，
                    // 与此同时，你应该删除相应的元素。
                    windows[view] = null;
                });
                if (callback) {
                    callback();
                }
                return function () {
                };
            };
            return lambda ? invoke : invoke();
        }
        view_1.createWindow = createWindow;
    })(view = workbench.view || (workbench.view = {}));
})(workbench || (workbench = {}));
var workbench;
(function (workbench) {
    var Shell;
    (function (Shell) {
        function Rweb() {
            var spawn = require("child_process").spawn;
            var bat = spawn("Rweb", [
                "--session",
                "--port", "7452",
                "--workspace", "./Rweb",
                "--show_error",
                "--n_threads", "2"
            ]);
            bat.stdout.on("data", function (data) {
                // Handle data...
            });
            bat.stderr.on("data", function (err) {
                // Handle error...
            });
            bat.on("exit", function (code) {
                // Handle exit
            });
            return bat;
        }
        Shell.Rweb = Rweb;
        function initialize() {
            // 导入Rstudio环境诊断组件
            var request = require('request');
            var script = "imports 'diagnostics' from 'Rstudio';";
            request("http://127.0.0.1:7452/exec?script=" + encodeURIComponent(script), function (error, response, body) {
                if (!error && response.statusCode == 200) {
                    console.log(body);
                }
            });
        }
        Shell.initialize = initialize;
    })(Shell = workbench.Shell || (workbench.Shell = {}));
})(workbench || (workbench = {}));
var workbench;
(function (workbench) {
    function osd() {
        var msg = new Notification({
            title: "Task Finish",
            body: "test task finished!"
        });
        msg.show();
        return msg;
    }
    workbench.osd = osd;
})(workbench || (workbench = {}));
/// <reference path="dev/splash.ts" />
/// <reference path="dev/renderMenu.ts" />
/// <reference path="dev/view.ts" />
/// <reference path="dev/shell.ts" />
/// <reference path="dev/osd.ts" />
/// <reference path="node_modules/electron/electron.d.ts" />
// load framework
var _a = require('electron'), app = _a.app, BrowserWindow = _a.BrowserWindow, Menu = _a.Menu, Notification = _a.Notification;
// import { app, BrowserWindow, Menu, Notification } from "electron";
// import * as path from "path";
// import * as url from "url";
// const { path } = require('path');
var mainView = require('path').join(__dirname, "views/index.html");
// start the R# backend environment
var backend = workbench.Shell.Rweb();
var defaultViewSize = [1440, 900];
// load internal app components
var template = require("./menu.json");
var menu = null;
var mainWindow;
// Electron 会在初始化后并准备
// 创建浏览器窗口时，调用这个函数。
// 部分 API 在 ready 事件触发后才能使用。
app.on("ready", function () {
    var windowOptions = {
        width: defaultViewSize[0],
        height: defaultViewSize[1],
        show: false,
    };
    mainWindow = workbench.view.initSplashScreen({
        windowOpts: windowOptions,
        templateUrl: require('path').join(__dirname, "views/startup.html"),
        delay: 0,
        minVisible: 2500,
        splashScreenOpts: {
            height: 500,
            width: 800,
            transparent: false
        },
    });
    mainWindow.loadURL("file://" + mainView);
    menu = workbench.view.renderAppMenu(template);
    // 在这个文件中，你可以续写应用剩下主进程代码。
    // 也可以拆分成几个文件，然后用 require 导入。
    workbench.Shell.initialize();
});
// 当全部窗口关闭时退出。
app.on('window-all-closed', function () {
    // 在 macOS 上，除非用户用 Cmd + Q 确定地退出，
    // 否则绝大部分应用及其菜单栏会保持激活。
    if (process.platform !== 'darwin') {
        app.quit();
    }
});
app.on('activate', function () {
    // 在macOS上，当单击dock图标并且没有其他窗口打开时，
    // 通常在应用程序中重新创建一个窗口。
    if (workbench.view.getMainWindow() === null) {
        workbench.view.createWindow(mainView, defaultViewSize);
    }
});
//# sourceMappingURL=index.js.map