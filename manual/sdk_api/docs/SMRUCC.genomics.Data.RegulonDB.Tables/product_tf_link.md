# product_tf_link
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](./index.md)_

--
 
 DROP TABLE IF EXISTS `product_tf_link`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `product_tf_link` (
 `transcription_factor_id` char(12) NOT NULL,
 `product_id` char(12) NOT NULL,
 `compon_coefficient` decimal(10,0) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




