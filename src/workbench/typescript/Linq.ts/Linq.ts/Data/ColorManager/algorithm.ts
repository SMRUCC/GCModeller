namespace TypeScript.ColorManager {

    export function hslToRgb(hue, sat, light) {
        var t1, t2, r, g, b;
        hue = hue / 60;
        if (light <= 0.5) {
            t2 = light * (sat + 1);
        } else {
            t2 = light + sat - (light * sat);
        }
        t1 = light * 2 - t2;
        r = hueToRgb(t1, t2, hue + 2) * 255;
        g = hueToRgb(t1, t2, hue) * 255;
        b = hueToRgb(t1, t2, hue - 2) * 255;
        return { r: r, g: g, b: b };
    }
    export function hueToRgb(t1, t2, hue) {
        if (hue < 0) hue += 6;
        if (hue >= 6) hue -= 6;
        if (hue < 1) return (t2 - t1) * hue + t1;
        else if (hue < 3) return t2;
        else if (hue < 4) return (t2 - t1) * (4 - hue) + t1;
        else return t1;
    }
    export function hwbToRgb(hue, white, black) {
        var i, rgb, rgbArr = [], tot;
        rgb = hslToRgb(hue, 1, 0.50);
        rgbArr[0] = rgb.r / 255;
        rgbArr[1] = rgb.g / 255;
        rgbArr[2] = rgb.b / 255;
        tot = white + black;
        if (tot > 1) {
            white = Number((white / tot).toFixed(2));
            black = Number((black / tot).toFixed(2));
        }
        for (i = 0; i < 3; i++) {
            rgbArr[i] *= (1 - (white) - (black));
            rgbArr[i] += (white);
            rgbArr[i] = Number(rgbArr[i] * 255);
        }
        return { r: rgbArr[0], g: rgbArr[1], b: rgbArr[2] };
    }
    export function cmykToRgb(c, m, y, k) {
        var r, g, b;
        r = 255 - ((Math.min(1, c * (1 - k) + k)) * 255);
        g = 255 - ((Math.min(1, m * (1 - k) + k)) * 255);
        b = 255 - ((Math.min(1, y * (1 - k) + k)) * 255);
        return { r: r, g: g, b: b };
    }

    export function rgbToHsl(r, g, b) {
        var min, max, i, l, s, maxcolor, h, rgb = [];
        rgb[0] = r / 255;
        rgb[1] = g / 255;
        rgb[2] = b / 255;
        min = rgb[0];
        max = rgb[0];
        maxcolor = 0;
        for (i = 0; i < rgb.length - 1; i++) {
            if (rgb[i + 1] <= min) { min = rgb[i + 1]; }
            if (rgb[i + 1] >= max) { max = rgb[i + 1]; maxcolor = i + 1; }
        }
        if (maxcolor == 0) {
            h = (rgb[1] - rgb[2]) / (max - min);
        }
        if (maxcolor == 1) {
            h = 2 + (rgb[2] - rgb[0]) / (max - min);
        }
        if (maxcolor == 2) {
            h = 4 + (rgb[0] - rgb[1]) / (max - min);
        }
        if (isNaN(h)) { h = 0; }
        h = h * 60;
        if (h < 0) { h = h + 360; }
        l = (min + max) / 2;
        if (min == max) {
            s = 0;
        } else {
            if (l < 0.5) {
                s = (max - min) / (max + min);
            } else {
                s = (max - min) / (2 - max - min);
            }
        }
        s = s;
        return { h: h, s: s, l: l };
    }
    export function rgbToHwb(r: number, g: number, b: number) {
        var h, w, bl;
        r = r / 255;
        g = g / 255;
        b = b / 255;
        let max = Math.max(r, g, b);
        let min = Math.min(r, g, b);
        let chroma = max - min;
        if (chroma == 0) {
            h = 0;
        } else if (r == max) {
            h = (((g - b) / chroma) % 6) * 360;
        } else if (g == max) {
            h = ((((b - r) / chroma) + 2) % 6) * 360;
        } else {
            h = ((((r - g) / chroma) + 4) % 6) * 360;
        }
        w = min;
        bl = 1 - max;
        return { h: h, w: w, b: bl };
    }
    export function rgbToCmyk(r: number, g: number, b: number) {
        var c, m, y, k;
        r = r / 255;
        g = g / 255;
        b = b / 255;
        let max = Math.max(r, g, b);
        k = 1 - max;
        if (k == 1) {
            c = 0;
            m = 0;
            y = 0;
        } else {
            c = (1 - r - k) / (1 - k);
            m = (1 - g - k) / (1 - k);
            y = (1 - b - k) / (1 - k);
        }
        return { c: c, m: m, y: y, k: k };
    }
}
