#Region "Microsoft.VisualBasic::167adf5e37e79b4857c929945e24bd98, GCModeller\data\MicrobesOnline\MySQL\genomics\term_definition.vb"

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


    ' Code Statistics:

    '   Total Lines: 67
    '    Code Lines: 37
    ' Comment Lines: 23
    '   Blank Lines: 7
    '     File Size: 3.30 KB


    ' Class term_definition
    ' 
    '     Properties: dbxref_id, reference, term_comment, term_definition, term_id
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
''' DROP TABLE IF EXISTS `term_definition`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `term_definition` (
'''   `term_id` int(11) NOT NULL DEFAULT '0',
'''   `term_definition` text NOT NULL,
'''   `dbxref_id` int(11) DEFAULT NULL,
'''   `term_comment` mediumtext,
'''   `reference` varchar(255) DEFAULT NULL,
'''   UNIQUE KEY `term_id` (`term_id`),
'''   KEY `dbxref_id` (`dbxref_id`),
'''   KEY `td1` (`term_id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("term_definition")>
Public Class term_definition: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("term_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property term_id As Long
    <DatabaseField("term_definition"), NotNull, DataType(MySqlDbType.Text)> Public Property term_definition As String
    <DatabaseField("dbxref_id"), DataType(MySqlDbType.Int64, "11")> Public Property dbxref_id As Long
    <DatabaseField("term_comment"), DataType(MySqlDbType.Text)> Public Property term_comment As String
    <DatabaseField("reference"), DataType(MySqlDbType.VarChar, "255")> Public Property reference As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `term_definition` (`term_id`, `term_definition`, `dbxref_id`, `term_comment`, `reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `term_definition` (`term_id`, `term_definition`, `dbxref_id`, `term_comment`, `reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `term_definition` WHERE `term_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `term_definition` SET `term_id`='{0}', `term_definition`='{1}', `dbxref_id`='{2}', `term_comment`='{3}', `reference`='{4}' WHERE `term_id` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, term_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, term_id, term_definition, dbxref_id, term_comment, reference)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, term_id, term_definition, dbxref_id, term_comment, reference)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, term_id, term_definition, dbxref_id, term_comment, reference, term_id)
    End Function
#End Region
End Class


End Namespace
