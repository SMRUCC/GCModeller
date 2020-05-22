/// <reference path="node_modules/electron/electron.d.ts" />
/**
 * Module handles configurable splashscreen to show while app is loading.
 */
declare namespace workbench.view {
    /** `electron-splashscreen` config object. */
    interface Config {
        /** Options for the window that is loading and having a splashscreen tied to. */
        windowOpts: Electron.BrowserWindowConstructorOptions;
        /**
         * URL to the splashscreen template. This is the path to an `HTML` or `SVG` file.
         * If you want to simply show a `PNG`, wrap it in an `HTML` file.
         */
        templateUrl: string;
        /**
         * Full set of browser window options for the splashscreen. We override key attributes to
         * make it look & feel like a splashscreen; the rest is up to you!
         */
        splashScreenOpts: Electron.BrowserWindowConstructorOptions;
        /** Number of ms the window will load before splashscreen appears (default: 500ms). */
        delay?: number;
        /** Minimum ms the splashscreen will be visible (default: 500ms). */
        minVisible?: number;
        /** Close window that is loading if splashscreen is closed by user (default: true). */
        closeWindow?: boolean;
    }
    /**
     * Initializes a splashscreen that will show/hide smartly (and handle show/hiding of main window).
     * @param config - Configures splashscren
     * @returns {BrowserWindow} the main browser window ready for loading
     */
    const initSplashScreen: (config: Config) => Electron.BrowserWindow;
    /** Return object for `initDynamicSplashScreen()`. */
    interface DynamicSplashScreen {
        /** The main browser window ready for loading */
        main: Electron.BrowserWindow;
        /** The splashscreen browser window so you can communicate with splashscreen in more complex use cases. */
        splashScreen: Electron.BrowserWindow;
    }
    /**
     * Initializes a splashscreen that will show/hide smartly (and handle show/hiding of main window).
     * Use this function if you need to send/receive info to the splashscreen (e.g., you want to send
     * IPC messages to the splashscreen to inform the user of the app's loading state).
     * @param config - Configures splashscren
     * @returns {DynamicSplashScreen} the main browser window and the created splashscreen
     */
    const initDynamicSplashScreen: (config: Config) => DynamicSplashScreen;
}
declare module workbench.view {
    function renderAppMenu(template: Electron.MenuItemConstructorOptions[]): Electron.Menu;
}
declare module workbench.view {
    function getMainWindow(): any;
    interface action {
        (): void;
    }
    function createWindow(view: string, size?: number[], callback?: action, lambda?: boolean, debug?: boolean): action;
}
declare module workbench.Shell {
    function Rweb(): any;
    function initialize(): void;
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
declare let mainWindow: Electron.BrowserWindow;
