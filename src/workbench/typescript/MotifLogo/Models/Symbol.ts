namespace GCModeller.Workbench {

    export class Symbol {

        public symbol: string;
        public scale: number;
        public colour: string;

        public constructor(index: number, scale: number, alphabet: Alphabet) {
            this.symbol = alphabet.getLetter(index);
            this.scale = scale;
            this.colour = alphabet.getColour(index);
        }

        public toString(): string {
            return this.symbol + " " + (Math.round(this.scale * 1000) / 10) + "%";
        }

        public static compareSymbol(sym1: Symbol, sym2: Symbol): number {
            if (sym1.scale < sym2.scale) {
                return -1;
            } else if (sym1.scale > sym2.scale) {
                return 1;
            } else {
                return 0;
            }
        }
    }
}