# PGMap
_namespace: [SMRUCC.genomics.Assembly.DOMINE.Tables](./index.md)_



> 
>  CREATE TABLE PGMAP
>  (
>  DomainAcc char(7),
>  GoTerm char(10),
>  PRIMARY KEY (DomainAcc, GoTerm),
>  FOREIGN KEY (DomainAcc) references PFAM(DomainAcc),
>  FOREIGN KEY (GoTerm) references GO(GoTerm)
>  );
>  



