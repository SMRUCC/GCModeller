namespace TypeScript {

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

        /**
         * 性能计数器起始的时间，这个时间点在所有的由同一个性能计数器对象所产生的checkpoint之间都是一样的
        */
        public start: number;
        /**
         * 创建当前的这个checkpoint的时候的时间戳
        */
        public time: number;

        /**
         * 创建这个checkpoint时间点的时候与上一次创建checkpoint的时间点之间的长度
         * 性能计数主要是查看这个属性值
        */
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