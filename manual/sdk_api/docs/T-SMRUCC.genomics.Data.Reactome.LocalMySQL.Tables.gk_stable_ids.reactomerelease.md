---
title: reactomerelease
---

# reactomerelease
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_stable_ids](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_stable_ids.html)_

--
 
 DROP TABLE IF EXISTS `reactomerelease`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reactomerelease` (
 `DB_ID` int(12) unsigned NOT NULL AUTO_INCREMENT,
 `release_num` int(12) NOT NULL,
 `database_name` text NOT NULL,
 PRIMARY KEY (`DB_ID`)
 ) ENGINE=MyISAM AUTO_INCREMENT=869251 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




