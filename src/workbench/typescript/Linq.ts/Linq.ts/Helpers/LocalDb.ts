namespace TypeScript.LocalDb {

    export interface IUseDBDatabase { (db: IDBDatabase): void }


    export class Open {

        private db: IDBDatabase;

        constructor(
            public dbName: string,
            private using: IUseDBDatabase,
            public version: number = 1) {

            this.processDbRequest(indexedDB.open(dbName, version));
        }

        private processDbRequest(request: IDBOpenDBRequest) {
            let vm = this;

            request.onerror = function (evt) {
                console.dir(evt);
            }
            request.onsuccess = function (evt) {
                vm.db = (<any>evt.target).result;
            }

            request.onupgradeneeded = function (evt) {
                vm.db = (<any>evt.target).result;
                vm.using(vm.db);
            }
        }
    }
}