function openView(view) {
    var remote = require('electron').remote;
    var BrowserWindow = remote.BrowserWindow;
    var win = new BrowserWindow({ width: 800, height: 600 });
    win.loadFile(view);
    // win.webContents.openDevTools();
    return win;
}
function ipc_sendData(key, value, win) {
    // main process
    win.webContents.send(key, value);
}
function getData(key, action) {
    // renderer process
    var ipcRenderer = require('electron').ipcRenderer;
    ipcRenderer.on(key, function (event, data) {
        action(data);
    });
}
//# sourceMappingURL=common.js.map