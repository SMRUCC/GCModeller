#Region "Microsoft.VisualBasic::1aa774ff59a03b7da214f4dcf5ad455e, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\physicalentity.vb"

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
''' DROP TABLE IF EXISTS `physicalentity`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `physicalentity` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `authored` int(10) unsigned DEFAULT NULL,
'''   `authored_class` varchar(64) DEFAULT NULL,
'''   `cellType` int(10) unsigned DEFAULT NULL,
'''   `cellType_class` varchar(64) DEFAULT NULL,
'''   `definition` text,
'''   `goCellularComponent` int(10) unsigned DEFAULT NULL,
'''   `goCellularComponent_class` varchar(64) DEFAULT NULL,
'''   `systematicName` text,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `authored` (`authored`),
'''   KEY `cellType` (`cellType`),
'''   KEY `goCellularComponent` (`goCellularComponent`),
'''   FULLTEXT KEY `definition` (`definition`),
'''   FULLTEXT KEY `systematicName` (`systematicName`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("physicalentity", Database:="gk_current")>
Public Class physicalentity: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("authored"), DataType(MySqlDbType.Int64, "10")> Public Property authored As Long
    <DatabaseField("authored_class"), DataType(MySqlDbType.VarChar, "64")> Public Property authored_class As String
    <DatabaseField("cellType"), DataType(MySqlDbType.Int64, "10")> Public Property cellType As Long
    <DatabaseField("cellType_class"), DataType(MySqlDbType.VarChar, "64")> Public Property cellType_class As String
    <DatabaseField("definition"), DataType(MySqlDbType.Text)> Public Property definition As String
    <DatabaseField("goCellularComponent"), DataType(MySqlDbType.Int64, "10")> Public Property goCellularComponent As Long
    <DatabaseField("goCellularComponent_class"), DataType(MySqlDbType.VarChar, "64")> Public Property goCellularComponent_class As String
    <DatabaseField("systematicName"), DataType(MySqlDbType.Text)> Public Property systematicName As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `physicalentity` (`DB_ID`, `authored`, `authored_class`, `cellType`, `cellType_class`, `definition`, `goCellularComponent`, `goCellularComponent_class`, `systematicName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `physicalentity` (`DB_ID`, `authored`, `authored_class`, `cellType`, `cellType_class`, `definition`, `goCellularComponent`, `goCellularComponent_class`, `systematicName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `physicalentity` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `physicalentity` SET `DB_ID`='{0}', `authored`='{1}', `authored_class`='{2}', `cellType`='{3}', `cellType_class`='{4}', `definition`='{5}', `goCellularComponent`='{6}', `goCellularComponent_class`='{7}', `systematicName`='{8}' WHERE `DB_ID` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, authored, authored_class, cellType, cellType_class, definition, goCellularComponent, goCellularComponent_class, systematicName)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, authored, authored_class, cellType, cellType_class, definition, goCellularComponent, goCellularComponent_class, systematicName)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, authored, authored_class, cellType, cellType_class, definition, goCellularComponent, goCellularComponent_class, systematicName, DB_ID)
    End Function
#End Region
End Class


End Namespace
