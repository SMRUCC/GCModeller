---
title: db_version
---

# db_version
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `db_version`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `db_version` (
 `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `version` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `entry_count` bigint(10) NOT NULL,
 `file_date` datetime NOT NULL,
 `load_date` datetime NOT NULL,
 PRIMARY KEY (`dbcode`),
 CONSTRAINT `fk_db_version$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




