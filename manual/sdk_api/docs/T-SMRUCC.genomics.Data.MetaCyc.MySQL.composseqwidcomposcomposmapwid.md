---
title: composseqwidcomposcomposmapwid
---

# composseqwidcomposcomposmapwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `composseqwidcomposcomposmapwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `composseqwidcomposcomposmapwid` (
 `CompositeSequenceWID` bigint(20) NOT NULL,
 `CompositeCompositeMapWID` bigint(20) NOT NULL,
 KEY `FK_ComposSeqWIDComposComposM1` (`CompositeSequenceWID`),
 KEY `FK_ComposSeqWIDComposComposM2` (`CompositeCompositeMapWID`),
 CONSTRAINT `FK_ComposSeqWIDComposComposM1` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ComposSeqWIDComposComposM2` FOREIGN KEY (`CompositeCompositeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




