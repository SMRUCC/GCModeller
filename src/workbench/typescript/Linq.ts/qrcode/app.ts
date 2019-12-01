/// <reference path="../../build/linq.d.ts" />

/**
 * @fileoverview
 * - Using the 'QRCode for Javascript library'
 * - Fixed dataset of 'QRCode for Javascript library' for support full-spec.
 * - this library has no dependencies.
 * 
 * @author davidshimjs
 * @see <a href="http://www.d-project.com/" target="_blank">http://www.d-project.com/</a>
 * @see <a href="http://jeromeetienne.github.com/jquery-qrcode/" target="_blank">http://jeromeetienne.github.com/jquery-qrcode/</a>
*/

//---------------------------------------------------------------------
// QRCode for JavaScript
//
// Copyright (c) 2009 Kazuhiko Arase
//
// URL: http://www.d-project.com/
//
// Licensed under the MIT license:
//   http://www.opensource.org/licenses/mit-license.php
//
// The word "QR Code" is registered trademark of 
// DENSO WAVE INCORPORATED
//   http://www.denso-wave.com/qrcode/faqpatent-e.html
//
//---------------------------------------------------------------------

/**
 * @class QRCode
 * @constructor
 * @example 
 * new QRCode(document.getElementById("test"), "http://jindo.dev.naver.com/collie");
 *
 * @example
 * var oQRCode = new QRCode("test", {
 *    text : "http://naver.com",
 *    width : 128,
 *    height : 128
 * });
 * 
 * oQRCode.clear(); // Clear the QRCode.
 * oQRCode.makeCode("http://map.naver.com"); // Re-create the QRCode.
 *
 * @param {HTMLElement|String} el target element or 'id' attribute of element.
 * @param {Object|String} vOption
 * @param {String} vOption.text QRCode link data
 * @param {Number} [vOption.width=256]
 * @param {Number} [vOption.height=256]
 * @param {String} [vOption.colorDark="#000000"]
 * @param {String} [vOption.colorLight="#ffffff"]
 * @param {QRCode.CorrectLevel} [vOption.correctLevel=QRCode.CorrectLevel.H] [L|M|Q|H] 
*/
class QRCode {
    public constructor(el, vOption) {
        this._htOption = {
            width: 256,
            height: 256,
            typeNumber: 4,
            colorDark: "#000000",
            colorLight: "#ffffff",
            correctLevel: QRErrorCorrectLevel.H
        };

        if (typeof vOption === 'string') {
            vOption = {
                text: vOption
            };
        }

        // Overwrites options
        if (vOption) {
            for (var i in vOption) {
                this._htOption[i] = vOption[i];
            }
        }

        if (typeof el == "string") {
            el = document.getElementById(el);
        }

        if (this._htOption.useSVG) {
            Drawing = svgDrawer;
        }

        this._android = _getAndroid();
        this._el = el;
        this._oQRCode = null;
        this._oDrawing = new Drawing(this._el, this._htOption);

        if (this._htOption.text) {
            this.makeCode(this._htOption.text);
        }
    };

	/**
	 * Make the QRCode
	 * 
	 * @param {String} sText link data
	 */
    makeCode(sText) {
        this._oQRCode = new QRCodeModel(_getTypeNumber(sText, this._htOption.correctLevel), this._htOption.correctLevel);
        this._oQRCode.addData(sText);
        this._oQRCode.make();
        this._el.title = sText;
        this._oDrawing.draw(this._oQRCode);
        this.makeImage();
    };

	/**
	 * Make the Image from Canvas element
	 * - It occurs automatically
	 * - Android below 3 doesn't support Data-URI spec.
	 * 
	 * @private
	 */
    makeImage() {
        if (typeof this._oDrawing.makeImage == "function" && (!this._android || this._android >= 3)) {
            this._oDrawing.makeImage();
        }
    };

	/**
	 * Clear the QRCode
	 */
    clear() {
        this._oDrawing.clear();
    };

	/**
	 * @name QRCode.CorrectLevel
	 */
    CorrectLevel = QRErrorCorrectLevel;
}