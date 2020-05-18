namespace System.ConsoleDevice {

    export class history {

        private command_index: number

        public get current_command_history(): historyItem | string {
            return this.command_history[this.command_index];
        }

        constructor(public command_history_key: string, public command_history: (historyItem | string)[] = []) {
            this.command_index = this.command_history.length;
        }

        get_last_command(): string {
            if (--this.command_index < 0) {
                this.command_index = -1;
                return "";
            } else {
                return this.command_history[this.command_index];
            }
        }

        get_next_command() {
            if (++this.command_index >= this.command_history.length) {
                this.command_index = this.command_history.length;
               return  "";
            } else {
                return this.command_history[this.command_index];
            }
        }

        delete_command_history(): string {
            let value: string;

            this.command_history.splice(this.command_index, 1);
            this.command_index = Math.max(0, this.command_index - 1)
            value = this.command_history[this.command_index] || "";
            this.save_command_history();

            return value;
        }

        tryPushCommand(command: string) {
            if (this.command_history[this.command_history.length - 1] !== command) {
                this.command_history.push(command);
            }
            this.command_index = this.command_history.length;
            this.save_command_history();
        }

        load_command_history() {
            try {
                this.command_history = JSON.parse(localStorage[this.command_history_key]);
                this.command_index = this.command_history.length;
            } catch (e) { }
        };

        save_command_history() {
            try {
                localStorage[this.command_history_key] = JSON.stringify(this.command_history);
            } catch (e) { }
        };

        clear_command_history() {
            this.command_history = [];
            this.save_command_history();
        };
    }
}