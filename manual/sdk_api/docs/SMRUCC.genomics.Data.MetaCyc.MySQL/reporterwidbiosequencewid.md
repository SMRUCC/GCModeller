﻿# reporterwidbiosequencewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `reporterwidbiosequencewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reporterwidbiosequencewid` (
 `ReporterWID` bigint(20) NOT NULL,
 `BioSequenceWID` bigint(20) NOT NULL,
 KEY `FK_ReporterWIDBioSequenceWID1` (`ReporterWID`),
 CONSTRAINT `FK_ReporterWIDBioSequenceWID1` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




