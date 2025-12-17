#Region "Microsoft.VisualBasic::0606059fcf3e972aebbd8bb17a85669f, Bio.Repository\test\Module1.vb"

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

'   Total Lines: 60
'    Code Lines: 42 (70.00%)
' Comment Lines: 3 (5.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 15 (25.00%)
'     File Size: 1.68 KB


' Module Module1
' 
'     Sub: loadIndexTest, Main, read1, readTest, write1
'          writeTest
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.KEGG.Metabolism

Module Module1

    Sub Main()
        Call loadIndexTest()
        Call queryTest()
        ' Call write1()
        ' Call read1()
        ' Call readTest()
        Call writeTest()
    End Sub

    Sub loadIndexTest()
        Dim genbankDb As AssemblySummaryGenbank = AssemblySummaryGenbank.CreateRepository("D:\datapool\assembly_summary_genbank.txt", "./genbank_index")


        Pause()
    End Sub

    Sub queryTest()
        Dim genbankDb As New AssemblySummaryGenbank(6, "./genbank_index")

        For Each key As String In {"GCA_000001545", "GCA_000002445", "GCF_052956585.1_ASM5295658v1_genomic.fna",
"GCA_003190765.1_ASM319076v1_genomic.fna",
"GCA_005281725.1_ASM528172v1_genomic.fna",
"GCA_025215515.1_ASM2521551v1_genomic.fna",
"GCA_031102545.1_ASM3110254v1_genomic.fna",
"GCA_031102585.1_ASM3110258v1_genomic.fna",
"GCA_031106005.1_ASM3110600v1_genomic.fna",
"GCA_031127735.1_ASM3112773v1_genomic.fna",
"GCA_031155695.1_ASM3115569v1_genomic.fna",
"GCA_038463185.1_ASM3846318v1_genomic.fna",
"GCA_038687005.1_ASM3868700v1_genomic.fna",
"GCF_000007545.1_ASM754v1_genomic.fna",
"GCF_000195955.2_ASM19595v2_genomic.fna",
"GCF_000208925.1_JCVI_ESG2_1.0_genomic.fna",
"GCF_000227135.1_ASM22713v2_genomic.fna",
"GCF_000277165.1_ASM27716v1_genomic.fna",
"GCF_000524195.1_ASM52419v1_genomic.fna",
"GCF_000820495.2_ViralMultiSegProj14656_genomic.fna",
"GCF_000855785.1_ViralMultiSegProj15044_genomic.fna",
"GCF_000857045.1_ViralProj15142_genomic.fna",
"GCF_000859625.1_ViralProj15144_genomic.fna",
"GCF_000862145.1_ViralProj15310_genomic.fna",
"GCF_000863025.1_ViralProj15315_genomic.fna",
"GCF_000865725.1_ViralMultiSegProj15521_genomic.fna",
"GCF_000871845.1_ViralProj20183_genomic.fna",
"GCF_002073495.2_ASM207349v2_genomic.fna",
"GCF_003033055.1_ASM303305v1_genomic.fna",
"GCF_003253775.1_ASM325377v1_genomic.fna",
"GCF_009858895.2_ASM985889v3_genomic.fna",
"GCF_022869645.1_ASM2286964v1_genomic.fna",
"GCF_045865195.1_ASM4586519v1_genomic.fna",
"GCF_046895985.1_ASM4689598v1_genomic.fna",
"GCF_050820025.1_ASM5082002v1_genomic.fna",
"GCF_051549835.1_ASM5154983v1_genomic.fna",
"GCF_051628085.1_ASM5162808v1_genomic.fna",
"GCF_051802275.1_ASM5180227v1_genomic.fna",
"GCF_051905025.1_ASM5190502v1_genomic.fna",
"GCF_052058455.1_ASM5205845v1_genomic.fna",
"GCF_052953925.1_ASM5295392v1_genomic.fna"}
            Call genbankDb.GetByAccessionId(key).GetJson.debug
        Next

        Pause()
    End Sub

    Sub write1()
        Dim list = ReactionClass.ScanRepository("E:\biodeep\biodeepdb_v3\KEGG\reaction_class").ToArray

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\reaction_class.repo".Open
            Call ReactionClassPack.WriteKeggDb(list, file)
        End Using
    End Sub

    Sub read1()
        Dim list As ReactionClass()

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\reaction_class.repo".Open
            list = ReactionClassPack.ReadKeggDb(file)
        End Using

        Pause()
    End Sub

    Sub writeTest()
        Dim list = CompoundRepository.ScanRepository("E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd").ToArray

        For Each cpd In list
            cpd.KCF = Nothing
        Next

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd.repo".Open
            Call KEGGCompoundPack.WriteKeggDb(list, file)
        End Using
    End Sub

    Sub readTest()
        Dim list As Compound()

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd.repo".Open(IO.FileMode.Open, doClear:=False, [readOnly]:=True)
            list = KEGGCompoundPack.ReadKeggDb(file)
        End Using

        Pause()
    End Sub
End Module
