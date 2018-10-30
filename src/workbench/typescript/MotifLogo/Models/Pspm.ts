namespace GCModeller.Workbench {

    export const evalueRegexp: RegExp = /^((?:[+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?)|inf)$/;

    export class Pspm {

        public name: string;
        public alph_length: number;
        public motif_length: number;
        public nsites: number
        public evalue: number
        public ltrim: number
        public rtrim: number
        public pspm: number[][] = [];

        public constructor(matrix: Pspm | string | number[][],
            name: string = null,
            ltrim: number = null,
            rtrim: number = null,
            nsites: number = null,
            evalue: number | string = null) {

            if (matrix instanceof Pspm) {
                this.copyInternal(matrix);
            } else {
                this.parseInternal(matrix, name, ltrim, rtrim, nsites, evalue);
            }
        }

        private parseInternal(matrix: string | number[][], name: string,
            ltrim: number, rtrim: number,
            nsites: number,
            evalue: string | number): void {

            // check parameters
            if (typeof ltrim === "undefined") {
                ltrim = 0;
            } else if (typeof ltrim !== "number" || ltrim % 1 !== 0 || ltrim < 0) {
                throw new Error("ltrim must be a non-negative integer, got: " + ltrim);
            }
            if (typeof rtrim === "undefined") {
                rtrim = 0;
            } else if (typeof rtrim !== "number" || rtrim % 1 !== 0 || rtrim < 0) {
                throw new Error("rtrim must be a non-negative integer, got: " + rtrim);
            }
            if (typeof nsites !== "undefined") {
                if (typeof nsites !== "number" || nsites <= 0) {
                    throw new Error("nsites must be a positive number, got: " + nsites);
                }
            }
            if (typeof evalue !== "undefined") {
                if (typeof evalue === "number") {
                    if (evalue < 0) {
                        throw new Error("evalue must be a non-negative number, got: " + evalue);
                    } else {
                        this.evalue = evalue;
                    }
                } else if (typeof evalue === "string") {
                    if (!evalueRegexp.test(evalue)) {
                        throw `"evalue must be a non-negative number, got: "${evalue}"`;
                    } else {
                        this.evalue = parseFloat(evalue);
                    }
                } else {
                    throw new Error("evalue must be a non-negative number, got: " + evalue);
                }
            } else {
                this.evalue = undefined;
            }

            // set properties
            this.name = name;
            this.nsites = nsites;
            this.ltrim = ltrim;
            this.rtrim = rtrim;

            if (typeof matrix === "string") {
                // string constructor
                this.matrixParseFromString(parse_pspm_string(matrix));
            } else {
                // assume pspm is a nested array
                this.createFromValueArray(matrix);
            }
        }

        private createFromValueArray(matrix: number[][]) {
            var row_sum: number;
            var delta: number;

            this.motif_length = matrix.length;
            this.alph_length = (matrix.length > 0 ? matrix[0].length : 0);

            if (typeof this.nsites === "undefined") {
                this.nsites = 20;
            }
            if (typeof this.evalue === "undefined") {
                this.evalue = 0;
            }

            this.pspm = [];

            // copy pspm and check
            for (var row: number = 0; row < this.motif_length; row++) {
                if (this.alph_length != matrix[row].length) {
                    throw new Error("COLUMN_MISMATCH");
                }
                this.pspm[row] = [];
                row_sum = 0;
                for (var col: number = 0; col < this.alph_length; col++) {
                    this.pspm[row][col] = matrix[row][col];
                    row_sum += this.pspm[row][col];
                }
                delta = 0.1;
                if (isNaN(row_sum) || (row_sum > 1 && (row_sum - 1) > delta) ||
                    (row_sum < 1 && (1 - row_sum) > delta)) {
                    throw new Error("INVALID_SUM");
                }
            }
        }

        private matrixParseFromString(data: IPspm) {
            this.alph_length = data.alph_length;
            this.motif_length = data.motif_length;
            this.pspm = data.pspm;

            if (typeof this.evalue === "undefined") {
                if (typeof data.evalue !== "undefined") {
                    this.evalue = data.evalue;
                } else {
                    this.evalue = 0;
                }
            }
            if (typeof this.nsites === "undefined") {
                if (typeof data.nsites === "number") {
                    this.nsites = data.nsites;
                } else {
                    this.nsites = 20;
                }
            }
        }

        /**         
         * copy constructor
        */
        private copyInternal(matrix: Pspm): void {
            this.alph_length = matrix.alph_length;
            this.motif_length = matrix.motif_length;
            this.name = matrix.name;
            this.nsites = matrix.nsites;
            this.evalue = matrix.evalue;
            this.ltrim = matrix.ltrim;
            this.rtrim = matrix.rtrim;
            this.pspm = [];

            for (var row: number = 0; row < matrix.motif_length; row++) {
                this.pspm[row] = [];
                for (var col: number = 0; col < matrix.alph_length; col++) {
                    this.pspm[row][col] = matrix.pspm[row][col];
                }
            }
        }

        public copy(): Pspm {
            return new Pspm(this);
        }

        public reverse_complement(alphabet: Alphabet): Pspm {
            "use strict";

            var x, y, temp, a_index, c_index, g_index, t_index, i, row, temp_trim;

            if (this.alph_length != alphabet.size) {
                throw new Error("ALPHABET_MISMATCH");
            }
            if (!alphabet.isNucleotide) {
                throw new Error("NO_PROTEIN_RC");
            }

            //reverse
            x = 0;
            y = this.motif_length - 1;
            while (x < y) {
                temp = this.pspm[x];
                this.pspm[x] = this.pspm[y];
                this.pspm[y] = temp;
                x++;
                y--;
            }

            //complement
            a_index = alphabet.getIndex("A");
            c_index = alphabet.getIndex("C");
            g_index = alphabet.getIndex("G");
            t_index = alphabet.getIndex("T");

            for (i = 0; i < this.motif_length; i++) {
                row = this.pspm[i];
                //swap A and T
                temp = row[a_index];
                row[a_index] = row[t_index];
                row[t_index] = temp;
                //swap C and G
                temp = row[c_index];
                row[c_index] = row[g_index];
                row[g_index] = temp;
            }

            //swap triming
            temp_trim = this.ltrim;
            this.ltrim = this.rtrim;
            this.rtrim = temp_trim;

            // note that ambigs are ignored because they don't effect motifs
            // allow function chaining...
            return this;
        }

        public get_stack(position, alphabet) {
            "use strict";
            var row, stack_ic, alphabet_ic, stack, i
            var sym: Symbol;
            if (this.alph_length != alphabet.get_size()) {
                throw new Error("ALPHABET_MISMATCH");
            }
            row = this.pspm[position];
            stack_ic = this.get_stack_ic(position, alphabet);
            alphabet_ic = alphabet.get_ic();
            stack = [];
            for (i = 0; i < this.alph_length; i++) {
                if (alphabet.is_ambig(i)) {
                    continue;
                }
                sym = new Symbol(i, row[i] * stack_ic / alphabet_ic, alphabet);
                if (sym.scale <= 0) {
                    continue;
                }
                stack.push(sym);
            }
            stack.sort(Symbol.compareSymbol);
            return stack;
        };

        public get_stack_ic(position: number, alphabet: Alphabet) {
            "use strict";

            if (this.alph_length != alphabet.size) {
                throw new Error("ALPHABET_MISMATCH");
            }

            var row: number[] = this.pspm[position];
            var H: number = 0;

            for (var i: number = 0; i < this.alph_length; i++) {
                if (alphabet.isAmbig(i)) {
                    continue;
                }
                if (row[i] === 0) {
                    continue;
                }
                H -= (row[i] * (Math.log(row[i]) / Math.LN2));
            }

            return alphabet.ic - H;
        }

        public getError(alphabet: Alphabet): number {
            "use strict";

            var asize: number;

            if (this.nsites === 0) {
                return 0;
            }

            if (alphabet.isNucleotide) {
                asize = 4;
            } else {
                asize = 20;
            }

            return (asize - 1) / (2 * Math.log(2) * this.nsites);
        }

        public get leftTrim(): number {
            "use strict";
            return this.ltrim;
        }

        public get rightTrim(): number {
            "use strict";
            return this.rtrim;
        }

        /**
         * 将当前的数据模型转换为Motif数据的字符串用于进行保存
        */
        public as_pspm(): string {
            "use strict";

            var out = "letter-probability matrix: alength= " + this.alph_length +
                " w= " + this.motif_length + " nsites= " + this.nsites +
                " E= " + (typeof this.evalue === "number" ?
                    this.evalue.toExponential() : this.evalue) + "\n";
            for (var row: number = 0; row < this.motif_length; row++) {
                for (var col: number = 0; col < this.alph_length; col++) {
                    if (col !== 0) {
                        out += " ";
                    }
                    out += this.pspm[row][col].toFixed(6);
                }
                out += "\n";
            }
            return out;
        }

        public as_pssm(alphabet: Alphabet, pseudo: number = null): string {
            "use strict";

            var p: number, bg: number, p2: number, score: number;

            if (typeof pseudo === "undefined") {
                pseudo = 0.1;
            } else if (typeof pseudo !== "number") {
                throw new Error("Expected number for pseudocount");
            }

            var out = "log-odds matrix: alength= " + this.alph_length +
                " w= " + this.motif_length +
                " E= " + (typeof this.evalue == "number" ?
                    this.evalue.toExponential() : this.evalue) + "\n";
            var log2 = Math.log(2);
            var total = this.nsites + pseudo;

            for (var row: number = 0; row < this.motif_length; row++) {
                for (var col: number = 0; col < this.alph_length; col++) {
                    if (col !== 0) {
                        out += " ";
                    }
                    p = this.pspm[row][col];
                    // to avoid log of zero we add a pseudo count
                    bg = alphabet.getBgfreq(col);
                    p2 = (p * this.nsites + bg * pseudo) / total;
                    // now calculate the score
                    score = -10000;
                    if (p2 > 0) {
                        score = Math.round((Math.log(p2 / bg) / log2) * 100);
                    }
                    out += score;
                }
                out += "\n";
            }

            return out;
        }

        public toString(): string {
            "use strict";

            var str: string = "";
            var row: number[];

            for (var i: number = 0; i < this.pspm.length; i++) {
                row = this.pspm[i];
                str += row.join("\t") + "\n";
            }
            return str;
        }
    }
}