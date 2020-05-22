interface ConsoleConfig {
    handleCommand: handleCommand;
    outputOnly?: boolean;
    placeholder?: string;
    autofocus?: boolean;
    storageID?: string;
}

interface handleCommand { (command: string): void; }