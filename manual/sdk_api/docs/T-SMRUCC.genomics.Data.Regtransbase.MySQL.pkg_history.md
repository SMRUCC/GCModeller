---
title: pkg_history
---

# pkg_history
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `pkg_history`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pkg_history` (
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `event_date` varchar(100) NOT NULL DEFAULT '',
 `event_operation` varchar(100) NOT NULL DEFAULT '',
 `user_by_id` int(11) DEFAULT '0',
 `user_by_name` varchar(100) DEFAULT '',
 `user_by_role` varchar(100) DEFAULT '',
 `user_by_email` varchar(100) DEFAULT '',
 `user_by_phone` varchar(100) DEFAULT '',
 `user_to_id` int(11) DEFAULT '0',
 `user_to_name` varchar(100) DEFAULT '',
 `user_to_role` varchar(100) DEFAULT '',
 `user_to_email` varchar(100) DEFAULT '',
 `user_to_phone` varchar(100) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




