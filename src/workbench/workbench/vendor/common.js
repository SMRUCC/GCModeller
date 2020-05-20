function openView(view) {
    const remote = require('electron').remote;
    const BrowserWindow = remote.BrowserWindow;

    let win = new BrowserWindow({ width: 800, height: 600 });
    win.loadFile(view);
	return win;
    // win.webContents.openDevTools();
}

function ipc_sendData(key, value, win) {
	// main process
	win.webContents.send(key, value);
}

function getData(key, action) {
	// renderer process
	let ipcRenderer = require('electron').ipcRenderer;
	
	ipcRenderer.on(key, function (event, data) {
		action(store);
	});	
}