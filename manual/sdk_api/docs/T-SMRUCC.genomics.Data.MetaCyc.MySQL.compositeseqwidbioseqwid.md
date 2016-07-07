---
title: compositeseqwidbioseqwid
---

# compositeseqwidbioseqwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `compositeseqwidbioseqwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `compositeseqwidbioseqwid` (
 `CompositeSequenceWID` bigint(20) NOT NULL,
 `BioSequenceWID` bigint(20) NOT NULL,
 KEY `FK_CompositeSeqWIDBioSeqWID1` (`CompositeSequenceWID`),
 CONSTRAINT `FK_CompositeSeqWIDBioSeqWID1` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




