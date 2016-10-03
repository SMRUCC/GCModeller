#Region "Microsoft.VisualBasic::154c7fd416375ff85707b2a15dc80adf, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\sigmanetworkset_tmp.vb"

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

REM  Dump @12/3/2015 8:08:18 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `sigmanetworkset_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `sigmanetworkset_tmp` (
'''   `sigma_promoter` varchar(80) DEFAULT NULL,
'''   `snws_promoter_name` varchar(100) DEFAULT NULL,
'''   `gene_coding` varchar(100) DEFAULT NULL,
'''   `gene_regulated` varchar(100) DEFAULT NULL,
'''   `bnumber` varchar(10) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("sigmanetworkset_tmp", Database:="regulondb_7_5")>
Public Class sigmanetworkset_tmp: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("sigma_promoter"), DataType(MySqlDbType.VarChar, "80")> Public Property sigma_promoter As String
    <DatabaseField("snws_promoter_name"), DataType(MySqlDbType.VarChar, "100")> Public Property snws_promoter_name As String
    <DatabaseField("gene_coding"), DataType(MySqlDbType.VarChar, "100")> Public Property gene_coding As String
    <DatabaseField("gene_regulated"), DataType(MySqlDbType.VarChar, "100")> Public Property gene_regulated As String
    <DatabaseField("bnumber"), DataType(MySqlDbType.VarChar, "10")> Public Property bnumber As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `sigmanetworkset_tmp` (`sigma_promoter`, `snws_promoter_name`, `gene_coding`, `gene_regulated`, `bnumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `sigmanetworkset_tmp` (`sigma_promoter`, `snws_promoter_name`, `gene_coding`, `gene_regulated`, `bnumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `sigmanetworkset_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `sigmanetworkset_tmp` SET `sigma_promoter`='{0}', `snws_promoter_name`='{1}', `gene_coding`='{2}', `gene_regulated`='{3}', `bnumber`='{4}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, sigma_promoter, snws_promoter_name, gene_coding, gene_regulated, bnumber)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, sigma_promoter, snws_promoter_name, gene_coding, gene_regulated, bnumber)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
