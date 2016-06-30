
//======================================================================
// start RasterizedAlphabet
//======================================================================

// Rasterize Alphabet
// 1) Measure width of text at default font for all symbols in alphabet
// 2) sort in width ascending
// 3) Drop the top and bottom 10% (designed to ignore outliers like 'W' and 'I')
// 4) Calculate the average as the maximum scaling factor (designed to stop I becoming a rectangular blob).
// 5) Assume scale of zero would result in width of zero, interpolate scale required to make perfect width font
// 6) Draw text onto temp canvas at calculated scale
// 7) Find bounds of drawn text
// 8) Paint on to another canvas at the desired height (but only scaling width to fit if larger).
var RasterizedAlphabet = function(alphabet, font, target_width) {
  "use strict";
  var default_size, safety_pad, canvas, ctx, middle, baseline, widths, count,
      letters, i, letter, size, tenpercent, avg_width, scale, 
      target_height, raster;
  //variable prototypes
  this.lookup = []; //a map of letter to index
  this.rasters = []; //a list of rasters
  this.dimensions = []; //a list of dimensions

  //construct
  default_size = 60; // size of square to assume as the default width
  safety_pad = 20; // pixels to pad around so we don't miss the edges
  // create a canvas to do our rasterizing on
  canvas = document.createElement("canvas");
  // assume the default font would fit in a canvas of 100 by 100
  canvas.width = default_size + 2 * safety_pad;
  canvas.height = default_size + 2 * safety_pad;
  // check for canvas support before attempting anything
  if (!canvas.getContext) {
    throw new Error("NO_CANVAS_SUPPORT");
  }
  ctx = canvas.getContext('2d');
  // check for html5 text drawing support
  if (!supports_text(ctx)) {
    throw new Error("NO_CANVAS_TEXT_SUPPORT");
  }
  // calculate the middle
  middle = Math.round(canvas.width / 2);
  // calculate the baseline
  baseline = Math.round(canvas.height - safety_pad);
  // list of widths
  widths = [];
  count = 0;
  letters = [];
  //now measure each letter in the alphabet
  for (i = 0; i < alphabet.get_size(); ++i) {
    if (alphabet.is_ambig(i)) {
      continue; //skip ambigs as they're never rendered
    }
    letter = alphabet.get_letter(i);
    letters.push(letter);
    this.lookup[letter] = count;
    //clear the canvas
    canvas.width = canvas.width;
    // get the context and prepare to draw our width test
    ctx = canvas.getContext('2d');
    ctx.font = font;
    ctx.fillStyle = alphabet.get_colour(i);
    ctx.textAlign = "center";
    ctx.translate(middle, baseline);
    // draw the test text
    ctx.fillText(letter, 0, 0);
    //measure
    size = canvas_bounds(ctx, canvas.width, canvas.height);
    if (size.width === 0) {
      throw new Error("INVISIBLE_LETTER"); //maybe the fill was white on white?
    }
    widths.push(size.width);
    this.dimensions[count] = size;
    count++;
  }
  //sort the widths
  widths.sort(function(a,b) {return a - b;});
  //drop 10% of the items off each end
  tenpercent = Math.floor(widths.length / 10);
  for (i = 0; i < tenpercent; ++i) {
    widths.pop();
    widths.shift();
  }
  //calculate average width
  avg_width = 0;
  for (i = 0; i < widths.length; ++i) {
    avg_width += widths[i];
  }
  avg_width /= widths.length;
  // calculate scales
  for (i = 0; i < this.dimensions.length; ++i) {
    size = this.dimensions[i];
    // calculate scale
    scale = target_width / Math.max(avg_width, size.width);
    // estimate scaled height
    target_height = size.height * scale;
    // create an approprately sized canvas
    raster = document.createElement("canvas");
    raster.width = target_width; // if it goes over the edge too bad...
    raster.height = target_height + safety_pad * 2;
    // calculate the middle
    middle = Math.round(raster.width / 2);
    // calculate the baseline
    baseline = Math.round(raster.height - safety_pad);
    // get the context and prepare to draw the rasterized text
    ctx = raster.getContext('2d');
    ctx.font = font;
    ctx.fillStyle = alphabet.get_colour(i);
    ctx.textAlign = "center";
    ctx.translate(middle, baseline);
    ctx.save();
    ctx.scale(scale, scale);
    // draw the rasterized text
    ctx.fillText(letters[i], 0, 0);
    ctx.restore();
    this.rasters[i] = raster;
    this.dimensions[i] = canvas_bounds(ctx, raster.width, raster.height);
  }
};

function canvas_bounds(ctx, cwidth, cheight) {
  "use strict";
  var data, r, c, top_line, bottom_line, left_line, right_line, 
      txt_width, txt_height;
  data = ctx.getImageData(0, 0, cwidth, cheight).data;
  r = 0; c = 0; // r: row, c: column
  top_line = -1; bottom_line = -1; left_line = -1; right_line = -1;
  txt_width = 0; txt_height = 0;
  // Find the top-most line with a non-white pixel
  for (r = 0; r < cheight; r++) {
    for (c = 0; c < cwidth; c++) {
      if (data[r * cwidth * 4 + c * 4 + 3]) {
        top_line = r;
        break;
      }
    }
    if (top_line != -1) {
      break;
    }
  }
  
  //find the last line with a non-white pixel
  if (top_line != -1) {
    for (r = cheight-1; r >= top_line; r--) {
      for(c = 0; c < cwidth; c++) {
        if(data[r * cwidth * 4 + c * 4 + 3]) {
          bottom_line = r;
          break;
        }
      }
      if (bottom_line != -1) {
        break;
      }
    }
    txt_height = bottom_line - top_line + 1;
  }

  // Find the left-most line with a non-white pixel
  for (c = 0; c < cwidth; c++) {
    for (r = 0; r < cheight; r++) {
      if (data[r * cwidth * 4 + c * 4 + 3]) {
        left_line = c;
        break;
      }
    }
    if (left_line != -1) {
      break;
    }
  }

  //find the right most line with a non-white pixel
  if (left_line != -1) {
    for (c = cwidth-1; c >= left_line; c--) {
      for(r = 0; r < cheight; r++) {
        if(data[r * cwidth * 4 + c * 4 + 3]) {
          right_line = c;
          break;
        }
      }
      if (right_line != -1) {
        break;
      }
    }
    txt_width = right_line - left_line + 1;
  }

  //return the bounds
  return {bound_top: top_line, bound_bottom: bottom_line, 
    bound_left: left_line, bound_right: right_line, width: txt_width, 
    height: txt_height};
}

RasterizedAlphabet.prototype.draw = function(ctx, letter, dx, dy, dWidth, dHeight) {
  "use strict";
  var index, raster, size;
  index = this.lookup[letter];
  raster = this.rasters[index];
  size = this.dimensions[index];
  ctx.drawImage(raster, 0, size.bound_top -1, raster.width, size.height+1, dx, dy, dWidth, dHeight);
};

//======================================================================
// end RasterizedAlphabet
//======================================================================
