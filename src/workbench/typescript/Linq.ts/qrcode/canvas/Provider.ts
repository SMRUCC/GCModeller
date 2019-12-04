module TypeScript.QRCode.canvas {

    export interface IDrawingProvider {
        draw(oQRCode);
        clear();
    }
}