---
title: composseqwidrepocomposmapwid
---

# composseqwidrepocomposmapwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `composseqwidrepocomposmapwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `composseqwidrepocomposmapwid` (
 `CompositeSequenceWID` bigint(20) NOT NULL,
 `ReporterCompositeMapWID` bigint(20) NOT NULL,
 KEY `FK_ComposSeqWIDRepoComposMap1` (`CompositeSequenceWID`),
 KEY `FK_ComposSeqWIDRepoComposMap2` (`ReporterCompositeMapWID`),
 CONSTRAINT `FK_ComposSeqWIDRepoComposMap1` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ComposSeqWIDRepoComposMap2` FOREIGN KEY (`ReporterCompositeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




