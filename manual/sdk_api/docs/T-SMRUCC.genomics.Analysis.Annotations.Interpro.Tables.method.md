---
title: method
---

# method
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `method`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `method` (
 `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `method_date` datetime NOT NULL,
 `skip_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL DEFAULT 'N',
 `candidate` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`method_ac`),
 KEY `fk_method$dbcode` (`dbcode`),
 CONSTRAINT `fk_method$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




