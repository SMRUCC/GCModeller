module network {

    export class dotCanvasSettings {
        public l: number;
        public z: number;
        public o: number;
        public c: string;
        public n: number;
    }

    function getTag(tagName: string): Element {
        return document.getElementsByTagName(tagName)[0];
    }

    function canvasResize() {
        r = uCanvas.width = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
        n = uCanvas.height = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    }

    function b() {
        var w = [f].concat(dots);
        var x, v, A, B, z, y;

        uContext.clearRect(0, 0, r, n);
        dots.forEach(
            function (i) {
                i.x += i.xa, i.y += i.ya, i.xa *= i.x > r || i.x < 0 ? -1 : 1, i.ya *= i.y > n || i.y < 0 ? -1 : 1, uContext.fillRect(i.x - 0.5, i.y - 0.5, 1, 1);
                for (v = 0; v < w.length; v++) {
                    x = w[v];
                    if (i !== x && null !== x.x && null !== x.y) {
                        B = i.x - x.x, z = i.y - x.y, y = B * B + z * z;
                        y < x.max && (x === f && y >= x.max / 2 && (i.x -= 0.03 * B, i.y -= 0.03 * z), A = (x.max - y) / x.max, uContext.beginPath(), uContext.lineWidth = A / 2, uContext.strokeStyle = "rgba(" + setting.c + "," + (A + 0.2) + ")", uContext.moveTo(i.x, i.y), uContext.lineTo(x.x, x.y), uContext.stroke())
                    }
                } w.splice(w.indexOf(i), 1)
            }), runFrame(b)
    }

    var uCanvas: HTMLCanvasElement = document.createElement("canvas");
    var uContext: CanvasRenderingContext2D = uCanvas.getContext("2d");
    var f: dot = <dot>{
        x: null, y: null, max: 20000
    }
    var r: number;
    var dots: dot[] = [];
    var n: number;
    var setting: dotCanvasSettings;
    var runFrame: (callback: FrameRequestCallback) => number

    function defaultCallback(callback: FrameRequestCallback): number {
        window.setTimeout(callback, 1000 / 45);
        return 0;
    }

    function registerDevice() {
        runFrame = window.requestAnimationFrame ||
            window.webkitRequestAnimationFrame ||
            (<any>window).mozRequestAnimationFrame ||
            (<any>window).oRequestAnimationFrame ||
            (<any>window).msRequestAnimationFrame ||
            defaultCallback;

        window.onresize = canvasResize;
        window.onmousemove = function (i: MouseEvent) {
            i = i || <MouseEvent>window.event;
            f.x = i.clientX;
            f.y = i.clientY;
        };
        window.onmouseout = function () {
            f.x = null;
            f.y = null;
        };
    }

    export function run(settings: dotCanvasSettings = <dotCanvasSettings>{
        l: n,
        z: -1,
        o: 0.9,
        c: "0,104,183",
        n: 399
    }) {

        var c = "c_n" + settings.l;

        setting = settings;
        uCanvas.id = c;
        uCanvas.style.cssText = `position:fixed; top:0; left:0; z-index: ${settings.z}; opacity: ${settings.o}`;
        getTag("body").appendChild(uCanvas);

        canvasResize();
        registerDevice();

        for (var p: number = 0; settings.n > p; p++) {
            var h = Math.random() * r;
            var g = Math.random() * n;
            var q = 2 * Math.random() - 1;
            var d = 2 * Math.random() - 1;

            dots.push(<dot>{
                x: h,
                y: g,
                xa: q,
                ya: d,
                max: 10000
            })
        }

        setTimeout(function () { b() }, 100)
    }

    class dot {
        public x: number;
        public y: number;
        public xa: number;
        public ya: number;
        public max: number;
    }
}

network.run();