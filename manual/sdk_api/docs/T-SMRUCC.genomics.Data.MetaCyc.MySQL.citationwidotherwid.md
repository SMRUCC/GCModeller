---
title: citationwidotherwid
---

# citationwidotherwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `citationwidotherwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `citationwidotherwid` (
 `OtherWID` bigint(20) NOT NULL,
 `CitationWID` bigint(20) NOT NULL,
 KEY `FK_CitationWIDOtherWID` (`CitationWID`),
 CONSTRAINT `FK_CitationWIDOtherWID` FOREIGN KEY (`CitationWID`) REFERENCES `citation` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




