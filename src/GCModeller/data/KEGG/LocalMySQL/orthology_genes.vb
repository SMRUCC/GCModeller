#Region "Microsoft.VisualBasic::3ab7318341193ee2f4789a57fe1a0e39, ..\GCModeller\CLI_tools\KEGG\LocalMySQL\orthology_genes.vb"

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

REM  Dump @12/3/2015 7:34:48 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `orthology_genes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `orthology_genes` (
'''   `ko` varchar(100) NOT NULL,
'''   `gene` varchar(100) NOT NULL,
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `url` text,
'''   `sp_code` varchar(45) DEFAULT NULL COMMENT 'The bacterial genome name brief code in KEGG database',
'''   `name` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`gene`,`ko`),
'''   UNIQUE KEY `id_UNIQUE` (`id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=9312 DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("orthology_genes", Database:="jp_kegg2")>
Public Class orthology_genes: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ko"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property ko As String
    <DatabaseField("gene"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property gene As String
    <DatabaseField("id"), AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("url"), DataType(MySqlDbType.Text)> Public Property url As String
''' <summary>
''' The bacterial genome name brief code in KEGG database
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("sp_code"), DataType(MySqlDbType.VarChar, "45")> Public Property sp_code As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `orthology_genes` (`ko`, `gene`, `url`, `sp_code`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `orthology_genes` (`ko`, `gene`, `url`, `sp_code`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `orthology_genes` WHERE `gene`='{0}' and `ko`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `orthology_genes` SET `ko`='{0}', `gene`='{1}', `id`='{2}', `url`='{3}', `sp_code`='{4}', `name`='{5}' WHERE `gene`='{6}' and `ko`='{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, gene, ko)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ko, gene, url, sp_code, name)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ko, gene, url, sp_code, name)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ko, gene, id, url, sp_code, name, gene, ko)
    End Function
#End Region
End Class


End Namespace
