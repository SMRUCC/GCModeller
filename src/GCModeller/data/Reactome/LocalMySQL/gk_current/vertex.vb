#Region "Microsoft.VisualBasic::1d8e7329ec1a834ce27613bdda1378bc, data\Reactome\LocalMySQL\gk_current\vertex.vb"

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

    ' Class vertex
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 9:40:28 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `vertex`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `vertex` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `height` int(10) DEFAULT NULL,
'''   `pathwayDiagram` int(10) unsigned DEFAULT NULL,
'''   `pathwayDiagram_class` varchar(64) DEFAULT NULL,
'''   `representedInstance` int(10) unsigned DEFAULT NULL,
'''   `representedInstance_class` varchar(64) DEFAULT NULL,
'''   `width` int(10) DEFAULT NULL,
'''   `x` int(10) DEFAULT NULL,
'''   `y` int(10) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `height` (`height`),
'''   KEY `pathwayDiagram` (`pathwayDiagram`),
'''   KEY `representedInstance` (`representedInstance`),
'''   KEY `width` (`width`),
'''   KEY `x` (`x`),
'''   KEY `y` (`y`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("vertex", Database:="gk_current", SchemaSQL:="
CREATE TABLE `vertex` (
  `DB_ID` int(10) unsigned NOT NULL,
  `height` int(10) DEFAULT NULL,
  `pathwayDiagram` int(10) unsigned DEFAULT NULL,
  `pathwayDiagram_class` varchar(64) DEFAULT NULL,
  `representedInstance` int(10) unsigned DEFAULT NULL,
  `representedInstance_class` varchar(64) DEFAULT NULL,
  `width` int(10) DEFAULT NULL,
  `x` int(10) DEFAULT NULL,
  `y` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `height` (`height`),
  KEY `pathwayDiagram` (`pathwayDiagram`),
  KEY `representedInstance` (`representedInstance`),
  KEY `width` (`width`),
  KEY `x` (`x`),
  KEY `y` (`y`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class vertex: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("height"), DataType(MySqlDbType.Int64, "10")> Public Property height As Long
    <DatabaseField("pathwayDiagram"), DataType(MySqlDbType.Int64, "10")> Public Property pathwayDiagram As Long
    <DatabaseField("pathwayDiagram_class"), DataType(MySqlDbType.VarChar, "64")> Public Property pathwayDiagram_class As String
    <DatabaseField("representedInstance"), DataType(MySqlDbType.Int64, "10")> Public Property representedInstance As Long
    <DatabaseField("representedInstance_class"), DataType(MySqlDbType.VarChar, "64")> Public Property representedInstance_class As String
    <DatabaseField("width"), DataType(MySqlDbType.Int64, "10")> Public Property width As Long
    <DatabaseField("x"), DataType(MySqlDbType.Int64, "10")> Public Property x As Long
    <DatabaseField("y"), DataType(MySqlDbType.Int64, "10")> Public Property y As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `vertex` (`DB_ID`, `height`, `pathwayDiagram`, `pathwayDiagram_class`, `representedInstance`, `representedInstance_class`, `width`, `x`, `y`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `vertex` (`DB_ID`, `height`, `pathwayDiagram`, `pathwayDiagram_class`, `representedInstance`, `representedInstance_class`, `width`, `x`, `y`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `vertex` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `vertex` SET `DB_ID`='{0}', `height`='{1}', `pathwayDiagram`='{2}', `pathwayDiagram_class`='{3}', `representedInstance`='{4}', `representedInstance_class`='{5}', `width`='{6}', `x`='{7}', `y`='{8}' WHERE `DB_ID` = '{9}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `vertex` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `vertex` (`DB_ID`, `height`, `pathwayDiagram`, `pathwayDiagram_class`, `representedInstance`, `representedInstance_class`, `width`, `x`, `y`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, height, pathwayDiagram, pathwayDiagram_class, representedInstance, representedInstance_class, width, x, y)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{height}', '{pathwayDiagram}', '{pathwayDiagram_class}', '{representedInstance}', '{representedInstance_class}', '{width}', '{x}', '{y}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `vertex` (`DB_ID`, `height`, `pathwayDiagram`, `pathwayDiagram_class`, `representedInstance`, `representedInstance_class`, `width`, `x`, `y`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, height, pathwayDiagram, pathwayDiagram_class, representedInstance, representedInstance_class, width, x, y)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `vertex` SET `DB_ID`='{0}', `height`='{1}', `pathwayDiagram`='{2}', `pathwayDiagram_class`='{3}', `representedInstance`='{4}', `representedInstance_class`='{5}', `width`='{6}', `x`='{7}', `y`='{8}' WHERE `DB_ID` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, height, pathwayDiagram, pathwayDiagram_class, representedInstance, representedInstance_class, width, x, y, DB_ID)
    End Function
#End Region
End Class


End Namespace
