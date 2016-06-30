/*jshint browser:true, indent:2, globalstrict: true, laxcomma: true, laxbreak: true */
/*global d3:true */

/*
 * colorlegend
 *
 * This script can be used to draw a color legend for a 
 * [d3.js scale](https://github.com/mbostock/d3/wiki/Scales) 
 * on a specified html div element. 
 * [d3.js](http://mbostock.github.com/d3/) is required.
 *
 */

'use strict';

var colorlegend = function (target, scale, type, options) {
  var scaleTypes = ['linear', 'quantile', 'ordinal']
    , found = false
    , opts = options || {}
    , boxWidth = opts.boxWidth || 20        // width of each box (int)
    , boxHeight = opts.boxHeight || 20      // height of each box (int)
    , title = opts.title || null            // draw title (string)
    , fill = opts.fill || false             // fill the element (boolean)
    , linearBoxes = opts.linearBoxes || 9   // number of boxes for linear scales (int)
    , htmlElement = document.getElementById(target.substring(0, 1) === '#' ? target.substring(1, target.length) : target)  // target container element - strip the prefix #
    , w = htmlElement.offsetWidth           // width of container element
    , h = htmlElement.offsetHeight          // height of container element
    , colors = []
    , padding = [2, 4, 10, 4]               // top, right, bottom, left
    , boxSpacing = type === 'ordinal' ? 3 : 0 // spacing between boxes
    , titlePadding = title ? 11 : 0
    , domain = scale.domain()
    , range = scale.range()    
    , i = 0
    , isVertical = opts.vertical || false;

  // check for valid input - 'quantize' not included
  for (i = 0 ; i < scaleTypes.length ; i++) {
    if (scaleTypes[i] === type) {
      found = true;
      break;
    }
  }
  if (! found)
    throw new Error('Scale type, ' + type + ', is not suported.');

  
  // setup the colors to use
  if (type === 'quantile') {
    colors = range;
  }
  else if (type === 'ordinal') {
    for (i = 0 ; i < domain.length ; i++) {
      colors[i] = range[i];
    }
  }
  else if (type === 'linear') {
    var min = domain[0];
    var max = domain[domain.length - 1];
    for (i = 0; i < linearBoxes ; i++) {
      colors[i] = scale(min + i * ((max - min) / linearBoxes));
    }
  }
  
  // check the width and height and adjust if necessary to fit in the element use the range if quantile
  if (!isVertical) {
    if (fill || w < (boxWidth + boxSpacing) * colors.length + padding[1] + padding[3]) {
    boxWidth = (w - padding[1] - padding[3] - (boxSpacing * colors.length)) / colors.length;
    }
    if (fill || h < boxHeight + padding[0] + padding[2] + titlePadding) {  
      boxHeight = h - padding[0] - padding[2] - titlePadding;    
    }

  } else {
    if (fill || h < (boxHeight + boxSpacing) * colors.length + padding[0] + padding[2]) {
    boxHeight = (h - padding[0] - padding[2] - (boxSpacing * colors.length)) / colors.length;
    }
    if (fill || w < boxWidth + padding[1] + padding[3] + titlePadding) {  
      boxWidth = w - padding[1] - padding[3] - titlePadding;    
    }    
  }
  
  
  // set up the legend graphics context
  var legend = d3.select(target)
    .append('svg')
      .attr('width', w)
      .attr('height', h)
    .append('g')
      .attr('class', 'colorlegend')
      .attr('transform', 'translate(' + padding[3] + ',' + padding[0] + ')')
      .style('font-size', '11px')
      .style('fill', '#666');
      
  var legendBoxes = legend.selectAll('g.legend')
      .data(colors)
    .enter().append('g');

  // value labels
  var valueLabels;
  if (!isVertical) {
    valueLabels = legendBoxes.append('text')
      .attr('class', 'colorlegend-labels')
      .attr('dy', '.71em')
      .attr('x', function (d, i) {
        return i * (boxWidth + boxSpacing) + (type !== 'ordinal' ? (boxWidth / 2) : 0);
      })
      .attr('y', function () {
        return boxHeight + 2;
      });
  } else {
    valueLabels = legendBoxes.append('text')
      .attr('class', 'colorlegend-labels')
      .attr('dy', padding[0])
      .attr('x', function () {
        // return boxWidth + titlePadding;
        return titlePadding;
      })
      .attr('y', function (d, i) {
        return i * (boxHeight + boxSpacing) + boxHeight / 2;
      });
  }
  valueLabels    
      .style('text-anchor', function () {
        return type === 'ordinal' ? 'start' : 'middle';
      })
      .style('pointer-events', 'none')
      .text(function (d, i) {
        // show label for all ordinal values
        if (type === 'ordinal') {
          return domain[i];
        }
        // show only the first and last for others
        else {
          if (i === 0)
            return domain[0];
          if (i === colors.length - 1) 
            return domain[domain.length - 1];
        }
      });


  // the colors, each color is drawn as a rectangle
  if (!isVertical) {
    legendBoxes.append('rect')
      .attr('x', function (d, i) { 
        return i * (boxWidth + boxSpacing);
      })
      .attr('width', boxWidth)
      .attr('height', boxHeight)
      .style('fill', function (d, i) { return colors[i]; });

  } else {
    legendBoxes.append('rect')
        .attr('y', function(d, i) {
          return i * (boxHeight + boxSpacing);
        })  
        .attr('x', function() {
          return w - boxWidth - padding[1] - padding[3];
        })
        .attr('width', boxWidth)
        .attr('height', boxHeight)
        .style('fill', function (d, i) { return colors[i]; });  
  }

  // show a title in center of legend (bottom)
  if (title) {
    var legendText = legend.append('text')
        .attr('class', 'colorlegend-title')   
        .style('text-anchor', 'middle')
        .style('pointer-events', 'none')
        .text(title);

    if (!isVertical) {
      legendText
        .attr('dy', '.71em')
        .attr('x', (colors.length * (boxWidth / 2)))
        .attr('y', boxHeight + titlePadding);

    } else {
      legendText       
        .attr('dy', '.51em')
        .attr('y', (colors.length * (boxHeight / 2)))
        .attr('transform', 'rotate(90, 5,' + (colors.length * (boxHeight / 2)) + ')');
    }
  }
    
  return this;
};