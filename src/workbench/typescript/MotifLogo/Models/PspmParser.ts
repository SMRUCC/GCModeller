namespace GCModeller.Workbench {

    export function parse_pspm_properties(str: string) {
        "use strict";
        var parts, i, eqpos, before, after, properties, prop, num, num_re;
        num_re = /^((?:[+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?)|inf)$/;
        parts = trim(str).split(/\s+/);
        // split up words containing =
        for (i = 0; i < parts.length;) {
            eqpos = parts[i].indexOf("=");
            if (eqpos != -1) {
                before = parts[i].substr(0, eqpos);
                after = parts[i].substr(eqpos + 1);
                if (before.length > 0 && after.length > 0) {
                    parts.splice(i, 1, before, "=", after);
                    i += 3;
                } else if (before.length > 0) {
                    parts.splice(i, 1, before, "=");
                    i += 2;
                } else if (after.length > 0) {
                    parts.splice(i, 1, "=", after);
                    i += 2;
                } else {
                    parts.splice(i, 1, "=");
                    i++;
                }
            } else {
                i++;
            }
        }
        properties = {};
        for (i = 0; i < parts.length; i += 3) {
            if (parts.length - i < 3) {
                throw new Error("Expected PSPM property was incomplete. " +
                    "Remaing parts are: " + parts.slice(i).join(" "));
            }
            if (parts[i + 1] !== "=") {
                throw new Error("Expected '=' in PSPM property between key and " +
                    "value but got " + parts[i + 1]);
            }
            prop = parts[i].toLowerCase();
            num = parts[i + 2];
            if (!num_re.test(num)) {
                throw new Error("Expected numeric value for PSPM property '" +
                    prop + "' but got '" + num + "'");
            }
            properties[prop] = num;
        }
        return properties;
    }

    export function parse_pspm_string(pspm_string: string): IPspm {
        "use strict";
        var header_re, lines, first_line, line_num, col_num, alph_length,
            motif_length, nsites, evalue, pspm, i, line, match, props, parts,
            j, prob;
        header_re = /^letter-probability\s+matrix:(.*)$/i;
        lines = pspm_string.split(/\n/);
        first_line = true;
        line_num = 0;
        col_num = 0;
        alph_length;
        motif_length;
        nsites;
        evalue;
        pspm = [];
        for (i = 0; i < lines.length; i++) {
            line = trim(lines[i]);
            if (line.length === 0) {
                continue;
            }
            // check the first line for a header though allow matrices without it
            if (first_line) {
                first_line = false;
                match = header_re.exec(line);
                if (match !== null) {
                    props = parse_pspm_properties(match[1]);
                    if (props.hasOwnProperty("alength")) {
                        alph_length = parseFloat(props["alength"]);
                        if (alph_length != 4 && alph_length != 20) {
                            throw new Error("PSPM property alength should be 4 or 20" +
                                " but got " + alph_length);
                        }
                    }
                    if (props.hasOwnProperty("w")) {
                        motif_length = parseFloat(props["w"]);
                        if (motif_length % 1 !== 0 || motif_length < 1) {
                            throw new Error("PSPM property w should be an integer larger " +
                                "than zero but got " + motif_length);
                        }
                    }
                    if (props.hasOwnProperty("nsites")) {
                        nsites = parseFloat(props["nsites"]);
                        if (nsites <= 0) {
                            throw new Error("PSPM property nsites should be larger than " +
                                "zero but got " + nsites);
                        }
                    }
                    if (props.hasOwnProperty("e")) {
                        evalue = props["e"];
                        if (evalue < 0) {
                            throw new Error("PSPM property evalue should be " +
                                "non-negative but got " + evalue);
                        }
                    }
                    continue;
                }
            }
            pspm[line_num] = [];
            col_num = 0;
            parts = line.split(/\s+/);
            for (j = 0; j < parts.length; j++) {
                prob = parseFloat(parts[j]);
                if (prob != parts[j] || prob < 0 || prob > 1) {
                    throw new Error("Expected probability but got '" + parts[j] + "'");
                }
                pspm[line_num][col_num] = prob;
                col_num++;
            }
            line_num++;
        }
        if (typeof motif_length === "number") {
            if (pspm.length != motif_length) {
                throw new Error("Expected PSPM to have a motif length of " +
                    motif_length + " but it was actually " + pspm.length);
            }
        } else {
            motif_length = pspm.length;
        }
        if (typeof alph_length !== "number") {
            alph_length = pspm[0].length;
            if (alph_length != 4 && alph_length != 20) {
                throw new Error("Expected length of first row in the PSPM to be " +
                    "either 4 or 20 but got " + alph_length);
            }
        }
        for (i = 0; i < pspm.length; i++) {
            if (pspm[i].length != alph_length) {
                throw new Error("Expected PSPM row " + i + " to have a length of " +
                    alph_length + " but the length was " + pspm[i].length);
            }
        }
        return {
            pspm: pspm,
            motif_length: motif_length,
            alph_length: alph_length,
            nsites: nsites,
            evalue: evalue
        };
    }

    export interface IPspm {
        pspm: number[][];
        motif_length: number;
        alph_length: number;
        nsites: number;
        evalue: number;
    }
}