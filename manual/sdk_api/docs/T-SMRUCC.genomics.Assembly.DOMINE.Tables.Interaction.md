---
title: Interaction
---

# Interaction
_namespace: [SMRUCC.genomics.Assembly.DOMINE.Tables](N-SMRUCC.genomics.Assembly.DOMINE.Tables.html)_



> 
>  CREATE TABLE INTERACTION
>  (
>  Domain1 char(7),
>  Domain2 char(7),
>  iPfam boolean,
>  3did boolean,
>  ME boolean,
>  RCDP boolean,
>  Pvalue boolean,
>  Fusion boolean,
>  DPEA boolean,
>  PE boolean,
>  GPE boolean,
>  DIPD boolean,
>  RDFF boolean,
>  KGIDDI boolean,
>  INSITE boolean,
>  DomainGA boolean,
>  PP boolean,
>  PredictionConfidence char(2),
>  SameGO boolean,
>  PRIMARY KEY (Domain1, Domain2),
>  FOREIGN KEY (Domain1) references PFAM(DomainAcc),
>  FOREIGN KEY (DOmain2) references PFAM(DomainAcc)
>  );
>  



