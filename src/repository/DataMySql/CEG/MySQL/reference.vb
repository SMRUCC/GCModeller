#Region "Microsoft.VisualBasic::025c884e86814cca502abd996ba08413, ..\repository\DataMySql\CEG\MySQL\reference.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 11:05:12 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace CEG.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `reference`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reference` (
'''   `oganismid` int(4) DEFAULT NULL,
'''   `abbr` varchar(5) NOT NULL,
'''   `oganism` varchar(255) DEFAULT NULL,
'''   `pubmedid` varchar(20) DEFAULT NULL,
'''   `pub_title` text NOT NULL
''' ) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
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
''' -- Dump completed on 2015-10-09  2:15:39
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reference", Database:="ceg", SchemaSQL:="
CREATE TABLE `reference` (
  `oganismid` int(4) DEFAULT NULL,
  `abbr` varchar(5) NOT NULL,
  `oganism` varchar(255) DEFAULT NULL,
  `pubmedid` varchar(20) DEFAULT NULL,
  `pub_title` text NOT NULL
) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;")>
Public Class reference: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("oganismid"), DataType(MySqlDbType.Int64, "4")> Public Property oganismid As Long
    <DatabaseField("abbr"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property abbr As String
    <DatabaseField("oganism"), DataType(MySqlDbType.VarChar, "255")> Public Property oganism As String
    <DatabaseField("pubmedid"), DataType(MySqlDbType.VarChar, "20")> Public Property pubmedid As String
    <DatabaseField("pub_title"), NotNull, DataType(MySqlDbType.Text)> Public Property pub_title As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `reference` (`oganismid`, `abbr`, `oganism`, `pubmedid`, `pub_title`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `reference` (`oganismid`, `abbr`, `oganism`, `pubmedid`, `pub_title`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `reference` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `reference` SET `oganismid`='{0}', `abbr`='{1}', `oganism`='{2}', `pubmedid`='{3}', `pub_title`='{4}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `reference` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `reference` (`oganismid`, `abbr`, `oganism`, `pubmedid`, `pub_title`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, oganismid, abbr, oganism, pubmedid, pub_title)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{oganismid}', '{abbr}', '{oganism}', '{pubmedid}', '{pub_title}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reference` (`oganismid`, `abbr`, `oganism`, `pubmedid`, `pub_title`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, oganismid, abbr, oganism, pubmedid, pub_title)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `reference` SET `oganismid`='{0}', `abbr`='{1}', `oganism`='{2}', `pubmedid`='{3}', `pub_title`='{4}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace

