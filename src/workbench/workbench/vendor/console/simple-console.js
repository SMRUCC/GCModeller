var System;
(function (System) {
    /**
     * 表示控制台应用程序的标准输入流、输出流和错误流。 此类不能被继承。
     *
     * > a typescript work derived from https://github.com/1j01/simple-console
    */
    var Console = /** @class */ (function () {
        function Console(options) {
            var _this = this;
            this.options = options;
            this.input_wrapper = $ts("<div>", {
                class: "simple-console-input-wrapper"
            });
            if (!options.handleCommand && !options.outputOnly) {
                throw new Error("You must specify either options.handleCommand(input) or options.outputOnly");
            }
            var vm = this;
            var output_only = options.outputOnly;
            var placeholder = options.placeholder || "";
            var autofocus = options.autofocus;
            var storage_id = options.storageID || "simple-console";
            var console_element = $ts("<div>", {
                class: "simple-console"
            });
            this.history = new System.ConsoleDevice.history(storage_id + " command history");
            this.element = console_element;
            this.output = $ts("<div>", {
                class: "simple-console-output",
                role: "log",
                "aria-live": "polite"
            });
            System.ConsoleUI.add_chevron(this.input_wrapper);
            this.input = $ts("<input>", {
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
            addEventListener("keydown", function (e) { return _this.keydown27(e); });
            new System.ConsoleUI.add_popup_menu_button(function () {
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
            this.input.addEventListener("keydown", function (e) { return _this.keydown(e); });
        }
        Object.defineProperty(Console.prototype, "get_last_entry", {
            get: function () {
                return this.last_entry;
            },
            enumerable: true,
            configurable: true
        });
        ;
        ;
        Console.prototype.addPopupButton = function (update) {
            return new System.ConsoleUI.add_popup_button(update, this);
        };
        Console.prototype.addPopupMenuButton = function (getHistories) {
            return new System.ConsoleUI.add_popup_menu_button(getHistories, this);
        };
        Console.prototype.addButton = function (action) {
            var button = $ts("<button>", {
                onclick: action
            });
            this.input_wrapper.appendChild(button);
            return button;
        };
        ;
        Console.prototype.populateHistoryItems = function () {
            var items = [];
            var command_history = this.history.command_history;
            var vm = this;
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
            }
            else {
                items.push({
                    label: "Command history empty",
                    action: function () { }
                });
            }
            return items;
        };
        Console.prototype.pushMyHistory = function (command, i) {
            var vm = this;
            return {
                label: command,
                action: function () {
                    vm.input.value = command;
                    vm.input.focus();
                    vm.input.setSelectionRange(vm.input.value.length, vm.input.value.length);
                }
            };
        };
        Console.prototype.handleUncaughtErrors = function () {
            window.onerror = this.error;
            return this;
        };
        ;
        Console.prototype.keydown27 = function (e) {
            var vm = this;
            if (e.keyCode === 27) {
                // Escape
                if (vm.open_popup_button) {
                    var popup_button = vm.open_popup_button;
                    e.preventDefault();
                    popup_button.closePopup();
                    popup_button.focus();
                }
                else if (e.target.closest(".simple-console") === this.element) {
                    vm.input.focus();
                }
            }
        };
        Console.prototype.keydown = function (e) {
            var history = this.history;
            if (e.keyCode === 13) {
                // Enter
                var command = this.input.value;
                if (command === "") {
                    return;
                }
                else {
                    this.input.value = "";
                    this.history.tryPushCommand(command);
                }
                var command_entry = this.log(command);
                command_entry.classList.add("input");
                System.ConsoleUI.add_chevron(command_entry);
                this.output.scroll_to_bottom();
                this.options.handleCommand(command);
            }
            else if (e.keyCode === 38) {
                // Up
                this.input.value = history.get_last_command();
                this.input.setSelectionRange(this.input.value.length, this.input.value.length);
                e.preventDefault();
            }
            else if (e.keyCode === 40) {
                // Down
                this.input.value = history.get_next_command();
                this.input.setSelectionRange(this.input.value.length, this.input.value.length);
                e.preventDefault();
            }
            else if (e.keyCode === 46 && e.shiftKey) {
                // Shift+Delete
                if (this.input.value === history.current_command_history) {
                    this.input.value = history.delete_command_history();
                }
                e.preventDefault();
            }
        };
        Console.prototype.clear = function () {
            this.output.innerHTML = "";
        };
        ;
        /**
         * 将指定的字符串值（后跟当前行终止符）写入标准输出流。
        */
        Console.prototype.log = function (content) {
            var was_scrolled_to_bottom = this.output.is_scrolled_to_bottom();
            var vm = this;
            var entry = $ts("<div>", {
                class: "entry"
            });
            if (content instanceof Element) {
                entry.appendChild(content);
            }
            else {
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
        ;
        Console.prototype.logHTML = function (html) {
            this.log("");
            this.get_last_entry.innerHTML = html;
        };
        ;
        Console.prototype.error = function (content) {
            this.log(content);
            this.get_last_entry.classList.add("error");
        };
        ;
        Console.prototype.warn = function (content) {
            this.log(content);
            this.get_last_entry.classList.add("warning");
        };
        ;
        Console.prototype.info = function (content) {
            this.log(content);
            this.get_last_entry.classList.add("info");
        };
        ;
        Console.prototype.success = function (content) {
            this.log(content);
            this.get_last_entry.classList.add("success");
        };
        ;
        return Console;
    }());
    System.Console = Console;
})(System || (System = {}));
/// <reference path="../../../../typescript/build/linq.d.ts" />
var System;
(function (System) {
    var ConsoleUI;
    (function (ConsoleUI) {
        var add_popup_button = /** @class */ (function () {
            function add_popup_button(update_popup, console) {
                var _this = this;
                this.update_popup = update_popup;
                this.console = console;
                var vm = this;
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
                ConsoleUI.add_command_history_icon(this.popup_button);
                console.input_wrapper
                    .appendElement(this.popup_button)
                    .appendElement(this.popup);
                this.popup_button.addEventListener("keydown", function (e) { return _this.keydown40(e); });
                this.popup.addEventListener("keydown", function (e) { return _this.keydown38(e); }, true);
                this.popup_button.addEventListener("click", function () {
                    vm.toggle_popup();
                });
                addEventListener("mousedown", function (e) { return _this.mousedown(e); });
                addEventListener("focusin", function (e) { return _this.focusin(e); });
                this.popup_button.popup = vm.popup;
                this.popup_button.openPopup = function () { return vm.open_popup(); };
                this.popup_button.closePopup = function () { return vm.close_popup(); };
                this.popup_button.togglePopup = function () { return vm.toggle_popup(); };
                this.popup_button.popupIsOpen = function () { return vm.popup_is_open(); };
            }
            ;
            add_popup_button.prototype.keydown40 = function (e) {
                if (e.keyCode === 40) {
                    // Down
                    e.preventDefault();
                    this.first_item = this.popup.querySelector("[tabindex='0']");
                    this.first_item.focus();
                }
            };
            add_popup_button.prototype.keydown38 = function (e) {
                if (e.keyCode === 38) {
                    // Up
                    this.first_item = this.popup.querySelector("[tabindex='0']");
                    if (document.activeElement === this.first_item) {
                        this.popup_button.focus();
                    }
                }
            };
            add_popup_button.prototype.open_popup = function () {
                this.popup_button.setAttribute("aria-expanded", "true");
                this.popup.setAttribute("aria-hidden", "false");
                this.console.open_popup_button = this.popup_button;
                this.update_popup(this.popup);
            };
            ;
            add_popup_button.prototype.close_popup = function () {
                this.popup_button.setAttribute("aria-expanded", "false");
                this.popup.setAttribute("aria-hidden", "true");
                this.console.open_popup_button = null;
            };
            ;
            add_popup_button.prototype.toggle_popup = function () {
                if (this.popup_is_open()) {
                    this.close_popup();
                }
                else {
                    this.open_popup();
                }
            };
            ;
            add_popup_button.prototype.mousedown = function (e) {
                if (this.popup_is_open()) {
                    var e_target = e.target;
                    if (!(e_target.closest(".popup-button") == this.popup_button ||
                        e_target.closest(".popup-menu") == this.popup)) {
                        this.close_popup();
                    }
                }
            };
            add_popup_button.prototype.focusin = function (e) {
                if (this.popup_is_open()) {
                    var e_target = e.target;
                    if (!(e_target.closest(".popup-button") == this.popup_button ||
                        e_target.closest(".popup-menu") == this.popup)) {
                        e.preventDefault();
                        this.close_popup();
                    }
                }
            };
            add_popup_button.prototype.popup_is_open = function () {
                return this.popup_button.getAttribute("aria-expanded") == "true";
            };
            ;
            return add_popup_button;
        }());
        ConsoleUI.add_popup_button = add_popup_button;
    })(ConsoleUI = System.ConsoleUI || (System.ConsoleUI = {}));
})(System || (System = {}));
var System;
(function (System) {
    var ConsoleUI;
    (function (ConsoleUI) {
        var add_popup_menu_button = /** @class */ (function () {
            function add_popup_menu_button(get_items, console) {
                var _this = this;
                var popup_button = new ConsoleUI.add_popup_button(function (menu) { return _this.menu_update(menu, get_items); }, console).popup_button;
                var menu = popup_button.popup;
                this.popup_button = popup_button;
                menu.addEventListener("click", function (e) { return _this.click(e); });
                menu.addEventListener("keydown", function (e) { return _this.keydown(e); });
            }
            add_popup_menu_button.prototype.menu_update = function (menu, get_items) {
                var divider;
                var menu_item;
                menu.innerHTML = "";
                for (var _i = 0, _a = get_items(); _i < _a.length; _i++) {
                    var item = _a[_i];
                    if (item.type === "divider") {
                        divider = document.createElement("hr");
                        divider.classList.add("menu-divider");
                        menu.appendChild(divider);
                    }
                    else {
                        menu_item = document.createElement("div");
                        menu_item.classList.add("menu-item");
                        menu_item.setAttribute("tabindex", "0");
                        menu_item.addEventListener("click", item.action);
                        menu_item.textContent = item.label;
                        menu.appendChild(menu_item);
                    }
                }
            };
            add_popup_menu_button.prototype.keydown = function (e) {
                if (e.keyCode === 38) {
                    // Up
                    var prev = document.activeElement.previousElementSibling;
                    e.preventDefault();
                    while (prev && prev.nodeName === "HR") {
                        prev = prev.previousElementSibling;
                    }
                    if (prev && prev.classList.contains("menu-item")) {
                        prev.focus();
                    }
                }
                else if (e.keyCode === 40) {
                    // Down                
                    var next = document.activeElement.nextElementSibling;
                    e.preventDefault();
                    while (next && next.nodeName === "HR") {
                        next = next.nextElementSibling;
                    }
                    if (next && next.classList.contains("menu-item")) {
                        next.focus();
                    }
                }
                else if (e.keyCode === 13 || e.keyCode === 32) {
                    // Enter or Space
                    e.preventDefault();
                    document.activeElement.click();
                }
            };
            add_popup_menu_button.prototype.click = function (e) {
                var menu_item = e.target.closest(".menu-item");
                if (menu_item) {
                    this.popup_button.closePopup();
                }
            };
            return add_popup_menu_button;
        }());
        ConsoleUI.add_popup_menu_button = add_popup_menu_button;
    })(ConsoleUI = System.ConsoleUI || (System.ConsoleUI = {}));
})(System || (System = {}));
var System;
(function (System) {
    var ConsoleUI;
    (function (ConsoleUI) {
        function add_svg(to_element, icon_class_name, svg, viewBox) {
            if (viewBox === void 0) { viewBox = "0 0 16 16"; }
            var icon = $ts("<span>", {
                class: icon_class_name
            }).display('<svg width="1em" height="1em" viewBox="' + viewBox + '">' + svg + '</svg>');
            to_element.insertBefore(icon, to_element.firstChild);
        }
        ConsoleUI.add_svg = add_svg;
        ;
        function add_chevron(to_element) {
            add_svg(to_element, "input-chevron", '<path d="M6,4L10,8L6,12" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"></path>');
        }
        ConsoleUI.add_chevron = add_chevron;
        ;
        function add_command_history_icon(to_element) {
            add_svg(to_element, "command-history-icon", '<path style="fill:currentColor" d="m 44.77595,87.58531 c -5.22521,-1.50964 -12.71218,-5.59862 -14.75245,-8.05699 -1.11544,-1.34403 -0.96175,-1.96515 1.00404,-4.05763 2.86639,-3.05114 3.32893,-3.0558 7.28918,-0.0735 18.67347,14.0622 46.68328,-0.57603 46.68328,-24.39719 0,-16.97629 -14.94179,-31.06679 -31.5,-29.70533 -14.50484,1.19263 -25.37729,11.25581 -28.04263,25.95533 l -0.67995,3.75 6.6362,0 6.6362,0 -7.98926,8 c -4.39409,4.4 -8.35335,8 -8.79836,8 -0.44502,0 -4.38801,-3.6 -8.7622,-8 l -7.95308,-8 6.11969,0 6.11969,0 1.09387,-6.20999 c 3.5237,-20.00438 20.82127,-33.32106 40.85235,-31.45053 11.43532,1.06785 21.61339,7.05858 27.85464,16.39502 13.06245,19.54044 5.89841,45.46362 -15.33792,55.50045 -7.49404,3.54188 -18.8573,4.55073 -26.47329,2.35036 z m 6.22405,-32.76106 c 0,-6.94142 0,-13.88283 0,-20.82425 2,0 4,0 6,0 0,6.01641 0,12.03283 0,18.04924 4.9478,2.93987 9.88614,5.89561 14.82688,8.84731 l -3.27407,4.64009 c -5.88622,-3.5132 -11.71924,-7.11293 -17.55281,-10.71239 z"/>', "0 0 102 102");
        }
        ConsoleUI.add_command_history_icon = add_command_history_icon;
        ;
    })(ConsoleUI = System.ConsoleUI || (System.ConsoleUI = {}));
})(System || (System = {}));
var System;
(function (System) {
    var ConsoleDevice;
    (function (ConsoleDevice) {
        var history = /** @class */ (function () {
            function history(command_history_key, command_history) {
                if (command_history === void 0) { command_history = []; }
                this.command_history_key = command_history_key;
                this.command_history = command_history;
                this.command_index = this.command_history.length;
            }
            Object.defineProperty(history.prototype, "current_command_history", {
                get: function () {
                    return this.command_history[this.command_index];
                },
                enumerable: true,
                configurable: true
            });
            history.prototype.get_last_command = function () {
                if (--this.command_index < 0) {
                    this.command_index = -1;
                    return "";
                }
                else {
                    return this.command_history[this.command_index];
                }
            };
            history.prototype.get_next_command = function () {
                if (++this.command_index >= this.command_history.length) {
                    this.command_index = this.command_history.length;
                    return "";
                }
                else {
                    return this.command_history[this.command_index];
                }
            };
            history.prototype.delete_command_history = function () {
                var value;
                this.command_history.splice(this.command_index, 1);
                this.command_index = Math.max(0, this.command_index - 1);
                value = this.command_history[this.command_index] || "";
                this.save_command_history();
                return value;
            };
            history.prototype.tryPushCommand = function (command) {
                if (this.command_history[this.command_history.length - 1] !== command) {
                    this.command_history.push(command);
                }
                this.command_index = this.command_history.length;
                this.save_command_history();
            };
            history.prototype.load_command_history = function () {
                try {
                    this.command_history = JSON.parse(localStorage[this.command_history_key]);
                    this.command_index = this.command_history.length;
                }
                catch (e) { }
            };
            ;
            history.prototype.save_command_history = function () {
                try {
                    localStorage[this.command_history_key] = JSON.stringify(this.command_history);
                }
                catch (e) { }
            };
            ;
            history.prototype.clear_command_history = function () {
                this.command_history = [];
                this.save_command_history();
            };
            ;
            return history;
        }());
        ConsoleDevice.history = history;
    })(ConsoleDevice = System.ConsoleDevice || (System.ConsoleDevice = {}));
})(System || (System = {}));
//# sourceMappingURL=simple-console.js.map