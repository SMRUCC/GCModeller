#Region "Microsoft.VisualBasic::cd2450aeba12ae004ec2453ba684f1c0, ..\GCModeller\analysis\Annotation\Interpro\interpro\journal.vb"

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
''' DROP TABLE IF EXISTS `journal`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `journal` (
'''   `issn` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `abbrev` varchar(60) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `uppercase` varchar(60) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `start_date` datetime DEFAULT NULL,
'''   `end_date` datetime DEFAULT NULL,
'''   PRIMARY KEY (`issn`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("journal", Database:="interpro")>
Public Class journal: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("issn"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9")> Public Property issn As String
    <DatabaseField("abbrev"), NotNull, DataType(MySqlDbType.VarChar, "60")> Public Property abbrev As String
    <DatabaseField("uppercase"), DataType(MySqlDbType.VarChar, "60")> Public Property uppercase As String
    <DatabaseField("start_date"), DataType(MySqlDbType.DateTime)> Public Property start_date As Date
    <DatabaseField("end_date"), DataType(MySqlDbType.DateTime)> Public Property end_date As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `journal` (`issn`, `abbrev`, `uppercase`, `start_date`, `end_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `journal` (`issn`, `abbrev`, `uppercase`, `start_date`, `end_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `journal` WHERE `issn` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `journal` SET `issn`='{0}', `abbrev`='{1}', `uppercase`='{2}', `start_date`='{3}', `end_date`='{4}' WHERE `issn` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, issn)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, issn, abbrev, uppercase, DataType.ToMySqlDateTimeString(start_date), DataType.ToMySqlDateTimeString(end_date))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, issn, abbrev, uppercase, DataType.ToMySqlDateTimeString(start_date), DataType.ToMySqlDateTimeString(end_date))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, issn, abbrev, uppercase, DataType.ToMySqlDateTimeString(start_date), DataType.ToMySqlDateTimeString(end_date), issn)
    End Function
#End Region
End Class


End Namespace

