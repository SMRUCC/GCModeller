#Region "Microsoft.VisualBasic::d56cb57df8e96fe381f24b20df03d6a3, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\operon.vb"

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

REM  Dump @12/3/2015 8:08:18 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `operon`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `operon` (
'''   `operon_id` char(12) NOT NULL,
'''   `operon_name` varchar(255) NOT NULL,
'''   `firstgeneposleft` decimal(10,0) NOT NULL,
'''   `lastgeneposright` decimal(10,0) NOT NULL,
'''   `regulationposleft` decimal(10,0) NOT NULL,
'''   `regulationposright` decimal(10,0) NOT NULL,
'''   `operon_strand` varchar(10) DEFAULT NULL,
'''   `operon_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("operon", Database:="regulondb_7_5")>
Public Class operon: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("operon_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property operon_id As String
    <DatabaseField("operon_name"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property operon_name As String
    <DatabaseField("firstgeneposleft"), NotNull, DataType(MySqlDbType.Decimal)> Public Property firstgeneposleft As Decimal
    <DatabaseField("lastgeneposright"), NotNull, DataType(MySqlDbType.Decimal)> Public Property lastgeneposright As Decimal
    <DatabaseField("regulationposleft"), NotNull, DataType(MySqlDbType.Decimal)> Public Property regulationposleft As Decimal
    <DatabaseField("regulationposright"), NotNull, DataType(MySqlDbType.Decimal)> Public Property regulationposright As Decimal
    <DatabaseField("operon_strand"), DataType(MySqlDbType.VarChar, "10")> Public Property operon_strand As String
    <DatabaseField("operon_internal_comment"), DataType(MySqlDbType.Text)> Public Property operon_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `operon` (`operon_id`, `operon_name`, `firstgeneposleft`, `lastgeneposright`, `regulationposleft`, `regulationposright`, `operon_strand`, `operon_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `operon` (`operon_id`, `operon_name`, `firstgeneposleft`, `lastgeneposright`, `regulationposleft`, `regulationposright`, `operon_strand`, `operon_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `operon` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `operon` SET `operon_id`='{0}', `operon_name`='{1}', `firstgeneposleft`='{2}', `lastgeneposright`='{3}', `regulationposleft`='{4}', `regulationposright`='{5}', `operon_strand`='{6}', `operon_internal_comment`='{7}', `key_id_org`='{8}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, operon_id, operon_name, firstgeneposleft, lastgeneposright, regulationposleft, regulationposright, operon_strand, operon_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, operon_id, operon_name, firstgeneposleft, lastgeneposright, regulationposleft, regulationposright, operon_strand, operon_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
