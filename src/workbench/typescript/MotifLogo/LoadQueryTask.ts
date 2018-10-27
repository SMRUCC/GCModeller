namespace GCModeller.Workbench {

    /**
     * Draw motif logo from this function
    */
    export class LoadQueryTask {

        public target_id: string;
        public motifPWM;
        public scaleLogo: number;
        public render: MotifLogo;

        public constructor(target_id: string, pwm, scale: number, render: MotifLogo) {
            this.target_id = target_id;
            this.motifPWM = pwm;
            this.scaleLogo = scale;
            this.render = render;
        }

        public run() {
            var alpha = new Alphabet("ACGT");
            var query_pspm = new Pspm(this.motifPWM, null);

            this.replace_logo(this.logo_1(alpha, "MEME Suite", query_pspm), this.target_id, this.scaleLogo, "Preview Logo", "block");
        }

        public logo_1(alphabet: Alphabet, fine_text: string, pspm: Pspm): Logo {
            "use strict";
            return new Logo(alphabet, fine_text).addPspm(pspm);
        }

        /**
         * Specifes that the element with the specified id
         * should be replaced with a generated logo.
        */
        public replace_logo(logo: Logo, replace_id: string, scale: number, title_txt: string, display_style: string) {
            "use strict";

            var element = document.getElementById(replace_id);

            if (!replace_id) {
                alert("Can't find specified id (" + replace_id + ")");
                return;
            }

            //found the element!
            var canvas = CanvasHelper.createCanvas([500, 1200], replace_id, title_txt, display_style);

            if (canvas === null) {
                return;
            }

            //draw the logo on the canvas
            this.render.draw_logo_on_canvas(logo, canvas, null, scale);

            //replace the element with the canvas
            element.parentNode.replaceChild(canvas, element);
        }
    }
}

