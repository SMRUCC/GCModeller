library(VennDiagram)

d0 <- c(1, 3, 5);
d1 <- c(2, 3);
d2 <- c(3, 4);
d3 <- c(3, 5);
d4 <- c(1, 2, 3, 4);
input_data <- list(objA=d0,objB=d1,objC=d2,objD=d3,objE=d4);
output_image_file <- "C:/Users/xieguigang/Desktop/test_venn_title_venn.tiff";
title <- "test_venn_title";
fill_color <- c("grey80","pink1","lawngreen","yellow","peachpuff");
venn.diagram(input_data,fill=fill_color,filename=output_image_file,width=5000,height=3000,main=title);
