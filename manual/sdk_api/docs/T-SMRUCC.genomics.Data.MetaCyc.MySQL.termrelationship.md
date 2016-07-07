---
title: termrelationship
---

# termrelationship
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `termrelationship`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `termrelationship` (
 `TermWID` bigint(20) NOT NULL,
 `RelatedTermWID` bigint(20) NOT NULL,
 `Relationship` varchar(10) NOT NULL,
 KEY `FK_TermRelationship1` (`TermWID`),
 KEY `FK_TermRelationship2` (`RelatedTermWID`),
 CONSTRAINT `FK_TermRelationship1` FOREIGN KEY (`TermWID`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_TermRelationship2` FOREIGN KEY (`RelatedTermWID`) REFERENCES `term` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




