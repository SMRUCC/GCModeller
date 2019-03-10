namespace CanvasHelper {

    var innerCanvas: HTMLCanvasElement;

    /**
     * Uses canvas.measureText to compute and return the width of the given text of given font in pixels.
     * 
     * @param {String} text The text to be rendered.
     * @param {String} font The css font descriptor that text is to be rendered with (e.g. "bold 14px verdana").
     * 
     * @see https://stackoverflow.com/questions/118241/calculate-text-width-with-javascript/21015393#21015393
     * 
     */
    export function getTextWidth(text: string, font: string): number {
        // re-use canvas object for better performance
        var canvas: HTMLCanvasElement = innerCanvas || (innerCanvas = $ts("<canvas>"));
        var context = canvas.getContext("2d");
        var metrics: TextMetrics;

        context.font = font;
        metrics = context.measureText(text);

        return metrics.width;
    }

    /**
     * found this trick at http://talideon.com/weblog/2005/02/detecting-broken-images-js.cfm
    */
    export function imageOk(img: HTMLImageElement): boolean {
        "use strict";

        // During the onload event, IE correctly identifies any images that
        // weren't downloaded as not complete. Others should too. Gecko-based
        // browsers act like NS4 in that they report this incorrectly.
        if (!img.complete) {
            return false;
        }

        // However, they do have two very useful properties: naturalWidth and
        // naturalHeight. These give the true size of the image. If it failed
        // to load, either of these should be zero.
        if (typeof img.naturalWidth !== "undefined" && img.naturalWidth === 0) {
            return false;
        }

        // No other way of checking: assume it's ok.
        return true;
    }

    /**
     * @param size [width, height]
    */
    export function createCanvas(size: [number, number], id: string, title: string, display: string = "block") {
        "use strict";

        var canvas = document.createElement("canvas");

        //check for canvas support before attempting anything
        if (!canvas.getContext) {
            return null;
        }

        var ctx: CanvasRenderingContext2D = canvas.getContext('2d');

        //check for html5 text drawing support
        if (!supportsText(ctx)) {
            return null;
        }

        //size the canvas
        canvas.width = size[0];
        canvas.height = size[1];
        canvas.id = id;
        canvas.title = title;
        canvas.style.display = display;

        return canvas;
    }

    export function supportsText(ctx: CanvasRenderingContext2D): boolean {
        if (!ctx.fillText) {
            return false;
        }
        if (!ctx.measureText) {
            return false;
        }

        return true;
    }

    export class fontSize {

        public point: number;
        public pixel: number;
        public em: number;
        public percent: number;

        public readonly sizes: fontSize[] = [

        ];
    }
}