---
title: reactionlikeevent_to_physicalentity
---

# reactionlikeevent_to_physicalentity
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current_dn](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current_dn.html)_

--
 
 DROP TABLE IF EXISTS `reactionlikeevent_to_physicalentity`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reactionlikeevent_to_physicalentity` (
 `reactionLikeEventId` int(32) NOT NULL DEFAULT '0',
 `physicalEntityId` int(32) NOT NULL DEFAULT '0',
 PRIMARY KEY (`reactionLikeEventId`,`physicalEntityId`),
 KEY `physicalEntityId` (`physicalEntityId`),
 CONSTRAINT `ReactionLikeEvent_To_PhysicalEntity_ibfk_1` FOREIGN KEY (`reactionLikeEventId`) REFERENCES `reactionlikeevent` (`id`),
 CONSTRAINT `ReactionLikeEvent_To_PhysicalEntity_ibfk_2` FOREIGN KEY (`physicalEntityId`) REFERENCES `physicalentity` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-10-08 22:03:45




