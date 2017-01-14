library(tools) 
library(Cairo)

#########################################################
### A) Installing and loading required packages
#########################################################

if (!require("gplots")) {
   install.packages("gplots", dependencies = TRUE)
   library(gplots)
   }
if (!require("RColorBrewer")) {
   install.packages("RColorBrewer", dependencies = TRUE)
   library(RColorBrewer)
   }

   
plotDEPs <- function(csv) {



#########################################################
### B) Reading in data and transform it into matrix format
#########################################################

data <- read.csv(csv, comment.char="#")
rnames <- data[,1]                            # assign labels in column 1 to "rnames"
mat_data <- data.matrix(data[,2:ncol(data)])  # transform column 2-5 into a matrix
rownames(mat_data) <- rnames                  # assign row names


#########################################################
### C) Customizing and plotting the heat map
#########################################################

# creates a own color palette from red to green
my_palette <- colorRampPalette(c("darkblue", "green", "red"))(n = 20)

# (optional) defines the color breaks manually for a "skewed" color transition
#col_breaks = c(seq(-1,0,length=100),  # for red
#  seq(0.01,0.8,length=100),           # for yellow
#  seq(0.81,1,length=100))             # for green

  
DIR <- dirname(csv)
DIR <- paste(DIR, file_path_sans_ext(basename(csv)), sep="/")
  
# creates a 5 x 5 inch image
png(paste(DIR, "heatmap.png", sep="-"),    # create PNG for the heat map        
  width = 5*300,        # 5 x 300 pixels
  height = 5*300,
  res = 300,            # 300 pixels per inch
  pointsize = 8)        # smaller font size

heatmap.2(mat_data,
  cellnote = mat_data,  # same data set for cell labels
  main = NA, # heat map title
  notecol="black",      # change font color of cell labels to black
  density.info="none",  # turns off density plot inside color legend
  trace="none",         # turns off trace lines inside the heat map
  margins =c(6, 21),     # widens margins around plot
  col=my_palette,       # use on color palette defined earlier
  # breaks=col_breaks,    # enable color transition at specified limits
  dendrogram="row",     # only draw a row dendrogram
  Colv="NA")            # turn off column clustering

dev.off()               # close the PNG device


}
