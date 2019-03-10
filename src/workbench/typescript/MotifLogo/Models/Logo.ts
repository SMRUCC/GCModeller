namespace GCModeller.Workbench {

    export class Logo {

        public alphabet: Alphabet;
        public fine_text: string;
        public pspm_list: Pspm[];
        public pspm_column: number[];
        public rows: number = 0;
        public columns: number = 0;

        public constructor(alphabet: Alphabet, fine_text: string) {
            this.alphabet = alphabet;
            this.fine_text = fine_text;
            this.pspm_list = [];
            this.pspm_column = [];
        }

        public addPspm(pspm: Pspm, column: number = null): Logo {
            var col;

            if (typeof column === "undefined") {
                column = 0;
            } else if (column < 0) {
                throw new Error("Column index out of bounds.");
            }
            this.pspm_list[this.rows] = pspm;
            this.pspm_column[this.rows] = column;
            this.rows++;
            col = column + pspm.motif_length;
            if (col > this.columns) {
                this.columns = col;
            }

            return this;
        }

        public getPspm(rowIndex: number): Pspm {
            if (rowIndex < 0 || rowIndex >= this.rows) {
                throw new Error("INDEX_OUT_OF_BOUNDS");
            }
            return this.pspm_list[rowIndex];
        }

        public getOffset(rowIndex): number {
            if (rowIndex < 0 || rowIndex >= this.rows) {
                throw new Error("INDEX_OUT_OF_BOUNDS");
            }
            return this.pspm_column[rowIndex];
        };
    }
}