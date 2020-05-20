declare function openView(view: string): any;
declare function ipc_sendData(key: string, value: string, win: any): void;
declare function getData(key: string, action: (data: string) => void): void;
