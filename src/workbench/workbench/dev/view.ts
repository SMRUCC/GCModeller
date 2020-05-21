module workbench.view {

    // 保持对window对象的全局引用，如果不这么做的话，当JavaScript对象被
    // 垃圾回收的时候，window对象将会自动的关闭
    let windows: Electron.BrowserWindow[] = <any>{};

    export function getMainWindow() {
        if (mainView in windows) {
            return windows[mainView];
        } else {
            return null;
        }
    }

    export interface action { (): void; }

    export function createWindow(view: string, size: number[] = [800, 600], callback: action = null, lambda: boolean = false, debug = false): action {
        let invoke: action = function (): action {
            // 创建浏览器窗口。
            let win = new BrowserWindow({ width: size[0], height: size[1] })

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
                windows[view] = null
            });

            if (callback) {
                callback();
            }

            return <action>function () {

            }
        }

        return lambda ? invoke : <any>invoke();
    }
}