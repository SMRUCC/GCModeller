module workbench.view {

    export function renderAppMenu(template: Electron.MenuItemConstructorOptions[]): Electron.Menu {
        const menu = Menu.buildFromTemplate(runRender(template));
        Menu.setApplicationMenu(menu);
        return menu;
    }

    function runRender(template: Electron.MenuItemConstructorOptions[]): Electron.MenuItemConstructorOptions[] {
        for (let item of template) {
            renderMenuTemplate(item);
        }

        return template;
    }

    /**
     * replace all url as menu click
    */
    function renderMenuTemplate(templ: Electron.MenuItemConstructorOptions): Electron.MenuItemConstructorOptions {
        if (!(templ.submenu instanceof Menu)) {
            templ.submenu
                .filter(sm => "click" in sm)
                .forEach(sm => {
                    var url: string = sm.click + "";

                    sm.click = function () {
                        require('electron').shell.openExternal(url);
                    };
                });
            templ.submenu
                .filter(sm => sm.label == "Quit")
                .forEach(sm => {
                    sm.click = function () {
                        app.quit();
                    }
                })
        }

        return templ;
    }    
}