#Region "Microsoft.VisualBasic::1727ced0747e6514b8e9871a6fcda265, DataMySql\CEG\MySQL\reference.vb"

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

    ' Class reference
    ' 
    '     Properties: abbr, oganism, oganismid, pub_title, pubmedid
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:39


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

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
) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;")>
Public Class reference: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("oganismid"), DataType(MySqlDbType.Int64, "4"), Column(Name:="oganismid")> Public Property oganismid As Long
    <DatabaseField("abbr"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="abbr")> Public Property abbr As String
    <DatabaseField("oganism"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="oganism")> Public Property oganism As String
    <DatabaseField("pubmedid"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="pubmedid")> Public Property pubmedid As String
    <DatabaseField("pub_title"), NotNull, DataType(MySqlDbType.Text), Column(Name:="pub_title")> Public Property pub_title As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `reference` (`oganismid`, `abbr`, `oganism`, `pubmedid`, `pub_title`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `reference` (`oganismid`, `abbr`, `oganism`, `pubmedid`, `pub_title`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `reference` (`oganismid`, `abbr`, `oganism`, `pubmedid`, `pub_title`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `reference` (`oganismid`, `abbr`, `oganism`, `pubmedid`, `pub_title`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `reference` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `reference` SET `oganismid`='{0}', `abbr`='{1}', `oganism`='{2}', `pubmedid`='{3}', `pub_title`='{4}' WHERE ;</SQL>

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
''' ```SQL
''' INSERT INTO `reference` (`oganismid`, `abbr`, `oganism`, `pubmedid`, `pub_title`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, oganismid, abbr, oganism, pubmedid, pub_title)
        Else
        Return String.Format(INSERT_SQL, oganismid, abbr, oganism, pubmedid, pub_title)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{oganismid}', '{abbr}', '{oganism}', '{pubmedid}', '{pub_title}')"
        Else
            Return $"('{oganismid}', '{abbr}', '{oganism}', '{pubmedid}', '{pub_title}')"
        End If
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
''' REPLACE INTO `reference` (`oganismid`, `abbr`, `oganism`, `pubmedid`, `pub_title`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, oganismid, abbr, oganism, pubmedid, pub_title)
        Else
        Return String.Format(REPLACE_SQL, oganismid, abbr, oganism, pubmedid, pub_title)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As reference
                         Return DirectCast(MyClass.MemberwiseClone, reference)
                     End Function
End Class


End Namespace
