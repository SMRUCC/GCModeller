/// <reference path="../../../typescript/build/linq.d.ts" />
declare namespace System {
    /**
     * 表示控制台应用程序的标准输入流、输出流和错误流。 此类不能被继承。
     *
     * > a typescript work derived from https://github.com/1j01/simple-console
    */
    class Console {
        options: ConsoleConfig;
        readonly input_wrapper: IHTMLElement;
        open_popup_button: any;
        output: ConsoleDevice.IConsoleOutputDevice;
        input: IHTMLInputElement;
        element: IHTMLElement;
        private last_entry;
        private history;
        readonly get_last_entry: any;
        constructor(options: ConsoleConfig);
        addPopupButton(update: Delegate.Sub): ConsoleUI.add_popup_button;
        addPopupMenuButton(getHistories: ConsoleDevice.get_historyItems): ConsoleUI.add_popup_menu_button;
        addButton(action: Delegate.Sub): IHTMLElement;
        populateHistoryItems(): ConsoleDevice.historyItem[];
        pushMyHistory(command: any, i: any): {
            label: any;
            action: () => void;
        };
        handleUncaughtErrors(): Console;
        keydown27(e: KeyboardEvent): void;
        keydown(e: KeyboardEvent): void;
        clear(): void;
        /**
         * 将指定的字符串值（后跟当前行终止符）写入标准输出流。
        */
        log(content: string | HTMLElement): IHTMLElement;
        logHTML(html: string): void;
        error(content: string | HTMLElement): void;
        warn(content: string | HTMLElement): void;
        info(content: string | HTMLElement): void;
        success(content: string | HTMLElement): void;
    }
}
interface ConsoleConfig {
    handleCommand: handleCommand;
    outputOnly?: boolean;
    placeholder?: string;
    autofocus?: boolean;
    storageID?: string;
}
interface handleCommand {
    (command: string): void;
}
declare namespace System.ConsoleUI {
    class add_popup_button {
        private update_popup;
        private console;
        popup_button: ConsoleDevice.IPopupButton;
        popup: IHTMLElement;
        private first_item;
        constructor(update_popup: Delegate.Sub, console: Console);
        keydown40(e: KeyboardEvent): void;
        keydown38(e: KeyboardEvent): void;
        open_popup(): void;
        close_popup(): void;
        toggle_popup(): void;
        mousedown(e: MouseEvent): void;
        focusin(e: FocusEvent): void;
        popup_is_open(): boolean;
    }
}
declare namespace System.ConsoleUI {
    class add_popup_menu_button {
        popup_button: ConsoleDevice.IPopupButton;
        constructor(get_items: ConsoleDevice.get_historyItems, console: Console);
        menu_update(menu: any, get_items: ConsoleDevice.get_historyItems): void;
        keydown(e: KeyboardEvent): void;
        click(e: MouseEvent): void;
    }
}
declare namespace System.ConsoleUI {
    function add_svg(to_element: HTMLElement, icon_class_name: string, svg: string, viewBox?: string): void;
    function add_chevron(to_element: HTMLElement): void;
    function add_command_history_icon(to_element: HTMLElement): void;
}
declare namespace System.ConsoleDevice {
    interface IConsoleOutputDevice extends IHTMLElement {
        is_scrolled_to_bottom: () => boolean;
        scroll_to_bottom: () => void;
    }
    interface IPopupButton extends IHTMLElement {
        popup: IHTMLElement;
        openPopup: Delegate.Action;
        closePopup: Delegate.Action;
        togglePopup: Delegate.Action;
        popupIsOpen: () => boolean;
    }
    interface historyItem {
        label?: string;
        action?: Delegate.Action;
        type?: string;
    }
    interface get_historyItems {
        (): historyItem[];
    }
}
declare namespace System.ConsoleDevice {
    class history {
        command_history_key: string;
        command_history: (historyItem | string)[];
        private command_index;
        readonly current_command_history: historyItem | string;
        constructor(command_history_key: string, command_history?: (historyItem | string)[]);
        get_last_command(): string;
        get_next_command(): string | historyItem;
        delete_command_history(): string;
        tryPushCommand(command: string): void;
        load_command_history(): void;
        save_command_history(): void;
        clear_command_history(): void;
    }
}
