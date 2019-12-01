module TypeScript.QRCode.QRMath {
    export function glog(n) {
        if (n < 1) { throw new Error("glog(" + n + ")"); }
        return QRMath.LOG_TABLE[n];
    }

    export function gexp(n) {
        while (n < 0) { n += 255; }
        while (n >= 256) { n -= 255; }
        return QRMath.EXP_TABLE[n];
    }

    export const EXP_TABLE = expTable();
    export const LOG_TABLE = logTable()

    function expTable() {
        let exp = new Array(256);

        for (var i = 0; i < 8; i++) { QRMath.EXP_TABLE[i] = 1 << i; }
        for (var i = 8; i < 256; i++) { QRMath.EXP_TABLE[i] = QRMath.EXP_TABLE[i - 4] ^ QRMath.EXP_TABLE[i - 5] ^ QRMath.EXP_TABLE[i - 6] ^ QRMath.EXP_TABLE[i - 8]; }

        return exp
    }

    function logTable() {
        let log = new Array(256)
        for (var i = 0; i < 255; i++) { QRMath.LOG_TABLE[QRMath.EXP_TABLE[i]] = i; }

        return log;
    }

}