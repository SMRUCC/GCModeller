function openView(view) {
    const remote = require('electron').remote;
    const BrowserWindow = remote.BrowserWindow;

    var win = new BrowserWindow({ width: 800, height: 600 });
    win.loadFile(view);
    win.webContents.openDevTools();
}