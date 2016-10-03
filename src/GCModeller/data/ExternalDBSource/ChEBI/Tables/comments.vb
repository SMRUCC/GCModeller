#Region "Microsoft.VisualBasic::f241399f2c79b9f65c28df1b460b305b, ..\GCModeller\data\ExternalDBSource\ChEBI\Tables\comments.vb"

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

REM  Dump @12/3/2015 7:55:56 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace ChEBI.Tables

    ''' <summary>
    ''' 
    ''' --
    ''' 
    ''' DROP TABLE IF EXISTS `comments`;
    ''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
    ''' /*!40101 SET character_set_client = utf8 */;
    ''' CREATE TABLE `comments` (
    '''   `id` int(11) NOT NULL,
    '''   `compound_id` int(11) NOT NULL,
    '''   `text` text NOT NULL,
    '''   `created_on` datetime NOT NULL,
    '''   `datatype` varchar(80) DEFAULT NULL,
    '''   `datatype_id` int(11) NOT NULL,
    '''   PRIMARY KEY (`id`),
    '''   KEY `compound_id` (`compound_id`),
    '''   CONSTRAINT `FK_COMMENTS_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
    ''' /*!40101 SET character_set_client = @saved_cs_client */;
    ''' 
    ''' --
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    <Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("comments", Database:="chebi")>
    Public Class comments : Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
        <DatabaseField("id"), PrimaryKey, NotNULL, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
        <DatabaseField("compound_id"), NotNULL, DataType(MySqlDbType.Int64, "11")> Public Property compound_id As Long
        <DatabaseField("text"), NotNULL, DataType(MySqlDbType.Text)> Public Property text As String
        <DatabaseField("created_on"), NotNULL, DataType(MySqlDbType.DateTime)> Public Property created_on As Date
        <DatabaseField("datatype"), DataType(MySqlDbType.VarChar, "80")> Public Property datatype As String
        <DatabaseField("datatype_id"), NotNULL, DataType(MySqlDbType.Int64, "11")> Public Property datatype_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
        Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `comments` (`id`, `compound_id`, `text`, `created_on`, `datatype`, `datatype_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
        Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `comments` (`id`, `compound_id`, `text`, `created_on`, `datatype`, `datatype_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
        Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `comments` WHERE `id` = '{0}';</SQL>
        Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `comments` SET `id`='{0}', `compound_id`='{1}', `text`='{2}', `created_on`='{3}', `datatype`='{4}', `datatype_id`='{5}' WHERE `id` = '{6}';</SQL>
#End Region
        Public Overrides Function GetDeleteSQL() As String
            Return String.Format(DELETE_SQL, id)
        End Function
        Public Overrides Function GetInsertSQL() As String
            Return String.Format(INSERT_SQL, id, compound_id, text, Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.DataType.ToMySqlDateTimeString(created_on), datatype, datatype_id)
        End Function
        Public Overrides Function GetReplaceSQL() As String
            Return String.Format(REPLACE_SQL, id, compound_id, text, Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.DataType.ToMySqlDateTimeString(created_on), datatype, datatype_id)
        End Function
        Public Overrides Function GetUpdateSQL() As String
            Return String.Format(UPDATE_SQL, id, compound_id, text, Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.DataType.ToMySqlDateTimeString(created_on), datatype, datatype_id, id)
        End Function
#End Region
End Class


End Namespace
