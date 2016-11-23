# Reference
_namespace: [SMRUCC.genomics.foundation.psidev.XML](./index.md)_

Refers to a unique object in an external database.




### Properties

#### db
Name of the external database. Taken from the controlled vocabulary of databases.
#### dbAc
Accession number of the database in the database CV. This element is controlled by the PSI-MI controlled
 vocabulary "database citation", root term id MI0444.
#### id
Primary identifier of the object in the external database, e.g. UniProt accession number.
#### refType
Reference type, e.g. "identity" if this reference referes to an identical object in the external database,
 Or "see-also" for additional information. Controlled by CV.
#### refTypeAc
Reference type accession number from the CV of reference types. This element is controlled by the PSI-MI
 controlled vocabulary "xref type", root term id MI:0353.
#### secondary
Secondary identifier of the object in the external database, e.g. UniProt ID.
#### version
The version number of the object in the external database.
