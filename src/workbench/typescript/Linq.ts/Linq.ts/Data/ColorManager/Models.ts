namespace TypeScript.ColorManager {

    export interface Irgb {
        r: number;
        g: number;
        b: number;
    }

    export interface Icmyk {
        c: number;
        m: number;
        y: number;
        k: number;
    }

    export interface Ihsl {
        h: number;
        s: number;
        l: number;
    }

    export class IW3color {
        public red: number;
        public blue: number;
        public green: number;
        public hue: number;
        public sat: number;
        public opacity: number;
        public whiteness: number;
        public lightness: number;
        public blackness: number;
        public cyan: number;
        public magenta: number;
        public yellow: number;
        public black: number;
        public ncol: string;
        public valid: boolean;
    }
}