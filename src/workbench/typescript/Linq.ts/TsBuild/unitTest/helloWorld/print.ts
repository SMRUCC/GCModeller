// demo test for the js parser
namespace demo.app.pages {

    /**
     * A demo web app for print hello world on console
    */
    export class printTest extends Bootstrap {

        public get appName(): string {
            return "index"
        };

        protected init(): void {
            console.log('Hello world');
        }
    }
}