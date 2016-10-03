#Region "Microsoft.VisualBasic::816804c13a1cc6e5b0f28af248519ec0, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\site.vb"

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
''' DROP TABLE IF EXISTS `site`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `site` (
'''   `site_id` char(12) NOT NULL,
'''   `site_posleft` decimal(10,0) NOT NULL,
'''   `site_posright` decimal(10,0) NOT NULL,
'''   `site_sequence` varchar(100) DEFAULT NULL,
'''   `site_note` varchar(2000) DEFAULT NULL,
'''   `site_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL,
'''   `site_length` decimal(10,0) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("site", Database:="regulondb_7_5")>
Public Class site: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("site_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property site_id As String
    <DatabaseField("site_posleft"), NotNull, DataType(MySqlDbType.Decimal)> Public Property site_posleft As Decimal
    <DatabaseField("site_posright"), NotNull, DataType(MySqlDbType.Decimal)> Public Property site_posright As Decimal
    <DatabaseField("site_sequence"), DataType(MySqlDbType.VarChar, "100")> Public Property site_sequence As String
    <DatabaseField("site_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property site_note As String
    <DatabaseField("site_internal_comment"), DataType(MySqlDbType.Text)> Public Property site_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
    <DatabaseField("site_length"), DataType(MySqlDbType.Decimal)> Public Property site_length As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `site` (`site_id`, `site_posleft`, `site_posright`, `site_sequence`, `site_note`, `site_internal_comment`, `key_id_org`, `site_length`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `site` (`site_id`, `site_posleft`, `site_posright`, `site_sequence`, `site_note`, `site_internal_comment`, `key_id_org`, `site_length`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `site` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `site` SET `site_id`='{0}', `site_posleft`='{1}', `site_posright`='{2}', `site_sequence`='{3}', `site_note`='{4}', `site_internal_comment`='{5}', `key_id_org`='{6}', `site_length`='{7}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, site_id, site_posleft, site_posright, site_sequence, site_note, site_internal_comment, key_id_org, site_length)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, site_id, site_posleft, site_posright, site_sequence, site_note, site_internal_comment, key_id_org, site_length)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
