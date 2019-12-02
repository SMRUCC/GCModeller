namespace TypeScript.ColorManager {

    /**
     * w3color.js ver.1.18 by w3schools.com (Do not remove this line)
    */
    export class w3color implements IW3color {

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

        public static get emptyObject(): IW3color {
            return <IW3color>{
                red: 0,
                green: 0,
                blue: 0,
                hue: 0,
                sat: 0,
                lightness: 0,
                whiteness: 0,
                blackness: 0,
                cyan: 0,
                magenta: 0,
                yellow: 0,
                black: 0,
                ncol: "R",
                opacity: 1,
                valid: false
            };
        }

        public constructor(color: string | IW3color = null, elmnt: HTMLElement = null) {
            if (!isNullOrUndefined(color)) {
                if (typeof color == "string") {
                    this.attachValues(toColorObject(color));
                } else {
                    this.attachValues(color);
                }
            } else {
                this.attachValues(w3color.emptyObject);
            }

            if (!isNullOrUndefined(elmnt)) {
                elmnt.style.backgroundColor = this.toRgbString();
            }
        }

        toRgbString() {
            return "rgb(" + this.red + ", " + this.green + ", " + this.blue + ")";
        }
        toRgbaString() {
            return "rgba(" + this.red + ", " + this.green + ", " + this.blue + ", " + this.opacity + ")";
        }
        toHwbString() {
            return "hwb(" + this.hue + ", " + Math.round(this.whiteness * 100) + "%, " + Math.round(this.blackness * 100) + "%)";
        }
        toHwbStringDecimal() {
            return "hwb(" + this.hue + ", " + this.whiteness + ", " + this.blackness + ")";
        }
        toHwbaString() {
            return "hwba(" + this.hue + ", " + Math.round(this.whiteness * 100) + "%, " + Math.round(this.blackness * 100) + "%, " + this.opacity + ")";
        }
        toHslString() {
            return "hsl(" + this.hue + ", " + Math.round(this.sat * 100) + "%, " + Math.round(this.lightness * 100) + "%)";
        }
        toHslStringDecimal() {
            return "hsl(" + this.hue + ", " + this.sat + ", " + this.lightness + ")";
        }
        toHslaString() {
            return "hsla(" + this.hue + ", " + Math.round(this.sat * 100) + "%, " + Math.round(this.lightness * 100) + "%, " + this.opacity + ")";
        }
        toCmykString() {
            return "cmyk(" + Math.round(this.cyan * 100) + "%, " + Math.round(this.magenta * 100) + "%, " + Math.round(this.yellow * 100) + "%, " + Math.round(this.black * 100) + "%)";
        }
        toCmykStringDecimal() {
            return "cmyk(" + this.cyan + ", " + this.magenta + ", " + this.yellow + ", " + this.black + ")";
        }
        toNcolString() {
            return this.ncol + ", " + Math.round(this.whiteness * 100) + "%, " + Math.round(this.blackness * 100) + "%";
        }
        toNcolStringDecimal() {
            return this.ncol + ", " + this.whiteness + ", " + this.blackness;
        }
        toNcolaString() {
            return this.ncol + ", " + Math.round(this.whiteness * 100) + "%, " + Math.round(this.blackness * 100) + "%, " + this.opacity;
        }
        toName() {
            var r, g, b, colorhexs = getColorArr('hexs');
            for (let i = 0; i < colorhexs.length; i++) {
                r = parseInt(colorhexs[i].substr(0, 2), 16);
                g = parseInt(colorhexs[i].substr(2, 2), 16);
                b = parseInt(colorhexs[i].substr(4, 2), 16);
                if (this.red == r && this.green == g && this.blue == b) {
                    return getColorArr('names')[i];
                }
            }
            return "";
        }
        toHexString() {
            var r = toHex(this.red);
            var g = toHex(this.green);
            var b = toHex(this.blue);
            return "#" + r + g + b;
        }
        toRgb() {
            return { r: this.red, g: this.green, b: this.blue, a: this.opacity };
        }
        toHsl() {
            return { h: this.hue, s: this.sat, l: this.lightness, a: this.opacity };
        }
        toHwb() {
            return { h: this.hue, w: this.whiteness, b: this.blackness, a: this.opacity };
        }
        toCmyk() {
            return { c: this.cyan, m: this.magenta, y: this.yellow, k: this.black, a: this.opacity };
        }
        toNcol() {
            return { ncol: this.ncol, w: this.whiteness, b: this.blackness, a: this.opacity };
        }
        isDark(n) {
            var m = (n || 128);
            return (((this.red * 299 + this.green * 587 + this.blue * 114) / 1000) < m);
        }
        saturate(n) {
            var x, rgb, color;
            x = (n / 100 || 0.1);
            this.sat += x;
            if (this.sat > 1) { this.sat = 1; }
            rgb = hslToRgb(this.hue, this.sat, this.lightness);
            color = colorObject(rgb, this.opacity, this.hue, this.sat);
            this.attachValues(color);
        }
        desaturate(n) {
            var x, rgb, color;
            x = (n / 100 || 0.1);
            this.sat -= x;
            if (this.sat < 0) { this.sat = 0; }
            rgb = hslToRgb(this.hue, this.sat, this.lightness);
            color = colorObject(rgb, this.opacity, this.hue, this.sat);
            this.attachValues(color);
        }
        lighter(n) {
            var x, rgb, color;
            x = (n / 100 || 0.1);
            this.lightness += x;
            if (this.lightness > 1) { this.lightness = 1; }
            rgb = hslToRgb(this.hue, this.sat, this.lightness);
            color = colorObject(rgb, this.opacity, this.hue, this.sat);
            this.attachValues(color);
        }
        darker(n) {
            var x, rgb, color;
            x = (n / 100 || 0.1);
            this.lightness -= x;
            if (this.lightness < 0) { this.lightness = 0; }
            rgb = hslToRgb(this.hue, this.sat, this.lightness);
            color = colorObject(rgb, this.opacity, this.hue, this.sat);
            this.attachValues(color);
        }

        private attachValues(color: IW3color) {
            this.red = color.red;
            this.green = color.green;
            this.blue = color.blue;
            this.hue = color.hue;
            this.sat = color.sat;
            this.lightness = color.lightness;
            this.whiteness = color.whiteness;
            this.blackness = color.blackness;
            this.cyan = color.cyan;
            this.magenta = color.magenta;
            this.yellow = color.yellow;
            this.black = color.black;
            this.ncol = color.ncol;
            this.opacity = color.opacity;
            this.valid = color.valid;
        }
    };
}