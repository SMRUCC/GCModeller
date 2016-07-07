---
title: Resources
---

# Resources
_namespace: [SMRUCC.genomics.Analysis.PFSNet.My.Resources](N-SMRUCC.genomics.Analysis.PFSNet.My.Resources.html)_

A strongly-typed resource class, for looking up localized strings, etc.




### Properties

#### Culture
Overrides the current thread's CurrentUICulture property for all
 resource lookups using this strongly typed resource class.
#### gpl
Looks up a localized string similar to GNU GENERAL PUBLIC LICENSE
 Version 3, 29 June 2007

 Copyright (C) 2007 Free Software Foundation, Inc. <http://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.

 Preamble

 The GNU General Public License is a free, copyleft license for
software and other kinds of works.

 The licenses for most software and other practical works are designed
to [rest of string was truncated]";.
#### pfsnet
Looks up a localized string similar to require(igraph)
#require(rJava)

loaddata<-function(file){
a<-read.table(file,row.names=1)
a
}

computew1<-function(expr,theta1,theta2){
ranks<-apply(expr,2,function(x){
rank(x)/length(x)
})
apply(ranks,2,function(x){
q2<-quantile(x,theta2,na.rm=T)
q1<-quantile(x,theta1,na.rm=T)
m<-median(x)
mx<-max(x)
sapply(x,function(y){
if(is.na(y)){
NA
}
else if(y>=q1)
1
else if(y>=q2)
(y-q2)/(q1-q2)
else
0
})
})
}

pfsnet.computegenelist [rest of string was truncated]";.
#### ResourceManager
Returns the cached ResourceManager instance used by this class.
