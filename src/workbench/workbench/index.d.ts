declare module workbench.view {
    function renderAppMenu(template: Electron.MenuItemConstructorOptions[]): Electron.Menu;
}
declare module workbench.view {
    function getMainWindow(): any;
    function createWindow(view: string, size?: number[], callback?: Delegate.Action, lambda?: boolean, debug?: boolean): Delegate.Action;
}
declare module workbench.Shell {
    function Rweb(): any;
}
declare module workbench {
    function osd(): Electron.Notification;
}
declare const app: Electron.App, BrowserWindow: typeof Electron.BrowserWindow, Menu: typeof Electron.Menu, Notification: typeof Electron.Notification;
declare const mainView: string;
declare const backend: any;
declare const defaultViewSize: number[];
declare let template: Electron.MenuItemConstructorOptions[];
declare let menu: Electron.Menu;
