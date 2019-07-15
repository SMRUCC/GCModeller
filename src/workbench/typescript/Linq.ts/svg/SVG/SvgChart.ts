abstract class SvgChart {

    public size: [number, number];
    public margin: Canvas.Margin;

    public get width(): number {
        return this.size["0"];
    }

    public get height(): number {
        return this.size["1"];
    }

    public constructor(
        size: Canvas.Size | number[] = [960, 600],
        margin: Canvas.Margin = <Canvas.Margin>{
            top: 20, right: 20, bottom: 30, left: 40
        }) {

        if (!Array.isArray(size)) {
            this.size = [size.width, size.height];
        } else {
            this.size = [size[0], size[1]];
        }

        this.margin = margin;
    }
}