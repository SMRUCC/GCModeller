---
title: imageacquisitionwidimagewid
---

# imageacquisitionwidimagewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `imageacquisitionwidimagewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `imageacquisitionwidimagewid` (
 `ImageAcquisitionWID` bigint(20) NOT NULL,
 `ImageWID` bigint(20) NOT NULL,
 KEY `FK_ImageAcquisitionWIDImageW1` (`ImageAcquisitionWID`),
 KEY `FK_ImageAcquisitionWIDImageW2` (`ImageWID`),
 CONSTRAINT `FK_ImageAcquisitionWIDImageW1` FOREIGN KEY (`ImageAcquisitionWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ImageAcquisitionWIDImageW2` FOREIGN KEY (`ImageWID`) REFERENCES `image` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




