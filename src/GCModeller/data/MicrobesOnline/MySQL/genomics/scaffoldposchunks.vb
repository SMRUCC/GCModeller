#Region "Microsoft.VisualBasic::7126460b611bbdbcf5ee6459f0e131ac, GCModeller\data\MicrobesOnline\MySQL\genomics\scaffoldposchunks.vb"

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

    '   Total Lines: 70
    '    Code Lines: 37
    ' Comment Lines: 26
    '   Blank Lines: 7
    '     File Size: 3.68 KB


    ' Class scaffoldposchunks
    ' 
    '     Properties: kbt, locusId, posId, scaffoldId, version
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

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
''' DROP TABLE IF EXISTS `scaffoldposchunks`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `scaffoldposchunks` (
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `version` int(2) unsigned NOT NULL DEFAULT '0',
'''   `posId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `scaffoldId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `kbt` int(10) unsigned NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`locusId`,`version`,`posId`,`scaffoldId`,`kbt`),
'''   KEY `locusIdVersion` (`locusId`,`version`),
'''   KEY `locusId` (`locusId`),
'''   KEY `scaffoldId` (`scaffoldId`),
'''   KEY `kbt` (`kbt`),
'''   KEY `scaffoldIdKbt` (`scaffoldId`,`kbt`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("scaffoldposchunks")>
Public Class scaffoldposchunks: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("posId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property posId As Long
    <DatabaseField("scaffoldId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property scaffoldId As Long
    <DatabaseField("kbt"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property kbt As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `scaffoldposchunks` (`locusId`, `version`, `posId`, `scaffoldId`, `kbt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `scaffoldposchunks` (`locusId`, `version`, `posId`, `scaffoldId`, `kbt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `scaffoldposchunks` WHERE `locusId`='{0}' and `version`='{1}' and `posId`='{2}' and `scaffoldId`='{3}' and `kbt`='{4}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `scaffoldposchunks` SET `locusId`='{0}', `version`='{1}', `posId`='{2}', `scaffoldId`='{3}', `kbt`='{4}' WHERE `locusId`='{5}' and `version`='{6}' and `posId`='{7}' and `scaffoldId`='{8}' and `kbt`='{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId, version, posId, scaffoldId, kbt)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locusId, version, posId, scaffoldId, kbt)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locusId, version, posId, scaffoldId, kbt)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locusId, version, posId, scaffoldId, kbt, locusId, version, posId, scaffoldId, kbt)
    End Function
#End Region
End Class


End Namespace
