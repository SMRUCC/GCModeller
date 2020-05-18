namespace System.ConsoleUI {

    export class add_popup_button {

        public popup_button: IHTMLElement;
        public popup: IHTMLElement;

        constructor(private update_popup: Delegate.Sub, private console: Console) {
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
                .appendElement(this.popup);

            this.popup_button.addEventListener("keydown", e => this.keydown40(e));
            this.popup.addEventListener("keydown", e => this.keydown38(e), true);
            this.popup_button.addEventListener("click", function () {
                vm.toggle_popup();
            });

            addEventListener("mousedown", e => this.mousedown(e));
            addEventListener("focusin", e => this.focusin(e));

            this.popup_button.popup = vm.popup;
            this.popup_button.openPopup = function () { return vm.open_popup() };
            this.popup_button.closePopup = function () { return vm.close_popup() }
            this.popup_button.togglePopup = function () { return vm.toggle_popup() }
            this.popup_button.popupIsOpen = function () { return vm.popup_is_open() }
        };

        keydown40(e: KeyboardEvent) {
            if (e.keyCode === 40) { // Down
                e.preventDefault();
                first_item = this.popup.querySelector("[tabindex='0']");
                first_item.focus();
            }
        }

        keydown38(e: KeyboardEvent) {
            if (e.keyCode === 38) { // Up
                first_item = this.popup.querySelector("[tabindex='0']");
                if (document.activeElement === first_item) {
                    this.popup_button.focus();
                }
            }
        }

        open_popup() {
            this.popup_button.setAttribute("aria-expanded", "true");
            this.popup.setAttribute("aria-hidden", "false");
            this.console.open_popup_button = this.popup_button;
            this.update_popup(this.popup);
        };

        close_popup() {
            this.popup_button.setAttribute("aria-expanded", "false");
            this.popup.setAttribute("aria-hidden", "true");
            this.console.open_popup_button = null;
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
                    e.target.closest(".popup-menu") == this.popup
                )) {
                    this.close_popup();
                }
            }
        }

        focusin(e: FocusEvent) {
            if (this.popup_is_open()) {
                if (!(
                    e.target.closest(".popup-button") == this.popup_button ||
                    e.target.closest(".popup-menu") == this.popup
                )) {
                    e.preventDefault();
                    this.close_popup();
                }
            }
        }

        popup_is_open(): boolean {
            return this.popup_button.getAttribute("aria-expanded") == "true";
        };
    }
}