module SvgUtils {

    /**
     * https://stackoverflow.com/questions/5623838/rgb-to-hex-and-hex-to-rgb
     * 
     * @param c The rgb color component numeric value
    */
    export function componentToHex(c): string {
        var hex = c.toString(16);
        return hex.length == 1 ? "0" + hex : hex;
    }

    export const HTML5svgFeature: string = "http://www.w3.org/TR/SVG2/feature#GraphicsAttribute";

    /**
     * 测试当前的浏览器是否支持HTML5的高级特性
    */
    export function hasSVG2Feature(): boolean {
        return document.implementation.hasFeature(HTML5svgFeature, "2.0");
    }

    /**
     * https://stackoverflow.com/questions/20539196/creating-svg-elements-dynamically-with-javascript-inside-html
     * 
     * @param n The svg node name
     * @param v The svg node attributes
     * 
     * @description 
     * 
     * ### HTML 5, inline SVG, and namespace awareness for SVG DOM
     * > https://stackoverflow.com/questions/23319537/html-5-inline-svg-and-namespace-awareness-for-svg-dom
     *
     * HTML5 defines ``HTML``, ``XHTML`` and the ``DOM``.
     * The ``DOM`` is namespace aware. When you use ``DOM`` methods you must take into account which namespace 
     * each element is in, but the default is the ``HTML`` (http://www.w3.org/1999/xhtml) namespace.
     * ``HTML`` and ``XHTML`` are serializations that are converted into ``DOMs`` by parsing.
     * ``XHTML`` is namespace aware and ``XHTML`` documents apply namespaces according to the rules of ``XML``, 
     * so all namespaces must be assigned to each element explicitly. ``XHTML`` is converted to a ``DOM`` using 
     * an ``XML`` parser.
     * 
     * ``HTML`` is also namespace aware, but namespaces are assigned implicitly. HTML is converted to a DOM using 
     * an HTML parser, which knows which elements go in which namespace. That is, it knows that <div> goes 
     * in the http://www.w3.org/1999/xhtml namespace and that <svg> goes in the http://www.w3.org/2000/svg 
     * namespace. Elements like <script> can go in either the http://www.w3.org/1999/xhtml or the http://www.w3.org/2000/svg 
     * namespace depending on the context in which they appear in the HTML code.
     * The HTML parser knows about HTML elements, SVG elements, and MathML elements and no others. There is no 
     * option to use elements from other namespaces, neither implicitly nor explicitly. 
     * That is, xmlns attributes have no effect.
    */
    export function svgNode(n: string, v: any = null): SVGElement {
        var node = document.createElementNS("http://www.w3.org/2000/svg", n);
        var name = "";

        if (v) {
            for (var p in v) {
                name = p.replace(/[A-Z]/g, function (m, p, o, s) {
                    return "-" + m.toLowerCase();
                });
                node.setAttributeNS(null, name, v[p]);
            }
        }

        return node;
    }
}
