---
title: composseqdimenswidcomposseqwid
---

# composseqdimenswidcomposseqwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `composseqdimenswidcomposseqwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `composseqdimenswidcomposseqwid` (
 `CompositeSequenceDimensionWID` bigint(20) NOT NULL,
 `CompositeSequenceWID` bigint(20) NOT NULL,
 KEY `FK_ComposSeqDimensWIDComposS1` (`CompositeSequenceDimensionWID`),
 KEY `FK_ComposSeqDimensWIDComposS2` (`CompositeSequenceWID`),
 CONSTRAINT `FK_ComposSeqDimensWIDComposS1` FOREIGN KEY (`CompositeSequenceDimensionWID`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ComposSeqDimensWIDComposS2` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




