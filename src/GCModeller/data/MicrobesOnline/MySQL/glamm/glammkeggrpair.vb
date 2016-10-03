#Region "Microsoft.VisualBasic::74f90a478fcb049899c8a21b89c28d11, ..\GCModeller\data\MicrobesOnline\MySQL\glamm\glammkeggrpair.vb"

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

REM  Dump @12/3/2015 8:22:12 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.glamm

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `glammkeggrpair`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `glammkeggrpair` (
'''   `guid` bigint(10) unsigned NOT NULL,
'''   `version` int(2) unsigned NOT NULL DEFAULT '1',
'''   `priority` tinyint(3) unsigned DEFAULT NULL,
'''   `created` date DEFAULT NULL,
'''   `reactionGuid` bigint(10) unsigned NOT NULL,
'''   `compound0Guid` bigint(10) unsigned NOT NULL,
'''   `compound1Guid` bigint(10) unsigned NOT NULL,
'''   `compound0KeggId` varchar(8) NOT NULL DEFAULT '',
'''   `compound1KeggId` varchar(8) NOT NULL DEFAULT '',
'''   `rpairRole` varchar(32) NOT NULL DEFAULT '',
'''   PRIMARY KEY (`guid`),
'''   KEY `reactionGuid` (`reactionGuid`),
'''   KEY `compound0Guid` (`compound0Guid`),
'''   KEY `compound1Guid` (`compound1Guid`),
'''   KEY `compound0KeggId` (`compound0KeggId`),
'''   KEY `compound1KeggId` (`compound1KeggId`),
'''   KEY `rpairRole` (`rpairRole`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("glammkeggrpair", Database:="glamm")>
Public Class glammkeggrpair: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property guid As Long
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("priority"), DataType(MySqlDbType.Int64, "3")> Public Property priority As Long
    <DatabaseField("created"), DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("reactionGuid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property reactionGuid As Long
    <DatabaseField("compound0Guid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property compound0Guid As Long
    <DatabaseField("compound1Guid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property compound1Guid As Long
    <DatabaseField("compound0KeggId"), NotNull, DataType(MySqlDbType.VarChar, "8")> Public Property compound0KeggId As String
    <DatabaseField("compound1KeggId"), NotNull, DataType(MySqlDbType.VarChar, "8")> Public Property compound1KeggId As String
    <DatabaseField("rpairRole"), NotNull, DataType(MySqlDbType.VarChar, "32")> Public Property rpairRole As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `glammkeggrpair` (`guid`, `version`, `priority`, `created`, `reactionGuid`, `compound0Guid`, `compound1Guid`, `compound0KeggId`, `compound1KeggId`, `rpairRole`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `glammkeggrpair` (`guid`, `version`, `priority`, `created`, `reactionGuid`, `compound0Guid`, `compound1Guid`, `compound0KeggId`, `compound1KeggId`, `rpairRole`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `glammkeggrpair` WHERE `guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `glammkeggrpair` SET `guid`='{0}', `version`='{1}', `priority`='{2}', `created`='{3}', `reactionGuid`='{4}', `compound0Guid`='{5}', `compound1Guid`='{6}', `compound0KeggId`='{7}', `compound1KeggId`='{8}', `rpairRole`='{9}' WHERE `guid` = '{10}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), reactionGuid, compound0Guid, compound1Guid, compound0KeggId, compound1KeggId, rpairRole)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), reactionGuid, compound0Guid, compound1Guid, compound0KeggId, compound1KeggId, rpairRole)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), reactionGuid, compound0Guid, compound1Guid, compound0KeggId, compound1KeggId, rpairRole, guid)
    End Function
#End Region
End Class


End Namespace
