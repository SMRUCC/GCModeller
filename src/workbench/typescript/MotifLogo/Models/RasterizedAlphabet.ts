namespace GCModeller.Workbench {

    export class RasterizedAlphabet {

        public lookup: number[];
        public rasters: HTMLCanvasElement[];
        public dimensions: RasterBounds[];

        public constructor(alphabet: Alphabet, font: string, target_width: number) {
            // variable prototypes
            this.lookup = []; //a map of letter to index
            this.rasters = []; //a list of rasters
            this.dimensions = []; //a list of dimensions

            // construct
            var default_size: number = 60; // size of square to assume as the default width
            var safety_pad: number = 20; // pixels to pad around so we don't miss the edges
            // create a canvas to do our rasterizing on
            var canvas: HTMLCanvasElement = <any>$ts("<canvas>", {
                // assume the default font would fit in a canvas of 100 by 100
                width: default_size + 2 * safety_pad,
                height: default_size + 2 * safety_pad
            });

            // check for canvas support before attempting anything
            if (!canvas.getContext) {
                throw new Error("NO_CANVAS_SUPPORT");
            }

            var ctx: CanvasRenderingContext2D = canvas.getContext('2d');

            // check for html5 text drawing support
            if (!CanvasHelper.supportsText(ctx)) {
                throw new Error("NO_CANVAS_TEXT_SUPPORT");
            }

            // calculate the middle
            var middle = Math.round(canvas.width / 2);
            // calculate the baseline
            var baseline = Math.round(canvas.height - safety_pad);
            // list of widths
            var widths: number[] = [];
            var count = 0;
            var letters: string[] = [];

            var letter: string;
            var size, tenpercent, avg_width, scale,
                target_height: number;
            var raster: HTMLCanvasElement;

            //now measure each letter in the alphabet
            for (var i: number = 0; i < alphabet.size; ++i) {
                if (alphabet.isAmbig(i)) {
                    continue; //skip ambigs as they're never rendered
                }
                letter = alphabet.getLetter(i);
                letters.push(letter);
                this.lookup[letter] = count;
                //clear the canvas
                canvas.width = canvas.width;
                // get the context and prepare to draw our width test
                ctx = canvas.getContext('2d');
                ctx.font = font;
                ctx.fillStyle = alphabet.getColour(i);
                ctx.textAlign = "center";
                ctx.translate(middle, baseline);
                // draw the test text
                ctx.fillText(letter, 0, 0);
                //measure
                size = RasterizedAlphabet.canvas_bounds(ctx, canvas.width, canvas.height);
                if (size.width === 0) {
                    throw new Error("INVISIBLE_LETTER"); //maybe the fill was white on white?
                }
                widths.push(size.width);
                this.dimensions[count] = size;
                count++;
            }
            //sort the widths
            widths.sort(function (a, b) { return a - b; });
            //drop 10% of the items off each end
            tenpercent = Math.floor(widths.length / 10);
            for (var i: number = 0; i < tenpercent; ++i) {
                widths.pop();
                widths.shift();
            }
            //calculate average width
            avg_width = 0;
            for (var i: number = 0; i < widths.length; ++i) {
                avg_width += widths[i];
            }
            avg_width /= widths.length;
            // calculate scales
            for (var i: number = 0; i < this.dimensions.length; ++i) {
                size = this.dimensions[i];
                // calculate scale
                scale = target_width / Math.max(avg_width, size.width);
                // estimate scaled height
                target_height = size.height * scale;

                // create an approprately sized canvas
                raster = <any>$ts("<canvas>", {
                    width: target_width, // if it goes over the edge too bad...
                    height: target_height + safety_pad * 2
                });

                // calculate the middle
                middle = Math.round(raster.width / 2);
                // calculate the baseline
                baseline = Math.round(raster.height - safety_pad);
                // get the context and prepare to draw the rasterized text
                ctx = raster.getContext('2d');
                ctx.font = font;
                ctx.fillStyle = alphabet.getColour(i);
                ctx.textAlign = "center";
                ctx.translate(middle, baseline);
                ctx.save();
                ctx.scale(scale, scale);
                // draw the rasterized text
                ctx.fillText(letters[i], 0, 0);
                ctx.restore();

                this.rasters[i] = raster;
                this.dimensions[i] = RasterizedAlphabet.canvas_bounds(ctx, raster.width, raster.height);
            }
        }

        public draw(ctx: CanvasRenderingContext2D, letter: string, dx: number, dy: number, dWidth: number, dHeight: number): void {
            var index = this.lookup[letter];
            var raster = this.rasters[index];
            var size = this.dimensions[index];

            ctx.drawImage(raster, 0, size.bound_top - 1, raster.width, size.height + 1, dx, dy, dWidth, dHeight);
        }

        public static canvas_bounds(ctx: CanvasRenderingContext2D, cwidth: number, cheight: number): RasterBounds {
            var data, r, c;
            var top_line: number, bottom_line: number, left_line: number, right_line: number;
            var txt_width: number, txt_height: number;

            data = ctx.getImageData(0, 0, cwidth, cheight).data;
            // r: row, c: column
            r = 0; c = 0;
            top_line = -1; bottom_line = -1; left_line = -1; right_line = -1;
            txt_width = 0; txt_height = 0;

            // Find the top-most line with a non-white pixel
            for (r = 0; r < cheight; r++) {
                for (c = 0; c < cwidth; c++) {
                    if (data[r * cwidth * 4 + c * 4 + 3]) {
                        top_line = r;
                        break;
                    }
                }
                if (top_line != -1) {
                    break;
                }
            }

            //find the last line with a non-white pixel
            if (top_line != -1) {
                for (r = cheight - 1; r >= top_line; r--) {
                    for (c = 0; c < cwidth; c++) {
                        if (data[r * cwidth * 4 + c * 4 + 3]) {
                            bottom_line = r;
                            break;
                        }
                    }
                    if (bottom_line != -1) {
                        break;
                    }
                }
                txt_height = bottom_line - top_line + 1;
            }

            // Find the left-most line with a non-white pixel
            for (c = 0; c < cwidth; c++) {
                for (r = 0; r < cheight; r++) {
                    if (data[r * cwidth * 4 + c * 4 + 3]) {
                        left_line = c;
                        break;
                    }
                }
                if (left_line != -1) {
                    break;
                }
            }

            //find the right most line with a non-white pixel
            if (left_line != -1) {
                for (c = cwidth - 1; c >= left_line; c--) {
                    for (r = 0; r < cheight; r++) {
                        if (data[r * cwidth * 4 + c * 4 + 3]) {
                            right_line = c;
                            break;
                        }
                    }
                    if (right_line != -1) {
                        break;
                    }
                }
                txt_width = right_line - left_line + 1;
            }

            // return the bounds
            return <RasterBounds>{
                bound_top: top_line, bound_bottom: bottom_line,
                bound_left: left_line, bound_right: right_line, width: txt_width,
                height: txt_height
            };
        }
    }

    export class RasterBounds {
        public bound_top: number;
        public bound_bottom: number;
        public bound_left: number;
        public bound_right: number;
        public width: number;
        public height: number;
    }
}