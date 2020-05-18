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

    export class Console {

        readonly input_wrapper = $ts("<div>", {
            class: "simple-console-input-wrapper"
        });

        private add_button(action: Delegate.Sub) {
            var button = $ts("<button>", {
                onclick: action
            });
            this.input_wrapper.appendChild(button);
            return button;
        };

        public open_popup_button;

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


            var output = $ts("<div>", {
                class: "simple-console-output",
                role: "log",
                "aria-live": "polite"
            });

            add_chevron(this.input_wrapper);

            var input = $ts("<input>", {
                class: "simple-console-input",
                autofocus: "autofocus",
                placeholder: placeholder,
                "aria-label": placeholder
            });

            console_element.appendChild(output);

            if (!output_only) {
                console_element.appendChild(this.input_wrapper);
            }
            this.input_wrapper.appendChild(input);

     

            addEventListener("keydown", function (e) {
                if (e.keyCode === 27) { // Escape
                    if (vm.open_popup_button) {
                        e.preventDefault();
                        var popup_button = vm.open_popup_button;
                        popup_button.closePopup();
                        popup_button.focus();
                    } else if (e.target.closest(".simple-console") === console_element) {
                        input.focus();
                    }
                }
            });

            add_popup_menu_button(function () {
                var items = [];

                if (command_history.length > 0) {
                    for (var i = 0; i < command_history.length; i++) {
                        var command = command_history[i];
                        (function (command, i) {
                            items.push({
                                label: command,
                                action: function () {
                                    input.value = command;
                                    input.focus();
                                    input.setSelectionRange(input.value.length, input.value.length);
                                }
                            });
                        }(command, i));
                    }

                    items.push({
                        type: "divider"
                    });

                    items.push({
                        label: "Clear command history",
                        action: clear_command_history
                    });
                } else {
                    items.push({
                        label: "Command history empty",
                        action: function () { }
                    });
                }

                return items;
            });

            var clear = function () {
                output.innerHTML = "";
            };

            var last_entry;
            var get_last_entry = function () {
                return last_entry;
            };

            var log = function (content) {
                var was_scrolled_to_bottom = output.is_scrolled_to_bottom();

                var entry = document.createElement("div");
                entry.className = "entry";
                if (content instanceof Element) {
                    entry.appendChild(content);
                } else {
                    entry.innerText = entry.textContent = content;
                }
                output.appendChild(entry);

                requestAnimationFrame(function () {
                    if (was_scrolled_to_bottom) {
                        output.scroll_to_bottom();
                    }
                });

                last_entry = entry;
                return entry;
            };

            var logHTML = function (html) {
                log("");
                get_last_entry().innerHTML = html;
            };

            var error = function (content) {
                log(content);
                get_last_entry().classList.add("error");
            };

            var warn = function (content) {
                log(content);
                get_last_entry().classList.add("warning");
            };

            var info = function (content) {
                log(content);
                get_last_entry().classList.add("info");
            };

            var success = function (content) {
                log(content);
                get_last_entry().classList.add("success");
            };

            output.is_scrolled_to_bottom = function () {
                // 1px margin of error needed in case the user is zoomed in
                return output.scrollTop + output.clientHeight + 1 >= output.scrollHeight;
            };

            output.scroll_to_bottom = function () {
                output.scrollTop = output.scrollHeight;
            };

            var command_history = [];
            var command_index = command_history.length;
            var command_history_key = storage_id + " command history";

            var load_command_history = function () {
                try {
                    command_history = JSON.parse(localStorage[command_history_key]);
                    command_index = command_history.length;
                } catch (e) { }
            };

            var save_command_history = function () {
                try {
                    localStorage[command_history_key] = JSON.stringify(command_history);
                } catch (e) { }
            };

            var clear_command_history = function () {
                command_history = [];
                save_command_history();
            };

            load_command_history();

            input.addEventListener("keydown", function (e) {
                if (e.keyCode === 13) { // Enter

                    var command = input.value;
                    if (command === "") {
                        return;
                    }
                    input.value = "";

                    if (command_history[command_history.length - 1] !== command) {
                        command_history.push(command);
                    }
                    command_index = command_history.length;
                    save_command_history();

                    var command_entry = log(command);
                    command_entry.classList.add("input");
                    add_chevron(command_entry);

                    output.scroll_to_bottom();

                    handle_command(command);

                } else if (e.keyCode === 38) { // Up

                    if (--command_index < 0) {
                        command_index = -1;
                        input.value = "";
                    } else {
                        input.value = command_history[command_index];
                    }
                    input.setSelectionRange(input.value.length, input.value.length);
                    e.preventDefault();

                } else if (e.keyCode === 40) { // Down

                    if (++command_index >= command_history.length) {
                        command_index = command_history.length;
                        input.value = "";
                    } else {
                        input.value = command_history[command_index];
                    }
                    input.setSelectionRange(input.value.length, input.value.length);
                    e.preventDefault();

                } else if (e.keyCode === 46 && e.shiftKey) { // Shift+Delete

                    if (input.value === command_history[command_index]) {
                        command_history.splice(command_index, 1);
                        command_index = Math.max(0, command_index - 1)
                        input.value = command_history[command_index] || "";
                        save_command_history();
                    }
                    e.preventDefault();

                }
            });

            this.element = console_element;
            this.input = input;
            this.addButton = add_button;
            this.addPopupButton = add_popup_button;
            this.addPopupMenuButton = add_popup_menu_button;

            this.handleUncaughtErrors = function () {
                window.onerror = error;
            };

            this.log = log;
            this.logHTML = logHTML;
            this.error = error;
            this.warn = warn;
            this.info = info;
            this.success = success;
            this.getLastEntry = get_last_entry;
            this.clear = clear;

        };
    }
}