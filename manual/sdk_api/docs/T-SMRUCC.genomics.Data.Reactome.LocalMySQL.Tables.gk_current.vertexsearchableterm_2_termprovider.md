---
title: vertexsearchableterm_2_termprovider
---

# vertexsearchableterm_2_termprovider
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `vertexsearchableterm_2_termprovider`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `vertexsearchableterm_2_termprovider` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `termProvider_rank` int(10) unsigned DEFAULT NULL,
 `termProvider` int(10) unsigned DEFAULT NULL,
 `termProvider_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `termProvider` (`termProvider`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




