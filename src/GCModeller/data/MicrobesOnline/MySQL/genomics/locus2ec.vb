#Region "Microsoft.VisualBasic::26f1b6afa2eaf2765b6b5a76e8ba38f5, ..\GCModeller\data\MicrobesOnline\MySQL\genomics\locus2ec.vb"

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

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `locus2ec`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `locus2ec` (
'''   `locusId` int(10) unsigned DEFAULT NULL,
'''   `version` int(2) unsigned DEFAULT '1',
'''   `scaffoldId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `ecNum` varchar(20) NOT NULL DEFAULT '',
'''   `evidence` varchar(50) DEFAULT NULL,
'''   UNIQUE KEY `combined` (`locusId`,`ecNum`,`version`),
'''   KEY `scaffoldId` (`scaffoldId`),
'''   KEY `ecNum` (`ecNum`),
'''   KEY `locusId` (`locusId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("locus2ec")>
Public Class locus2ec: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), PrimaryKey, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), PrimaryKey, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("scaffoldId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property scaffoldId As Long
    <DatabaseField("ecNum"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property ecNum As String
    <DatabaseField("evidence"), DataType(MySqlDbType.VarChar, "50")> Public Property evidence As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `locus2ec` (`locusId`, `version`, `scaffoldId`, `ecNum`, `evidence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `locus2ec` (`locusId`, `version`, `scaffoldId`, `ecNum`, `evidence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `locus2ec` WHERE `locusId`='{0}' and `ecNum`='{1}' and `version`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `locus2ec` SET `locusId`='{0}', `version`='{1}', `scaffoldId`='{2}', `ecNum`='{3}', `evidence`='{4}' WHERE `locusId`='{5}' and `ecNum`='{6}' and `version`='{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId, ecNum, version)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locusId, version, scaffoldId, ecNum, evidence)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locusId, version, scaffoldId, ecNum, evidence)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locusId, version, scaffoldId, ecNum, evidence, locusId, ecNum, version)
    End Function
#End Region
End Class


End Namespace
