#Region "Microsoft.VisualBasic::c7c88b2f307d1704e45c7247a9afd694, ..\GCModeller\CLI_tools\KEGG\LocalMySQL\module.vb"

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

REM  Dump @12/3/2015 7:34:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL

    ''' <summary>
    ''' 
    ''' --
    ''' 
    ''' DROP TABLE IF EXISTS `module`;
    ''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
    ''' /*!40101 SET character_set_client = utf8 */;
    ''' CREATE TABLE `module` (
    '''   `entry` varchar(45) NOT NULL,
    '''   `name` longtext,
    '''   `definition` longtext,
    '''   `class` text,
    '''   `category` text,
    '''   `type` text,
    '''   PRIMARY KEY (`entry`),
    '''   UNIQUE KEY `entry_UNIQUE` (`entry`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
    ''' /*!40101 SET character_set_client = @saved_cs_client */;
    ''' 
    ''' --
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    <Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("module", Database:="jp_kegg2")>
    Public Class [module] : Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
        <DatabaseField("entry"), PrimaryKey, NotNULL, DataType(MySqlDbType.VarChar, "45")> Public Property entry As String
        <DatabaseField("name"), DataType(MySqlDbType.Text)> Public Property name As String
        <DatabaseField("definition"), DataType(MySqlDbType.Text)> Public Property definition As String
        <DatabaseField("class"), DataType(MySqlDbType.Text)> Public Property [class] As String
        <DatabaseField("category"), DataType(MySqlDbType.Text)> Public Property category As String
        <DatabaseField("type"), DataType(MySqlDbType.Text)> Public Property type As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
        Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `module` (`entry`, `name`, `definition`, `class`, `category`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
        Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `module` (`entry`, `name`, `definition`, `class`, `category`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
        Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `module` WHERE `entry` = '{0}';</SQL>
        Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `module` SET `entry`='{0}', `name`='{1}', `definition`='{2}', `class`='{3}', `category`='{4}', `type`='{5}' WHERE `entry` = '{6}';</SQL>
#End Region
        Public Overrides Function GetDeleteSQL() As String
            Return String.Format(DELETE_SQL, entry)
        End Function
        Public Overrides Function GetInsertSQL() As String
            Return String.Format(INSERT_SQL, entry, name, definition, [class], category, type)
        End Function
        Public Overrides Function GetReplaceSQL() As String
            Return String.Format(REPLACE_SQL, entry, name, definition, [class], category, type)
        End Function
        Public Overrides Function GetUpdateSQL() As String
            Return String.Format(UPDATE_SQL, entry, name, definition, [class], category, type, entry)
        End Function
#End Region
End Class


End Namespace

