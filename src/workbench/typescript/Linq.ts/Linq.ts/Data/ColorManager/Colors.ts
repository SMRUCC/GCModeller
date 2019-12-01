namespace TypeScript.ColorManager {

    export function toColorObject(c: string): IW3color {
        var x, y, typ, arr = [], arrlength, i, opacity, match, a, hue, sat, rgb, colornames = [], colorhexs = [];
        c = w3trim(c.toLowerCase());
        x = c.substr(0, 1).toUpperCase();
        y = c.substr(1);
        a = 1;
        if ((x == "R" || x == "Y" || x == "G" || x == "C" || x == "B" || x == "M" || x == "W") && !isNaN(y)) {
            if (c.length == 6 && c.indexOf(",") == -1) {
            } else {
                c = "ncol(" + c + ")";
            }
        }
        if (c.length != 3 && c.length != 6 && !isNaN(parseFloat(c))) {
            c = "ncol(" + c + ")";
        }
        if (c.indexOf(",") > 0 && c.indexOf("(") == -1) {
            c = "ncol(" + c + ")";
        }
        if (c.substr(0, 3) == "rgb" || c.substr(0, 3) == "hsl" || c.substr(0, 3) == "hwb" || c.substr(0, 4) == "ncol" || c.substr(0, 4) == "cmyk") {
            if (c.substr(0, 4) == "ncol") {
                if (c.split(",").length == 4 && c.indexOf("ncola") == -1) {
                    c = c.replace("ncol", "ncola");
                }
                typ = "ncol";
                c = c.substr(4);
            } else if (c.substr(0, 4) == "cmyk") {
                typ = "cmyk";
                c = c.substr(4);
            } else {
                typ = c.substr(0, 3);
                c = c.substr(3);
            }
            arrlength = 3;
            opacity = false;
            if (c.substr(0, 1).toLowerCase() == "a") {
                arrlength = 4;
                opacity = true;
                c = c.substr(1);
            } else if (typ == "cmyk") {
                arrlength = 4;
                if (c.split(",").length == 5) {
                    arrlength = 5;
                    opacity = true;
                }
            }
            c = c.replace("(", "");
            c = c.replace(")", "");
            arr = c.split(",");
            if (typ == "rgb") {
                if (arr.length != arrlength) {
                    return w3color.emptyObject;
                }
                for (i = 0; i < arrlength; i++) {
                    if (arr[i] == "" || arr[i] == " ") { arr[i] = "0"; }
                    if (arr[i].indexOf("%") > -1) {
                        arr[i] = arr[i].replace("%", "");
                        arr[i] = Number(arr[i] / 100);
                        if (i < 3) { arr[i] = Math.round(arr[i] * 255); }
                    }
                    if (isNaN(arr[i])) { return w3color.emptyObject; }
                    if (parseInt(arr[i]) > 255) { arr[i] = 255; }
                    if (i < 3) { arr[i] = parseInt(arr[i]); }
                    if (i == 3 && Number(arr[i]) > 1) { arr[i] = 1; }
                }
                rgb = { r: arr[0], g: arr[1], b: arr[2] };
                if (opacity == true) { a = Number(arr[3]); }
            }
            if (typ == "hsl" || typ == "hwb" || typ == "ncol") {
                while (arr.length < arrlength) { arr.push("0"); }
                if (typ == "hsl" || typ == "hwb") {
                    if (parseInt(arr[0]) >= 360) { arr[0] = 0; }
                }
                for (i = 1; i < arrlength; i++) {
                    if (arr[i].indexOf("%") > -1) {
                        arr[i] = arr[i].replace("%", "");
                        arr[i] = Number(arr[i]);
                        if (isNaN(arr[i])) { return w3color.emptyObject; }
                        arr[i] = arr[i] / 100;
                    } else {
                        arr[i] = Number(arr[i]);
                    }
                    if (Number(arr[i]) > 1) { arr[i] = 1; }
                    if (Number(arr[i]) < 0) { arr[i] = 0; }
                }
                if (typ == "hsl") { rgb = hslToRgb(arr[0], arr[1], arr[2]); hue = Number(arr[0]); sat = Number(arr[1]); }
                if (typ == "hwb") { rgb = hwbToRgb(arr[0], arr[1], arr[2]); }
                if (typ == "ncol") { rgb = ncolToRgb(arr[0], arr[1], arr[2]); }
                if (opacity == true) { a = Number(arr[3]); }
            }
            if (typ == "cmyk") {
                while (arr.length < arrlength) { arr.push("0"); }
                for (i = 0; i < arrlength; i++) {
                    if (arr[i].indexOf("%") > -1) {
                        arr[i] = arr[i].replace("%", "");
                        arr[i] = Number(arr[i]);
                        if (isNaN(arr[i])) { return w3color.emptyObject; }
                        arr[i] = arr[i] / 100;
                    } else {
                        arr[i] = Number(arr[i]);
                    }
                    if (Number(arr[i]) > 1) { arr[i] = 1; }
                    if (Number(arr[i]) < 0) { arr[i] = 0; }
                }
                rgb = cmykToRgb(arr[0], arr[1], arr[2], arr[3]);
                if (opacity == true) { a = Number(arr[4]); }
            }
        } else if (c.substr(0, 3) == "ncs") {
            rgb = ncsToRgb(c);
        } else {
            match = false;
            colornames = getColorArr('names');
            for (i = 0; i < colornames.length; i++) {
                if (c.toLowerCase() == colornames[i].toLowerCase()) {
                    colorhexs = getColorArr('hexs');
                    match = true;
                    rgb = {
                        r: parseInt(colorhexs[i].substr(0, 2), 16),
                        g: parseInt(colorhexs[i].substr(2, 2), 16),
                        b: parseInt(colorhexs[i].substr(4, 2), 16)
                    };
                    break;
                }
            }
            if (match == false) {
                c = c.replace("#", "");
                if (c.length == 3) { c = c.substr(0, 1) + c.substr(0, 1) + c.substr(1, 1) + c.substr(1, 1) + c.substr(2, 1) + c.substr(2, 1); }
                for (i = 0; i < c.length; i++) {
                    if (!isHex(c.substr(i, 1))) { return w3color.emptyObject; }
                }
                arr[0] = parseInt(c.substr(0, 2), 16);
                arr[1] = parseInt(c.substr(2, 2), 16);
                arr[2] = parseInt(c.substr(4, 2), 16);
                for (i = 0; i < 3; i++) {
                    if (isNaN(arr[i])) { return w3color.emptyObject; }
                }
                rgb = {
                    r: arr[0],
                    g: arr[1],
                    b: arr[2]
                };
            }
        }
        return colorObject(rgb, a, hue, sat);
    }

    export function colorObject(rgb, a, h, s) {
        var hsl, hwb, cmyk, ncol, color, hue, sat;
        if (!rgb) { return w3color.emptyObject; }
        if (!a) { a = 1; }
        hsl = rgbToHsl(rgb.r, rgb.g, rgb.b);
        hwb = rgbToHwb(rgb.r, rgb.g, rgb.b);
        cmyk = rgbToCmyk(rgb.r, rgb.g, rgb.b);
        hue = (h || hsl.h);
        sat = (s || hsl.s);
        ncol = hueToNcol(hue);
        color = {
            red: rgb.r,
            green: rgb.g,
            blue: rgb.b,
            hue: hue,
            sat: sat,
            lightness: hsl.l,
            whiteness: hwb.w,
            blackness: hwb.b,
            cyan: cmyk.c,
            magenta: cmyk.m,
            yellow: cmyk.y,
            black: cmyk.k,
            ncol: ncol,
            opacity: a,
            valid: true
        };
        color = roundDecimals(color);
        return color;
    }

    export function getColorArr(x: "names" | "hexs"): string[] {
        if (x == "names") {
            return ['AliceBlue', 'AntiqueWhite', 'Aqua', 'Aquamarine', 'Azure', 'Beige', 'Bisque', 'Black', 'BlanchedAlmond', 'Blue', 'BlueViolet', 'Brown', 'BurlyWood', 'CadetBlue', 'Chartreuse', 'Chocolate', 'Coral', 'CornflowerBlue', 'Cornsilk', 'Crimson', 'Cyan', 'DarkBlue', 'DarkCyan', 'DarkGoldenRod', 'DarkGray', 'DarkGrey', 'DarkGreen', 'DarkKhaki', 'DarkMagenta', 'DarkOliveGreen', 'DarkOrange', 'DarkOrchid', 'DarkRed', 'DarkSalmon', 'DarkSeaGreen', 'DarkSlateBlue', 'DarkSlateGray', 'DarkSlateGrey', 'DarkTurquoise', 'DarkViolet', 'DeepPink', 'DeepSkyBlue', 'DimGray', 'DimGrey', 'DodgerBlue', 'FireBrick', 'FloralWhite', 'ForestGreen', 'Fuchsia', 'Gainsboro', 'GhostWhite', 'Gold', 'GoldenRod', 'Gray', 'Grey', 'Green', 'GreenYellow', 'HoneyDew', 'HotPink', 'IndianRed', 'Indigo', 'Ivory', 'Khaki', 'Lavender', 'LavenderBlush', 'LawnGreen', 'LemonChiffon', 'LightBlue', 'LightCoral', 'LightCyan', 'LightGoldenRodYellow', 'LightGray', 'LightGrey', 'LightGreen', 'LightPink', 'LightSalmon', 'LightSeaGreen', 'LightSkyBlue', 'LightSlateGray', 'LightSlateGrey', 'LightSteelBlue', 'LightYellow', 'Lime', 'LimeGreen', 'Linen', 'Magenta', 'Maroon', 'MediumAquaMarine', 'MediumBlue', 'MediumOrchid', 'MediumPurple', 'MediumSeaGreen', 'MediumSlateBlue', 'MediumSpringGreen', 'MediumTurquoise', 'MediumVioletRed', 'MidnightBlue', 'MintCream', 'MistyRose', 'Moccasin', 'NavajoWhite', 'Navy', 'OldLace', 'Olive', 'OliveDrab', 'Orange', 'OrangeRed', 'Orchid', 'PaleGoldenRod', 'PaleGreen', 'PaleTurquoise', 'PaleVioletRed', 'PapayaWhip', 'PeachPuff', 'Peru', 'Pink', 'Plum', 'PowderBlue', 'Purple', 'RebeccaPurple', 'Red', 'RosyBrown', 'RoyalBlue', 'SaddleBrown', 'Salmon', 'SandyBrown', 'SeaGreen', 'SeaShell', 'Sienna', 'Silver', 'SkyBlue', 'SlateBlue', 'SlateGray', 'SlateGrey', 'Snow', 'SpringGreen', 'SteelBlue', 'Tan', 'Teal', 'Thistle', 'Tomato', 'Turquoise', 'Violet', 'Wheat', 'White', 'WhiteSmoke', 'Yellow', 'YellowGreen'];
        }
        if (x == "hexs") {
            return ['f0f8ff', 'faebd7', '00ffff', '7fffd4', 'f0ffff', 'f5f5dc', 'ffe4c4', '000000', 'ffebcd', '0000ff', '8a2be2', 'a52a2a', 'deb887', '5f9ea0', '7fff00', 'd2691e', 'ff7f50', '6495ed', 'fff8dc', 'dc143c', '00ffff', '00008b', '008b8b', 'b8860b', 'a9a9a9', 'a9a9a9', '006400', 'bdb76b', '8b008b', '556b2f', 'ff8c00', '9932cc', '8b0000', 'e9967a', '8fbc8f', '483d8b', '2f4f4f', '2f4f4f', '00ced1', '9400d3', 'ff1493', '00bfff', '696969', '696969', '1e90ff', 'b22222', 'fffaf0', '228b22', 'ff00ff', 'dcdcdc', 'f8f8ff', 'ffd700', 'daa520', '808080', '808080', '008000', 'adff2f', 'f0fff0', 'ff69b4', 'cd5c5c', '4b0082', 'fffff0', 'f0e68c', 'e6e6fa', 'fff0f5', '7cfc00', 'fffacd', 'add8e6', 'f08080', 'e0ffff', 'fafad2', 'd3d3d3', 'd3d3d3', '90ee90', 'ffb6c1', 'ffa07a', '20b2aa', '87cefa', '778899', '778899', 'b0c4de', 'ffffe0', '00ff00', '32cd32', 'faf0e6', 'ff00ff', '800000', '66cdaa', '0000cd', 'ba55d3', '9370db', '3cb371', '7b68ee', '00fa9a', '48d1cc', 'c71585', '191970', 'f5fffa', 'ffe4e1', 'ffe4b5', 'ffdead', '000080', 'fdf5e6', '808000', '6b8e23', 'ffa500', 'ff4500', 'da70d6', 'eee8aa', '98fb98', 'afeeee', 'db7093', 'ffefd5', 'ffdab9', 'cd853f', 'ffc0cb', 'dda0dd', 'b0e0e6', '800080', '663399', 'ff0000', 'bc8f8f', '4169e1', '8b4513', 'fa8072', 'f4a460', '2e8b57', 'fff5ee', 'a0522d', 'c0c0c0', '87ceeb', '6a5acd', '708090', '708090', 'fffafa', '00ff7f', '4682b4', 'd2b48c', '008080', 'd8bfd8', 'ff6347', '40e0d0', 'ee82ee', 'f5deb3', 'ffffff', 'f5f5f5', 'ffff00', '9acd32'];
        }
    }

    function roundDecimals(c: IW3color) {
        c.red = Number(c.red.toFixed(0));
        c.green = Number(c.green.toFixed(0));
        c.blue = Number(c.blue.toFixed(0));
        c.hue = Number(c.hue.toFixed(0));
        c.sat = Number(c.sat.toFixed(2));
        c.lightness = Number(c.lightness.toFixed(2));
        c.whiteness = Number(c.whiteness.toFixed(2));
        c.blackness = Number(c.blackness.toFixed(2));
        c.cyan = Number(c.cyan.toFixed(2));
        c.magenta = Number(c.magenta.toFixed(2));
        c.yellow = Number(c.yellow.toFixed(2));
        c.black = Number(c.black.toFixed(2));
        c.ncol = c.ncol.substr(0, 1) + Math.round(Number(c.ncol.substr(1)));
        c.opacity = Number(c.opacity.toFixed(2));
        return c;
    }

    function ncolToRgb(ncol: string, white: number, black: number) {
        var letter, percent, h, w, b;
        h = ncol;
        if (isNaN(parseFloat(ncol.substr(0, 1)))) {
            letter = ncol.substr(0, 1).toUpperCase();
            percent = ncol.substr(1);
            if (percent == "") { percent = 0; }
            percent = Number(percent);
            if (isNaN(percent)) { return false; }
            if (letter == "R") { h = 0 + (percent * 0.6); }
            if (letter == "Y") { h = 60 + (percent * 0.6); }
            if (letter == "G") { h = 120 + (percent * 0.6); }
            if (letter == "C") { h = 180 + (percent * 0.6); }
            if (letter == "B") { h = 240 + (percent * 0.6); }
            if (letter == "M") { h = 300 + (percent * 0.6); }
            if (letter == "W") {
                h = 0;
                white = 1 - (percent / 100);
                black = (percent / 100);
            }
        }
        return hwbToRgb(h, white, black);
    }
    function hueToNcol(hue: number) {
        while (hue >= 360) {
            hue = hue - 360;
        }
        if (hue < 60) { return "R" + (hue / 0.6); }
        if (hue < 120) { return "Y" + ((hue - 60) / 0.6); }
        if (hue < 180) { return "G" + ((hue - 120) / 0.6); }
        if (hue < 240) { return "C" + ((hue - 180) / 0.6); }
        if (hue < 300) { return "B" + ((hue - 240) / 0.6); }
        if (hue < 360) { return "M" + ((hue - 300) / 0.6); }
    }
    function ncsToRgb(ncs) {
        var black, chroma, bc, percent, black1, chroma1, red1, factor1, blue1, red1, red2, green1, green2, blue2, max, factor2, grey, r, g, b;

        ncs = w3trim(ncs).toUpperCase();
        ncs = ncs.replace("(", "");
        ncs = ncs.replace(")", "");
        ncs = ncs.replace("NCS", "NCS ");
        ncs = ncs.replace(/  /g, " ");
        if (ncs.indexOf("NCS") == -1) { ncs = "NCS " + ncs; }
        ncs = ncs.match(/^(?:NCS|NCS\sS)\s(\d{2})(\d{2})-(N|[A-Z])(\d{2})?([A-Z])?$/);
        if (ncs === null) return false;
        black = parseInt(ncs[1], 10);
        chroma = parseInt(ncs[2], 10);
        bc = ncs[3];
        if (bc != "N" && bc != "Y" && bc != "R" && bc != "B" && bc != "G") { return false; }
        percent = parseInt(ncs[4], 10) || 0;
        if (bc !== 'N') {
            black1 = (1.05 * black - 5.25);
            chroma1 = chroma;
            if (bc === 'Y' && percent <= 60) {
                red1 = 1;
            } else if ((bc === 'Y' && percent > 60) || (bc === 'R' && percent <= 80)) {
                if (bc === 'Y') {
                    factor1 = percent - 60;
                } else {
                    factor1 = percent + 40;
                }
                red1 = ((Math.sqrt(14884 - Math.pow(factor1, 2))) - 22) / 100;
            } else if ((bc === 'R' && percent > 80) || (bc === 'B')) {
                red1 = 0;
            } else if (bc === 'G') {
                factor1 = (percent - 170);
                red1 = ((Math.sqrt(33800 - Math.pow(factor1, 2))) - 70) / 100;
            }
            if (bc === 'Y' && percent <= 80) {
                blue1 = 0;
            } else if ((bc === 'Y' && percent > 80) || (bc === 'R' && percent <= 60)) {
                if (bc === 'Y') {
                    factor1 = (percent - 80) + 20.5;
                } else {
                    factor1 = (percent + 20) + 20.5;
                }
                blue1 = (104 - (Math.sqrt(11236 - Math.pow(factor1, 2)))) / 100;
            } else if ((bc === 'R' && percent > 60) || (bc === 'B' && percent <= 80)) {
                if (bc === 'R') {
                    factor1 = (percent - 60) - 60;
                } else {
                    factor1 = (percent + 40) - 60;
                }
                blue1 = ((Math.sqrt(10000 - Math.pow(factor1, 2))) - 10) / 100;
            } else if ((bc === 'B' && percent > 80) || (bc === 'G' && percent <= 40)) {
                if (bc === 'B') {
                    factor1 = (percent - 80) - 131;
                } else {
                    factor1 = (percent + 20) - 131;
                }
                blue1 = (122 - (Math.sqrt(19881 - Math.pow(factor1, 2)))) / 100;
            } else if (bc === 'G' && percent > 40) {
                blue1 = 0;
            }
            if (bc === 'Y') {
                green1 = (85 - 17 / 20 * percent) / 100;
            } else if (bc === 'R' && percent <= 60) {
                green1 = 0;
            } else if (bc === 'R' && percent > 60) {
                factor1 = (percent - 60) + 35;
                green1 = (67.5 - (Math.sqrt(5776 - Math.pow(factor1, 2)))) / 100;
            } else if (bc === 'B' && percent <= 60) {
                factor1 = (1 * percent - 68.5);
                green1 = (6.5 + (Math.sqrt(7044.5 - Math.pow(factor1, 2)))) / 100;
            } else if ((bc === 'B' && percent > 60) || (bc === 'G' && percent <= 60)) {
                green1 = 0.9;
            } else if (bc === 'G' && percent > 60) {
                factor1 = (percent - 60);
                green1 = (90 - (1 / 8 * factor1)) / 100;
            }
            factor1 = (red1 + green1 + blue1) / 3;
            red2 = ((factor1 - red1) * (100 - chroma1) / 100) + red1;
            green2 = ((factor1 - green1) * (100 - chroma1) / 100) + green1;
            blue2 = ((factor1 - blue1) * (100 - chroma1) / 100) + blue1;
            if (red2 > green2 && red2 > blue2) {
                max = red2;
            } else if (green2 > red2 && green2 > blue2) {
                max = green2;
            } else if (blue2 > red2 && blue2 > green2) {
                max = blue2;
            } else {
                max = (red2 + green2 + blue2) / 3;
            }
            factor2 = 1 / max;
            r = parseInt(<any>((red2 * factor2 * (100 - black1) / 100) * 255), 10);
            g = parseInt(<any>((green2 * factor2 * (100 - black1) / 100) * 255), 10);
            b = parseInt(<any>((blue2 * factor2 * (100 - black1) / 100) * 255), 10);
            if (r > 255) { r = 255; }
            if (g > 255) { g = 255; }
            if (b > 255) { b = 255; }
            if (r < 0) { r = 0; }
            if (g < 0) { g = 0; }
            if (b < 0) { b = 0; }
        } else {
            grey = parseInt(<any>((1 - black / 100) * 255), 10);
            if (grey > 255) { grey = 255; }
            if (grey < 0) { grey = 0; }
            r = grey;
            g = grey;
            b = grey;
        }
        return {
            r: r,
            g: g,
            b: b
        };
    }

    export function w3SetColorsByAttribute() {
        var z, i, att;
        z = document.getElementsByTagName("*");
        for (i = 0; i < z.length; i++) {
            att = z[i].getAttribute("data-w3-color");
            if (att) {
                z[i].style.backgroundColor = new w3color(att).toRgbString();
            }
        }
    }
}