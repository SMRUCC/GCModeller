#Region "Microsoft.VisualBasic::05f14f097de145361d0abef4375b5771, ..\GCModeller\analysis\Annotation\Interpro\Tables\book2author.vb"

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

REM  Dump @12/3/2015 8:52:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `book2author`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `book2author` (
'''   `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `author_id` int(9) NOT NULL,
'''   `order_in` int(3) NOT NULL,
'''   PRIMARY KEY (`isbn`,`order_in`,`author_id`),
'''   UNIQUE KEY `uq_book2author$1` (`isbn`,`order_in`),
'''   KEY `i_book2author$fk_author_id` (`author_id`),
'''   CONSTRAINT `fk_book2author$author_id` FOREIGN KEY (`author_id`) REFERENCES `author` (`author_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_book2author$isbn` FOREIGN KEY (`isbn`) REFERENCES `book` (`isbn`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("book2author", Database:="interpro")>
Public Class book2author: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("isbn"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "10")> Public Property isbn As String
    <DatabaseField("author_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "9")> Public Property author_id As Long
    <DatabaseField("order_in"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "3")> Public Property order_in As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `book2author` (`isbn`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `book2author` (`isbn`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `book2author` WHERE `isbn`='{0}' and `order_in`='{1}' and `author_id`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `book2author` SET `isbn`='{0}', `author_id`='{1}', `order_in`='{2}' WHERE `isbn`='{3}' and `order_in`='{4}' and `author_id`='{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, isbn, order_in, author_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, isbn, author_id, order_in)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, isbn, author_id, order_in)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, isbn, author_id, order_in, isbn, order_in, author_id)
    End Function
#End Region
End Class


End Namespace

