#Region "Microsoft.VisualBasic::8aeef778710b47f2bb162143f94a0c70, data\MicrobesOnline\MySQL\genomics\seq.vb"

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

    ' Class seq
    ' 
    '     Properties: description, display_id, id, md5checksum, moltype
    '                 seq, seq_len, timestamp
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
''' DROP TABLE IF EXISTS `seq`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `seq` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `display_id` varchar(64) DEFAULT NULL,
'''   `description` varchar(255) DEFAULT NULL,
'''   `seq` mediumtext,
'''   `seq_len` int(11) DEFAULT NULL,
'''   `md5checksum` varchar(32) DEFAULT NULL,
'''   `moltype` varchar(25) DEFAULT NULL,
'''   `timestamp` int(11) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `seq0` (`id`),
'''   UNIQUE KEY `display_id` (`display_id`,`md5checksum`),
'''   KEY `seq1` (`display_id`),
'''   KEY `seq2` (`md5checksum`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("seq")>
Public Class seq: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("display_id"), DataType(MySqlDbType.VarChar, "64")> Public Property display_id As String
    <DatabaseField("description"), DataType(MySqlDbType.VarChar, "255")> Public Property description As String
    <DatabaseField("seq"), DataType(MySqlDbType.Text)> Public Property seq As String
    <DatabaseField("seq_len"), DataType(MySqlDbType.Int64, "11")> Public Property seq_len As Long
    <DatabaseField("md5checksum"), DataType(MySqlDbType.VarChar, "32")> Public Property md5checksum As String
    <DatabaseField("moltype"), DataType(MySqlDbType.VarChar, "25")> Public Property moltype As String
    <DatabaseField("timestamp"), DataType(MySqlDbType.Int64, "11")> Public Property timestamp As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `seq` (`display_id`, `description`, `seq`, `seq_len`, `md5checksum`, `moltype`, `timestamp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `seq` (`display_id`, `description`, `seq`, `seq_len`, `md5checksum`, `moltype`, `timestamp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `seq` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `seq` SET `id`='{0}', `display_id`='{1}', `description`='{2}', `seq`='{3}', `seq_len`='{4}', `md5checksum`='{5}', `moltype`='{6}', `timestamp`='{7}' WHERE `id` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, display_id, description, seq, seq_len, md5checksum, moltype, timestamp)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, display_id, description, seq, seq_len, md5checksum, moltype, timestamp)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, display_id, description, seq, seq_len, md5checksum, moltype, timestamp, id)
    End Function
#End Region
End Class


End Namespace
