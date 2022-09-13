#Region "Microsoft.VisualBasic::d9ec5bba40d9a6a052f4c884e569d56d, GCModeller\data\MicrobesOnline\MySQL\genomics\locus2regprecise.vb"

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

    '   Total Lines: 68
    '    Code Lines: 39
    ' Comment Lines: 22
    '   Blank Lines: 7
    '     File Size: 3.99 KB


    ' Class locus2regprecise
    ' 
    '     Properties: geneInOperonIndex, locusId, regulationMode, regulatorLocusId, regulatorName
    '                 regulonId, sourceTypeTermId
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
''' DROP TABLE IF EXISTS `locus2regprecise`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `locus2regprecise` (
'''   `locusId` int(11) NOT NULL,
'''   `geneInOperonIndex` int(11) NOT NULL,
'''   `sourceTypeTermId` int(11) NOT NULL,
'''   `regulonId` int(11) NOT NULL,
'''   `regulatorName` varchar(255) NOT NULL,
'''   `regulatorLocusId` int(11) NOT NULL,
'''   `regulationMode` varchar(255) NOT NULL
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("locus2regprecise")>
Public Class locus2regprecise: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property locusId As Long
    <DatabaseField("geneInOperonIndex"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property geneInOperonIndex As Long
    <DatabaseField("sourceTypeTermId"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property sourceTypeTermId As Long
    <DatabaseField("regulonId"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property regulonId As Long
    <DatabaseField("regulatorName"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property regulatorName As String
    <DatabaseField("regulatorLocusId"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property regulatorLocusId As Long
    <DatabaseField("regulationMode"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property regulationMode As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `locus2regprecise` (`locusId`, `geneInOperonIndex`, `sourceTypeTermId`, `regulonId`, `regulatorName`, `regulatorLocusId`, `regulationMode`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `locus2regprecise` (`locusId`, `geneInOperonIndex`, `sourceTypeTermId`, `regulonId`, `regulatorName`, `regulatorLocusId`, `regulationMode`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `locus2regprecise` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `locus2regprecise` SET `locusId`='{0}', `geneInOperonIndex`='{1}', `sourceTypeTermId`='{2}', `regulonId`='{3}', `regulatorName`='{4}', `regulatorLocusId`='{5}', `regulationMode`='{6}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locusId, geneInOperonIndex, sourceTypeTermId, regulonId, regulatorName, regulatorLocusId, regulationMode)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locusId, geneInOperonIndex, sourceTypeTermId, regulonId, regulatorName, regulatorLocusId, regulationMode)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
