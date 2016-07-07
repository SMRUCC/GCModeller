---
title: proteinwidfunctionwid
---

# proteinwidfunctionwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `proteinwidfunctionwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `proteinwidfunctionwid` (
 `ProteinWID` bigint(20) NOT NULL,
 `FunctionWID` bigint(20) NOT NULL,
 KEY `FK_ProteinWIDFunctionWID2` (`ProteinWID`),
 KEY `FK_ProteinWIDFunctionWID3` (`FunctionWID`),
 CONSTRAINT `FK_ProteinWIDFunctionWID2` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ProteinWIDFunctionWID3` FOREIGN KEY (`FunctionWID`) REFERENCES `function` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




