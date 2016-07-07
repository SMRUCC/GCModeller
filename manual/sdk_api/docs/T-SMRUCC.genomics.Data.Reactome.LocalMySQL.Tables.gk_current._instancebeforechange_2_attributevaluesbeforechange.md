---
title: _instancebeforechange_2_attributevaluesbeforechange
---

# _instancebeforechange_2_attributevaluesbeforechange
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `_instancebeforechange_2_attributevaluesbeforechange`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `_instancebeforechange_2_attributevaluesbeforechange` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `attributeValuesBeforeChange_rank` int(10) unsigned DEFAULT NULL,
 `attributeValuesBeforeChange` int(10) unsigned DEFAULT NULL,
 `attributeValuesBeforeChange_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `attributeValuesBeforeChange` (`attributeValuesBeforeChange`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




