function openView(view: string) {
    const remote = require('electron').remote;
    const BrowserWindow = remote.BrowserWindow;

    let win = new BrowserWindow({ width: 800, height: 600 });
    win.loadFile(view);
    // win.webContents.openDevTools();
    return win;
}

function ipc_sendData(key: string, value: string, win) {
    // main process
    win.webContents.send(key, value);
}

function getData(key: string, action: (data: string) => void) {
    // renderer process
    let ipcRenderer = require('electron').ipcRenderer;

    ipcRenderer.on(key, function (event, data) {
        action(data);
    });
}