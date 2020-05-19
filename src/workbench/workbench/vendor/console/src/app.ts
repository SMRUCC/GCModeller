/// <reference path="../../../../typescript/build/linq.d.ts" />

namespace System {

    /**
     * 表示控制台应用程序的标准输入流、输出流和错误流。 此类不能被继承。
     * 
     * > a typescript work derived from https://github.com/1j01/simple-console
    */
    export class Console {

        readonly input_wrapper = $ts("<div>", {
            class: "simple-console-input-wrapper"
        });

        public open_popup_button;
        public output: ConsoleDevice.IConsoleOutputDevice;
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
            var placeholder = options.placeholder || "";
            var autofocus = options.autofocus;
            var storage_id = options.storageID || "simple-console";
            var console_element = $ts("<div>", {
                class: "simple-console"
            });

            this.history = new ConsoleDevice.history(storage_id + " command history");
            this.element = console_element;
            this.output = <any>$ts("<div>", {
                class: "simple-console-output",
                role: "log",
                "aria-live": "polite"
            });

            ConsoleUI.add_chevron(this.input_wrapper);

            this.input = <any>$ts("<input>", {
                class: "simple-console-input",
                autofocus: autofocus,
                placeholder: placeholder,
                "aria-label": placeholder
            });

            this.element.appendChild(this.output);

            if (!output_only) {
                this.element.appendChild(this.input_wrapper);
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

        public addPopupMenuButton(getHistories: ConsoleDevice.get_historyItems) {
            return new ConsoleUI.add_popup_menu_button(getHistories, this);
        }

        public addButton(action: Delegate.Sub) {
            var button = $ts("<button>", {
                onclick: action
            });
            this.input_wrapper.appendChild(button);
            return button;
        };

        populateHistoryItems(): ConsoleDevice.historyItem[] {
            let items: ConsoleDevice.historyItem[] = [];
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

        handleUncaughtErrors(): Console {
            (<any>window).onerror = this.error;
            return this;
        };

        keydown27(e: KeyboardEvent) {
            let vm = this;

            if (e.keyCode === 27) {
                // Escape
                if (vm.open_popup_button) {
                    let popup_button = vm.open_popup_button;

                    e.preventDefault();
                    popup_button.closePopup();
                    popup_button.focus();
                } else if ((<any>e.target).closest(".simple-console") === this.element) {
                    vm.input.focus();
                }
            }
        }

        keydown(e: KeyboardEvent) {
            let history = this.history;

            if (e.keyCode === 13) {
                // Enter
                let command = this.input.value;

                if (command === "") {
                    return;
                } else {
                    this.input.value = "";
                    this.history.tryPushCommand(command);
                }

                let command_entry = this.log(command);
                command_entry.classList.add("input");
                ConsoleUI.add_chevron(command_entry);

                this.output.scroll_to_bottom();
                this.options.handleCommand(command);

            } else if (e.keyCode === 38) {
                // Up
                this.input.value = history.get_last_command();
                this.input.setSelectionRange(this.input.value.length, this.input.value.length);

                e.preventDefault();

            } else if (e.keyCode === 40) {
                // Down
                this.input.value = <string>history.get_next_command();
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

        /**
         * 将指定的字符串值（后跟当前行终止符）写入标准输出流。
        */
        public log(content: string | HTMLElement) {
            let was_scrolled_to_bottom = this.output.is_scrolled_to_bottom();
            let vm = this;
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
                    vm.output.scroll_to_bottom();
                }
            });

            this.last_entry = entry;

            return entry;
        };

        public logHTML(html: string) {
            this.log("");
            this.get_last_entry.innerHTML = html;
        };

        public error(content: string | HTMLElement) {
            this.log(content);
            this.get_last_entry.classList.add("error");
        };

        public warn(content: string | HTMLElement) {
            this.log(content);
            this.get_last_entry.classList.add("warning");
        };

        public info(content: string | HTMLElement) {
            this.log(content);
            this.get_last_entry.classList.add("info");
        };

        public success(content: string | HTMLElement) {
            this.log(content);
            this.get_last_entry.classList.add("success");
        };
    }
}