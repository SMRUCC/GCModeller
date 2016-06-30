//======================================================================
// start Symbol object
//======================================================================
var Symbol = function(alph_index, scale, alphabet) {
  "use strict";
  //variable prototype
  this.symbol = alphabet.get_letter(alph_index);
  this.scale = scale;
  this.colour = alphabet.get_colour(alph_index);
};

Symbol.prototype.get_symbol = function() {
  "use strict";
  return this.symbol;
};

Symbol.prototype.get_scale = function() {
  "use strict";
  return this.scale;
};

Symbol.prototype.get_colour = function() {
  "use strict";
  return this.colour;
};

Symbol.prototype.toString = function() {
  "use strict";
  return this.symbol + " " + (Math.round(this.scale*1000)/10) + "%";
};

function compare_symbol(sym1, sym2) {
  "use strict";
  if (sym1.get_scale() < sym2.get_scale()) {
    return -1;
  } else if (sym1.get_scale() > sym2.get_scale()) {
    return 1;
  } else {
    return 0;
  }
}
//======================================================================
// end Symbol object
//======================================================================
