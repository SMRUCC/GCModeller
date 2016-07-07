---
title: frontpage_2_frontpageitem
---

# frontpage_2_frontpageitem
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `frontpage_2_frontpageitem`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `frontpage_2_frontpageitem` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `frontPageItem_rank` int(10) unsigned DEFAULT NULL,
 `frontPageItem` int(10) unsigned DEFAULT NULL,
 `frontPageItem_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `frontPageItem` (`frontPageItem`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




