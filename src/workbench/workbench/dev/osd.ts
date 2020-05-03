module workbench {

    export function osd() {
        let msg: Electron.Notification = new Notification({
            title: "Task Finish",
            body: "test task finished!"
        });
        msg.show();
        return msg;
    }
}