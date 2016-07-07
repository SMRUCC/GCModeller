---
title: referencegeneproduct_2_referencegene
---

# referencegeneproduct_2_referencegene
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `referencegeneproduct_2_referencegene`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `referencegeneproduct_2_referencegene` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `referenceGene_rank` int(10) unsigned DEFAULT NULL,
 `referenceGene` int(10) unsigned DEFAULT NULL,
 `referenceGene_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `referenceGene` (`referenceGene`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




