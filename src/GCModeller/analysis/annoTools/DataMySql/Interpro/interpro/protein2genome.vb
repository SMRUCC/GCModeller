#Region "Microsoft.VisualBasic::67f1e6f0d3a7e72ee565eb443bedcfc0, ..\GCModeller\analysis\annoTools\DataMySql\Interpro\interpro\protein2genome.vb"

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

REM  Dump @12/3/2015 8:52:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `protein2genome`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein2genome` (
'''   `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   PRIMARY KEY (`oscode`,`protein_ac`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein2genome", Database:="interpro")>
Public Class protein2genome: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6")> Public Property protein_ac As String
    <DatabaseField("oscode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property oscode As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `protein2genome` (`protein_ac`, `oscode`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `protein2genome` (`protein_ac`, `oscode`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `protein2genome` WHERE `oscode`='{0}' and `protein_ac`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `protein2genome` SET `protein_ac`='{0}', `oscode`='{1}' WHERE `oscode`='{2}' and `protein_ac`='{3}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, oscode, protein_ac)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, oscode)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, oscode)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, protein_ac, oscode, oscode, protein_ac)
    End Function
#End Region
End Class


End Namespace
