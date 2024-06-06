#Region "Microsoft.VisualBasic::b347b085eb37d11d53d01ac0680f6613, data\Reactome\LocalMySQL\gk_current\vertexsearchableterm_2_vertex.vb"

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


    ' Code Statistics:

    '   Total Lines: 176
    '    Code Lines: 79 (44.89%)
    ' Comment Lines: 75 (42.61%)
    '    - Xml Docs: 92.00%
    ' 
    '   Blank Lines: 22 (12.50%)
    '     File Size: 7.24 KB


    ' Class vertexsearchableterm_2_vertex
    ' 
    '     Properties: DB_ID, vertex, vertex_class, vertex_rank
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

REM  Dump @2018/5/23 13:13:41


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `vertexsearchableterm_2_vertex`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `vertexsearchableterm_2_vertex` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `vertex_rank` int(10) unsigned DEFAULT NULL,
'''   `vertex` int(10) unsigned DEFAULT NULL,
'''   `vertex_class` varchar(64) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `vertex` (`vertex`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' -- Dumping events for database 'gk_current'
''' --
''' 
''' --
''' -- Dumping routines for database 'gk_current'
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
''' -- Dump completed on 2017-03-29 21:34:14
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("vertexsearchableterm_2_vertex", Database:="gk_current", SchemaSQL:="
CREATE TABLE `vertexsearchableterm_2_vertex` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `vertex_rank` int(10) unsigned DEFAULT NULL,
  `vertex` int(10) unsigned DEFAULT NULL,
  `vertex_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `vertex` (`vertex`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class vertexsearchableterm_2_vertex: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("vertex_rank"), DataType(MySqlDbType.Int64, "10"), Column(Name:="vertex_rank")> Public Property vertex_rank As Long
    <DatabaseField("vertex"), DataType(MySqlDbType.Int64, "10"), Column(Name:="vertex")> Public Property vertex As Long
    <DatabaseField("vertex_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="vertex_class")> Public Property vertex_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `vertexsearchableterm_2_vertex` (`DB_ID`, `vertex_rank`, `vertex`, `vertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `vertexsearchableterm_2_vertex` (`DB_ID`, `vertex_rank`, `vertex`, `vertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `vertexsearchableterm_2_vertex` (`DB_ID`, `vertex_rank`, `vertex`, `vertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `vertexsearchableterm_2_vertex` (`DB_ID`, `vertex_rank`, `vertex`, `vertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `vertexsearchableterm_2_vertex` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `vertexsearchableterm_2_vertex` SET `DB_ID`='{0}', `vertex_rank`='{1}', `vertex`='{2}', `vertex_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `vertexsearchableterm_2_vertex` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `vertexsearchableterm_2_vertex` (`DB_ID`, `vertex_rank`, `vertex`, `vertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, vertex_rank, vertex, vertex_class)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `vertexsearchableterm_2_vertex` (`DB_ID`, `vertex_rank`, `vertex`, `vertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, vertex_rank, vertex, vertex_class)
        Else
        Return String.Format(INSERT_SQL, DB_ID, vertex_rank, vertex, vertex_class)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{vertex_rank}', '{vertex}', '{vertex_class}')"
        Else
            Return $"('{DB_ID}', '{vertex_rank}', '{vertex}', '{vertex_class}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `vertexsearchableterm_2_vertex` (`DB_ID`, `vertex_rank`, `vertex`, `vertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, vertex_rank, vertex, vertex_class)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `vertexsearchableterm_2_vertex` (`DB_ID`, `vertex_rank`, `vertex`, `vertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, vertex_rank, vertex, vertex_class)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, vertex_rank, vertex, vertex_class)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `vertexsearchableterm_2_vertex` SET `DB_ID`='{0}', `vertex_rank`='{1}', `vertex`='{2}', `vertex_class`='{3}' WHERE `DB_ID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, vertex_rank, vertex, vertex_class, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As vertexsearchableterm_2_vertex
                         Return DirectCast(MyClass.MemberwiseClone, vertexsearchableterm_2_vertex)
                     End Function
End Class


End Namespace
