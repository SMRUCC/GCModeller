namespace DOM.Events {

    export class StatusChanged {

        private agree: boolean;

        public get changed(): boolean {
            let test: boolean = this.predicate();

            // 当前的测试状态为yes，并且之前的状态也为yes
            // 则肯定没有发生变化
            if (test && this.agree) {
                return false;
            } else if (test && !this.agree) {
                // 当前的测试状态为yes，并且之前的状态为no
                // 则肯定发生了变化
                this.agree = true;

                return true;
            } else if (!test && this.agree) {
                // 当前的测试状态为no，并且之前的状态为yes
                // 则肯定发生了变化
                this.agree = false;

                return this.triggerNo;
            } else {
                return false;
            }
        }

        public constructor(
            private predicate: Delegate.Func<boolean>,
            private triggerNo: boolean = true) {

            this.agree = predicate();

            if (isNullOrUndefined(triggerNo)) {
                this.triggerNo = false;
            }
        }
    }
}