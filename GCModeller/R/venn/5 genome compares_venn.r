library(VennDiagram)

d0 <- c(1, 2, 3, 4)
d1 <- c(1, 4)
d2 <- c(1, 5)
d3 <- c(1, 3)
d4 <- c(1, 2, 5)
input_data <- list(aciad=d0,ecoli=d1,ftn=d2,pa14=d3,Xcc8004=d4)
fill_color <- c("red", "green", "black", "yellow", "blue")
venn__plots_out <- venn.diagram(x=input_data, 
filename="G:/GCModeller/GCModeller/R/venn/diagram.tiff", 
height=4000, 
width=7000, 
resolution=600, 
imagetype="tiff", 
units="px", 
compression="lzw", 
na="stop", 
main="5 genome compares", 
sub=NULL, 
main.pos=c(0.5,1.05), 
main.fontface="plain", 
main.fontfamily="serif", 
main.col="black", 
main.cex=3, 
main.just=c(0.5,1), 
sub.pos=c(0.5,1.05), 
sub.fontface="plain", 
sub.fontfamily="serif", 
sub.col="black", 
sub.cex=1, 
sub.just=c(0.5,1), 
category.names=c("ACIAD", "Ecoli. K12", "FTN", "PA14", "Xanthomonas campestris pv. campestris str. 8004"), 
force.unique=TRUE, 
print.mode="raw", 
sigdigs=3, 
direct.area=FALSE, 
area.vector=0, 
hyper.test=FALSE, 
total.population=NULL, 
fill=fill_color)
