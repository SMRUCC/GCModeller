/// <reference path="../../Collections/Abstract/Enumerator.ts" />

namespace Internal {

    /**
     * 程序的堆栈追踪信息
     * 
     * 这个对象是调用堆栈``StackFrame``片段对象的序列集合
    */
    export class StackTrace extends IEnumerator<StackFrame> {

        public constructor(frames: IEnumerator<StackFrame> | StackFrame[]) {
            super(frames);
        }

        /**
         * 导出当前的程序运行位置的调用堆栈信息
        */
        public static Dump(): StackTrace {
            var err = new Error().stack.split("\n");
            var trace = From(err)
                //   1 是第一行 err 字符串, 
                // + 1 是跳过当前的这个Dump函数的栈信息
                .Skip(1 + 1)
                .Select(StackFrame.Parse);

            return new StackTrace(trace);
        }

        /**
         * 获取函数调用者的名称的帮助函数
        */
        public static GetCallerMember(): StackFrame {
            var trace = StackTrace.Dump().ToArray();
            // index = 1 是GetCallerMemberName这个函数的caller的栈片段
            // index = 2 就是caller的caller的栈片段，即该caller的CallerMemberName
            var caller = trace[1 + 1];

            return caller;
        }

        public toString(): string {
            var sb = new StringBuilder();

            this.ForEach(frame => {
                sb.AppendLine(`  at ${frame.toString()}`);
            });

            return sb.toString();
        }
    }
}