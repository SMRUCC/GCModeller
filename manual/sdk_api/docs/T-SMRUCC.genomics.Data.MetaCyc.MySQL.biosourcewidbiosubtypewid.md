---
title: biosourcewidbiosubtypewid
---

# biosourcewidbiosubtypewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `biosourcewidbiosubtypewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `biosourcewidbiosubtypewid` (
 `BioSourceWID` bigint(20) NOT NULL,
 `BioSubtypeWID` bigint(20) NOT NULL,
 KEY `FK_BioSourceWIDBioSubtypeWID1` (`BioSourceWID`),
 KEY `FK_BioSourceWIDBioSubtypeWID2` (`BioSubtypeWID`),
 CONSTRAINT `FK_BioSourceWIDBioSubtypeWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioSourceWIDBioSubtypeWID2` FOREIGN KEY (`BioSubtypeWID`) REFERENCES `biosubtype` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




