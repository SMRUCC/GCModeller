namespace GCModeller.Workbench {

    export class MotifLogo {

        public task_queue: LoadQueryTask[] = [];
        public task_delay: number = 100;
        public draw_logo_on_canvas: CanvasRender;

        public constructor() {
            this.draw_logo_on_canvas = new CanvasRender(this);
        }

        public drawLogo(div_id: string, pwm, scale: number) {
            this.push_task(new LoadQueryTask(div_id, pwm, scale, this));
        }

        /**
         * draws the scale, returns the width
        */
        public draw_scale(ctx: CanvasRenderingContext2D, metrics: LogoMetrics, alphabet_ic: number) {
            "use strict";

            var tic_height = metrics.stack_height / alphabet_ic;
            ctx.save();
            ctx.lineWidth = 1.5;
            ctx.translate(metrics.y_label_height, metrics.y_num_height / 2);

            //draw the axis label
            ctx.save();
            ctx.font = metrics.y_label_font;
            ctx.translate(0, metrics.stack_height / 2);
            ctx.save();
            ctx.rotate(-(Math.PI / 2));
            ctx.textAlign = "center";
            ctx.fillText("bits", 0, 0);
            ctx.restore();
            ctx.restore();

            ctx.translate(metrics.y_label_spacer + metrics.y_num_width, 0);

            //draw the axis tics
            ctx.save();
            ctx.translate(0, metrics.stack_height);
            ctx.font = metrics.y_num_font;
            ctx.textAlign = "right";
            ctx.textBaseline = "middle";

            for (var i: number = 0; i <= alphabet_ic; i++) {
                //draw the number
                ctx.fillText("" + i, 0, 0);
                //draw the tic
                ctx.beginPath();
                ctx.moveTo(0, 0);
                ctx.lineTo(metrics.y_tic_width, 0);
                ctx.stroke();
                //prepare for next tic
                ctx.translate(0, -tic_height);
            }

            ctx.restore();
            ctx.translate(metrics.y_tic_width, 0);
            ctx.beginPath();
            ctx.moveTo(0, 0);
            ctx.lineTo(0, metrics.stack_height);
            ctx.stroke();
            ctx.restore();
        }

        public draw_stack_num(ctx: CanvasRenderingContext2D, metrics: LogoMetrics, row_index: number) {
            "use strict";

            ctx.save();
            ctx.font = metrics.x_num_font;
            ctx.translate(metrics.stack_width / 2, metrics.stack_height + metrics.x_num_above);
            ctx.save();
            ctx.rotate(-(Math.PI / 2));
            ctx.textBaseline = "middle";
            ctx.textAlign = "right";
            ctx.fillText("" + (row_index + 1), 0, 0);
            ctx.restore();
            ctx.restore();
        }

        public draw_stack(ctx: CanvasRenderingContext2D, metrics: LogoMetrics, symbols: Symbol[], raster: RasterizedAlphabet) {
            "use strict";

            var sym, sym_height, pad;
            var preferred_pad = 0;
            var sym_min = 5;

            ctx.save();//1
            ctx.translate(0, metrics.stack_height);

            for (var i: number = 0; i < symbols.length; i++) {
                sym = symbols[i];
                sym_height = metrics.stack_height * sym.get_scale();

                pad = preferred_pad;

                if (sym_height - pad < sym_min) {
                    pad = Math.min(pad, Math.max(0, sym_height - sym_min));
                }

                sym_height -= pad;

                //translate to the correct position
                ctx.translate(0, -(pad / 2 + sym_height));
                //draw
                raster.draw(ctx, sym.get_symbol(), 0, 0, metrics.stack_width, sym_height);
                //translate past the padding
                ctx.translate(0, -(pad / 2));
            }

            ctx.restore();//1
        }

        public draw_dashed_line(
            ctx: CanvasRenderingContext2D,
            pattern,
            start: number,
            x1: number, y1: number, x2: number, y2: number) {

            "use strict";

            var len, i, dx, dy, tlen, theta: number, mulx: number, muly: number, lx, ly;
            dx = x2 - x1;
            dy = y2 - y1;
            tlen = Math.pow(dx * dx + dy * dy, 0.5);
            theta = Math.atan2(dy, dx);
            mulx = Math.cos(theta);
            muly = Math.sin(theta);
            lx = [];
            ly = [];

            for (i = 0; i < pattern; ++i) {
                lx.push(pattern[i] * mulx);
                ly.push(pattern[i] * muly);
            }

            i = start;

            var x = x1;
            var y = y1;

            len = 0;
            ctx.beginPath();
            while (len + pattern[i] < tlen) {
                ctx.moveTo(x, y);
                x += lx[i];
                y += ly[i];
                ctx.lineTo(x, y);
                len += pattern[i];
                i = (i + 1) % pattern.length;
                x += lx[i];
                y += ly[i];
                len += pattern[i];
                i = (i + 1) % pattern.length;
            }
            if (len < tlen) {
                ctx.moveTo(x, y);
                x += mulx * (tlen - len);
                y += muly * (tlen - len);
                ctx.lineTo(x, y);
            }
            ctx.stroke();
        }

        public draw_trim_background(ctx: CanvasRenderingContext2D, metrics: LogoMetrics, pspm: Pspm, offset: number) {
            "use strict";
            var lwidth, rwidth, mwidth, rstart;
            lwidth = metrics.stack_width * pspm.get_left_trim();
            rwidth = metrics.stack_width * pspm.get_right_trim();
            mwidth = metrics.stack_width * pspm.get_motif_length();
            rstart = mwidth - rwidth;
            ctx.save();//s8
            ctx.translate(offset * metrics.stack_width, 0);
            ctx.fillStyle = "rgb(240, 240, 240)";
            if (pspm.get_left_trim() > 0) {
                ctx.fillRect(0, 0, lwidth, metrics.stack_height);
            }
            if (pspm.get_right_trim() > 0) {
                ctx.fillRect(rstart, 0, rwidth, metrics.stack_height);
            }
            ctx.fillStyle = "rgb(51, 51, 51)";
            if (pspm.get_left_trim() > 0) {
                this.draw_dashed_line(ctx, [3], 0, lwidth - 0.5, 0, lwidth - 0.5, metrics.stack_height);
            }
            if (pspm.get_right_trim() > 0) {
                this.draw_dashed_line(ctx, [3], 0, rstart + 0.5, 0, rstart + 0.5, metrics.stack_height);
            }
            ctx.restore();//s8
        }

        public size_logo_on_canvas(logo: Logo, canvas: HTMLCanvasElement, show_names: boolean, scale: number) {
            "use strict";

            var metrics: LogoMetrics;
            var draw_name = (typeof show_names === "boolean" ? show_names : (logo.rows > 1));

            if (canvas.width !== 0 && canvas.height !== 0) {
                return;
            } else {
                metrics = new LogoMetrics(canvas.getContext('2d'), logo.columns, logo.rows, draw_name);
            }

            if (typeof scale == "number") {
                //resize the canvas to fit the scaled logo
                canvas.width = metrics.summed_width * scale;
                canvas.height = metrics.summed_height * scale;
            } else {
                if (canvas.width === 0 && canvas.height === 0) {
                    canvas.width = metrics.summed_width;
                    canvas.height = metrics.summed_height;
                } else if (canvas.width === 0) {
                    canvas.width = metrics.summed_width * (canvas.height / metrics.summed_height);
                } else if (canvas.height === 0) {
                    canvas.height = metrics.summed_height * (canvas.width / metrics.summed_width);
                }
            }
        }

        public push_task(task: LoadQueryTask) {
            this.task_queue.push(task);

            if (this.task_queue.length == 1) {
                window.setTimeout("process_tasks()", this.task_delay);
            }
        }

        public process_tasks() {
            if (this.task_queue.length == 0) {
                // no more tasks
                return;
            }

            //get next task
            var task = this.task_queue.shift();
            task.run();
            //allow UI updates between tasks
            window.setTimeout("process_tasks()", this.task_delay);
        }

    }
}