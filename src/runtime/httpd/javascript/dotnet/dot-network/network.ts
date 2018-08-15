module network {

    function readAttr(x: HTMLElement, name: string, Default: string): string {
        return x.getAttribute(name) || Default;
    }

    function getTag(tagName: string): NodeListOf<Element> {
        return document.getElementsByTagName(tagName);
    }

    function load(): canvasSettings {
        var nodes = getTag("script");
        var n: number = nodes.length;
        var last = nodes[n - 1];

        return <canvasSettings>{
            l: n,
            z: <number><any>readAttr(<HTMLElement>last, "zIndex", "-1"),
            o: <number><any>readAttr(<HTMLElement>last, "opacity", "0.5"),
            c: readAttr(<HTMLElement>last, "color", "0,0,0"),
            n: <number><any>readAttr(<HTMLElement>last, "count", "99")
        }
    }

    function k() {
        r = u.width = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
        n = u.height = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    }

    function b() {
        e.clearRect(0, 0, r, n);
        var w = [f].concat(t);
        var x, v, A, B, z, y; t.forEach(
            function (i) {
                i.x += i.xa, i.y += i.ya, i.xa *= i.x > r || i.x < 0 ? -1 : 1, i.ya *= i.y > n || i.y < 0 ? -1 : 1, e.fillRect(i.x - 0.5, i.y - 0.5, 1, 1);
                for (v = 0; v < w.length; v++) {
                    x = w[v];
                    if (i !== x && null !== x.x && null !== x.y) {
                        B = i.x - x.x, z = i.y - x.y, y = B * B + z * z;
                        y < x.max && (x === f && y >= x.max / 2 && (i.x -= 0.03 * B, i.y -= 0.03 * z), A = (x.max - y) / x.max, e.beginPath(), e.lineWidth = A / 2, e.strokeStyle = "rgba(" + s.c + "," + (A + 0.2) + ")", e.moveTo(i.x, i.y), e.lineTo(x.x, x.y), e.stroke())
                    }
                } w.splice(w.indexOf(i), 1)
            }), m(b)
    }

    var u = document.createElement("canvas");
    var e = u.getContext("2d");
    var f = {
        x: null, y: null, max: 20000
    }
    var r;
    var t: dot[] = [];
    var n;
    var s: canvasSettings = load();
    var m = window.requestAnimationFrame || window.webkitRequestAnimationFrame || (<any>window).mozRequestAnimationFrame || (<any>window).oRequestAnimationFrame || (<any>window).msRequestAnimationFrame || function (i) {
        window.setTimeout(i, 1000 / 45)
    }

    export function run() {
        var c = "c_n" + s.l;
        var a = Math.random;

        u.id = c;
        u.style.cssText = "position:fixed;top:0;left:0;z-index:" + s.z + ";opacity:" + s.o;
        getTag("body")[0].appendChild(u);

        k(), window.onresize = k;

        window.onmousemove = function (i: MouseEvent) {
            i = i || <MouseEvent>window.event;
            f.x = i.clientX;
            f.y = i.clientY;
        }

        window.onmouseout = function () {
            f.x = null;
            f.y = null;
        };

        for (var p = 0; s.n > p; p++) {
            var h = a() * r, g = a() * n, q = 2 * a() - 1, d = 2 * a() - 1;
            t.push(<dot>{ x: h, y: g, xa: q, ya: d, max: 10000 })
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

class canvasSettings {
    public l: number;
    public z: number;
    public o: number;
    public c: string;
    public n: number;
}