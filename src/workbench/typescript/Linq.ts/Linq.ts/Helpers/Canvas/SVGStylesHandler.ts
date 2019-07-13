namespace CanvasHelper.saveSvgAsPng {

    export class Options {

        public selectorRemap: (selectorText: string) => string;
        public modifyStyle: (cssText: string) => string;
        public encoderType: string;
        public encoderOptions: number;
        public backgroundColor: string;
        public canvg: (canvas: HTMLCanvasElement, src: HTMLImageElement) => void;
        public scale: number;
        public responsive: boolean;
        public width: number;
        public height: number;
        public left: number;
        public top: number;

        public static Default(): Options {
            return <Options>{
                encoderType: "image/png",
                encoderOptions: 0.8,
                scale: 1,
                responsive: false,
                left: 0,
                top: 0
            };
        }
    }

    export class styles {

        public static doStyles(el: SVGSVGElement, options: Options, cssLoadedCallback: (css: string) => void) {
            var css: string = "";
            // each font that has extranl link is saved into queue, and processed
            // asynchronously
            var fontsQueue: font[] = [];
            var sheets: StyleSheetList = document.styleSheets;

            for (var i = 0; i < sheets.length; i++) {
                var rules: CSSRuleList;

                try {
                    rules = (<any>sheets[i]).cssRules;
                } catch (e) {
                    if (TypeScript.logging.outputWarning) {
                        console.warn("Stylesheet could not be loaded: " + sheets[i].href);
                    }
                    continue;
                }

                if (rules != null) {
                    css += this.processCssRules(el, rules, options, sheets[i].href, fontsQueue);
                }
            }

            // Now all css is processed, it's time to handle scheduled fonts
            this.processFontQueue(fontsQueue, css, cssLoadedCallback);
        }

        private static processCssRules(
            el: SVGSVGElement,
            rules: CSSRuleList,
            options: Options,
            sheetHref: string,
            fontsQueue: font[]): string {

            var css: string = "";

            for (var j: number = 0, match; j < rules.length; j++ , match = null) {
                var rule: CSSStyleRule = <any>rules[j];

                if (typeof (rule.style) == "undefined") {
                    continue;
                }

                var selectorText: string;

                try {
                    selectorText = rule.selectorText;
                } catch (err) {
                    if (TypeScript.logging.outputWarning) {
                        console.warn(`The following CSS rule has an invalid selector: "${rule}"`, err);
                    }
                }

                try {
                    if (selectorText) {
                        match = el.querySelector(selectorText) || (<HTMLElement>el.parentNode).querySelector(selectorText);
                    }
                } catch (err) {
                    if (TypeScript.logging.outputWarning) {
                        console.warn(`Invalid CSS selector "${selectorText}"`, err);
                    }
                }

                if (match) {
                    var selector = options.selectorRemap ? options.selectorRemap(rule.selectorText) : rule.selectorText;
                    var cssText = options.modifyStyle ? options.modifyStyle(rule.style.cssText) : rule.style.cssText;

                    css += selector + " { " + cssText + " }\n";
                } else if (rule.cssText.match(/^@font-face/)) {
                    // below we are trying to find matches to external link. E.g.
                    // @font-face {
                    //   // ...
                    //   src: local('Abel'), url(https://fonts.gstatic.com/s/abel/v6/UzN-iejR1VoXU2Oc-7LsbvesZW2xOQ-xsNqO47m55DA.woff2);
                    // }
                    //
                    // This regex will save extrnal link into first capture group
                    var fontUrlRegexp = /url\(["']?(.+?)["']?\)/;
                    // TODO: This needs to be changed to support multiple url declarations per font.
                    var fontUrlMatch = rule.cssText.match(fontUrlRegexp);

                    var externalFontUrl = (fontUrlMatch && fontUrlMatch[1]) || '';
                    var fontUrlIsDataURI = externalFontUrl.match(/^data:/);

                    if (fontUrlIsDataURI) {
                        // We should ignore data uri - they are already embedded
                        externalFontUrl = '';
                    }

                    if (externalFontUrl === 'about:blank') {
                        // no point trying to load this
                        externalFontUrl = '';
                    }

                    if (externalFontUrl) {
                        // okay, we are lucky. We can fetch this font later

                        //handle url if relative
                        if (externalFontUrl["startsWith"]('../')) {
                            externalFontUrl = sheetHref + '/../' + externalFontUrl
                        } else if (externalFontUrl["startsWith"]('./')) {
                            externalFontUrl = sheetHref + '/.' + externalFontUrl
                        }

                        fontsQueue.push(<font>{
                            text: rule.cssText,
                            // Pass url regex, so that once font is downladed, we can run `replace()` on it
                            fontUrlRegexp: fontUrlRegexp,
                            format: styles.getFontMimeTypeFromUrl(externalFontUrl),
                            url: externalFontUrl
                        });
                    } else {
                        // otherwise, use previous logic
                        css += rule.cssText + '\n';
                    }
                }
            }

            return css;
        }

        private static processFontQueue(queue: font[], css: string, cssLoadedCallback: (css: string) => void) {
            var style = this;

            if (queue.length > 0) {
                // load fonts one by one until we have anything in the queue:
                var font = queue.pop();
                processNext(font);
            } else {
                // no more fonts to load.
                cssLoadedCallback(css);
            }

            /**
             * 在这里通过网络下载字体文件，然后序列化为base64字符串，最后以URI的形式写入到SVG之中
            */
            function processNext(font: font) {
                // TODO: This could benefit from caching.
                var oReq = new XMLHttpRequest();
                oReq.addEventListener('load', fontLoaded);
                oReq.addEventListener('error', transferFailed);
                oReq.addEventListener('abort', transferFailed);
                oReq.open('GET', font.url);
                oReq.responseType = 'arraybuffer';
                oReq.send();

                function fontLoaded() {
                    // TODO: it may be also worth to wait until fonts are fully loaded before
                    // attempting to rasterize them. (e.g. use https://developer.mozilla.org/en-US/docs/Web/API/FontFaceSet )
                    var fontBits = oReq.response;
                    var fontInBase64 = DataExtensions.arrayBufferToBase64(fontBits);

                    updateFontStyle(font, fontInBase64);
                }

                function transferFailed(e) {
                    if (TypeScript.logging.outputWarning) {
                        console.warn('Failed to load font from: ' + font.url);
                        console.warn(e)
                    }
                    css += font.text + '\n';
                    style.processFontQueue(queue, css, cssLoadedCallback);
                }

                function updateFontStyle(font: font, fontInBase64: string) {
                    var dataUrl = `url("data:${font.format};base64,${fontInBase64}")`;
                    css += font.text.replace(font.fontUrlRegexp, dataUrl) + '\n';

                    // schedule next font download on next tick.
                    setTimeout(function () {
                        style.processFontQueue(queue, css, cssLoadedCallback);
                    }, 0);
                }
            }
        }

        private static getFontMimeTypeFromUrl(fontUrl: string): string {
            var extensions = Object.keys(supportedFormats);

            for (var i = 0; i < extensions.length; ++i) {
                var extension = extensions[i];
                // TODO: This is not bullet proof, it needs to handle edge cases...
                if (fontUrl.indexOf('.' + extension) > 0) {
                    return supportedFormats[extension];
                }
            }

            this.warnFontNotSupport(fontUrl);

            return 'application/octet-stream';
        }

        private static warnFontNotSupport(fontUrl: string) {
            // If you see this error message, you probably need to update code above.
            console.warn(`Unknown font format for ${fontUrl}; Fonts may not be working correctly`);
        }
    }

    const supportedFormats = {
        'woff2': 'font/woff2',
        'woff': 'font/woff',
        'otf': 'application/x-font-opentype',
        'ttf': 'application/x-font-ttf',
        'eot': 'application/vnd.ms-fontobject',
        'sfnt': 'application/font-sfnt',
        'svg': 'image/svg+xml'
    };

    class font {
        public text: string;
        public fontUrlRegexp: RegExp;
        public format: string;
        public url: string;
    }
}