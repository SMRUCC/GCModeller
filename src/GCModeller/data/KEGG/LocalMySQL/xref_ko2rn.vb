#Region "Microsoft.VisualBasic::7fd2e65a786a00bfd72949a69020ecf8, data\KEGG\LocalMySQL\xref_ko2rn.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    ' Class xref_ko2rn
    ' 
    '     Properties: ko, rn, uid, url
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:06:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL

''' <summary>
''' ```SQL
''' kegg orthology corss reference to kegg reactions database.
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `xref_ko2rn`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `xref_ko2rn` (
'''   `uid` int(11) NOT NULL AUTO_INCREMENT,
'''   `ko` varchar(45) NOT NULL,
'''   `rn` varchar(45) NOT NULL,
'''   `url` text,
'''   PRIMARY KEY (`ko`,`rn`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='kegg orthology corss reference to kegg reactions database.';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' -- Dumping events for database 'jp_kegg2'
''' --
''' 
''' --
''' -- Dumping routines for database 'jp_kegg2'
''' --
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
''' -- Dump completed on 2017-03-07 22:10:00
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("xref_ko2rn", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `xref_ko2rn` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `ko` varchar(45) NOT NULL,
  `rn` varchar(45) NOT NULL,
  `url` text,
  PRIMARY KEY (`ko`,`rn`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='kegg orthology corss reference to kegg reactions database.';")>
Public Class xref_ko2rn: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property uid As Long
    <DatabaseField("ko"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property ko As String
    <DatabaseField("rn"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property rn As String
    <DatabaseField("url"), DataType(MySqlDbType.Text)> Public Property url As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `xref_ko2rn` (`ko`, `rn`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `xref_ko2rn` (`ko`, `rn`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `xref_ko2rn` WHERE `ko`='{0}' and `rn`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `xref_ko2rn` SET `uid`='{0}', `ko`='{1}', `rn`='{2}', `url`='{3}' WHERE `ko`='{4}' and `rn`='{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `xref_ko2rn` WHERE `ko`='{0}' and `rn`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ko, rn)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `xref_ko2rn` (`ko`, `rn`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ko, rn, url)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{ko}', '{rn}', '{url}', '{3}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `xref_ko2rn` (`ko`, `rn`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ko, rn, url)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `xref_ko2rn` SET `uid`='{0}', `ko`='{1}', `rn`='{2}', `url`='{3}' WHERE `ko`='{4}' and `rn`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, ko, rn, url, ko, rn)
    End Function
#End Region
End Class


End Namespace
