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

        public x: number;
        public y: number;

        public constructor(x: number, y: number) {
            this.x = x;
            this.y = y;
        }

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
         * 宽度
        */
        public width: number;
        /**
         * 高度
        */
        public height: number;

        public constructor(width: number, height: number) {
            this.width = width;
            this.height = height;
        }

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

        public top: number;
        public right: number;
        public bottom: number;
        public left: number;

        public get horizontal(): number {
            return this.left + this.right;
        }

        public get vertical(): number {
            return this.top + this.bottom;
        }

        public constructor(top: number, right: number, bottom: number, left: number) {
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.left = left;
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