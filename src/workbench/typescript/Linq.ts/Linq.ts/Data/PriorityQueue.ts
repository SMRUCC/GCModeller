namespace TsLinq {

    export class PriorityQueue<T> extends IEnumerator<QueueItem<T>> {

        private events;

        /**
         * 队列元素
        */
        public get Q(): QueueItem<T>[] {
            return this.sequence;
        }

        public constructor(events) {
            super([]);
            this.events = events;
        }

        public enqueue(obj: T) {
            var last = this.Last;
            var q = this.Q;
            var x = new QueueItem(obj);

            q.push(x);

            if (last) {
                last.below = x;
                x.above = last;
            }
        }

        public extract(i: number): QueueItem<T> {
            var q = this.Q;
            var x_above = q[i - 1];
            var x_below = q[i + 1];
            var x = q.splice(i, 1)[0];

            if (x_above) {
                x_above.below = x_below;
            }
            if (x_below) {
                x_below.above = x_above;
            }

            return x;
        }

        public dequeue(): QueueItem<T> {
            return this.extract(0);
        }
    }

    export class QueueItem<T> {
        public value: T;
        public below: QueueItem<T>;
        public above: QueueItem<T>;

        public constructor(x: T) {
            this.value = x;
        }

        public toString(): string {
            return this.value.toString();
        }
    }
}