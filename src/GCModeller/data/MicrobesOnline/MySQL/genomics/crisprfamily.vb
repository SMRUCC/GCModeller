#Region "Microsoft.VisualBasic::959ba3bea5daee061700cc23d6fcd231, ..\GCModeller\data\MicrobesOnline\MySQL\genomics\crisprfamily.vb"

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
''' DROP TABLE IF EXISTS `crisprfamily`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `crisprfamily` (
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `familyId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `type` smallint(5) unsigned NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`locusId`,`familyId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("crisprfamily")>
Public Class crisprfamily: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("familyId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property familyId As Long
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property type As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `crisprfamily` (`locusId`, `familyId`, `type`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `crisprfamily` (`locusId`, `familyId`, `type`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `crisprfamily` WHERE `locusId`='{0}' and `familyId`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `crisprfamily` SET `locusId`='{0}', `familyId`='{1}', `type`='{2}' WHERE `locusId`='{3}' and `familyId`='{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId, familyId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locusId, familyId, type)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locusId, familyId, type)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locusId, familyId, type, locusId, familyId)
    End Function
#End Region
End Class


End Namespace
