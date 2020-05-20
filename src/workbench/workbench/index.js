var workbench;
(function (workbench) {
    var view;
    (function (view) {
        function renderAppMenu(template) {
            const menu = Menu.buildFromTemplate(runRender(template));
            Menu.setApplicationMenu(menu);
            return menu;
        }
        view.renderAppMenu = renderAppMenu;
        function runRender(template) {
            for (let item of template) {
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
                    .filter(sm => "click" in sm)
                    .forEach(sm => {
                    var url = sm.click + "";
                    sm.click = function () {
                        require('electron').shell.openExternal(url);
                    };
                });
                templ.submenu
                    .filter(sm => sm.label == "Quit")
                    .forEach(sm => {
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
        let windows = {};
        function getMainWindow() {
            if (mainView in windows) {
                return windows[mainView];
            }
            else {
                return null;
            }
        }
        view_1.getMainWindow = getMainWindow;
        function createWindow(view, size = [800, 600], callback = null, lambda = false, debug = false) {
            let invoke = function () {
                // 创建浏览器窗口。
                let win = new BrowserWindow({ width: size[0], height: size[1] });
                // 然后加载应用的 index.html。
                win.loadFile(view);
                if (debug) {
                    // 打开开发者工具
                    win.webContents.openDevTools();
                }
                // 当 window 被关闭，这个事件会被触发。
                win.on('closed', () => {
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
            let spawn = require("child_process").spawn;
            let bat = spawn("Rweb", [
                "--session",
                "--port", "7452",
                "--workspace", "./Rweb",
                "--show_error",
                "--n_threads", "2"
            ]);
            bat.stdout.on("data", (data) => {
                // Handle data...
            });
            bat.stderr.on("data", (err) => {
                // Handle error...
            });
            bat.on("exit", (code) => {
                // Handle exit
            });
            return bat;
        }
        Shell.Rweb = Rweb;
        function initialize() {
            // 导入Rstudio环境诊断组件
            let request = require('request');
            let script = "imports 'diagnostics' from 'Rstudio';";
            request(`http://127.0.0.1:7452/exec?script=${encodeURIComponent(script)}`, function (error, response, body) {
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
        let msg = new Notification({
            title: "Task Finish",
            body: "test task finished!"
        });
        msg.show();
        return msg;
    }
    workbench.osd = osd;
})(workbench || (workbench = {}));
/// <reference path="node_modules/electron/electron.d.ts" />
/// <reference path="dev/renderMenu.ts" />
/// <reference path="dev/view.ts" />
/// <reference path="dev/shell.ts" />
/// <reference path="dev/osd.ts" />
//// <reference path="vendor/linq.d.ts" />
// load framework
const { app, BrowserWindow, Menu, Notification } = require('electron');
const mainView = "./views/index.html";
// start the R# backend environment
const backend = workbench.Shell.Rweb();
const defaultViewSize = [1440, 900];
// load internal app components
let template = require("./menu.json");
let menu = null;
// Electron 会在初始化后并准备
// 创建浏览器窗口时，调用这个函数。
// 部分 API 在 ready 事件触发后才能使用。
app.on('ready', workbench.view.createWindow(mainView, defaultViewSize, function () {
    menu = workbench.view.renderAppMenu(template);
}, true));
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
// 在这个文件中，你可以续写应用剩下主进程代码。
// 也可以拆分成几个文件，然后用 require 导入。
workbench.Shell.initialize();
//# sourceMappingURL=index.js.map