namespace TsLinq {

    /**
     * 性能计数器
    */
    export class Benchmark {

        public readonly start: number;

        private lastCheck: number;

        public constructor() {
            this.start = (new Date).getTime();
            this.lastCheck = this.start;
        }

        public Tick(): CheckPoint {
            var now: number = (new Date).getTime();
            var checkpoint: CheckPoint = new CheckPoint();

            checkpoint.start = this.start;
            checkpoint.time = now;
            checkpoint.sinceFromStart = now - this.start;
            checkpoint.sinceLastCheck = now - this.lastCheck;

            this.lastCheck = now;

            return checkpoint;
        }
    }

    /**
     * 单位都是毫秒
    */
    export class CheckPoint {

        public start: number;
        public time: number;

        public sinceLastCheck: number;
        public sinceFromStart: number;

        /**
         * 获取从``time``到当前时间所流逝的毫秒计数
        */
        public get elapsedMilisecond(): number {
            return (new Date).getTime() - this.time;
        }
    }
}