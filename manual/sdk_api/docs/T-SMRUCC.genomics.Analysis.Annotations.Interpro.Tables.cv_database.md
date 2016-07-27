---
title: cv_database
---

# cv_database
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `cv_database`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `cv_database` (
 `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `dbname` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `dborder` int(5) NOT NULL,
 `dbshort` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 PRIMARY KEY (`dbcode`),
 UNIQUE KEY `uq_cv_database$database` (`dbname`),
 UNIQUE KEY `uq_cv_database$dborder` (`dborder`),
 UNIQUE KEY `uq_cv_database$dbshort` (`dbshort`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




