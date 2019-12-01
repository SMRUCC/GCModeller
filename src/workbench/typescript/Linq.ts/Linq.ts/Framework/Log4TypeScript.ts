///<reference path="./Modes.ts" />

namespace TypeScript {

    const warningLevel: number = Modes.development;
    const anyoutputLevel: number = Modes.debug;
    const errorOnly: number = Modes.production;

    /**
     * Console logging helper
    */
    export abstract class logging {

        private constructor() {
        }

        /**
         * 应用程序的开发模式：只会输出框架的警告信息
        */
        public static get outputWarning(): boolean {
            return $ts.mode <= warningLevel;
        }

        /**
         * 框架开发调试模式：会输出所有的调试信息到终端之上
        */
        public static get outputEverything(): boolean {
            return $ts.mode == anyoutputLevel;
        }

        /**
         * 生产模式：只会输出错误信息
        */
        public static get outputError(): boolean {
            return $ts.mode == errorOnly;
        }

        public static warning(msg: any) {
            if (this.outputWarning) {
                console.warn(msg);
            }
        }

        /**
         * 使用这个函数显示object的时候，将不会发生样式的变化
        */
        public static log(obj, color: string | ConsoleColors = ConsoleColors.NA) {
            if (typeof color != "string") {
                color = ConsoleColors[color].toLowerCase();
            } else {
                color = color.toLowerCase();
            }

            if (this.outputEverything) {
                if (color == "na" || !TypeExtensions.isPrimitive(obj)) {
                    console.log(obj);
                } else {
                    console.log("%c" + obj, `color:${color}`);
                }
            } else {
                // go silent
            }
        }

        public static table<T extends {}>(objects: T[] | string | IEnumerator<T>) {
            if (this.outputEverything) {
                if (isNullOrUndefined(objects)) {
                    objects = [];
                } else if (typeof objects == "string") {
                    objects = JSON.parse(objects);
                } else if (!Array.isArray(objects)) {
                    objects = (<IEnumerator<T>>objects).ToArray(false);
                }

                console.table(objects);
            } else {
                // go silent
            }
        }

        public static runGroup(title: string, program: Delegate.Action): void {
            let startTime: number = Date.now();

            console.group(title);
            program();
            console.groupEnd();

            let endTime: number = Date.now();
            let costTime: number = endTime - startTime;

            logging.log(`Program '${title}' cost ${costTime}ms to run.`, "darkblue");
        }
    }

    export enum ConsoleColors {

        /**
         * do not set the colors
        */
        NA = -1,

        /**
         * The color black.
        */
        Black = 0,

        /**
         * The color blue.
        */
        Blue = 9,

        /**
         * The color cyan (blue - green).
        */
        Cyan = 11,

        /**
         * The color dark blue.
        */
        DarkBlue = 1,

        /**
         * The color dark cyan(dark blue - green).
        */
        DarkCyan = 3,

        /**
         * The color dark gray.
        */
        DarkGray = 8,

        /**
         * The color dark green.
        */
        DarkGreen = 2,

        /**
         * The color dark magenta(dark purplish - red).
        */
        DarkMagenta = 5,

        /**
         * The color dark red.
        */
        DarkRed = 4,

        /**
         * The color dark yellow(ochre).
        */
        DarkYellow = 6,

        /**
         * The color gray.
        */
        Gray = 7,

        /**
         * The color green.
        */
        Green = 10,

        /**
         * The color magenta(purplish - red).
        */
        Magenta = 13,

        /**
         * The color red.
        */
        Red = 12,

        /**
         * The color white.
        */
        White = 15,

        /**
         * The color yellow.
        */
        Yellow = 14,
    }
}