---
title: relatedterm
---

# relatedterm
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `relatedterm`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `relatedterm` (
 `TermWID` bigint(20) NOT NULL,
 `OtherWID` bigint(20) NOT NULL,
 `Relationship` varchar(50) DEFAULT NULL,
 KEY `FK_RelatedTerm1` (`TermWID`),
 CONSTRAINT `FK_RelatedTerm1` FOREIGN KEY (`TermWID`) REFERENCES `term` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




