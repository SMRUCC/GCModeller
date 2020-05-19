/// <reference path="../../typescript/build/linq.d.ts" />
/// <reference path="../vendor/console/simple-console.d.ts" />
interface RInvoke {
    code: number;
    info: string;
    content_type: string;
    server_time: number;
    warnings: RMessage[];
    err: RMessage;
}
interface RMessage {
    message: string[];
    level: string | number;
    environmentStack: StackFrame[];
    trace: StackFrame;
}
interface StackFrame {
    Method: Method;
    File: string;
    Line: number;
}
interface Method {
    Namespace: string;
    Module: string;
    Method: string;
}
declare namespace RWeb.shell {
    function handle_command(command: string): void;
}
declare namespace RWeb {
    const console: System.Console;
    function run_app(): void;
}
