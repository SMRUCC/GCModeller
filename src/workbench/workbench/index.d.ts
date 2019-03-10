declare module helpers {
    function renderAppMenu(template: Electron.MenuItemConstructorOptions[]): Electron.Menu;
    interface Sub {
        (): void;
    }
    function getMainWindow(): any;
    function createWindow(view: string, size?: number[], callback?: Sub, lambda?: boolean, debug?: boolean): Sub;
}
declare const app: Electron.App, BrowserWindow: typeof Electron.BrowserWindow, Menu: typeof Electron.Menu, Notification: typeof Electron.Notification;
declare const mainView: string;
declare var template: Electron.MenuItemConstructorOptions[];
declare let menu: Electron.Menu;
