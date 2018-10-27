/// <reference path="SvgUtils.ts"/>
/// <reference path="Canvas/Canvas.ts"/>
/// <reference path="Canvas/Pen.ts"/>

/**
 * 提供类似于VB.NET之中的Graphics对象的模拟
*/
class Graphics {

    private svg: SVGElement;
    private container: HTMLElement;

    /**
     * The css z-index layer order
    */
    private z: number;

    /**
     * 创建一个SVG画布对象
     * 
     * @param div div id
    */
    public constructor(div: string) {
        this.svg = SvgUtils.svgNode("svg", { "version": "1.1" });
        this.container = document.getElementById(div);
        this.container.appendChild(this.svg);
    }

    /**
     * Set the size value of the svg canvas
    */
    public size(width: number, height: number): Graphics {
        this.svg.setAttribute("width", width.toString() + "px");
        this.svg.setAttribute("height", height.toString() + "px");
        return this;
    }

    /**
     * The viewBox attribute allows you to specify that a given set of graphics stretch to 
     * fit a particular container element.
     *
     * The value of the viewBox attribute is a list of four numbers min-x, min-y, width and 
     * height, separated by whitespace and/or a comma, which specify a rectangle in user 
     * space which should be mapped to the bounds of the viewport established by the given 
     * element, taking into account attribute preserveAspectRatio.
     *
     * Negative values for width or height are not permitted and a value of zero disables 
     * rendering of the element.
    */
    public viewBox(minX: number, minY: number, width: number, height: number): Graphics {
        var box: string = `${minX} ${minY} ${width} ${height}`;
        this.svg.setAttribute("viewBox", box);
        return this;
    }

    /**
     * Draw a basic svg line shape
     * 
     * @param pen Defines the line border: color and line width
    */
    public drawLine(pen: Canvas.Pen, a: Canvas.Point, b: Canvas.Point,
        id: string = null,
        className: string = null): Graphics {

        var attrs = {
            "x1": a.x.toString(),
            "y1": a.y.toString(),
            "x2": b.x.toString(),
            "y2": b.y.toString(),
            "z-index": ++this.z
        };

        if (id) attrs["id"] = id;
        if (className) attrs["class"] = className;

        var node = pen.Styling(SvgUtils.svgNode("line", attrs));
        this.svg.appendChild(node);

        return this;
    }

    public drawCircle(center: Canvas.Point, radius: number,
        border: Canvas.Pen = Canvas.Pens.Black(),
        fill: Canvas.Color = null,
        id: string = null,
        className: string = null): Graphics {

        var attrs = {
            "cx": center.x.toString(),
            "cy": center.y.toString(),
            "r": radius,
            "z-index": ++this.z
        };

        if (id) attrs["id"] = id;
        if (className) attrs["class"] = className;
        if (fill) attrs["fill"] = fill.ToHtmlColor();

        var node = border.Styling(SvgUtils.svgNode("circle", attrs));
        this.svg.appendChild(node);

        return this;
    }

    /**
     * The ``<ellipse>`` element is an SVG basic shape, used to create ellipses 
     * based on a center coordinate, and both their x and y radius.
     * 
     * @description Note: Ellipses are unable to specify the exact orientation of 
     * the ellipse (if, for example, you wanted to draw an ellipse tilted at a 45 
     * degree angle), but it can be rotated by using the ``transform`` attribute.
    */
    public drawEllipse(center: Canvas.Point, rx: number, ry: number,
        border: Canvas.Pen = Canvas.Pens.Black(),
        fill: Canvas.Color = null,
        id: string = null,
        className: string = null): Graphics {

        var attrs = {
            "cx": center.x,
            "cy": center.y,
            "rx": rx,
            "ry": ry,
            "z-index": ++this.z
        };

        if (id) attrs["id"] = id;
        if (className) attrs["class"] = className;
        if (fill) attrs["fill"] = fill.ToHtmlColor();

        var node = border.Styling(SvgUtils.svgNode("ellipse", attrs));
        this.svg.appendChild(node);

        return this;
    }

    /**
     * Draw a basic svg rectangle shape
    */
    public drawRectangle(rect: Canvas.Rectangle,
        border: Canvas.Pen = Canvas.Pens.Black(),
        fill: Canvas.Color = null,
        id: string = null,
        className: string = null): Graphics {

        var attrs = {
            "x": rect.left.toString(),
            "y": rect.top.toString(),
            "width": rect.width.toString(),
            "height": rect.height.toString(),
            "z-index": ++this.z
        };

        if (id) attrs["id"] = id;
        if (className) attrs["class"] = className;
        if (fill) attrs["fill"] = fill.ToHtmlColor();

        var node = border.Styling(SvgUtils.svgNode("rect", attrs));
        this.svg.appendChild(node);

        return this;
    }

    /**
     * The ``<path>`` SVG element is the generic element to define a shape. 
     * All the basic shapes can be created with a path element.
    */
    public drawPath(path: Canvas.Path,
        border: Canvas.Pen = Canvas.Pens.Black(),
        fill: Canvas.Color = null,
        id: string = null,
        className: string = null): Graphics {

        var attrs = {
            "d": path.d,
            "z-index": ++this.z
        };

        if (id) attrs["id"] = id;
        if (className) attrs["class"] = className;
        if (fill) attrs["fill"] = fill.ToHtmlColor();

        var node = border.Styling(SvgUtils.svgNode("path", attrs));
        this.svg.appendChild(node);

        return this;
    }
}