#Region "Microsoft.VisualBasic::c45c30209ccc0187c2fd2fa8d47c68a0, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\UniProt\IdMapping.vb"

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

    '   Total Lines: 143
    '    Code Lines: 104
    ' Comment Lines: 28
    '   Blank Lines: 11
    '     File Size: 4.96 KB


    '     Class IdMapping
    ' 
    '         Properties: Additional_PubMed, EMBL, EMBL_CDS, Ensembl, Ensembl_PRO
    '                     Ensembl_TRS, GeneID_EntrezGene, GI, GO, MIM
    '                     NCBI_Taxon, PDB, PIR, PubMed, RefSeq
    '                     UniGene, UniParc, UniProtKB_AC, UniProtKB_ID, UniRef100
    '                     UniRef50, UniRef90
    ' 
    '         Function: LoadDoc, mapObjectParser, Synonym, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.Uniprot

    ''' <summary>
    ''' idmapping_selected.tab
    ''' We also provide this tab-delimited table which includes
    ''' the following mappings delimited by tab
    ''' 
    ''' 1. UniProtKB-AC
    ''' 2. UniProtKB-ID
    ''' 3. GeneID (EntrezGene)
    ''' 4. RefSeq
    ''' 5. GI
    ''' 6. PDB
    ''' 7. GO
    ''' 8. UniRef100
    ''' 9. UniRef90
    ''' 10. UniRef50
    ''' 11. UniParc
    ''' 12. PIR
    ''' 13. NCBI-taxon
    ''' 14. MIM
    ''' 15. UniGene
    ''' 16. PubMed
    ''' 17. EMBL
    ''' 18. EMBL-CDS
    ''' 19. Ensembl
    ''' 20. Ensembl_TRS
    ''' 21. Ensembl_PRO
    ''' 22. Additional PubMed
    ''' </summary>
    Public Class IdMapping

        Public Property UniProtKB_AC As String
        Public Property UniProtKB_ID As String
        Public Property GeneID_EntrezGene As String
        Public Property RefSeq As String
        Public Property GI As String
        Public Property PDB As String
        Public Property GO As String
        Public Property UniRef100 As String
        Public Property UniRef90 As String
        Public Property UniRef50 As String
        Public Property UniParc As String
        Public Property PIR As String
        Public Property NCBI_Taxon As String
        Public Property MIM As String
        Public Property UniGene As String
        Public Property PubMed As String
        Public Property EMBL As String
        Public Property EMBL_CDS As String
        Public Property Ensembl As String
        Public Property Ensembl_TRS As String
        Public Property Ensembl_PRO As String
        Public Property Additional_PubMed As String

        Public Function Synonym() As Synonym
            Return New Synonym With {
                .accessionID = UniProtKB_ID,
                .[alias] = {
                    UniProtKB_ID,
                    GeneID_EntrezGene,
                    RefSeq,
                    GI,
                    PDB,
                    GO,
                    UniRef100,
                    UniRef90,
                    UniRef50,
                    UniParc,
                    PIR,
                    NCBI_Taxon,
                    MIM,
                    UniGene,
                    PubMed,
                    EMBL,
                    EMBL_CDS,
                    Ensembl,
                    Ensembl_TRS,
                    Ensembl_PRO,
                    Additional_PubMed
                }
            }
        End Function

        Public Overrides Function ToString() As String
            Return UniProtKB_ID
        End Function

        Public Shared Function LoadDoc(path As String) As LinkedList(Of IdMapping)
            Dim Reader As New PartitionedStream(path, 1024)
            Dim list As New LinkedList(Of IdMapping)

            Do While Not Reader.EOF
                Dim lines As String() = Reader.ReadPartition
                If lines.IsNullOrEmpty Then
                    Continue Do
                End If
                Dim data As IdMapping() = lines.Select(Function(line) mapObjectParser(line)).ToArray
                Call list.AddRange(data)
            Loop

            Return list
        End Function

        Private Shared Function mapObjectParser(line As String) As IdMapping
            Dim tokens As String() = Strings.Split(line, vbTab)
            Dim p As i32 = 0
            Dim maps As New IdMapping

            With maps
                .UniProtKB_AC = tokens.ElementAtOrDefault(++p)
                .UniProtKB_ID = tokens.ElementAtOrDefault(++p)
                .GeneID_EntrezGene = tokens.ElementAtOrDefault(++p)
                .RefSeq = tokens.ElementAtOrDefault(++p)
                .GI = tokens.ElementAtOrDefault(++p)
                .PDB = tokens.ElementAtOrDefault(++p)
                .GO = tokens.ElementAtOrDefault(++p)
                .UniRef100 = tokens.ElementAtOrDefault(++p)
                .UniRef90 = tokens.ElementAtOrDefault(++p)
                .UniRef50 = tokens.ElementAtOrDefault(++p)
                .UniParc = tokens.ElementAtOrDefault(++p)
                .PIR = tokens.ElementAtOrDefault(++p)
                .NCBI_Taxon = tokens.ElementAtOrDefault(++p)
                .MIM = tokens.ElementAtOrDefault(++p)
                .UniGene = tokens.ElementAtOrDefault(++p)
                .PubMed = tokens.ElementAtOrDefault(++p)
                .EMBL = tokens.ElementAtOrDefault(++p)
                .EMBL_CDS = tokens.ElementAtOrDefault(++p)
                .Ensembl = tokens.ElementAtOrDefault(++p)
                .Ensembl_TRS = tokens.ElementAtOrDefault(++p)
                .Ensembl_PRO = tokens.ElementAtOrDefault(++p)
                .Additional_PubMed = tokens.ElementAtOrDefault(++p)
            End With

            Return maps
        End Function
    End Class
End Namespace
