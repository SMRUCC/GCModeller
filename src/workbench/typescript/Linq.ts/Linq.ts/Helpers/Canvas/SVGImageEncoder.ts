namespace CanvasHelper.saveSvgAsPng {

    export const xmlns: string = "http://www.w3.org/2000/xmlns/";
    /**
     * ##### 2018-10-12 XMl标签必须要一开始就出现，否则会出现错误
     * 
     * error on line 2 at column 14: XML declaration allowed only at the start of the document
    */
    export const doctype: string = `<?xml version="1.0" standalone="no"?>
            <!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1//EN" "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd" [<!ENTITY nbsp "&#160;">]>`;

    /**
     * https://github.com/exupero/saveSvgAsPng
    */
    export class Encoder {

        public static prepareSvg(el: SVGSVGElement, options: Options = new Options(), cb?: (html: string | HTMLImageElement, width: number, height: number) => void) {
            requireDomNode(el);

            options.scale = options.scale || 1;
            options.responsive = options.responsive || false;

            inlineImages(el, function () {
                Encoder.doInlineImages(el, options, cb)
            });
        }

        private static doInlineImages(el: SVGSVGElement, options: Options, cb: (html: string | HTMLImageElement, width: number, height: number) => void) {
            var outer = $ts("<div>");
            var clone: SVGSVGElement = <any>el.cloneNode(true);
            var width: number, height: number;

            if (el.tagName == 'svg') {
                width = options.width || getDimension(el, clone, 'width');
                height = options.height || getDimension(el, clone, 'height');
            } else if (el.getBBox) {
                var box = el.getBBox();
                width = box.x + box.width;
                height = box.y + box.height;
                clone.setAttribute('transform', clone.getAttribute('transform').replace(/translate\(.*?\)/, ''));

                var svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg')
                svg.appendChild(clone)
                clone = svg;
            } else {
                console.error('Attempted to render non-SVG element', el);
                return;
            }

            clone.setAttribute("version", "1.1");

            if (!clone.getAttribute('xmlns')) {
                clone.setAttributeNS(xmlns, "xmlns", "http://www.w3.org/2000/svg");
            }
            if (!clone.getAttribute('xmlns:xlink')) {
                clone.setAttributeNS(xmlns, "xmlns:xlink", "http://www.w3.org/1999/xlink");
            }

            if (options.responsive) {
                clone.removeAttribute('width');
                clone.removeAttribute('height');
                clone.setAttribute('preserveAspectRatio', 'xMinYMin meet');
            } else {
                clone.setAttribute("width", (width * options.scale).toString());
                clone.setAttribute("height", (height * options.scale).toString());
            }

            clone.setAttribute("viewBox", [
                options.left || 0,
                options.top || 0,
                width,
                height
            ].join(" "));

            var fos = clone.querySelectorAll('foreignObject > *');

            for (var i = 0; i < fos.length; i++) {
                if (!fos[i].getAttribute('xmlns')) {
                    fos[i].setAttributeNS(xmlns, "xmlns", "http://www.w3.org/1999/xhtml");
                }
            }

            outer.appendChild(clone);

            // In case of custom fonts we need to fetch font first, and then inline
            // its url into data-uri format (encode as base64). That's why style
            // processing is done asynchonously. Once all inlining is finshed
            // cssLoadedCallback() is called.
            styles.doStyles(el, options, cssLoadedCallback);

            function cssLoadedCallback(css) {
                // here all fonts are inlined, so that we can render them properly.
                var s: HTMLStyleElement = <any>$ts('<style>',
                    {
                        type: 'text/css'
                    }).display(`<![CDATA[\n${css}\n]]>`);
                var defs: HTMLElement = $ts('<defs>').display(s);

                clone.insertBefore(defs, clone.firstChild);

                if (cb) {
                    var outHtml: string = outer.innerHTML;
                    outHtml = outHtml.replace(/NS\d+:href/gi, 'xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href');
                    cb(outHtml, width, height);
                }
            }
        }

        public static svgAsDataUri(el, options, cb: (uri: string) => void = null) {
            this.prepareSvg(el, options, function (svg) {
                var uri = 'data:image/svg+xml;base64,' + window.btoa(reEncode(doctype + svg));

                if (cb) {
                    cb(uri);
                }
            });
        }

        /**
         * 将svg转换为base64 data uri
        */
        private static convertToPng(src: HTMLImageElement, w: number, h: number, options: Options): string {
            var canvas: HTMLCanvasElement = <any>$ts('<canvas>', {
                width: w,
                height: h
            });
            var context = canvas.getContext('2d');

            if (options.canvg) {
                options.canvg(canvas, src);
            } else {
                context.drawImage(src, 0, 0);
            }

            if (options.backgroundColor) {
                context.globalCompositeOperation = 'destination-over';
                context.fillStyle = options.backgroundColor;
                context.fillRect(0, 0, canvas.width, canvas.height);
            }

            // base64 uri
            var png: string;

            try {
                png = canvas.toDataURL(options.encoderType, options.encoderOptions);
            } catch (e) {
                // 20181013 在typescript之中还不支持SecurityError??
                // (typeof SecurityError !== 'undefined' && e instanceof SecurityError) || 

                if (e.name == "SecurityError") {
                    console.error("Rendered SVG images cannot be downloaded in this browser.");
                    return;
                } else {
                    throw e;
                }
            }

            return png;
        }

        public static svgAsPngUri(el, options: Options = new Options(), cb: (uri: string) => void) {
            requireDomNode(el);

            options.encoderType = options.encoderType || 'image/png';
            options.encoderOptions = options.encoderOptions || 0.8;

            var convertToPng = function (src: HTMLImageElement, w: number, h: number) {
                cb(Encoder.convertToPng(src, w, h, options));
            }

            if (options.canvg) {
                this.prepareSvg(el, options, convertToPng);
            } else {
                this.svgAsDataUri(el, options, function (uri) {
                    var image = new Image();

                    image.onload = function () {
                        convertToPng(image, image.width, image.height);
                    }

                    image.onerror = function () {
                        console.error(
                            'There was an error loading the data URI as an image on the following SVG\n',
                            window.atob(uri.slice(26)), '\n',
                            "Open the following link to see browser's diagnosis\n",
                            uri);
                    }

                    image.src = uri;
                });
            }
        }

        public static saveSvg(el, name, options) {
            requireDomNode(el);

            options = options || {};
            this.svgAsDataUri(el, options, uri => DOM.download(name, uri));
        }

        /**
         * 将指定的SVG节点保存为png图片
         * 
         * @param svg 需要进行保存为图片的svg节点的对象实例或者对象的节点id值
         * @param name 所保存的文件名
         * @param options 配置参数，直接留空使用默认值就好了
        */
        public static saveSvgAsPng(
            svg: string | SVGElement,
            name: string,
            options: Options = Options.Default()) {

            if (typeof svg == "string") {
                svg = <SVGElement><any>$ts(svg)
                requireDomNode(svg);
            } else {
                requireDomNode(<SVGElement>svg);
            }

            this.svgAsPngUri(svg, options, uri => DOM.download(name, uri));
        }
    }
}