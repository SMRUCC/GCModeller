#Region "Microsoft.VisualBasic::894a3f65b93c9ddbfc68aa7e8d0cda26, DataMySql\Interpro\Tables\pub2author.vb"

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

    ' Class pub2author
    ' 
    '     Properties: author_id, order_in, pub_id
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Interpro.Tables

''' <summary>
''' ```SQL
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
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pub2author", Database:="interpro", SchemaSQL:="
CREATE TABLE `pub2author` (
  `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `author_id` int(9) NOT NULL,
  `order_in` int(3) NOT NULL,
  PRIMARY KEY (`pub_id`,`author_id`,`order_in`),
  KEY `fk_pub2author$author_id` (`author_id`),
  CONSTRAINT `fk_pub2author$author_id` FOREIGN KEY (`author_id`) REFERENCES `author` (`author_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_pub2author$pub_id` FOREIGN KEY (`pub_id`) REFERENCES `pub` (`pub_id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class pub2author: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pub_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "11"), Column(Name:="pub_id"), XmlAttribute> Public Property pub_id As String
    <DatabaseField("author_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "9"), Column(Name:="author_id"), XmlAttribute> Public Property author_id As Long
    <DatabaseField("order_in"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "3"), Column(Name:="order_in"), XmlAttribute> Public Property order_in As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `pub2author` WHERE `pub_id`='{0}' and `author_id`='{1}' and `order_in`='{2}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `pub2author` SET `pub_id`='{0}', `author_id`='{1}', `order_in`='{2}' WHERE `pub_id`='{3}' and `author_id`='{4}' and `order_in`='{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `pub2author` WHERE `pub_id`='{0}' and `author_id`='{1}' and `order_in`='{2}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pub_id, author_id, order_in)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pub_id, author_id, order_in)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pub_id, author_id, order_in)
        Else
        Return String.Format(INSERT_SQL, pub_id, author_id, order_in)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pub_id}', '{author_id}', '{order_in}')"
        Else
            Return $"('{pub_id}', '{author_id}', '{order_in}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pub_id, author_id, order_in)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pub_id, author_id, order_in)
        Else
        Return String.Format(REPLACE_SQL, pub_id, author_id, order_in)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `pub2author` SET `pub_id`='{0}', `author_id`='{1}', `order_in`='{2}' WHERE `pub_id`='{3}' and `author_id`='{4}' and `order_in`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pub_id, author_id, order_in, pub_id, author_id, order_in)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As pub2author
                         Return DirectCast(MyClass.MemberwiseClone, pub2author)
                     End Function
End Class


End Namespace
