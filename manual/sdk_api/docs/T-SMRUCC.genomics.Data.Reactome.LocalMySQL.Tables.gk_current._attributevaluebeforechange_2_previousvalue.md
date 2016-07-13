---
title: _attributevaluebeforechange_2_previousvalue
---

# _attributevaluebeforechange_2_previousvalue
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `_attributevaluebeforechange_2_previousvalue`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `_attributevaluebeforechange_2_previousvalue` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `previousValue_rank` int(10) unsigned DEFAULT NULL,
 `previousValue` text,
 KEY `DB_ID` (`DB_ID`),
 FULLTEXT KEY `previousValue` (`previousValue`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




