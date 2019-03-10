﻿# labeledextractwidcompoundwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `labeledextractwidcompoundwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `labeledextractwidcompoundwid` (
 `LabeledExtractWID` bigint(20) NOT NULL,
 `CompoundWID` bigint(20) NOT NULL,
 KEY `FK_LabeledExtractWIDCompound1` (`LabeledExtractWID`),
 KEY `FK_LabeledExtractWIDCompound2` (`CompoundWID`),
 CONSTRAINT `FK_LabeledExtractWIDCompound1` FOREIGN KEY (`LabeledExtractWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_LabeledExtractWIDCompound2` FOREIGN KEY (`CompoundWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




