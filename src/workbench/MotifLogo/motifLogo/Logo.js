
//======================================================================
// start Logo object
//======================================================================

var Logo = function(alphabet, fine_text) {
  "use strict";
  this.alphabet = alphabet;
  this.fine_text = fine_text;
  this.pspm_list = [];
  this.pspm_column = [];
  this.rows = 0;
  this.columns = 0;
};

Logo.prototype.add_pspm = function(pspm, column) {
  "use strict";
  var col;
  if (typeof column === "undefined") {
    column = 0;
  } else if (column < 0) {
    throw new Error("Column index out of bounds.");
  }
  this.pspm_list[this.rows] = pspm;
  this.pspm_column[this.rows] = column;
  this.rows++;
  col = column + pspm.get_motif_length();
  if (col > this.columns) {
    this.columns = col;
  }
};

Logo.prototype.get_columns = function() {
  "use strict";
  return this.columns;
};

Logo.prototype.get_rows = function() {
  "use strict";
  return this.rows;
};

Logo.prototype.get_pspm = function(row_index) {
  "use strict";
  if (row_index < 0 || row_index >= this.rows) {
    throw new Error("INDEX_OUT_OF_BOUNDS");
  }
  return this.pspm_list[row_index];
};

Logo.prototype.get_offset = function(row_index) {
  "use strict";
  if (row_index < 0 || row_index >= this.rows) {
    throw new Error("INDEX_OUT_OF_BOUNDS");
  }
  return this.pspm_column[row_index];
};

//======================================================================
// end Logo object
//======================================================================
