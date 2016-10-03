#Region "Microsoft.VisualBasic::f19ac876605b350a61e0d8620ce1a566, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\edge.vb"

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

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:15:49 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
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
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("edge", Database:="gk_current")>
Public Class edge: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("edgeType"), DataType(MySqlDbType.Int64, "10")> Public Property edgeType As Long
    <DatabaseField("pathwayDiagram"), DataType(MySqlDbType.Int64, "10")> Public Property pathwayDiagram As Long
    <DatabaseField("pathwayDiagram_class"), DataType(MySqlDbType.VarChar, "64")> Public Property pathwayDiagram_class As String
    <DatabaseField("pointCoordinates"), DataType(MySqlDbType.Text)> Public Property pointCoordinates As String
    <DatabaseField("sourceVertex"), DataType(MySqlDbType.Int64, "10")> Public Property sourceVertex As Long
    <DatabaseField("sourceVertex_class"), DataType(MySqlDbType.VarChar, "64")> Public Property sourceVertex_class As String
    <DatabaseField("targetVertex"), DataType(MySqlDbType.Int64, "10")> Public Property targetVertex As Long
    <DatabaseField("targetVertex_class"), DataType(MySqlDbType.VarChar, "64")> Public Property targetVertex_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `edge` (`DB_ID`, `edgeType`, `pathwayDiagram`, `pathwayDiagram_class`, `pointCoordinates`, `sourceVertex`, `sourceVertex_class`, `targetVertex`, `targetVertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `edge` (`DB_ID`, `edgeType`, `pathwayDiagram`, `pathwayDiagram_class`, `pointCoordinates`, `sourceVertex`, `sourceVertex_class`, `targetVertex`, `targetVertex_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `edge` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `edge` SET `DB_ID`='{0}', `edgeType`='{1}', `pathwayDiagram`='{2}', `pathwayDiagram_class`='{3}', `pointCoordinates`='{4}', `sourceVertex`='{5}', `sourceVertex_class`='{6}', `targetVertex`='{7}', `targetVertex_class`='{8}' WHERE `DB_ID` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates, sourceVertex, sourceVertex_class, targetVertex, targetVertex_class)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates, sourceVertex, sourceVertex_class, targetVertex, targetVertex_class)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, edgeType, pathwayDiagram, pathwayDiagram_class, pointCoordinates, sourceVertex, sourceVertex_class, targetVertex, targetVertex_class, DB_ID)
    End Function
#End Region
End Class


End Namespace
