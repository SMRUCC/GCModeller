---
title: id_to_externalidentifier
---

# id_to_externalidentifier
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current_dn](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current_dn.html)_

--
 
 DROP TABLE IF EXISTS `id_to_externalidentifier`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `id_to_externalidentifier` (
 `id` int(32) NOT NULL DEFAULT '0',
 `referenceDatabase` varchar(255) NOT NULL DEFAULT '',
 `externalIdentifier` varchar(32) NOT NULL DEFAULT '',
 `description` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`,`referenceDatabase`,`externalIdentifier`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




