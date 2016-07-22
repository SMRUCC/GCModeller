#Region "Microsoft.VisualBasic::28ed24c09276a7f545a3c6ac6c2a721c, ..\GCModeller\analysis\Annotation\UniprotSprot\uniprotKB\id_mappings.vb"

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

REM  Dump @12/3/2015 8:59:58 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace UniprotKB.MySQL.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `id_mappings`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `id_mappings` (
'''   `UniProtKB_AC` int(11) NOT NULL,
'''   `UniProtKB_ID` varchar(45) DEFAULT NULL,
'''   `GeneID_EntrezGene` varchar(45) DEFAULT NULL,
'''   `RefSeq` varchar(45) DEFAULT NULL,
'''   `GI` varchar(45) DEFAULT NULL,
'''   `pdb` varchar(45) DEFAULT NULL,
'''   `go` varchar(45) DEFAULT NULL,
'''   `UniRef100` varchar(45) DEFAULT NULL,
'''   `UniRef90` varchar(45) DEFAULT NULL,
'''   `UniRef50` varchar(45) DEFAULT NULL,
'''   `UniParc` varchar(45) DEFAULT NULL,
'''   `pir` varchar(45) DEFAULT NULL,
'''   `NCBI_Taxon` varchar(45) DEFAULT NULL,
'''   `MIM` varchar(45) DEFAULT NULL,
'''   `UniGene` varchar(45) DEFAULT NULL,
'''   `PubMed` varchar(45) DEFAULT NULL,
'''   `EMBL` varchar(45) DEFAULT NULL,
'''   `EMBL_CDS` varchar(45) DEFAULT NULL,
'''   `Ensembl` varchar(45) DEFAULT NULL,
'''   `Ensembl_TRS` varchar(45) DEFAULT NULL,
'''   `Ensembl_PRO` varchar(45) DEFAULT NULL,
'''   `Additional_PubMed` text,
'''   PRIMARY KEY (`UniProtKB_AC`),
'''   UNIQUE KEY `UniProtKB_AC_UNIQUE` (`UniProtKB_AC`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("id_mappings", Database:="uniprot")>
Public Class id_mappings: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("UniProtKB_AC"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property UniProtKB_AC As Long
    <DatabaseField("UniProtKB_ID"), DataType(MySqlDbType.VarChar, "45")> Public Property UniProtKB_ID As String
    <DatabaseField("GeneID_EntrezGene"), DataType(MySqlDbType.VarChar, "45")> Public Property GeneID_EntrezGene As String
    <DatabaseField("RefSeq"), DataType(MySqlDbType.VarChar, "45")> Public Property RefSeq As String
    <DatabaseField("GI"), DataType(MySqlDbType.VarChar, "45")> Public Property GI As String
    <DatabaseField("pdb"), DataType(MySqlDbType.VarChar, "45")> Public Property pdb As String
    <DatabaseField("go"), DataType(MySqlDbType.VarChar, "45")> Public Property go As String
    <DatabaseField("UniRef100"), DataType(MySqlDbType.VarChar, "45")> Public Property UniRef100 As String
    <DatabaseField("UniRef90"), DataType(MySqlDbType.VarChar, "45")> Public Property UniRef90 As String
    <DatabaseField("UniRef50"), DataType(MySqlDbType.VarChar, "45")> Public Property UniRef50 As String
    <DatabaseField("UniParc"), DataType(MySqlDbType.VarChar, "45")> Public Property UniParc As String
    <DatabaseField("pir"), DataType(MySqlDbType.VarChar, "45")> Public Property pir As String
    <DatabaseField("NCBI_Taxon"), DataType(MySqlDbType.VarChar, "45")> Public Property NCBI_Taxon As String
    <DatabaseField("MIM"), DataType(MySqlDbType.VarChar, "45")> Public Property MIM As String
    <DatabaseField("UniGene"), DataType(MySqlDbType.VarChar, "45")> Public Property UniGene As String
    <DatabaseField("PubMed"), DataType(MySqlDbType.VarChar, "45")> Public Property PubMed As String
    <DatabaseField("EMBL"), DataType(MySqlDbType.VarChar, "45")> Public Property EMBL As String
    <DatabaseField("EMBL_CDS"), DataType(MySqlDbType.VarChar, "45")> Public Property EMBL_CDS As String
    <DatabaseField("Ensembl"), DataType(MySqlDbType.VarChar, "45")> Public Property Ensembl As String
    <DatabaseField("Ensembl_TRS"), DataType(MySqlDbType.VarChar, "45")> Public Property Ensembl_TRS As String
    <DatabaseField("Ensembl_PRO"), DataType(MySqlDbType.VarChar, "45")> Public Property Ensembl_PRO As String
    <DatabaseField("Additional_PubMed"), DataType(MySqlDbType.Text)> Public Property Additional_PubMed As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `id_mappings` (`UniProtKB_AC`, `UniProtKB_ID`, `GeneID_EntrezGene`, `RefSeq`, `GI`, `pdb`, `go`, `UniRef100`, `UniRef90`, `UniRef50`, `UniParc`, `pir`, `NCBI_Taxon`, `MIM`, `UniGene`, `PubMed`, `EMBL`, `EMBL_CDS`, `Ensembl`, `Ensembl_TRS`, `Ensembl_PRO`, `Additional_PubMed`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `id_mappings` (`UniProtKB_AC`, `UniProtKB_ID`, `GeneID_EntrezGene`, `RefSeq`, `GI`, `pdb`, `go`, `UniRef100`, `UniRef90`, `UniRef50`, `UniParc`, `pir`, `NCBI_Taxon`, `MIM`, `UniGene`, `PubMed`, `EMBL`, `EMBL_CDS`, `Ensembl`, `Ensembl_TRS`, `Ensembl_PRO`, `Additional_PubMed`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `id_mappings` WHERE `UniProtKB_AC` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `id_mappings` SET `UniProtKB_AC`='{0}', `UniProtKB_ID`='{1}', `GeneID_EntrezGene`='{2}', `RefSeq`='{3}', `GI`='{4}', `pdb`='{5}', `go`='{6}', `UniRef100`='{7}', `UniRef90`='{8}', `UniRef50`='{9}', `UniParc`='{10}', `pir`='{11}', `NCBI_Taxon`='{12}', `MIM`='{13}', `UniGene`='{14}', `PubMed`='{15}', `EMBL`='{16}', `EMBL_CDS`='{17}', `Ensembl`='{18}', `Ensembl_TRS`='{19}', `Ensembl_PRO`='{20}', `Additional_PubMed`='{21}' WHERE `UniProtKB_AC` = '{22}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, UniProtKB_AC)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, UniProtKB_AC, UniProtKB_ID, GeneID_EntrezGene, RefSeq, GI, pdb, go, UniRef100, UniRef90, UniRef50, UniParc, pir, NCBI_Taxon, MIM, UniGene, PubMed, EMBL, EMBL_CDS, Ensembl, Ensembl_TRS, Ensembl_PRO, Additional_PubMed)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, UniProtKB_AC, UniProtKB_ID, GeneID_EntrezGene, RefSeq, GI, pdb, go, UniRef100, UniRef90, UniRef50, UniParc, pir, NCBI_Taxon, MIM, UniGene, PubMed, EMBL, EMBL_CDS, Ensembl, Ensembl_TRS, Ensembl_PRO, Additional_PubMed)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, UniProtKB_AC, UniProtKB_ID, GeneID_EntrezGene, RefSeq, GI, pdb, go, UniRef100, UniRef90, UniRef50, UniParc, pir, NCBI_Taxon, MIM, UniGene, PubMed, EMBL, EMBL_CDS, Ensembl, Ensembl_TRS, Ensembl_PRO, Additional_PubMed, UniProtKB_AC)
    End Function
#End Region
End Class


End Namespace

