---
title: biosourcewidcontactwid
---

# biosourcewidcontactwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `biosourcewidcontactwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `biosourcewidcontactwid` (
 `BioSourceWID` bigint(20) NOT NULL,
 `ContactWID` bigint(20) NOT NULL,
 KEY `FK_BioSourceWIDContactWID1` (`BioSourceWID`),
 KEY `FK_BioSourceWIDContactWID2` (`ContactWID`),
 CONSTRAINT `FK_BioSourceWIDContactWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioSourceWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




