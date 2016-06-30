
//======================================================================
// start LogoMetrics object
//======================================================================

var LogoMetrics = function(ctx, logo_columns, logo_rows, allow_space_for_names) {
  "use strict";
  var i, row_height;
  if (typeof allow_space_for_names === "undefined") {
    allow_space_for_names = false;
  }
  //variable prototypes
  this.pad_top = 5;
  this.pad_left = 10;
  this.pad_right = 5;
  this.pad_bottom = 0;
  this.pad_middle = 20;
  this.name_height = 14;
  this.name_font = "bold " + this.name_height + "px Times, sans-serif";
  this.name_spacer = 0;
  this.y_label = "bits";
  this.y_label_height = 12;
  this.y_label_font = "bold " + this.y_label_height + "px Helvetica, sans-serif";
  this.y_label_spacer = 3;
  this.y_num_height = 12;
  this.y_num_width = 0;
  this.y_num_font = "bold " + this.y_num_height + "px Helvetica, sans-serif";
  this.y_tic_width = 5;
  this.stack_pad_left = 0;
  this.stack_font = "bold 25px Helvetica, sans-serif";
  this.stack_height = 90;
  this.stack_width = 26;
  this.stacks_pad_right = 5;
  this.x_num_above = 2;
  this.x_num_height = 12;
  this.x_num_width = 0;
  this.x_num_font = "bold " + this.x_num_height + "px Helvetica, sans-serif";
  this.fine_txt_height = 6;
  this.fine_txt_above = 2;
  this.fine_txt_font = "normal " + this.fine_txt_height + "px Helvetica, sans-serif";
  this.letter_metrics = new Array();
  this.summed_width = 0;
  this.summed_height = 0;
  //calculate the width of the y axis numbers
  ctx.font = this.y_num_font;
  for (i = 0; i <= 2; i++) {
    this.y_num_width = Math.max(this.y_num_width, ctx.measureText("" + i).width);
  }
  //calculate the width of the x axis numbers (but they are rotated so it becomes height)
  ctx.font = this.x_num_font;
  for (i = 1; i <= logo_columns; i++) {
    this.x_num_width = Math.max(this.x_num_width, ctx.measureText("" + i).width);
  }
  
  //calculate how much vertical space we want to draw this
  //first we add the padding at the top and bottom since that's always there
  this.summed_height += this.pad_top + this.pad_bottom;
  //all except the last row have the same amount of space allocated to them
  if (logo_rows > 1) {
    row_height = this.stack_height + this.pad_middle;
    if (allow_space_for_names) {
      row_height += this.name_height;
      //the label is allowed to overlap into the spacer
      row_height += Math.max(this.y_num_height/2, this.name_spacer); 
      //the label is allowed to overlap the space used by the other label
      row_height += Math.max(this.y_num_height/2, this.x_num_height + this.x_num_above); 
    } else {
      row_height += this.y_num_height/2; 
      //the label is allowed to overlap the space used by the other label
      row_height += Math.max(this.y_num_height/2, this.x_num_height + this.x_num_above); 
    }
    this.summed_height += row_height * (logo_rows - 1);
  }
  //the last row has the name and fine text below it but no padding
  this.summed_height += this.stack_height + this.y_num_height/2;
  if (allow_space_for_names) {
    this.summed_height += this.fine_txt_height + this.fine_txt_above + this.name_height;
    this.summed_height += Math.max(this.y_num_height/2, 
        this.x_num_height + this.x_num_above + this.name_spacer);
  } else {
    this.summed_height += Math.max(this.y_num_height/2, 
        this.x_num_height + this.x_num_above + this.fine_txt_height + this.fine_txt_above);
  }

  //calculate how much horizontal space we want to draw this
  //first add the padding at the left and right since that's always there
  this.summed_width += this.pad_left + this.pad_right;
  //add on the space for the y-axis label
  this.summed_width += this.y_label_height + this.y_label_spacer;
  //add on the space for the y-axis
  this.summed_width += this.y_num_width + this.y_tic_width;
  //add on the space for the stacks
  this.summed_width += (this.stack_pad_left + this.stack_width) * logo_columns;
  //add on the padding after the stacks (an offset from the fine text)
  this.summed_width += this.stacks_pad_right;

};

//======================================================================
// end LogoMetrics object
//======================================================================
