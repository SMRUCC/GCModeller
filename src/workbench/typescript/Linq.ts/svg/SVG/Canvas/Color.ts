namespace Canvas {

    /**
     * RGB color data model
    */
    export class Color {

        public r: number;
        public g: number;
        public b: number;

        constructor(r: number, g: number, b: number) {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        /**
         * https://stackoverflow.com/questions/5623838/rgb-to-hex-and-hex-to-rgb
        */
        public static FromHtmlColor(htmlColor: string): Color {
            // Expand shorthand form (e.g. "03F") to full form (e.g. "0033FF")
            var hex: string = htmlColor;
            var shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i;

            hex = hex.replace(shorthandRegex, function (m, r, g, b) {
                return r + r + g + g + b + b;
            });

            var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);

            return result ? new Color(
                parseInt(result[1], 16),
                parseInt(result[2], 16),
                parseInt(result[3], 16)
            ) : null;
        }

        public ToHtmlColor(): string {
            var r = SvgUtils.componentToHex(this.r);
            var g = SvgUtils.componentToHex(this.g);
            var b = SvgUtils.componentToHex(this.b);

            return `#${r}${g}${b}`;
        }

        public ToRGBColor(): string {
            return `rgb(${this.r},${this.g},${this.b})`;
        }
    }
}