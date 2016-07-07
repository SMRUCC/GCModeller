---
title: experimentrelationship
---

# experimentrelationship
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `experimentrelationship`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `experimentrelationship` (
 `ExperimentWID` bigint(20) NOT NULL,
 `RelatedExperimentWID` bigint(20) NOT NULL,
 KEY `FK_ExpRelationship1` (`ExperimentWID`),
 KEY `FK_ExpRelationship2` (`RelatedExperimentWID`),
 CONSTRAINT `FK_ExpRelationship1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ExpRelationship2` FOREIGN KEY (`RelatedExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




