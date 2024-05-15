#Region "Microsoft.VisualBasic::96717b41f0e05c091df96c8af82c709f, annotations\GSEA\GSVA\test\ArabidopsisThaliana.vb"

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

    '   Total Lines: 72
    '    Code Lines: 63
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 2.90 KB


    ' Module ArabidopsisThalianaTest
    ' 
    '     Function: (+2 Overloads) LoadKEGG
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSVA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Module ArabidopsisThalianaTest

    Dim base As String = "E:\GCModeller\src\workbench\R#\demo\HTS\GSVA"

    Sub Main()
        Dim expr As Matrix = Matrix.LoadData($"{base}/ath_norm.csv")
        Dim kegg As New Background With {
            .clusters = LoadKEGG.ToArray,
            .name = "Arabidopsis Thaliana",
            .id = "ath"
        }
        Dim scores = GSVA.gsva(expr, kegg)

        Call scores.SaveMatrix($"{base}/gsva.csv", "kegg_pathways")

        Pause()
    End Sub

    Private Iterator Function LoadKEGG() As IEnumerable(Of Cluster)
        Using file = $"{base}/ath.db".Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Using pack As New StreamPack(file, [readonly]:=True)
                Dim pathways As StreamGroup = pack.GetObject("/pathways/")

                For Each cl In LoadKEGG(pack, pathways)
                    Yield cl
                Next
            End Using
        End Using
    End Function

    Private Iterator Function LoadKEGG(pack As StreamPack, dir As StreamGroup) As IEnumerable(Of Cluster)
        For Each file As StreamObject In dir.files
            If TypeOf file Is StreamGroup Then
                For Each cl In LoadKEGG(pack, file)
                    Yield cl
                Next
            Else
                Dim xml As String = pack.ReadText(file.referencePath.ToString)
                Dim pathway As Pathway = xml.LoadFromXml(Of Pathway)()

                Yield New Cluster With {
                    .ID = pathway.name,
                    .description = pathway.description,
                    .names = pathway.name,
                    .members = pathway.genes _
                        .SafeQuery _
                        .Select(Function(g)
                                    Return New BackgroundGene With {
                                        .accessionID = g.geneId,
                                        .[alias] = {},
                                        .locus_tag = New NamedValue With {.name = g.geneId, .text = g.geneName},
                                        .name = g.geneName,
                                        .term_id = BackgroundGene.UnknownTerms(g.KO).ToArray
                                    }
                                End Function) _
                        .ToArray
                }
            End If
        Next
    End Function
End Module
