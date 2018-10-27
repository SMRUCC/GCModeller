namespace GCModeller.Workbench {

    export class Alphabet {

        public static readonly is_letter: RegExp = /^\w$/;
        public static readonly is_prob: RegExp = /^((1(\.0+)?)|(0(\.\d+)?))$/;

        public freqs: number[] = [];
        public alphabet: string[] = [];

        private letter_count: number = 0;

        public get ic(): number {
            if (this.isNucleotide) {
                return 2;
            } else {
                return Math.log(20) / Math.LN2;
            }
        }

        public get size(): number {
            return this.letter_count;
        }

        public get isNucleotide(): boolean {
            //TODO basic method, make better
            if (this.letter_count < 20) {
                return true;
            }
            return false;
        }

        public constructor(alphabet: string, bg: string = null) {
            var letter: string;

            for (var pos: number = 0; pos < alphabet.length; pos++) {
                letter = alphabet.charAt(pos);

                if (Alphabet.is_letter.test(letter)) {
                    this.alphabet[this.letter_count] = letter.toUpperCase();
                    this.freqs[this.letter_count] = -1;
                    this.letter_count++;
                }
            }

            if (typeof bg !== "undefined") {
                this.parseBackground(bg.split(/\s+/));
            } else {
                var freq: number;

                // assume uniform background
                freq = 1.0 / this.letter_count;

                for (pos = 0; pos < this.letter_count; pos++) {
                    this.freqs[pos] = freq;
                }
            }
        }

        private parseBackground(parts: string[]): void {
            var parts: string[];
            var letter: string;
            var freq: string;

            for (var i: number = 0, pos = 0; (i + 1) < parts.length; i += 2) {
                letter = parts[i];
                freq = parts[i + 1];

                if (Alphabet.is_letter.test(letter) && Alphabet.is_prob.test(freq)) {
                    // find the letter it matches
                    letter = letter.toUpperCase();

                    for (; pos < this.letter_count; pos++) {
                        if (this.alphabet[pos] == letter) {
                            break;
                        }
                    }
                    if (pos >= this.letter_count) {
                        throw new Error("NOT_IN_ALPHABET");
                    }
                    this.freqs[pos] = parseFloat(freq);
                }
            }
        }

        public toString(): string {
            return (this.isNucleotide ? "Nucleotide" : "Protein") + " Alphabet " + (this.alphabet.join(""));
        }

        public getLetter(index: number): string {
            if (index < 0 || index >= this.letter_count) {
                throw new Error("BAD_ALPHABET_INDEX");
            }

            return this.alphabet[index];
        }

        public getBgfreq(index: number): number {
            if (index < 0 || index >= this.letter_count) {
                throw new Error("BAD_ALPHABET_INDEX");
            }
            if (this.freqs[index] == -1) {
                throw new Error("BG_FREQ_NOT_SET");
            }

            return this.freqs[index];
        }

        public getColour(index: number): string {
            if (index < 0 || index >= this.letter_count) {
                throw new Error("BAD_ALPHABET_INDEX");
            }
            if (this.isNucleotide) {
                return AlphabetColors.nucleotideColor(this.alphabet[index]);
            } else {
                return AlphabetColors.proteinColor(this.alphabet[index]);
            }

            return "black";
        }

        public isAmbig(index: number): boolean {
            if (index < 0 || index >= this.letter_count) {
                throw new Error("BAD_ALPHABET_INDEX");
            }
            if (this.isNucleotide) {
                return ("ACGT".indexOf(this.alphabet[index]) == -1);
            } else {
                return ("ACDEFGHIKLMNPQRSTVWY".indexOf(this.alphabet[index]) == -1);
            }
        }

        public getIndex(letter: string): number {
            for (var i: number = 0; i < this.letter_count; i++) {
                if (this.alphabet[i] == letter.toUpperCase()) {
                    return i;
                }
            }

            throw new Error("UNKNOWN_LETTER");
        }
    }
}