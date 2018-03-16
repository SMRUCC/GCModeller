#Region "Microsoft.VisualBasic::099dec8ba12bc6c9c6e9e4ebd4fa19e2, data\MicrobesOnline\MySQL\glamm\glammreactionparticipant.vb"

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

    ' Class glammreactionparticipant
    ' 
    '     Properties: coefficient, compoundGuid, created, guid, priority
    '                 pType, reactionGuid, version
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

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
''' DROP TABLE IF EXISTS `glammreactionparticipant`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `glammreactionparticipant` (
'''   `guid` bigint(10) unsigned NOT NULL,
'''   `version` int(2) unsigned NOT NULL DEFAULT '1',
'''   `priority` tinyint(3) unsigned DEFAULT NULL,
'''   `created` date DEFAULT NULL,
'''   `reactionGuid` bigint(10) unsigned NOT NULL,
'''   `compoundGuid` bigint(10) unsigned NOT NULL,
'''   `coefficient` tinyint(3) unsigned NOT NULL DEFAULT '1',
'''   `pType` enum('REACTANT','PRODUCT') NOT NULL,
'''   PRIMARY KEY (`guid`),
'''   KEY `reactionGuid` (`reactionGuid`),
'''   KEY `compoundGuid` (`compoundGuid`),
'''   KEY `pType` (`pType`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("glammreactionparticipant", Database:="glamm")>
Public Class glammreactionparticipant: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property guid As Long
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("priority"), DataType(MySqlDbType.Int64, "3")> Public Property priority As Long
    <DatabaseField("created"), DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("reactionGuid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property reactionGuid As Long
    <DatabaseField("compoundGuid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property compoundGuid As Long
    <DatabaseField("coefficient"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property coefficient As Long
    <DatabaseField("pType"), NotNull, DataType(MySqlDbType.String)> Public Property pType As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `glammreactionparticipant` (`guid`, `version`, `priority`, `created`, `reactionGuid`, `compoundGuid`, `coefficient`, `pType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `glammreactionparticipant` (`guid`, `version`, `priority`, `created`, `reactionGuid`, `compoundGuid`, `coefficient`, `pType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `glammreactionparticipant` WHERE `guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `glammreactionparticipant` SET `guid`='{0}', `version`='{1}', `priority`='{2}', `created`='{3}', `reactionGuid`='{4}', `compoundGuid`='{5}', `coefficient`='{6}', `pType`='{7}' WHERE `guid` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), reactionGuid, compoundGuid, coefficient, pType)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), reactionGuid, compoundGuid, coefficient, pType)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), reactionGuid, compoundGuid, coefficient, pType, guid)
    End Function
#End Region
End Class


End Namespace
