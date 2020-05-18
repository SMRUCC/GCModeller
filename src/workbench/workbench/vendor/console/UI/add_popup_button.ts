namespace System.ConsoleUI {

    export class add_popup_button {

        public popup_button: IHTMLElement;
        public popup: IHTMLElement;

        constructor(update_popup: Delegate.Sub, console: Console) {
            let vm = this;

            this.popup = $ts("<div>", {
                class: "popup-menu",
                role: "menu",
                "aria-hidden": true,
                id: "popup" + (~~(Math.random() * 0xffffff)).toString(0x10)
            });
            this.popup_button = $ts("<button>", {
                class: "popup-button",
                title: "Command history",
                "aria-haspopup": true,
                "aria-owns": this.popup.id,
                "aria-expanded": false,
                "aria-label": "Command history"
            });

            add_command_history_icon(this.popup_button);

            console.input_wrapper
                .appendElement(this.popup_button)
                .appendElement(popup);

            this.popup_button.addEventListener("keydown", function (e) {
                if (e.keyCode === 40) { // Down
                    e.preventDefault();
                    first_item = popup.querySelector("[tabindex='0']");
                    first_item.focus();
                }
            });

            popup.addEventListener("keydown", function (e) {
                if (e.keyCode === 38) { // Up
                    first_item = popup.querySelector("[tabindex='0']");
                    if (document.activeElement === first_item) {
                        popup_button.focus();
                    }
                }
            }, true);





            popup_button.addEventListener("click", toggle_popup);

            addEventListener("mousedown", e => this.mousedown(e));
            addEventListener("focusin", e => this.focusin(e));

            popup_button.popup = popup;
            popup_button.openPopup = open_popup;
            popup_button.closePopup = close_popup;
            popup_button.togglePopup = toggle_popup;
            popup_button.popupIsOpen = popup_is_open;
        };

        open_popup() {
            this.popup_button.setAttribute("aria-expanded", "true");
            popup.setAttribute("aria-hidden", "false");
            open_popup_button = vm.popup_button;
            update_popup(popup);
        };

        close_popup() {
            this.popup_button.setAttribute("aria-expanded", "false");
            popup.setAttribute("aria-hidden", "true");
            open_popup_button = null;
        };

        toggle_popup() {
            if (this.popup_is_open()) {
                this.close_popup();
            } else {
                this.open_popup();
            }
        };

        mousedown(e: MouseEvent) {
            if (this.popup_is_open()) {
                if (!(
                    e.target.closest(".popup-button") == this.popup_button ||
                    e.target.closest(".popup-menu") == popup
                )) {
                    close_popup();
                }
            }
        }

        focusin(e: FocusEvent) {
            if (this.popup_is_open()) {
                if (!(
                    e.target.closest(".popup-button") == this.popup_button ||
                    e.target.closest(".popup-menu") == popup
                )) {
                    e.preventDefault();
                    close_popup();
                }
            }
        }

        popup_is_open(): boolean {
            return this.popup_button.getAttribute("aria-expanded") == "true";
        };
    }
}