---
title: pathwayhierarchy
---

# pathwayhierarchy
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current_dn](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current_dn.html)_

--
 
 DROP TABLE IF EXISTS `pathwayhierarchy`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathwayhierarchy` (
 `pathwayId` int(32) NOT NULL DEFAULT '0',
 `childPathwayId` int(32) NOT NULL DEFAULT '0',
 PRIMARY KEY (`pathwayId`,`childPathwayId`),
 CONSTRAINT `PathwayHierarchy_ibfk_1` FOREIGN KEY (`pathwayId`) REFERENCES `pathway` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




