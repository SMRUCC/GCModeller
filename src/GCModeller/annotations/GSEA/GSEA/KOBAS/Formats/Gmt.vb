#Region "Microsoft.VisualBasic::7e333b3bd5410b16558225bbcb1c5aa9, GCModeller\annotations\GSEA\GSEA\KOBAS\Formats\Gmt.vb"

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

    '   Total Lines: 92
    '    Code Lines: 61
    ' Comment Lines: 21
    '   Blank Lines: 10
    '     File Size: 3.89 KB


    '     Class Gmt
    ' 
    '         Properties: attributes, clusters, database, species
    ' 
    '         Function: GetEnumerator, IEnumerable_GetEnumerator, LoadFile
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports tsv = Microsoft.VisualBasic.Data.csv.IO.File

Namespace KOBAS

    ''' <summary>
    ''' #### GMT: Gene Matrix Transposed file format (``*.gmt``)
    ''' 
    ''' The GMT file format is a tab delimited file format that 
    ''' describes gene sets. In the GMT format, each row represents 
    ''' a gene set; in the GMX format, each column represents a 
    ''' gene set. 
    ''' 
    ''' Each gene set is described by a name, a description, and the 
    ''' genes in the gene set. GSEA uses the description field to 
    ''' determine what hyperlink to provide in the report for the 
    ''' gene set description: if the description is ``na``, GSEA provides 
    ''' a link to the named gene set in MSigDB; if the description is 
    ''' a URL, GSEA provides a link to that URL.
    ''' </summary>
    Public Class Gmt : Implements IEnumerable(Of Cluster)

        Public ReadOnly Property species As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return attributes.TryGetValue({NameOf(species), "Species"})
            End Get
        End Property

        Public ReadOnly Property database As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return attributes.TryGetValue({NameOf(database), "Database"})
            End Get
        End Property

        Public Property attributes As Dictionary(Of String, String)
        Public Property clusters As Cluster()

        ''' <summary>
        ''' parse gmt file
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function LoadFile(path As String) As Gmt
            Dim table = tsv.LoadTsv(path, Encodings.UTF8)
            Dim attrs As Dictionary(Of String, String) = table _
                .Comments _
                .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                .ToDictionary(Function(t) t.Name.Trim("#"c, " "c),
                              Function(t) t.Value)
            Dim parseClusters As Cluster() =
                Iterator Function() As IEnumerable(Of Cluster)
                    ' 假设注释只出现在前面
                    For Each row As RowObject In table.Skip(attrs.Count)
                        Yield New Cluster With {
                                .ID = row(0),
                                .description = row(1),
                                .names = .ID,
                                .members = row.Skip(2) _
                                    .Select(Function(name)
                                                Return New Synonym With {
                                                    .accessionID = name,
                                                    .[alias] = {name}
                                                }
                                            End Function) _
                                    .ToArray
                            }
                    Next
                End Function().ToArray

            Return New Gmt With {
                .attributes = attrs,
                .clusters = parseClusters
            }
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Cluster) Implements IEnumerable(Of Cluster).GetEnumerator
            For Each cluster As Cluster In clusters
                Yield cluster
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class

End Namespace
