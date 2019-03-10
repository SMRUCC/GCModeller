# text_index_entry
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](./index.md)_

--
 
 DROP TABLE IF EXISTS `text_index_entry`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `text_index_entry` (
 `id` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `field` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `text` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




