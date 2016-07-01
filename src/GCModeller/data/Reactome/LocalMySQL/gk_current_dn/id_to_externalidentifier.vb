#Region "Microsoft.VisualBasic::ecbb155fe4fbafd4c5c31b42a8cc7424, ..\GCModeller\data\Reactome\LocalMySQL\gk_current_dn\id_to_externalidentifier.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

REM  Dump @12/3/2015 8:16:51 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current_dn

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `id_to_externalidentifier`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `id_to_externalidentifier` (
'''   `id` int(32) NOT NULL DEFAULT '0',
'''   `referenceDatabase` varchar(255) NOT NULL DEFAULT '',
'''   `externalIdentifier` varchar(32) NOT NULL DEFAULT '',
'''   `description` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`id`,`referenceDatabase`,`externalIdentifier`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("id_to_externalidentifier", Database:="gk_current_dn")>
Public Class id_to_externalidentifier: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "32")> Public Property id As Long
    <DatabaseField("referenceDatabase"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property referenceDatabase As String
    <DatabaseField("externalIdentifier"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "32")> Public Property externalIdentifier As String
    <DatabaseField("description"), DataType(MySqlDbType.VarChar, "255")> Public Property description As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `id_to_externalidentifier` (`id`, `referenceDatabase`, `externalIdentifier`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `id_to_externalidentifier` (`id`, `referenceDatabase`, `externalIdentifier`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `id_to_externalidentifier` WHERE `id`='{0}' and `referenceDatabase`='{1}' and `externalIdentifier`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `id_to_externalidentifier` SET `id`='{0}', `referenceDatabase`='{1}', `externalIdentifier`='{2}', `description`='{3}' WHERE `id`='{4}' and `referenceDatabase`='{5}' and `externalIdentifier`='{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id, referenceDatabase, externalIdentifier)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, referenceDatabase, externalIdentifier, description)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, referenceDatabase, externalIdentifier, description)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, referenceDatabase, externalIdentifier, description, id, referenceDatabase, externalIdentifier)
    End Function
#End Region
End Class


End Namespace

