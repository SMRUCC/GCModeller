---
title: referenceisoform_2_isoformparent
---

# referenceisoform_2_isoformparent
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `referenceisoform_2_isoformparent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `referenceisoform_2_isoformparent` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `isoformParent_rank` int(10) unsigned DEFAULT NULL,
 `isoformParent` int(10) unsigned DEFAULT NULL,
 `isoformParent_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `isoformParent` (`isoformParent`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




