/// <reference path="../../build/linq.d.ts" />
declare namespace GCModeller.Workbench {
    class CanvasRender {
        raster_scale: number;
        raster_cache: RasterizedAlphabet;
        host: MotifLogo;
        constructor(host: MotifLogo);
        doRender(logo: Logo, canvas: HTMLCanvasElement, show_names: boolean, scale: number): void;
    }
}
declare namespace GCModeller.Workbench {
    /**
     * Fast string trimming implementation found at
     * http://blog.stevenlevithan.com/archives/faster-trim-javascript
     *
     * Note that regex is good at removing leading space but
     * bad at removing trailing space as it has to first go through
     * the whole string.
    */
    function trim(str: string): string;
}
declare namespace GCModeller.Workbench {
    /**
     * Draw motif logo from this function
    */
    class LoadQueryTask {
        target_id: string;
        motifPWM: Pspm;
        scaleLogo: number;
        render: MotifLogo;
        constructor(target_id: string, pwm: Pspm, scale: number, render: MotifLogo);
        run(): void;
        logo_1(alphabet: Alphabet, fine_text: string, pspm: Pspm): Logo;
        /**
         * Specifes that the element with the specified id
         * should be replaced with a generated logo.
        */
        replace_logo(logo: Logo, replace_id: string, scale: number, title_txt: string, display_style: string): void;
    }
}
declare namespace GCModeller.Workbench {
    class MotifLogo {
        task_queue: LoadQueryTask[];
        task_delay: number;
        draw_logo_on_canvas: CanvasRender;
        constructor();
        /**
         * @param div_id 需要进行显示的div的id编号字符串，不带``#``符号前缀
         * @param pwm Motif数据文本
         * @param scale 缩放倍数
        */
        drawLogo(div_id: string, pwm: Pspm, scale?: number): void;
        /**
         * draws the scale, returns the width
        */
        draw_scale(ctx: CanvasRenderingContext2D, metrics: LogoMetrics, alphabet_ic: number): void;
        draw_stack_num(ctx: CanvasRenderingContext2D, metrics: LogoMetrics, row_index: number): void;
        draw_stack(ctx: CanvasRenderingContext2D, metrics: LogoMetrics, symbols: Symbol[], raster: RasterizedAlphabet): void;
        draw_dashed_line(ctx: CanvasRenderingContext2D, pattern: number[], start: number, x1: number, y1: number, x2: number, y2: number): void;
        draw_trim_background(ctx: CanvasRenderingContext2D, metrics: LogoMetrics, pspm: Pspm, offset: number): void;
        size_logo_on_canvas(logo: Logo, canvas: HTMLCanvasElement, show_names: boolean, scale: number): void;
    }
}
declare namespace GCModeller.Workbench {
    class Alphabet {
        static readonly is_letter: RegExp;
        static readonly is_prob: RegExp;
        freqs: number[];
        alphabet: string[];
        private letter_count;
        readonly ic: number;
        readonly size: number;
        readonly isNucleotide: boolean;
        constructor(alphabet: string, bg?: string);
        private parseBackground;
        toString(): string;
        getLetter(index: number): string;
        getBgfreq(index: number): number;
        getColour(index: number): string;
        isAmbig(index: number): boolean;
        getIndex(letter: string): number;
    }
}
declare namespace GCModeller.Workbench.AlphabetColors {
    const red: string;
    const blue: string;
    const orange: string;
    const green: string;
    const yellow: string;
    const purple: string;
    const magenta: string;
    const pink: string;
    const turquoise: string;
    function nucleotideColor(alphabet: string): string;
    function proteinColor(alphabet: string): string;
}
declare namespace GCModeller.Workbench {
    class Logo {
        alphabet: Alphabet;
        fine_text: string;
        pspm_list: Pspm[];
        pspm_column: number[];
        rows: number;
        columns: number;
        constructor(alphabet: Alphabet, fine_text: string);
        addPspm(pspm: Pspm, column?: number): Logo;
        getPspm(rowIndex: number): Pspm;
        getOffset(rowIndex: any): number;
    }
}
declare namespace GCModeller.Workbench {
    /**
     * 大小与布局计算程序
    */
    class LogoMetrics {
        pad_top: number;
        pad_left: number;
        pad_right: number;
        pad_bottom: number;
        pad_middle: number;
        name_height: number;
        name_font: string;
        name_spacer: number;
        y_label: string;
        y_label_height: number;
        y_label_font: string;
        y_label_spacer: number;
        y_num_height: number;
        y_num_width: number;
        y_num_font: string;
        y_tic_width: number;
        stack_pad_left: number;
        stack_font: string;
        stack_height: number;
        stack_width: number;
        stacks_pad_right: number;
        x_num_above: number;
        x_num_height: number;
        x_num_width: number;
        x_num_font: string;
        fine_txt_height: number;
        fine_txt_above: number;
        fine_txt_font: string;
        letter_metrics: any[];
        summed_width: number;
        summed_height: number;
        constructor(ctx: CanvasRenderingContext2D, logo_columns: number, logo_rows: number, allow_space_for_names: boolean);
    }
}
declare namespace GCModeller.Workbench {
    const evalueRegexp: RegExp;
    class Pspm {
        name: string;
        alph_length: number;
        motif_length: number;
        nsites: number;
        evalue: number;
        ltrim: number;
        rtrim: number;
        pspm: number[][];
        constructor(matrix: Pspm | string | number[][], name?: string, ltrim?: number, rtrim?: number, nsites?: number, evalue?: number | string);
        private parseInternal;
        private createFromValueArray;
        private matrixParseFromString;
        /**
         * copy constructor
        */
        private copyInternal;
        copy(): Pspm;
        reverse_complement(alphabet: Alphabet): Pspm;
        get_stack(position: number, alphabet: Alphabet): Symbol[];
        get_stack_ic(position: number, alphabet: Alphabet): number;
        getError(alphabet: Alphabet): number;
        readonly leftTrim: number;
        readonly rightTrim: number;
        /**
         * 将当前的数据模型转换为Motif数据的字符串用于进行保存
        */
        as_pspm(): string;
        as_pssm(alphabet: Alphabet, pseudo?: number): string;
        toString(): string;
    }
}
declare namespace GCModeller.Workbench {
    function parse_pspm_properties(str: string): any;
    function parse_pspm_string(pspm_string: string): IPspm;
    interface IPspm {
        pspm: number[][];
        motif_length: number;
        alph_length: number;
        nsites: number;
        evalue: number;
    }
}
declare namespace GCModeller.Workbench {
    class RasterizedAlphabet {
        lookup: number[];
        rasters: HTMLCanvasElement[];
        dimensions: RasterBounds[];
        constructor(alphabet: Alphabet, font: string, target_width: number);
        draw(ctx: CanvasRenderingContext2D, letter: string, dx: number, dy: number, dWidth: number, dHeight: number): void;
        static canvas_bounds(ctx: CanvasRenderingContext2D, cwidth: number, cheight: number): RasterBounds;
    }
    class RasterBounds {
        bound_top: number;
        bound_bottom: number;
        bound_left: number;
        bound_right: number;
        width: number;
        height: number;
    }
}
declare namespace GCModeller.Workbench {
    class Symbol {
        symbol: string;
        scale: number;
        colour: string;
        constructor(index: number, scale: number, alphabet: Alphabet);
        toString(): string;
        static compareSymbol(sym1: Symbol, sym2: Symbol): number;
    }
}
