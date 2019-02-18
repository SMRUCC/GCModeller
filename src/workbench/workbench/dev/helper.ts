module helpers {

    export function renderAppMenu(template: Electron.MenuItemConstructorOptions[]): Electron.Menu {

        // replace all url as menu click
        template.forEach(function (m) {
            if (!(m.submenu instanceof Menu)) {
                m.submenu
                    .filter(sm => "click" in sm)
                    .forEach(sm => {
                        var url: string = sm.click + "";

                        sm.click = function () {
                            require('electron').shell.openExternal(url);
                        };
                    });
                m.submenu
                    .filter(sm => sm.label == "Quit")
                    .forEach(sm => {
                        sm.click = function () {
                            app.quit();
                        }
                    })
            }
        });

        console.log(template);

        const menu = Menu.buildFromTemplate(template);
        Menu.setApplicationMenu(menu);

        return menu;
    }

    // 保持对window对象的全局引用，如果不这么做的话，当JavaScript对象被
    // 垃圾回收的时候，window对象将会自动的关闭
    var windows: Electron.BrowserWindow[] = <any>{};

    export interface Sub {
        (): void;
    }

    export function getMainWindow() {
        if (mainView in windows) {
            return windows[mainView];
        } else {
            return null;
        }
    }

    export function createWindow(view: string, size: number[] = [800, 600], callback: Sub = null, lambda: boolean = false, debug = false): Sub {
        var invoke: Sub = function (): Sub {
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

            return <Sub>function () {

            }
        }

        return lambda ? invoke : <any>invoke();
    }
}