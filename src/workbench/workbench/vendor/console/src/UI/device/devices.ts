namespace System.ConsoleDevice {

    export interface IConsoleOutputDevice extends IHTMLElement {
        is_scrolled_to_bottom: () => boolean; 
        scroll_to_bottom: () => void;
    }

    export interface IPopupButton extends IHTMLElement {
        popup: IHTMLElement;
        openPopup: Delegate.Action;
        closePopup: Delegate.Action;
        togglePopup: Delegate.Action;
        popupIsOpen: () => boolean;
    }

    export interface historyItem {
        label?: string;
        action?: Delegate.Action;
        type?: string;
    }

    export interface get_historyItems { (): historyItem[]; }
}