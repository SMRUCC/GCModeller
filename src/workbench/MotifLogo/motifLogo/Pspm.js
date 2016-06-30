//======================================================================
// start Pspm object
//======================================================================
var Pspm = function(matrix, name, ltrim, rtrim, nsites, evalue) {
  "use strict";
  var row, col, data, row_sum, delta, evalue_re;
  if (typeof name !== "string") {
    name = "";
  }
  this.name = name;
  //construct
  if (matrix instanceof Pspm) {
    // copy constructor
    this.alph_length = matrix.alph_length;
    this.motif_length = matrix.motif_length;
    this.name = matrix.name;
    this.nsites = matrix.nsites;
    this.evalue = matrix.evalue;
    this.ltrim = matrix.ltrim;
    this.rtrim = matrix.rtrim;
    this.pspm = [];
    for (row = 0; row < matrix.motif_length; row++) {
      this.pspm[row] = [];
      for (col = 0; col < matrix.alph_length; col++) {
        this.pspm[row][col] = matrix.pspm[row][col];
      }
    }
  } else {
    // check parameters
    if (typeof ltrim === "undefined") {
      ltrim = 0;
    } else if (typeof ltrim !== "number" || ltrim % 1 !== 0 || ltrim < 0) {
      throw new Error("ltrim must be a non-negative integer, got: " + ltrim);
    }
    if (typeof rtrim === "undefined") {
      rtrim = 0;
    } else if (typeof rtrim !== "number" || rtrim % 1 !== 0 || rtrim < 0) {
      throw new Error("rtrim must be a non-negative integer, got: " + rtrim);
    }
    if (typeof nsites !== "undefined") {
      if (typeof nsites !== "number" || nsites <= 0) {
        throw new Error("nsites must be a positive number, got: " + nsites);
      }
    }
    if (typeof evalue !== "undefined") {
      if (typeof evalue === "number") {
        if (evalue < 0) {
          throw new Error("evalue must be a non-negative number, got: " + evalue);
        }
      } else if (typeof evalue === "string") {
        evalue_re = /^((?:[+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?)|inf)$/;
        if (!evalue_re.test(evalue)) {
          throw new Error("evalue must be a non-negative number, got: " + evalue);
        }
      } else {
        throw new Error("evalue must be a non-negative number, got: " + evalue);
      }
    }
    // set properties
    this.name = name;
    this.nsites = nsites;
    this.evalue = evalue;
    this.ltrim = ltrim;
    this.rtrim = rtrim;
    if (typeof matrix === "string") {
      // string constructor
      data = parse_pspm_string(matrix);
      this.alph_length = data["alph_length"];
      this.motif_length = data["motif_length"];
      this.pspm = data["pspm"];
      if (typeof this.evalue === "undefined") {
        if (typeof data["evalue"] !== "undefined") {
          this.evalue = data["evalue"];
        } else {
          this.evalue = 0;
        }
      }
      if (typeof this.nsites === "undefined") {
        if (typeof data["nsites"] === "number") {
          this.nsites = data["nsites"];
        } else {
          this.nsites = 20;
        }
      }
    } else {
      // assume pspm is a nested array
      this.motif_length = matrix.length;
      this.alph_length = (matrix.length > 0 ? matrix[0].length : 0);
      if (typeof this.nsites === "undefined") {
        this.nsites = 20;
      }
      if (typeof this.evalue === "undefined") {
        this.evalue = 0;
      }
      this.pspm = [];
      // copy pspm and check
      for (row = 0; row < this.motif_length; row++) {
        if (this.alph_length != matrix[row].length) {
          throw new Error("COLUMN_MISMATCH");
        }
        this.pspm[row] = [];
        row_sum = 0;
        for (col = 0; col < this.alph_length; col++) {
          this.pspm[row][col] = matrix[row][col];
          row_sum += this.pspm[row][col];
        }
        delta = 0.1;
        if (isNaN(row_sum) || (row_sum > 1 && (row_sum - 1) > delta) || 
            (row_sum < 1 && (1 - row_sum) > delta)) {
          throw new Error("INVALID_SUM");
        }
      }
    }
  }
};

Pspm.prototype.copy = function() {
  "use strict";
  return new Pspm(this);
};

Pspm.prototype.reverse_complement = function(alphabet) {
  "use strict";
  var x, y, temp, a_index, c_index, g_index, t_index, i, row, temp_trim;
  if (this.alph_length != alphabet.get_size()) {
    throw new Error("ALPHABET_MISMATCH");
  }
  if (!alphabet.is_nucleotide()) {
    throw new Error("NO_PROTEIN_RC");
  }
  //reverse
  x = 0;
  y = this.motif_length-1;
  while (x < y) {
    temp = this.pspm[x];
    this.pspm[x] = this.pspm[y];
    this.pspm[y] = temp;
    x++;
    y--;
  }
  //complement
  a_index = alphabet.get_index("A");
  c_index = alphabet.get_index("C");
  g_index = alphabet.get_index("G");
  t_index = alphabet.get_index("T");
  for (i = 0; i < this.motif_length; i++) {
    row = this.pspm[i];
    //swap A and T
    temp = row[a_index];
    row[a_index] = row[t_index];
    row[t_index] = temp;
    //swap C and G
    temp = row[c_index];
    row[c_index] = row[g_index];
    row[g_index] = temp;
  }
  //swap triming
  temp_trim = this.ltrim;
  this.ltrim = this.rtrim;
  this.rtrim = temp_trim;
  //note that ambigs are ignored because they don't effect motifs
  return this; //allow function chaining...
};

Pspm.prototype.get_stack = function(position, alphabet) {
  "use strict";
  var row, stack_ic, alphabet_ic, stack, i, sym;
  if (this.alph_length != alphabet.get_size()) {
    throw new Error("ALPHABET_MISMATCH");
  }
  row = this.pspm[position];
  stack_ic = this.get_stack_ic(position, alphabet);
  alphabet_ic = alphabet.get_ic();
  stack = [];
  for (i = 0; i < this.alph_length; i++) {
    if (alphabet.is_ambig(i)) {
      continue;
    }
    sym = new Symbol(i, row[i]*stack_ic/alphabet_ic, alphabet);
    if (sym.get_scale() <= 0) {
      continue;
    }
    stack.push(sym);
  }
  stack.sort(compare_symbol);
  return stack;
};

Pspm.prototype.get_stack_ic = function(position, alphabet) {
  "use strict";
  var row, H, i;
  if (this.alph_length != alphabet.get_size()) {
    throw new Error("ALPHABET_MISMATCH");
  }
  row = this.pspm[position];
  H = 0;
  for (i = 0; i < this.alph_length; i++) {
    if (alphabet.is_ambig(i)) {
      continue;
    }
    if (row[i] === 0) {
      continue;
    }
    H -= (row[i] * (Math.log(row[i]) / Math.LN2));
  }
  return alphabet.get_ic() - H;
};

Pspm.prototype.get_error = function(alphabet) {
  "use strict";
  var asize;
  if (this.nsites === 0) {
    return 0;
  }
  if (alphabet.is_nucleotide()) {
    asize = 4;
  } else {
    asize = 20;
  }
  return (asize-1) / (2 * Math.log(2)*this.nsites);
};

Pspm.prototype.get_motif_length = function() {
  "use strict";
  return this.motif_length;
};

Pspm.prototype.get_alph_length = function() {
  "use strict";
  return this.alph_length;
};

Pspm.prototype.get_left_trim = function() {
  "use strict";
  return this.ltrim;
};

Pspm.prototype.get_right_trim = function() {
  "use strict";
  return this.rtrim;
};

Pspm.prototype.as_pspm = function() {
  "use strict";
  var out, row, col;
  out = "letter-probability matrix: alength= " + this.alph_length + 
      " w= " + this.motif_length + " nsites= " + this.nsites + 
      " E= " + (typeof this.evalue === "number" ? 
          this.evalue.toExponential() : this.evalue) + "\n";
  for (row = 0; row < this.motif_length; row++) {
    for (col = 0; col < this.alph_length; col++) {
      if (col !== 0) {
        out += " ";
      }
      out += this.pspm[row][col].toFixed(6);
    }
    out += "\n";
  }
  return out;
};

Pspm.prototype.as_pssm = function(alphabet, pseudo) {
  "use strict";
  var out, log2, total, row, col, p, bg, p2, score;
  if (typeof pseudo === "undefined") {
    pseudo = 0.1;
  } else if (typeof pseudo !== "number") {
    throw new Error("Expected number for pseudocount");
  }
  out = "log-odds matrix: alength= " + this.alph_length + 
      " w= " + this.motif_length + 
      " E= " + (typeof this.evalue == "number" ? 
          this.evalue.toExponential() : this.evalue) + "\n";
  log2 = Math.log(2);
  total = this.nsites + pseudo;
  for (row = 0; row < this.motif_length; row++) {
    for (col = 0; col < this.alph_length; col++) {
      if (col !== 0) {
        out += " ";
      }
      p = this.pspm[row][col];
      // to avoid log of zero we add a pseudo count
      bg = alphabet.get_bg_freq(col);
      p2 = (p * this.nsites + bg * pseudo) / total;
      // now calculate the score
      score = -10000;
      if (p2 > 0) {
        score = Math.round((Math.log(p2 / bg) / log2) * 100);
      }
      out += score;
    }
    out += "\n";
  }
  return out;
};

Pspm.prototype.toString = function() {
  "use strict";
  var str, i, row;
  str = "";
  for (i = 0; i < this.pspm.length; i++) {
    row = this.pspm[i];
    str += row.join("\t") + "\n";
  }
  return str;
};

function parse_pspm_properties(str) {
  "use strict";
  var parts, i, eqpos, before, after, properties, prop, num, num_re;
  num_re = /^((?:[+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?)|inf)$/;
  parts = trim(str).split(/\s+/);
  // split up words containing =
  for (i = 0; i < parts.length;) {
    eqpos = parts[i].indexOf("=");
    if (eqpos != -1) {
      before = parts[i].substr(0, eqpos);
      after = parts[i].substr(eqpos+1);
      if (before.length > 0 && after.length > 0) {
        parts.splice(i, 1, before, "=", after);
        i += 3;
      } else if (before.length > 0) {
        parts.splice(i, 1, before, "=");
        i += 2;
      } else if (after.length > 0) {
        parts.splice(i, 1, "=", after);
        i += 2;
      } else {
        parts.splice(i, 1, "=");
        i++;
      }
    } else {
      i++;
    }
  }
  properties = {};
  for (i = 0; i < parts.length; i += 3) {
    if (parts.length - i < 3) {
      throw new Error("Expected PSPM property was incomplete. "+
          "Remaing parts are: " + parts.slice(i).join(" "));
    }
    if (parts[i+1] !== "=") {
      throw new Error("Expected '=' in PSPM property between key and " +
          "value but got " + parts[i+1]); 
    }
    prop = parts[i].toLowerCase();
    num = parts[i+2];
    if (!num_re.test(num)) {
      throw new Error("Expected numeric value for PSPM property '" + 
          prop + "' but got '" + num + "'");
    }
    properties[prop] = num;
  }
  return properties;
}

function parse_pspm_string(pspm_string) {
  "use strict";
  var header_re, lines, first_line, line_num, col_num, alph_length, 
      motif_length, nsites, evalue, pspm, i, line, match, props, parts,
      j, prob;
  header_re = /^letter-probability\s+matrix:(.*)$/i;
  lines = pspm_string.split(/\n/);
  first_line = true;
  line_num = 0;
  col_num = 0;
  alph_length;
  motif_length;
  nsites;
  evalue;
  pspm = [];
  for (i = 0; i < lines.length; i++) {
    line = trim(lines[i]);
    if (line.length === 0) { 
      continue;
    }
    // check the first line for a header though allow matrices without it
    if (first_line) {
      first_line = false;
      match = header_re.exec(line);
      if (match !== null) {
        props = parse_pspm_properties(match[1]);
        if (props.hasOwnProperty("alength")) {
          alph_length = parseFloat(props["alength"]);
          if (alph_length != 4 && alph_length != 20) {
            throw new Error("PSPM property alength should be 4 or 20" +
                " but got " + alph_length);
          }
        }
        if (props.hasOwnProperty("w")) {
          motif_length = parseFloat(props["w"]);
          if (motif_length % 1 !== 0 || motif_length < 1) {
            throw new Error("PSPM property w should be an integer larger " +
                "than zero but got " + motif_length);
          }
        }
        if (props.hasOwnProperty("nsites")) {
          nsites = parseFloat(props["nsites"]);
          if (nsites <= 0) {
            throw new Error("PSPM property nsites should be larger than " +
                "zero but got " + nsites);
          }
        }
        if (props.hasOwnProperty("e")) {
          evalue = props["e"];
          if (evalue < 0) {
            throw new Error("PSPM property evalue should be " +
                "non-negative but got " + evalue);
          }
        }
        continue;
      }
    }
    pspm[line_num] = [];
    col_num = 0;
    parts = line.split(/\s+/);
    for (j = 0; j < parts.length; j++) {
      prob = parseFloat(parts[j]);
      if (prob != parts[j] || prob < 0 || prob > 1) {
        throw new Error("Expected probability but got '" + parts[j] + "'"); 
      }
      pspm[line_num][col_num] = prob;
      col_num++;
    }
    line_num++;
  }
  if (typeof motif_length === "number") {
    if (pspm.length != motif_length) {
      throw new Error("Expected PSPM to have a motif length of " + 
          motif_length + " but it was actually " + pspm.length);
    }
  } else {
    motif_length = pspm.length;
  }
  if (typeof alph_length !== "number") {
    alph_length = pspm[0].length;
    if (alph_length != 4 && alph_length != 20) {
      throw new Error("Expected length of first row in the PSPM to be " +
          "either 4 or 20 but got " + alph_length);
    }
  }
  for (i = 0; i < pspm.length; i++) {
    if (pspm[i].length != alph_length) {
      throw new Error("Expected PSPM row " + i + " to have a length of " + 
          alph_length + " but the length was " + pspm[i].length);
    }
  }
  return {"pspm": pspm, "motif_length": motif_length, 
    "alph_length": alph_length, "nsites": nsites, "evalue": evalue};
}
//======================================================================
// end Pspm object
//======================================================================
