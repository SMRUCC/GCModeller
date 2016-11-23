# dict_exp_result_types
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `dict_exp_result_types`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dict_exp_result_types` (
 `exp_result_type_guid` int(11) NOT NULL DEFAULT '0',
 `name` varchar(100) DEFAULT NULL,
 PRIMARY KEY (`exp_result_type_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




