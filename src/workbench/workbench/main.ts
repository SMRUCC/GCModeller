/// <reference path="node_modules/electron/electron.d.ts" />
/// <reference path="dev/renderMenu.ts" />
/// <reference path="dev/view.ts" />
/// <reference path="dev/shell.ts" />
/// <reference path="dev/osd.ts" />
//// <reference path="vendor/linq.d.ts" />

// load framework
const { app, BrowserWindow, Menu, Notification } = require('electron');

const mainView: string = "./views/index.html";
// start the R# backend environment
const backend = workbench.Shell.Rweb();
const defaultViewSize = [1440, 900];

// load internal app components
let template: Electron.MenuItemConstructorOptions[] = require("./menu.json");
let menu: Electron.Menu = null;

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
        app.quit()
    }
});

app.on('activate', function () {
    // 在macOS上，当单击dock图标并且没有其他窗口打开时，
    // 通常在应用程序中重新创建一个窗口。
    if (workbench.view.getMainWindow() === null) {
        workbench.view.createWindow(mainView, defaultViewSize);
    }
})

// 在这个文件中，你可以续写应用剩下主进程代码。
// 也可以拆分成几个文件，然后用 require 导入。
workbench.Shell.initialize();