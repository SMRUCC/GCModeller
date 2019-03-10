if(length(x) == 2){
        venn.plot<-venn.diagram(x, filename="venn.tiff",fill = c("red", "green"),alpha=0.50, cat.default.pos = "outer",margin=0.1,cat.dist=c(0.1,0.1),inverted=TRUE,cat.pos=0,cex = 2.5,cat.cex=1.5, cat.fontface="bold")
} else if(length(x) == 3){
        venn.plot<-venn.diagram(x, filename="venn.tiff",height = 3000, width = 3600,col="black", fill = c("red", "blue", "green"),alpha=0.50,label.col = c("darkred", "white", "darkblue", "white","white", "white", "darkgreen"), cex = 2.5, cat.col = c("darkred", "darkblue", "darkgreen"),fontfamily = "serif",cat.default.pos = "text",cat.dist = c(0.06, 0.06, 0.03),cat.cex=1.5, cat.fontface="bold",cat.pos = 0);
} else if(length(x) == 5){
        venn.plot<-venn.diagram(x, filename="venn.tiff",col="black", fill=c("dodgerblue","goldenrod1","darkorange1","seagreen3","orchid3"),alpha=0.50, cex=c(1.5,1.5,1.5,1.5,1.5,1,0.8,1,0.8,1,0.8,1,0.8, 1,0.8,1,0.55,1,0.55,1,0.55,1,0.55,1,0.55,1,1,1,1,1,1.5), cat.col=c("dodgerblue","goldenrod1","darkorange1","seagreen3","orchid3"),cat.cex=1.5, cat.fontface="bold",margin=0.05);
} else if(length(x) == 4){
        venn.plot<-venn.diagram(x, filename="venn.tiff",col="black", fill =c("orange", "red", "green", "blue"),cat.col = c("orange", "red", "green", "blue"),cat.cex=2,lty = "dashed",cex=2, cat.fontface="bold",margin=0.05);
} else if(length(x) > 5){
        print("too many samples")
} else if(length(x) < 2){
        print("too few samples")
}