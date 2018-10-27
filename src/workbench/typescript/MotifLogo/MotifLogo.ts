namespace GCModeller.Workbench {

    export class MotifLogo {

        public show_opts_link;
        public task_queue: LoadQueryTask[] = [];
        public task_delay: number = 100;
        public my_alphabet;
        public query_pspm;

        public drawLogo(div_id: string, pwm, scale: number) {
            this.push_task(new LoadQueryTask(div_id, pwm, scale));
        }

        //draws the scale, returns the width
        public draw_scale(ctx: CanvasRenderingContext2D, metrics, alphabet_ic) {
            "use strict";

            var tic_height, i;
            tic_height = metrics.stack_height / alphabet_ic;
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

            for (i = 0; i <= alphabet_ic; i++) {
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

        public draw_stack_num(ctx: CanvasRenderingContext2D, metrics, row_index) {
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

        public draw_stack(ctx: CanvasRenderingContext2D, metrics, symbols, raster) {
            "use strict";

            var preferred_pad, sym_min, i, sym, sym_height, pad;
            preferred_pad = 0;
            sym_min = 5;

            ctx.save();//1
            ctx.translate(0, metrics.stack_height);
            for (i = 0; i < symbols.length; i++) {
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

        public draw_dashed_line(ctx: CanvasRenderingContext2D, pattern, start, x1, y1, x2, y2) {
            "use strict";

            var x, y, len, i, dx, dy, tlen, theta, mulx, muly, lx, ly;
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
            x = x1;
            y = y1;
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

        public draw_trim_background(ctx: CanvasRenderingContext2D, metrics, pspm, offset) {
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

        public size_logo_on_canvas(logo, canvas: HTMLCanvasElement, show_names: boolean, scale: number) {
            "use strict";

            var metrics: LogoMetrics;
            var draw_name = (typeof show_names === "boolean" ? show_names : (logo.get_rows() > 1));

            if (canvas.width !== 0 && canvas.height !== 0) {
                return;
            } else {
                metrics = new LogoMetrics(canvas.getContext('2d'), logo.get_columns(), logo.get_rows(), draw_name);
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

        public draw_logo_on_canvas(logo: Logo, canvas: HTMLCanvasElement, show_names: boolean, scale: number) {
            "use strict";
            var draw_name: boolean
            var ctx: CanvasRenderingContext2D, metrics: LogoMetrics, raster, pspm_i, pspm,
                offset, col_index, motif_position;
            draw_name = (typeof show_names === "boolean" ? show_names : (logo.rows > 1));
            ctx = canvas.getContext('2d');
            //assume that the user wants the canvas scaled equally so calculate what the best width for this image should be
            metrics = new LogoMetrics(ctx, logo.columns, logo.rows, draw_name);
            if (typeof scale == "number") {
                //resize the canvas to fit the scaled logo
                canvas.width = metrics.summed_width * scale;
                canvas.height = metrics.summed_height * scale;
            } else {
                if (canvas.width === 0 && canvas.height === 0) {
                    scale = 1;
                    canvas.width = metrics.summed_width;
                    canvas.height = metrics.summed_height;
                } else if (canvas.width === 0) {
                    scale = canvas.height / metrics.summed_height;
                    canvas.width = metrics.summed_width * scale;
                } else if (canvas.height === 0) {
                    scale = canvas.width / metrics.summed_width;
                    canvas.height = metrics.summed_height * scale;
                } else {
                    scale = Math.min(canvas.width / metrics.summed_width, canvas.height / metrics.summed_height);
                }
            }
            // cache the raster based on the assumption that we will be drawing a lot
            // of logos the same size
            if (typeof this.draw_logo_on_canvas.raster_scale === "number" &&
                Math.abs(this.draw_logo_on_canvas.raster_scale - scale) < 0.1) {
                raster = this.draw_logo_on_canvas.raster_cache;
            } else {
                raster = new RasterizedAlphabet(logo.alphabet, metrics.stack_font, metrics.stack_width * scale * 2);
                this.draw_logo_on_canvas.raster_cache = raster;
                this.draw_logo_on_canvas.raster_scale = scale;
            }
            ctx = canvas.getContext('2d');
            ctx.save();//s1
            ctx.scale(scale, scale);
            ctx.save();//s2
            ctx.save();//s7
            //create margin
            ctx.translate(metrics.pad_left, metrics.pad_top);
            for (pspm_i = 0; pspm_i < logo.get_rows(); ++pspm_i) {
                pspm = logo.get_pspm(pspm_i);
                offset = logo.get_offset(pspm_i);
                //optionally draw name if this isn't the last row or is the only row 
                if (draw_name && (logo.get_rows() == 1 || pspm_i != (logo.get_rows() - 1))) {
                    ctx.save();//s4
                    ctx.translate(metrics.summed_width / 2, metrics.name_height);
                    ctx.font = metrics.name_font;
                    ctx.textAlign = "center";
                    ctx.fillText(pspm.name, 0, 0);
                    ctx.restore();//s4
                    ctx.translate(0, metrics.name_height +
                        Math.min(0, metrics.name_spacer - metrics.y_num_height / 2));
                }
                //draw scale
                this.draw_scale(ctx, metrics, logo.alphabet.get_ic());
                ctx.save();//s5
                //translate across past the scale
                ctx.translate(metrics.y_label_height + metrics.y_label_spacer +
                    metrics.y_num_width + metrics.y_tic_width, 0);
                //draw the trimming background
                if (pspm.get_left_trim() > 0 || pspm.get_right_trim() > 0) {
                    this.draw_trim_background(ctx, metrics, pspm, offset);
                }
                //draw letters
                ctx.translate(0, metrics.y_num_height / 2);
                for (col_index = 0; col_index < logo.get_columns(); col_index++) {
                    ctx.translate(metrics.stack_pad_left, 0);
                    if (col_index >= offset && col_index < (offset + pspm.get_motif_length())) {
                        motif_position = col_index - offset;
                        this.draw_stack_num(ctx, metrics, motif_position);
                        this.draw_stack(ctx, metrics, pspm.get_stack(motif_position, logo.alphabet), raster);
                    }
                    ctx.translate(metrics.stack_width, 0);
                }
                ctx.restore();//s5
                ////optionally draw name if this is the last row but isn't the only row 
                if (draw_name && (logo.get_rows() != 1 && pspm_i == (logo.get_rows() - 1))) {
                    //translate vertically past the stack and axis's        
                    ctx.translate(0, metrics.y_num_height / 2 + metrics.stack_height +
                        Math.max(metrics.y_num_height / 2, metrics.x_num_above + metrics.x_num_width + metrics.name_spacer));

                    ctx.save();//s6
                    ctx.translate(metrics.summed_width / 2, metrics.name_height);
                    ctx.font = metrics.name_font;
                    ctx.textAlign = "center";
                    ctx.fillText(pspm.name, 0, 0);
                    ctx.restore();//s6
                    ctx.translate(0, metrics.name_height);
                } else {
                    //translate vertically past the stack and axis's        
                    ctx.translate(0, metrics.y_num_height / 2 + metrics.stack_height +
                        Math.max(metrics.y_num_height / 2, metrics.x_num_above + metrics.x_num_width));
                }
                //if not the last row then add middle padding
                if (pspm_i != (logo.get_rows() - 1)) {
                    ctx.translate(0, metrics.pad_middle);
                }
            }
            ctx.restore();//s7
            ctx.translate(metrics.summed_width - metrics.pad_right, metrics.summed_height - metrics.pad_bottom);
            ctx.font = metrics.fine_txt_font;
            ctx.textAlign = "right";
            ctx.fillText(logo.fine_text, 0, 0);
            ctx.restore();//s2
            ctx.restore();//s1
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