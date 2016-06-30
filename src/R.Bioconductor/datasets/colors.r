f0 <- function(rgb1,rgb2,n) {
   return (mapply(seq,rgb1,rgb2,len=n));
}

red <- c(0.60,0.00,0.00)
yellow <- c(1.00,0.86,0.10)
mid <- c(0.85,1.00,0.85)
cyan <- c(0.10,0.86,1.00)
blue <- c(0.00,0.00,0.50)
r2y <- f0(red,yellow,400)
c2b <- f0(cyan,blue,400)
y2m <- f0(yellow, mid, 202)
y2m <- y2m[-c(1,nrow(y2m)),]
m2c <- f0(mid,cyan,202)
m2c <- m2c[-nrow(m2c),]
color <- rbind(r2y,y2m,m2c,c2b)
color <- color[nrow(color):1,]

result <- character(len=nrow(color))

for (i in 1:nrow(color)) {
   result[i] = rgb(color[i,1],color[i,2],color[i,3])
}

png("x:/colors.png")

par(mar=c(10,4,10,4))
plot(1,type='n',ann=F,axes=F,xlim=c(0,1201),ylim=c(0,1))

for(i in 1:length(result)) {
    rect(i-1,0,i,1,border=NA,col=result[i]);
}

dev.off()