namespace CanvasHelper.saveSvgAsPng {

    export const xlink: string = "http://www.w3.org/1999/xlink";

    export function isElement(obj: any): boolean {
        return obj instanceof HTMLElement || obj instanceof SVGElement;
    }

    export function requireDomNode(el: any): any {
        if (!isElement(el)) {
            throw new Error('an HTMLElement or SVGElement is required; got ' + el);
        } else {
            return el;
        }
    }

    /**
     * 判断所给定的url指向的资源是否是来自于外部域的资源？
    */
    export function isExternal(url: string): boolean {
        return url && url.lastIndexOf('http', 0) == 0 && url.lastIndexOf(window.location.host) == -1;
    }

    export function inlineImages(el: SVGSVGElement, callback: Delegate.Sub) {
        requireDomNode(el);

        var images: NodeListOf<SVGImageElement> = el.querySelectorAll('image');
        var left: number = images.length;
        var checkDone = function (count: number) {
            if (count === 0) {
                callback();
            }
        };

        checkDone(left);

        for (var i: number = 0; i < images.length; i++) {
            left = renderInlineImage(images[i], left, checkDone);
        }
    }

    function renderInlineImage(image: SVGImageElement, left: number, checkDone: (left: number) => void): number {
        var href = image.getAttributeNS(saveSvgAsPng.xlink, "href");

        if (href) {
            if (typeof href != "string") {
                href = (<any>href).value;
            }

            if (isExternal(href)) {
                if (TypeScript.logging.outputWarning) {
                    console.warn("Cannot render embedded images linking to external hosts: " + href);
                }
                return;
            }
        }

        var canvas: HTMLCanvasElement = <any>$ts('<canvas>');
        var ctx = canvas.getContext('2d');
        var img = new Image();

        img.crossOrigin = "anonymous";
        href = href || image.getAttribute('href');

        if (href) {
            img.src = href;
            img.onload = function () {
                canvas.width = img.width;
                canvas.height = img.height;
                ctx.drawImage(img, 0, 0);
                image.setAttributeNS(saveSvgAsPng.xlink, "href", canvas.toDataURL('image/png'));
                left--;
                checkDone(left);
            }
            img.onerror = function () {
                console.error("Could not load " + href);
                left--;
                checkDone(left);
            }
        } else {
            left--;
            checkDone(left);
        }

        return left;
    }

    /**
     * 获取得到width或者height的值
    */
    export function getDimension(el: SVGSVGElement, clone: SVGSVGElement, dim: string): number {
        var v: string = (el.viewBox && el.viewBox.baseVal && el.viewBox.baseVal[dim]) ||
            (clone.getAttribute(dim) !== null && !clone.getAttribute(dim).match(/%$/) && parseInt(clone.getAttribute(dim))) ||
            el.getBoundingClientRect()[dim] ||
            parseInt(clone.style[dim]) ||
            parseInt(window.getComputedStyle(el).getPropertyValue(dim));

        if (typeof v === 'undefined' || v === null) {
            return 0;
        } else {
            var val: number = parseFloat(v);
            return isNaN(val) ? 0 : val;
        }
    }

    export function reEncode(data: string) {
        data = encodeURIComponent(data);
        data = data.replace(/%([0-9A-F]{2})/g, function (match, p1) {
            var c = String.fromCharCode(<any>('0x' + p1));
            return c === '%' ? '%25' : c;
        });

        return decodeURIComponent(data);
    }
}