---
title: arraygroupwidarraywid
---

# arraygroupwidarraywid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `arraygroupwidarraywid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `arraygroupwidarraywid` (
 `ArrayGroupWID` bigint(20) NOT NULL,
 `ArrayWID` bigint(20) NOT NULL,
 KEY `FK_ArrayGroupWIDArrayWID1` (`ArrayGroupWID`),
 KEY `FK_ArrayGroupWIDArrayWID2` (`ArrayWID`),
 CONSTRAINT `FK_ArrayGroupWIDArrayWID1` FOREIGN KEY (`ArrayGroupWID`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ArrayGroupWIDArrayWID2` FOREIGN KEY (`ArrayWID`) REFERENCES `array_` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




