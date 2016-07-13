---
title: db_users
---

# db_users
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `db_users`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `db_users` (
 `id` int(11) NOT NULL DEFAULT '0',
 `user_role_id` int(11) DEFAULT NULL,
 `name` varchar(20) DEFAULT NULL,
 `full_name` varchar(100) DEFAULT NULL,
 `phone` varchar(100) DEFAULT '',
 `email` varchar(100) DEFAULT '',
 `fl_active` int(1) DEFAULT NULL,
 PRIMARY KEY (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




