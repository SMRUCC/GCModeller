namespace System.ConsoleUI {

    export class add_popup_menu_button {

        public popup_button: ConsoleDevice.IPopupButton;

        constructor(get_items: get_historyItems, console: Console) {
            let popup_button = new add_popup_button(menu => this.menu_update(menu, get_items), console).popup_button;
            let menu = popup_button.popup;

            this.popup_button = popup_button;

            menu.addEventListener("click", e => this.click(e));
            menu.addEventListener("keydown", e => this.keydown(e));
        }

        menu_update(menu, get_items: get_historyItems) {
            menu.innerHTML = "";
            let items: historyItem[] = get_items();

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if (item.type === "divider") {
                    var divider = document.createElement("hr");
                    divider.classList.add("menu-divider");
                    menu.appendChild(divider);
                } else {
                    var menu_item = document.createElement("div");
                    menu_item.classList.add("menu-item");
                    menu_item.setAttribute("tabindex", "0");
                    menu_item.addEventListener("click", item.action);
                    menu_item.textContent = item.label;
                    menu.appendChild(menu_item);
                }
            }
        }

        keydown(e: KeyboardEvent) {
            if (e.keyCode === 38) {
                // Up
                let prev: HTMLElement = <any>document.activeElement.previousElementSibling;

                e.preventDefault();
                while (prev && prev.nodeName === "HR") {
                    prev = <any>prev.previousElementSibling;
                }

                if (prev && prev.classList.contains("menu-item")) {
                    prev.focus();
                }
            } else if (e.keyCode === 40) {
                // Down                
                let next: HTMLElement = <any>document.activeElement.nextElementSibling;

                e.preventDefault();
                while (next && next.nodeName === "HR") {
                    next = <any>next.nextElementSibling;
                }
                if (next && next.classList.contains("menu-item")) {
                    next.focus();
                }
            } else if (e.keyCode === 13 || e.keyCode === 32) {
                // Enter or Space
                e.preventDefault();
                (<HTMLElement>document.activeElement).click();
            }
        }

        click(e: MouseEvent) {
            let menu_item = (<HTMLElement>e.target).closest(".menu-item");

            if (menu_item) {
                this.popup_button.closePopup();
            }
        }
    }
}