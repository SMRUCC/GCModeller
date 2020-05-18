/// <reference path="../linq.d.ts" />

namespace System {

    function add_svg(to_element: HTMLElement, icon_class_name: string, svg: string, viewBox: string = "0 0 16 16") {
        let icon = $ts("<span>", {
            class: icon_class_name
        }).display('<svg width="1em" height="1em" viewBox="' + viewBox + '">' + svg + '</svg>');

        to_element.insertBefore(icon, to_element.firstChild);
    };

    function add_chevron(to_element: HTMLElement) {
        add_svg(to_element, "input-chevron",
            '<path d="M6,4L10,8L6,12" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"></path>'
        );
    };

    export function add_command_history_icon(to_element: HTMLElement) {
        add_svg(to_element, "command-history-icon",
            '<path style="fill:currentColor" d="m 44.77595,87.58531 c -5.22521,-1.50964 -12.71218,-5.59862 -14.75245,-8.05699 -1.11544,-1.34403 -0.96175,-1.96515 1.00404,-4.05763 2.86639,-3.05114 3.32893,-3.0558 7.28918,-0.0735 18.67347,14.0622 46.68328,-0.57603 46.68328,-24.39719 0,-16.97629 -14.94179,-31.06679 -31.5,-29.70533 -14.50484,1.19263 -25.37729,11.25581 -28.04263,25.95533 l -0.67995,3.75 6.6362,0 6.6362,0 -7.98926,8 c -4.39409,4.4 -8.35335,8 -8.79836,8 -0.44502,0 -4.38801,-3.6 -8.7622,-8 l -7.95308,-8 6.11969,0 6.11969,0 1.09387,-6.20999 c 3.5237,-20.00438 20.82127,-33.32106 40.85235,-31.45053 11.43532,1.06785 21.61339,7.05858 27.85464,16.39502 13.06245,19.54044 5.89841,45.46362 -15.33792,55.50045 -7.49404,3.54188 -18.8573,4.55073 -26.47329,2.35036 z m 6.22405,-32.76106 c 0,-6.94142 0,-13.88283 0,-20.82425 2,0 4,0 6,0 0,6.01641 0,12.03283 0,18.04924 4.9478,2.93987 9.88614,5.89561 14.82688,8.84731 l -3.27407,4.64009 c -5.88622,-3.5132 -11.71924,-7.11293 -17.55281,-10.71239 z"/>',
            "0 0 102 102"
        );
    };

    export interface historyItem {
        label?: string;
        action?: Delegate.Action;
        type?: string;
    }

    export interface get_historyItems { (): historyItem[]; }

    export class Console {

        readonly input_wrapper = $ts("<div>", {
            class: "simple-console-input-wrapper"
        });

        public open_popup_button;
        public output: IHTMLElement;
        public input: IHTMLInputElement;
        public element: IHTMLElement;

        private last_entry;
        private history: ConsoleDevice.history;

        public get get_last_entry() {
            return this.last_entry;
        };

        constructor(public options: ConsoleConfig) {

            if (!options.handleCommand && !options.outputOnly) {
                throw new Error("You must specify either options.handleCommand(input) or options.outputOnly");
            }

            let vm = this;

            var output_only = options.outputOnly;
            var handle_command = options.handleCommand;
            var placeholder = options.placeholder || "";
            var autofocus = options.autofocus;
            var storage_id = options.storageID || "simple-console";
            var console_element = $ts("<div>", {
                class: "simple-console"
            });

            this.history = new ConsoleDevice.history(storage_id + " command history");
            this.element = console_element;
            this.output = $ts("<div>", {
                class: "simple-console-output",
                role: "log",
                "aria-live": "polite"
            });

            add_chevron(this.input_wrapper);

            this.input = <any>$ts("<input>", {
                class: "simple-console-input",
                autofocus: "autofocus",
                placeholder: placeholder,
                "aria-label": placeholder
            });

            console_element.appendChild(this.output);

            if (!output_only) {
                console_element.appendChild(this.input_wrapper);
            }
            this.input_wrapper.appendChild(this.input);



            addEventListener("keydown", e => this.keydown27(e));

            new ConsoleUI.add_popup_menu_button(function () {
                return vm.populateHistoryItems();
            }, this);

            this.output.is_scrolled_to_bottom = function () {
                // 1px margin of error needed in case the user is zoomed in
                return vm.output.scrollTop + vm.output.clientHeight + 1 >= vm.output.scrollHeight;
            };

            this.output.scroll_to_bottom = function () {
                vm.output.scrollTop = vm.output.scrollHeight;
            };

            this.history.load_command_history();
            this.input.addEventListener("keydown", e => this.keydown(e));
        };

        public addPopupButton(update: Delegate.Sub) {
            return new ConsoleUI.add_popup_button(update, this);
        }

        public addPopupMenuButton(getHistories: get_historyItems) {
            return new ConsoleUI.add_popup_menu_button(getHistories, this);
        }

        public addButton(action: Delegate.Sub) {
            var button = $ts("<button>", {
                onclick: action
            });
            this.input_wrapper.appendChild(button);
            return button;
        };

        populateHistoryItems(): historyItem[] {
            let items: historyItem[] = [];
            let command_history = this.history.command_history;
            let vm = this;

            if (command_history.length > 0) {
                for (var i = 0; i < command_history.length; i++) {
                    var command = command_history[i];
                    items.push(this.pushMyHistory(command, i));
                }

                items.push({
                    type: "divider"
                });

                items.push({
                    label: "Clear command history",
                    action: function () {
                        vm.history.clear_command_history();
                    }
                });
            } else {
                items.push({
                    label: "Command history empty",
                    action: function () { }
                });
            }

            return items;
        }

        pushMyHistory(command, i) {
            let vm = this;

            return {
                label: command,
                action: function () {
                    vm.input.value = command;
                    vm.input.focus();
                    vm.input.setSelectionRange(vm.input.value.length, vm.input.value.length);
                }
            }
        }

        handleUncaughtErrors() {
            window.onerror = this.error;
        };

        keydown27(e: KeyboardEvent) {
            let vm = this;

            if (e.keyCode === 27) { // Escape
                if (vm.open_popup_button) {
                    e.preventDefault();
                    var popup_button = vm.open_popup_button;
                    popup_button.closePopup();
                    popup_button.focus();
                } else if (e.target.closest(".simple-console") === console_element) {
                    vm.input.focus();
                }
            }
        }

        keydown(e: KeyboardEvent) {
            let history = this.history;

            if (e.keyCode === 13) { // Enter

                var command = this.input.value;
                if (command === "") {
                    return;
                } else {
                    this.input.value = "";
                    this.history.tryPushCommand(command);
                }

                let command_entry = this.log(command);
                command_entry.classList.add("input");
                add_chevron(command_entry);

                this.output.scroll_to_bottom();
                this.handle_command(command);

            } else if (e.keyCode === 38) {
                // Up
                this.input.value = history.get_last_command();
                this.input.setSelectionRange(this.input.value.length, this.input.value.length);

                e.preventDefault();

            } else if (e.keyCode === 40) {
                // Down

                this.input.value = history.get_next_command();
                this.input.setSelectionRange(this.input.value.length, this.input.value.length);
                e.preventDefault();

            } else if (e.keyCode === 46 && e.shiftKey) {
                // Shift+Delete

                if (this.input.value === history.current_command_history) {
                    this.input.value = history.delete_command_history();
                }
                e.preventDefault();

            }
        }

        public clear() {
            this.output.innerHTML = "";
        };

        public log(content) {
            let was_scrolled_to_bottom = this.output.is_scrolled_to_bottom();
            let entry = $ts("<div>", {
                class: "entry"
            });

            if (content instanceof Element) {
                entry.appendChild(content);
            } else {
                entry.innerText = entry.textContent = content;
            }
            this.output.appendChild(entry);

            requestAnimationFrame(function () {
                if (was_scrolled_to_bottom) {
                    this.output.scroll_to_bottom();
                }
            });

            this.last_entry = entry;

            return entry;
        };

        public logHTML(html) {
            this.log("");
            this.get_last_entry.innerHTML = html;
        };

        public error(content) {
            this.log(content);
            this.get_last_entry.classList.add("error");
        };

        public warn(content) {
            this.log(content);
            this.get_last_entry.classList.add("warning");
        };

        public info(content) {
            this.log(content);
            this.get_last_entry.classList.add("info");
        };

        public success(content) {
            this.log(content);
            this.get_last_entry.classList.add("success");
        };
    }
}