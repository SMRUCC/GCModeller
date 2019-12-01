/// <reference path="../SvgUtils.ts"/>

/**
 * SVG画布元素
*/
namespace Canvas {

    /**
     * CSS style object model
    */
    export interface ICSSStyle {

        /**
         * Apply CSS style to a given svg node element
         * 
         * @param node a given svg document node object
        */
        Styling(node: SVGElement): SVGElement;
        /**
         * Generate css style string value from this 
         * css style object model.
        */
        CSSStyle(): string;
    }

    /**
     * The object location data model 
    */
    export class Point {

        public constructor(public x: number, public y: number) {}

        public toString(): string {
            return `[${this.x}, ${this.y}]`;
        }

        /**
         * Calculate the 2d distance to other point from this point.
        */
        public dist(p2: Point): number {
            var dx: number = p2.x - this.x;
            var dy: number = p2.y - this.y;

            return dx * dx + dy * dy;
        }

        /**
         * Is this point equals to a given point by numeric value equals 
         * of the ``x`` and ``y``?
        */
        public Equals(p2: Point): boolean {
            return this.x == p2.x && this.y == p2.y;
        }
    }

    /**
     * 表示一个矩形区域的大小
    */
    export class Size {

        /**
         * @param width 宽度
         * @param height 高度
        */
        public constructor(public width: number, public height: number) {}

        public toString(): string {
            return `[${this.width}, ${this.height}]`;
        }
    }

    /**
     * 表示一个二维平面上的矩形区域
    */
    export class Rectangle extends Size {

        public left: number;
        public top: number;

        public constructor(x: number, y: number, width: number, height: number) {
            super(width, height);

            this.left = x;
            this.top = y;
        }

        public Location(): Point {
            return new Point(this.left, this.top);
        }

        public Size(): Size {
            return new Size(this.width, this.height);
        }

        public toString(): string {
            return `Size: ${this.Size().toString()} @ ${this.Location().toString()}`;
        }
    }

    export class Margin {

        public get horizontal(): number {
            return this.left + this.right;
        }

        public get vertical(): number {
            return this.top + this.bottom;
        }

        public constructor(public top: number, public right: number, public bottom: number, public left: number) {
        }

        public static Object(obj: { top: number, right: number, bottom: number, left: number } | number[]): Margin {
            if (Array.isArray(obj)) {
                return new Margin(obj[0], obj[1], obj[2], obj[3]);
            } else {
                return new Margin(obj.top, obj.right, obj.bottom, obj.left);
            }
        }

        public toString(): string {
            return `[${this.top}, ${this.right}, ${this.bottom}, ${this.left}]`;
        }
    }
}