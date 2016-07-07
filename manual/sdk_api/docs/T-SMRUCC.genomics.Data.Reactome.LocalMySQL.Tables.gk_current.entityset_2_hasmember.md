---
title: entityset_2_hasmember
---

# entityset_2_hasmember
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `entityset_2_hasmember`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entityset_2_hasmember` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `hasMember_rank` int(10) unsigned DEFAULT NULL,
 `hasMember` int(10) unsigned DEFAULT NULL,
 `hasMember_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `hasMember` (`hasMember`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




