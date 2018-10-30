namespace GCModeller.Workbench {

    export class CanvasRender {

        public raster_scale: number;
        public raster_cache: RasterizedAlphabet;
        public host: MotifLogo;

        public constructor(host: MotifLogo) {
            this.host = host;
        }

        public doRender(logo: Logo, canvas: HTMLCanvasElement, show_names: boolean, scale: number) {
            "use strict";

            var ctx: CanvasRenderingContext2D, metrics: LogoMetrics, raster: RasterizedAlphabet, pspm_i;
            var pspm: Pspm, offset: number, col_index: number, motif_position: number;
            var draw_name: boolean = (typeof show_names === "boolean" ? show_names : (logo.rows > 1));

            ctx = canvas.getContext('2d');
            // assume that the user wants the canvas scaled equally so calculate 
            // what the best width for this image should be
            metrics = new LogoMetrics(ctx, logo.columns, logo.rows, draw_name);

            if (typeof scale == "number") {
                // resize the canvas to fit the scaled logo
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
            if (typeof this.raster_scale === "number" &&
                Math.abs(this.raster_scale - scale) < 0.1) {
                raster = this.raster_cache;
            } else {
                raster = new RasterizedAlphabet(logo.alphabet, metrics.stack_font, metrics.stack_width * scale * 2);
                this.raster_cache = raster;
                this.raster_scale = scale;
            }

            ctx = canvas.getContext('2d');
            ctx.save();//s1
            ctx.scale(scale, scale);
            ctx.save();//s2
            ctx.save();//s7
            // create margin
            ctx.translate(metrics.pad_left, metrics.pad_top);

            for (pspm_i = 0; pspm_i < logo.rows; ++pspm_i) {
                pspm = logo.getPspm(pspm_i);
                offset = logo.getOffset(pspm_i);

                //optionally draw name if this isn't the last row or is the only row 
                if (draw_name && (logo.rows == 1 || pspm_i != (logo.rows - 1))) {
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
                this.host.draw_scale(ctx, metrics, logo.alphabet.ic);
                ctx.save();//s5
                //translate across past the scale
                ctx.translate(metrics.y_label_height + metrics.y_label_spacer +
                    metrics.y_num_width + metrics.y_tic_width, 0);
                //draw the trimming background
                if (pspm.get_left_trim() > 0 || pspm.get_right_trim() > 0) {
                    this.host.draw_trim_background(ctx, metrics, pspm, offset);
                }
                //draw letters
                ctx.translate(0, metrics.y_num_height / 2);
                for (col_index = 0; col_index < logo.columns; col_index++) {
                    ctx.translate(metrics.stack_pad_left, 0);
                    if (col_index >= offset && col_index < (offset + pspm.get_motif_length())) {
                        motif_position = col_index - offset;
                        this.host.draw_stack_num(ctx, metrics, motif_position);
                        this.host.draw_stack(ctx, metrics, pspm.get_stack(motif_position, logo.alphabet), raster);
                    }
                    ctx.translate(metrics.stack_width, 0);
                }
                ctx.restore();//s5
                ////optionally draw name if this is the last row but isn't the only row 
                if (draw_name && (logo.rows != 1 && pspm_i == (logo.rows - 1))) {
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
                if (pspm_i != (logo.rows - 1)) {
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
    }
}