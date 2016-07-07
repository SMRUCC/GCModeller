---
title: composgrpwidcompossequencewid
---

# composgrpwidcompossequencewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `composgrpwidcompossequencewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `composgrpwidcompossequencewid` (
 `CompositeGroupWID` bigint(20) NOT NULL,
 `CompositeSequenceWID` bigint(20) NOT NULL,
 KEY `FK_ComposGrpWIDComposSequenc1` (`CompositeGroupWID`),
 KEY `FK_ComposGrpWIDComposSequenc2` (`CompositeSequenceWID`),
 CONSTRAINT `FK_ComposGrpWIDComposSequenc1` FOREIGN KEY (`CompositeGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ComposGrpWIDComposSequenc2` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




