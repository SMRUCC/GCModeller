#Region "Microsoft.VisualBasic::bbb25cddbbe56cf4fdb04c56e3b66099, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\motif.vb"

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
''' DROP TABLE IF EXISTS `motif`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `motif` (
'''   `motif_id` char(12) NOT NULL,
'''   `product_id` char(12) NOT NULL,
'''   `motif_posleft` decimal(10,0) NOT NULL,
'''   `motif_posright` decimal(10,0) NOT NULL,
'''   `motif_sequence` varchar(3000) DEFAULT NULL,
'''   `motif_description` varchar(4000) DEFAULT NULL,
'''   `motif_type` varchar(255) DEFAULT NULL,
'''   `motif_note` varchar(2000) DEFAULT NULL,
'''   `motif_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("motif", Database:="regulondb_7_5")>
Public Class motif: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("motif_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property motif_id As String
    <DatabaseField("product_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property product_id As String
    <DatabaseField("motif_posleft"), NotNull, DataType(MySqlDbType.Decimal)> Public Property motif_posleft As Decimal
    <DatabaseField("motif_posright"), NotNull, DataType(MySqlDbType.Decimal)> Public Property motif_posright As Decimal
    <DatabaseField("motif_sequence"), DataType(MySqlDbType.VarChar, "3000")> Public Property motif_sequence As String
    <DatabaseField("motif_description"), DataType(MySqlDbType.VarChar, "4000")> Public Property motif_description As String
    <DatabaseField("motif_type"), DataType(MySqlDbType.VarChar, "255")> Public Property motif_type As String
    <DatabaseField("motif_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property motif_note As String
    <DatabaseField("motif_internal_comment"), DataType(MySqlDbType.Text)> Public Property motif_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `motif` (`motif_id`, `product_id`, `motif_posleft`, `motif_posright`, `motif_sequence`, `motif_description`, `motif_type`, `motif_note`, `motif_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `motif` (`motif_id`, `product_id`, `motif_posleft`, `motif_posright`, `motif_sequence`, `motif_description`, `motif_type`, `motif_note`, `motif_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `motif` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `motif` SET `motif_id`='{0}', `product_id`='{1}', `motif_posleft`='{2}', `motif_posright`='{3}', `motif_sequence`='{4}', `motif_description`='{5}', `motif_type`='{6}', `motif_note`='{7}', `motif_internal_comment`='{8}', `key_id_org`='{9}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, motif_id, product_id, motif_posleft, motif_posright, motif_sequence, motif_description, motif_type, motif_note, motif_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, motif_id, product_id, motif_posleft, motif_posright, motif_sequence, motif_description, motif_type, motif_note, motif_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
