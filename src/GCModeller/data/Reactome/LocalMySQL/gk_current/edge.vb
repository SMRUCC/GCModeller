#Region "Microsoft.VisualBasic::60b557f135780f614fec5b0a6b848431, data\Reactome\LocalMySQL\gk_current\edge.vb"

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

    '   Total Lines: 182
    '    Code Lines: 93
    ' Comment Lines: 67
    '   Blank Lines: 22
    '     File Size: 10.12 KB


    ' Class edge
    ' 
    '     Properties: DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates
    '                 sourceVertex, sourceVertex_class, targetVertex, targetVertex_class
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
''' DROP TABLE IF EXISTS `edge`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `edge` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `edgeType` int(10) DEFAULT NULL,
'''   `pathwayDiagram` int(10) unsigned DEFAULT NULL,
'''   `pathwayDiagram_class` varchar(64) DEFAULT NULL,
'''   `pointCoordinates` text,
'''   `sourceVertex` int(10) unsigned DEFAULT NULL,
'''   `sourceVertex_class` varchar(64) DEFAULT NULL,
'''   `targetVertex` int(10) unsigned DEFAULT NULL,
'''   `targetVertex_class` varchar(64) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `edgeType` (`edgeType`),
'''   KEY `pathwayDiagram` (`pathwayDiagram`),
'''   KEY `sourceVertex` (`sourceVertex`),
'''   KEY `targetVertex` (`targetVertex`),
'''   FULLTEXT KEY `pointCoordinates` (`pointCoordinates`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("edge", Database:="gk_current", SchemaSQL:="
CREATE TABLE `edge` (
  `DB_ID` int(10) unsigned NOT NULL,
  `edgeType` int(10) DEFAULT NULL,
  `pathwayDiagram` int(10) unsigned DEFAULT NULL,
  `pathwayDiagram_class` varchar(64) DEFAULT NULL,
  `pointCoordinates` text,
  `sourceVertex` int(10) unsigned DEFAULT NULL,
  `sourceVertex_class` varchar(64) DEFAULT NULL,
  `targetVertex` int(10) unsigned DEFAULT NULL,
  `targetVertex_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `edgeType` (`edgeType`),
  KEY `pathwayDiagram` (`pathwayDiagram`),
  KEY `sourceVertex` (`sourceVertex`),
  KEY `targetVertex` (`targetVertex`),
  FULLTEXT KEY `pointCoordinates` (`pointCoordinates`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class edge: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("edgeType"), DataType(MySqlDbType.Int64, "10"), Column(Name:="edgeType")> Public Property edgeType As Long
    <DatabaseField("pathwayDiagram"), DataType(MySqlDbType.Int64, "10"), Column(Name:="pathwayDiagram")> Public Property pathwayDiagram As Long
    <DatabaseField("pathwayDiagram_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="pathwayDiagram_class")> Public Property pathwayDiagram_class As String
    <DatabaseField("pointCoordinates"), DataType(MySqlDbType.Text), Column(Name:="pointCoordinates")> Public Property pointCoordinates As String
    <DatabaseField("sourceVertex"), DataType(MySqlDbType.Int64, "10"), Column(Name:="sourceVertex")> Public Property sourceVertex As Long
    <DatabaseField("sourceVertex_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="sourceVertex_class")> Public Property sourceVertex_class As String
    <DatabaseField("targetVertex"), DataType(MySqlDbType.Int64, "10"), Column(Name:="targetVertex")> Public Property targetVertex As Long
    <DatabaseField("targetVertex_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="targetVertex_class")> Public Property targetVertex_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `edge` (`DB_ID`, `edgeType`, `pathwayDiagram`, `pathwayDiagram_class`, `pointCoordinates`, `sourceVertex`, `sourceVertex_class`, `targetVertex`, `targetVertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `edge` (`DB_ID`, `edgeType`, `pathwayDiagram`, `pathwayDiagram_class`, `pointCoordinates`, `sourceVertex`, `sourceVertex_class`, `targetVertex`, `targetVertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `edge` (`DB_ID`, `edgeType`, `pathwayDiagram`, `pathwayDiagram_class`, `pointCoordinates`, `sourceVertex`, `sourceVertex_class`, `targetVertex`, `targetVertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `edge` (`DB_ID`, `edgeType`, `pathwayDiagram`, `pathwayDiagram_class`, `pointCoordinates`, `sourceVertex`, `sourceVertex_class`, `targetVertex`, `targetVertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `edge` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `edge` SET `DB_ID`='{0}', `edgeType`='{1}', `pathwayDiagram`='{2}', `pathwayDiagram_class`='{3}', `pointCoordinates`='{4}', `sourceVertex`='{5}', `sourceVertex_class`='{6}', `targetVertex`='{7}', `targetVertex_class`='{8}' WHERE `DB_ID` = '{9}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `edge` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `edge` (`DB_ID`, `edgeType`, `pathwayDiagram`, `pathwayDiagram_class`, `pointCoordinates`, `sourceVertex`, `sourceVertex_class`, `targetVertex`, `targetVertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates, sourceVertex, sourceVertex_class, targetVertex, targetVertex_class)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `edge` (`DB_ID`, `edgeType`, `pathwayDiagram`, `pathwayDiagram_class`, `pointCoordinates`, `sourceVertex`, `sourceVertex_class`, `targetVertex`, `targetVertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates, sourceVertex, sourceVertex_class, targetVertex, targetVertex_class)
        Else
        Return String.Format(INSERT_SQL, DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates, sourceVertex, sourceVertex_class, targetVertex, targetVertex_class)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{edgeType}', '{pathwayDiagram}', '{pathwayDiagram_class}', '{pointCoordinates}', '{sourceVertex}', '{sourceVertex_class}', '{targetVertex}', '{targetVertex_class}')"
        Else
            Return $"('{DB_ID}', '{edgeType}', '{pathwayDiagram}', '{pathwayDiagram_class}', '{pointCoordinates}', '{sourceVertex}', '{sourceVertex_class}', '{targetVertex}', '{targetVertex_class}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `edge` (`DB_ID`, `edgeType`, `pathwayDiagram`, `pathwayDiagram_class`, `pointCoordinates`, `sourceVertex`, `sourceVertex_class`, `targetVertex`, `targetVertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates, sourceVertex, sourceVertex_class, targetVertex, targetVertex_class)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `edge` (`DB_ID`, `edgeType`, `pathwayDiagram`, `pathwayDiagram_class`, `pointCoordinates`, `sourceVertex`, `sourceVertex_class`, `targetVertex`, `targetVertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates, sourceVertex, sourceVertex_class, targetVertex, targetVertex_class)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates, sourceVertex, sourceVertex_class, targetVertex, targetVertex_class)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `edge` SET `DB_ID`='{0}', `edgeType`='{1}', `pathwayDiagram`='{2}', `pathwayDiagram_class`='{3}', `pointCoordinates`='{4}', `sourceVertex`='{5}', `sourceVertex_class`='{6}', `targetVertex`='{7}', `targetVertex_class`='{8}' WHERE `DB_ID` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates, sourceVertex, sourceVertex_class, targetVertex, targetVertex_class, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As edge
                         Return DirectCast(MyClass.MemberwiseClone, edge)
                     End Function
End Class


End Namespace
