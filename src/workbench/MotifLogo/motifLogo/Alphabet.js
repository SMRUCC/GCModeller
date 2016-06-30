var Alphabet = function (alphabet, bg) {
  "use strict";
  var is_letter, is_prob, pos, letter, parts, i, freq;
  //variable prototype
  this.freqs = new Array();
  this.alphabet = new Array();
  this.letter_count = 0;
  //construct
  is_letter = /^\w$/;
  is_prob = /^((1(\.0+)?)|(0(\.\d+)?))$/;
  for (pos = 0; pos < alphabet.length; pos++) {
    letter = alphabet.charAt(pos);
    if (is_letter.test(letter)) {
      this.alphabet[this.letter_count] = letter.toUpperCase();
      this.freqs[this.letter_count] = -1;
      this.letter_count++;
    }
  }
  if (typeof bg !== "undefined") {
    parts = bg.split(/\s+/);
    for (i = 0, pos = 0; (i + 1) < parts.length; i += 2) {
      letter = parts[i];
      freq = parts[i+1];
      if (is_letter.test(letter) && is_prob.test(freq)) {
        letter = letter.toUpperCase();          //find the letter it matches
        for (;pos < this.letter_count; pos++) {
          if (this.alphabet[pos] == letter) {
            break;
          }
        }
        if (pos >= this.letter_count) {
          throw new Error("NOT_IN_ALPHABET");
        }
        this.freqs[pos] = (+freq);
      }
    }
  } else {
    //assume uniform background
    freq = 1.0 / this.letter_count;
    for (pos = 0; pos < this.letter_count; pos++) {
      this.freqs[pos] = freq;
    }
  }
};


Alphabet.prototype.get_ic = function() {
  "use strict";
  if (this.is_nucleotide()) {
    return 2;
  } else {
    return Math.log(20) / Math.LN2;
  }
};

Alphabet.prototype.get_size = function() {
  "use strict";
  return this.letter_count;
};

Alphabet.prototype.get_letter = function(alph_index) {
  "use strict";
  if (alph_index < 0 || alph_index >= this.letter_count) {
    throw new Error("BAD_ALPHABET_INDEX");
  }
  return this.alphabet[alph_index];
};

Alphabet.prototype.get_bg_freq = function(alph_index) {
  "use strict";
  if (alph_index < 0 || alph_index >= this.letter_count) {
    throw new Error("BAD_ALPHABET_INDEX");
  }
  if (this.freqs[alph_index] == -1) {
    throw new Error("BG_FREQ_NOT_SET");
  }
  return this.freqs[alph_index];
};

Alphabet.prototype.get_colour = function(alph_index) {
  "use strict";
  var red, blue, orange, green, yellow, purple, magenta, pink, turquoise;
  red = "rgb(204,0,0)";
  blue = "rgb(0,0,204)";
  orange = "rgb(255,179,0)";
  green = "rgb(0,128,0)";
  yellow = "rgb(255,255,0)";
  purple = "rgb(204,0,204)";
  magenta = "rgb(255,0,255)";
  pink = "rgb(255,204,204)";
  turquoise = "rgb(51,230,204)";
  if (alph_index < 0 || alph_index >= this.letter_count) {
    throw new Error("BAD_ALPHABET_INDEX");
  }
  if (this.is_nucleotide()) {
    switch (this.alphabet[alph_index]) {
      case "A":
        return red;
      case "C":
        return blue;
      case "G":
        return orange;
      case "T":
        return green;
      default:
        throw new Error("Invalid nucleotide letter");
    }
  } else {
    switch (this.alphabet[alph_index]) {
      case "A":
      case "C":
      case "F":
      case "I":
      case "L":
      case "V":
      case "W":
      case "M":
        return blue;
      case "N":
      case "Q":
      case "S":
      case "T":
        return green;
      case "D":
      case "E":
        return magenta;
      case "K":
      case "R":
        return red;
      case "H":
        return pink;
      case "G":
        return orange;
      case "P":
        return yellow;
      case "Y":
        return turquoise;
      default:
        throw new Error("Invalid protein letter");
    }
  }
  return "black";
};

Alphabet.prototype.is_ambig = function(alph_index) {
  "use strict";
  if (alph_index < 0 || alph_index >= this.letter_count) {
    throw new Error("BAD_ALPHABET_INDEX");
  }
  if (this.is_nucleotide()) {
    return ("ACGT".indexOf(this.alphabet[alph_index]) == -1);
  } else {
    return ("ACDEFGHIKLMNPQRSTVWY".indexOf(this.alphabet[alph_index]) == -1);
  }
};

Alphabet.prototype.get_index = function(letter) {
  "use strict";
  var i;
  for (i = 0; i < this.letter_count; i++) {
    if (this.alphabet[i] == letter.toUpperCase()) {
      return i;
    }
  }
  throw new Error("UNKNOWN_LETTER");
};

Alphabet.prototype.is_nucleotide = function() {
  "use strict";
  //TODO basic method, make better
  if (this.letter_count < 20) {
    return true;
  }
  return false;
};

Alphabet.prototype.toString = function() {
  "use strict";
  return (this.is_nucleotide() ? "Nucleotide" : "Protein") + 
    " Alphabet " + (this.alphabet.join(""));
};