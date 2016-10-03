#Region "Microsoft.VisualBasic::eb78b3377fc4e67f88b6fa22fed00c87, ..\GCModeller\analysis\annoTools\DataMySql\Interpro\interpro\pub2author.vb"

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

REM  Dump @12/3/2015 8:52:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `pub2author`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pub2author` (
'''   `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `author_id` int(9) NOT NULL,
'''   `order_in` int(3) NOT NULL,
'''   PRIMARY KEY (`pub_id`,`author_id`,`order_in`),
'''   KEY `fk_pub2author$author_id` (`author_id`),
'''   CONSTRAINT `fk_pub2author$author_id` FOREIGN KEY (`author_id`) REFERENCES `author` (`author_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_pub2author$pub_id` FOREIGN KEY (`pub_id`) REFERENCES `pub` (`pub_id`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pub2author", Database:="interpro")>
Public Class pub2author: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pub_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "11")> Public Property pub_id As String
    <DatabaseField("author_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "9")> Public Property author_id As Long
    <DatabaseField("order_in"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "3")> Public Property order_in As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pub2author` WHERE `pub_id`='{0}' and `author_id`='{1}' and `order_in`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pub2author` SET `pub_id`='{0}', `author_id`='{1}', `order_in`='{2}' WHERE `pub_id`='{3}' and `author_id`='{4}' and `order_in`='{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pub_id, author_id, order_in)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pub_id, author_id, order_in)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pub_id, author_id, order_in)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pub_id, author_id, order_in, pub_id, author_id, order_in)
    End Function
#End Region
End Class


End Namespace
