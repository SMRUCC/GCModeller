#Region "Microsoft.VisualBasic::4a57547c87275ccba563cfc2617d0801, GCModeller\data\MicrobesOnline\MySQL\genomics\term_synonym.vb"

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

    '   Total Lines: 70
    '    Code Lines: 37
    ' Comment Lines: 26
    '   Blank Lines: 7
    '     File Size: 3.68 KB


    ' Class term_synonym
    ' 
    '     Properties: acc_synonym, synonym_category_id, synonym_type_id, term_id, term_synonym
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
''' DROP TABLE IF EXISTS `term_synonym`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `term_synonym` (
'''   `term_id` int(11) NOT NULL DEFAULT '0',
'''   `term_synonym` text,
'''   `acc_synonym` varchar(255) DEFAULT NULL,
'''   `synonym_type_id` int(11) NOT NULL DEFAULT '0',
'''   `synonym_category_id` int(11) DEFAULT NULL,
'''   UNIQUE KEY `term_id` (`term_id`,`term_synonym`(100)),
'''   KEY `synonym_type_id` (`synonym_type_id`),
'''   KEY `synonym_category_id` (`synonym_category_id`),
'''   KEY `ts1` (`term_id`),
'''   KEY `ts2` (`term_synonym`(100)),
'''   KEY `ts3` (`term_id`,`term_synonym`(100))
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("term_synonym")>
Public Class term_synonym: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("term_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property term_id As Long
    <DatabaseField("term_synonym"), DataType(MySqlDbType.Text)> Public Property term_synonym As String
    <DatabaseField("acc_synonym"), DataType(MySqlDbType.VarChar, "255")> Public Property acc_synonym As String
    <DatabaseField("synonym_type_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property synonym_type_id As Long
    <DatabaseField("synonym_category_id"), DataType(MySqlDbType.Int64, "11")> Public Property synonym_category_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `term_synonym` (`term_id`, `term_synonym`, `acc_synonym`, `synonym_type_id`, `synonym_category_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `term_synonym` (`term_id`, `term_synonym`, `acc_synonym`, `synonym_type_id`, `synonym_category_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `term_synonym` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `term_synonym` SET `term_id`='{0}', `term_synonym`='{1}', `acc_synonym`='{2}', `synonym_type_id`='{3}', `synonym_category_id`='{4}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, term_id, term_synonym, acc_synonym, synonym_type_id, synonym_category_id)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, term_id, term_synonym, acc_synonym, synonym_type_id, synonym_category_id)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
