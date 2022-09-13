#Region "Microsoft.VisualBasic::0cca00bc31c12701b1b63d7e33d35c0d, GCModeller\data\MicrobesOnline\MySQL\BioCyc\promoter.vb"

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

    '   Total Lines: 64
    '    Code Lines: 36
    ' Comment Lines: 21
    '   Blank Lines: 7
    '     File Size: 2.89 KB


    ' Class promoter
    ' 
    '     Properties: evidence, name, promoterId, taxId
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:32:12 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.BioCyc

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `promoter`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `promoter` (
'''   `taxId` int(20) unsigned NOT NULL,
'''   `promoterId` varchar(100) NOT NULL,
'''   `name` varchar(20) DEFAULT NULL,
'''   `evidence` varchar(100) DEFAULT NULL,
'''   PRIMARY KEY (`promoterId`),
'''   KEY `taxId` (`taxId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("promoter", Database:="biocyc")>
Public Class promoter: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("taxId"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property taxId As Long
    <DatabaseField("promoterId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property promoterId As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "20")> Public Property name As String
    <DatabaseField("evidence"), DataType(MySqlDbType.VarChar, "100")> Public Property evidence As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `promoter` (`taxId`, `promoterId`, `name`, `evidence`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `promoter` (`taxId`, `promoterId`, `name`, `evidence`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `promoter` WHERE `promoterId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `promoter` SET `taxId`='{0}', `promoterId`='{1}', `name`='{2}', `evidence`='{3}' WHERE `promoterId` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, promoterId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, taxId, promoterId, name, evidence)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, taxId, promoterId, name, evidence)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, taxId, promoterId, name, evidence, promoterId)
    End Function
#End Region
End Class


End Namespace
