namespace GCModeller.Workbench {

    export class Pspm {

        public name: string;
        public alph_length;
        public motif_length;
        public nsites
        public evalue
        public ltrim
        public rtrim
        public pspm = [];

        public constructor(matrix, name: string = null, ltrim = null, rtrim = null, nsites = null, evalue = null) {
            // construct

            if (matrix instanceof Pspm) {
                this.copyInternal(matrix);
            } else {
                this.parseInternal(matrix, name, ltrim, rtrim, nsites, evalue);
            }
        }

        private parseInternal(matrix, name, ltrim, rtrim, nsites, evalue): void {
            var row, col, data, row_sum, delta, evalue_re;

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
                    }
                } else if (typeof evalue === "string") {
                    evalue_re = /^((?:[+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?)|inf)$/;
                    if (!evalue_re.test(evalue)) {
                        throw new Error("evalue must be a non-negative number, got: " + evalue);
                    }
                } else {
                    throw new Error("evalue must be a non-negative number, got: " + evalue);
                }
            }
            // set properties
            this.name = name;
            this.nsites = nsites;
            this.evalue = evalue;
            this.ltrim = ltrim;
            this.rtrim = rtrim;
            if (typeof matrix === "string") {
                // string constructor
                data = parse_pspm_string(matrix);
                this.alph_length = data["alph_length"];
                this.motif_length = data["motif_length"];
                this.pspm = data["pspm"];
                if (typeof this.evalue === "undefined") {
                    if (typeof data["evalue"] !== "undefined") {
                        this.evalue = data["evalue"];
                    } else {
                        this.evalue = 0;
                    }
                }
                if (typeof this.nsites === "undefined") {
                    if (typeof data["nsites"] === "number") {
                        this.nsites = data["nsites"];
                    } else {
                        this.nsites = 20;
                    }
                }
            } else {
                // assume pspm is a nested array
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
                for (row = 0; row < this.motif_length; row++) {
                    if (this.alph_length != matrix[row].length) {
                        throw new Error("COLUMN_MISMATCH");
                    }
                    this.pspm[row] = [];
                    row_sum = 0;
                    for (col = 0; col < this.alph_length; col++) {
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
        }

        private copyInternal(matrix: Pspm): void {

            // copy constructor
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

        public reverse_complement(alphabet) {
            "use strict";
            var x, y, temp, a_index, c_index, g_index, t_index, i, row, temp_trim;
            if (this.alph_length != alphabet.get_size()) {
                throw new Error("ALPHABET_MISMATCH");
            }
            if (!alphabet.is_nucleotide()) {
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
            a_index = alphabet.get_index("A");
            c_index = alphabet.get_index("C");
            g_index = alphabet.get_index("G");
            t_index = alphabet.get_index("T");
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
            //note that ambigs are ignored because they don't effect motifs
            return this; //allow function chaining...
        };

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

        public get_stack_ic(position, alphabet) {
            "use strict";
            var row, H, i;
            if (this.alph_length != alphabet.get_size()) {
                throw new Error("ALPHABET_MISMATCH");
            }
            row = this.pspm[position];
            H = 0;
            for (i = 0; i < this.alph_length; i++) {
                if (alphabet.is_ambig(i)) {
                    continue;
                }
                if (row[i] === 0) {
                    continue;
                }
                H -= (row[i] * (Math.log(row[i]) / Math.LN2));
            }
            return alphabet.get_ic() - H;
        };

        public get_error(alphabet) {
            "use strict";
            var asize;
            if (this.nsites === 0) {
                return 0;
            }
            if (alphabet.is_nucleotide()) {
                asize = 4;
            } else {
                asize = 20;
            }
            return (asize - 1) / (2 * Math.log(2) * this.nsites);
        };

        public get_motif_length() {
            "use strict";
            return this.motif_length;
        };

        public get_alph_length() {
            "use strict";
            return this.alph_length;
        };

        public get_left_trim() {
            "use strict";
            return this.ltrim;
        };

        public get_right_trim() {
            "use strict";
            return this.rtrim;
        };

        public as_pspm() {
            "use strict";
            var out, row, col;
            out = "letter-probability matrix: alength= " + this.alph_length +
                " w= " + this.motif_length + " nsites= " + this.nsites +
                " E= " + (typeof this.evalue === "number" ?
                    this.evalue.toExponential() : this.evalue) + "\n";
            for (row = 0; row < this.motif_length; row++) {
                for (col = 0; col < this.alph_length; col++) {
                    if (col !== 0) {
                        out += " ";
                    }
                    out += this.pspm[row][col].toFixed(6);
                }
                out += "\n";
            }
            return out;
        };

        public as_pssm(alphabet, pseudo) {
            "use strict";
            var out, log2, total, row, col, p, bg, p2, score;
            if (typeof pseudo === "undefined") {
                pseudo = 0.1;
            } else if (typeof pseudo !== "number") {
                throw new Error("Expected number for pseudocount");
            }
            out = "log-odds matrix: alength= " + this.alph_length +
                " w= " + this.motif_length +
                " E= " + (typeof this.evalue == "number" ?
                    this.evalue.toExponential() : this.evalue) + "\n";
            log2 = Math.log(2);
            total = this.nsites + pseudo;
            for (row = 0; row < this.motif_length; row++) {
                for (col = 0; col < this.alph_length; col++) {
                    if (col !== 0) {
                        out += " ";
                    }
                    p = this.pspm[row][col];
                    // to avoid log of zero we add a pseudo count
                    bg = alphabet.get_bg_freq(col);
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
        };

        public toString(): string {
            "use strict";
            var str, i, row;
            str = "";
            for (i = 0; i < this.pspm.length; i++) {
                row = this.pspm[i];
                str += row.join("\t") + "\n";
            }
            return str;
        }               
    }
}