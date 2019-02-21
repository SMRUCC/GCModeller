var helpers;
(function (helpers) {
    function renderAppMenu(template) {
        // replace all url as menu click
        template.forEach(function (m) {
            if (!(m.submenu instanceof Menu)) {
                m.submenu
                    .filter(sm => "click" in sm)
                    .forEach(sm => {
                    var url = sm.click + "";
                    sm.click = function () {
                        require('electron').shell.openExternal(url);
                    };
                });
                m.submenu
                    .filter(sm => sm.label == "Quit")
                    .forEach(sm => {
                    sm.click = function () {
                        app.quit();
                    };
                });
            }
        });
        console.log(template);
        const menu = Menu.buildFromTemplate(template);
        Menu.setApplicationMenu(menu);
        return menu;
    }
    helpers.renderAppMenu = renderAppMenu;
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
    helpers.getMainWindow = getMainWindow;
    function createWindow(view, size = [800, 600], callback = null, lambda = false, debug = false) {
        var invoke = function () {
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
    helpers.createWindow = createWindow;
})(helpers || (helpers = {}));
/// <reference path="node_modules/electron/electron.d.ts" />
/// <reference path="dev/helper.ts" />
// load framework
const { app, BrowserWindow, Menu, Notification } = require('electron');
const mainView = "./views/index.html";
// load internal app components
var template = require("./menu.json");
let menu = null;
// Electron 会在初始化后并准备
// 创建浏览器窗口时，调用这个函数。
// 部分 API 在 ready 事件触发后才能使用。
app.on('ready', helpers.createWindow(mainView, [800, 600], function () {
    menu = helpers.renderAppMenu(template);
    var msg = new Notification({ title: "Task Finish", body: "test task finished!" });
    msg.show();
}, true));
// 当全部窗口关闭时退出。
app.on('window-all-closed', () => {
    // 在 macOS 上，除非用户用 Cmd + Q 确定地退出，
    // 否则绝大部分应用及其菜单栏会保持激活。
    if (process.platform !== 'darwin') {
        app.quit();
    }
});
app.on('activate', () => {
    // 在macOS上，当单击dock图标并且没有其他窗口打开时，
    // 通常在应用程序中重新创建一个窗口。
    if (helpers.getMainWindow() === null) {
        helpers.createWindow(mainView);
    }
});
// 在这个文件中，你可以续写应用剩下主进程代码。
// 也可以拆分成几个文件，然后用 require 导入。
//# sourceMappingURL=index.js.map