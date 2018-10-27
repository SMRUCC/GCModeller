namespace GCModeller.Workbench.AlphabetColors {

    export const red: string = "rgb(204,0,0)";
    export const blue: string = "rgb(0,0,204)";
    export const orange: string = "rgb(255,179,0)";
    export const green: string = "rgb(0,128,0)";
    export const yellow: string = "rgb(255,255,0)";
    export const purple: string = "rgb(204,0,204)";
    export const magenta: string = "rgb(255,0,255)";
    export const pink: string = "rgb(255,204,204)";
    export const turquoise: string = "rgb(51,230,204)";

    export function nucleotideColor(alphabet: string): string {
        switch (alphabet) {
            case "A":
                return AlphabetColors.red;
            case "C":
                return AlphabetColors.blue;
            case "G":
                return AlphabetColors.orange;
            case "T":
                return AlphabetColors.green;
            default:
                throw new Error("Invalid nucleotide letter");
        }
    }

    export function proteinColor(alphabet: string): string {
        switch (alphabet) {
            case "A":
            case "C":
            case "F":
            case "I":
            case "L":
            case "V":
            case "W":
            case "M":
                return AlphabetColors.blue;
            case "N":
            case "Q":
            case "S":
            case "T":
                return AlphabetColors.green;
            case "D":
            case "E":
                return AlphabetColors.magenta;
            case "K":
            case "R":
                return AlphabetColors.red;
            case "H":
                return AlphabetColors.pink;
            case "G":
                return AlphabetColors.orange;
            case "P":
                return AlphabetColors.yellow;
            case "Y":
                return AlphabetColors.turquoise;
            default:
                throw new Error("Invalid protein letter");
        }
    }
}