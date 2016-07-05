#Region "Microsoft.VisualBasic::99de73fb56d4c09e21ab2718bf0e60b1, ..\GCModeller\data\GO_gene-ontology\MySQL\Tables\term_synonym.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:49:20 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `term_synonym`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `term_synonym` (
'''   `term_id` int(11) NOT NULL,
'''   `term_synonym` varchar(996) DEFAULT NULL,
'''   `acc_synonym` varchar(255) DEFAULT NULL,
'''   `synonym_type_id` int(11) NOT NULL,
'''   `synonym_category_id` int(11) DEFAULT NULL,
'''   UNIQUE KEY `term_id` (`term_id`,`term_synonym`),
'''   KEY `synonym_type_id` (`synonym_type_id`),
'''   KEY `synonym_category_id` (`synonym_category_id`),
'''   KEY `ts1` (`term_id`),
'''   KEY `ts2` (`term_synonym`),
'''   KEY `ts3` (`term_id`,`term_synonym`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
''' 
''' /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
''' /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
''' /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
''' /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
''' /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
''' /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
''' /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
''' 
''' -- Dump completed on 2015-12-03 20:47:31
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("term_synonym", Database:="go")>
Public Class term_synonym: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("term_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property term_id As Long
    <DatabaseField("term_synonym"), PrimaryKey, DataType(MySqlDbType.VarChar, "996")> Public Property term_synonym As String
    <DatabaseField("acc_synonym"), DataType(MySqlDbType.VarChar, "255")> Public Property acc_synonym As String
    <DatabaseField("synonym_type_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property synonym_type_id As Long
    <DatabaseField("synonym_category_id"), DataType(MySqlDbType.Int64, "11")> Public Property synonym_category_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `term_synonym` (`term_id`, `term_synonym`, `acc_synonym`, `synonym_type_id`, `synonym_category_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `term_synonym` (`term_id`, `term_synonym`, `acc_synonym`, `synonym_type_id`, `synonym_category_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `term_synonym` WHERE `term_id`='{0}' and `term_synonym`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `term_synonym` SET `term_id`='{0}', `term_synonym`='{1}', `acc_synonym`='{2}', `synonym_type_id`='{3}', `synonym_category_id`='{4}' WHERE `term_id`='{5}' and `term_synonym`='{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, term_id, term_synonym)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, term_id, term_synonym, acc_synonym, synonym_type_id, synonym_category_id)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, term_id, term_synonym, acc_synonym, synonym_type_id, synonym_category_id)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, term_id, term_synonym, acc_synonym, synonym_type_id, synonym_category_id, term_id, term_synonym)
    End Function
#End Region
End Class


End Namespace
