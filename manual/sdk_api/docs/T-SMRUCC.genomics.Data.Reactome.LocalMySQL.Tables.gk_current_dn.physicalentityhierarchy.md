---
title: physicalentityhierarchy
---

# physicalentityhierarchy
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current_dn](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current_dn.html)_

--
 
 DROP TABLE IF EXISTS `physicalentityhierarchy`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `physicalentityhierarchy` (
 `physicalEntityId` int(32) NOT NULL DEFAULT '0',
 `childPhysicalEntityId` int(32) NOT NULL DEFAULT '0',
 PRIMARY KEY (`physicalEntityId`,`childPhysicalEntityId`),
 CONSTRAINT `PhysicalEntityHierarchy_ibfk_1` FOREIGN KEY (`physicalEntityId`) REFERENCES `physicalentity` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




