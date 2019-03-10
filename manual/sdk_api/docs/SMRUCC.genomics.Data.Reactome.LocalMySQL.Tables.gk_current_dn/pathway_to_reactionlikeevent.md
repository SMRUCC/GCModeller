﻿# pathway_to_reactionlikeevent
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current_dn](./index.md)_

--
 
 DROP TABLE IF EXISTS `pathway_to_reactionlikeevent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathway_to_reactionlikeevent` (
 `pathwayId` int(32) NOT NULL DEFAULT '0',
 `reactionLikeEventId` int(32) NOT NULL DEFAULT '0',
 PRIMARY KEY (`pathwayId`,`reactionLikeEventId`),
 KEY `reactionLikeEventId` (`reactionLikeEventId`),
 CONSTRAINT `Pathway_To_ReactionLikeEvent_ibfk_1` FOREIGN KEY (`pathwayId`) REFERENCES `pathway` (`id`),
 CONSTRAINT `Pathway_To_ReactionLikeEvent_ibfk_2` FOREIGN KEY (`reactionLikeEventId`) REFERENCES `reactionlikeevent` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




