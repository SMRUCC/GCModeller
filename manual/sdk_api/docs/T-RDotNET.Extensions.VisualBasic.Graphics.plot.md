---
title: plot
---

# plot
_namespace: [RDotNET.Extensions.VisualBasic.Graphics](N-RDotNET.Extensions.VisualBasic.Graphics.html)_

Generic function for plotting of R objects. For more details about the graphical parameter arguments, see par.
 For simple scatter plots, plot.default will be used. 
 However, there are plot methods for many R objects, including functions, data.frames, density objects, etc. Use methods(plot) And the documentation for these.

> 
>  The two step types differ in their x-y preference: Going from (x1,y1) to (x2,y2) with x1 < x2, type = "s" moves first horizontal, then vertical, whereas type = "S" moves the other way around.
>  



### Properties

#### asp
the y / x aspect ratio, see plot.window.
#### main
an overall title For the plot: see title.
#### sub
A Sub() title for the plot: see title.
#### type
what type Of plot should be drawn. Possible types are
 
 "p" for points,
 "l" for lines,
 "b" for both,
 "c" for the lines part alone of "b",
 "o" for both 'overplotted’,
 "h" for 'histogram’ like (or ‘high-density’) vertical lines,
 "s" for stair steps,
 "S" for other steps, see 'Details’ below,
 "n" for no plotting.

 All other types give a warning Or an Error; Using, e.g., type = "punkte" being equivalent To type = "p" For S compatibility. 
 Note that some methods, e.g. plot.factor, Do Not accept this.
#### x
the coordinates Of points In the plot. Alternatively, a Single plotting Structure, Function Or any R Object With a plot method can be provided.
#### xlab
A title For the x axis: see title.
#### y
the y coordinates Of points In the plot, Optional If x Is an appropriate Structure.
#### ylab
A title For the y axis: see title.
