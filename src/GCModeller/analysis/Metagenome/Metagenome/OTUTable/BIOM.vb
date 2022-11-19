#Region "Microsoft.VisualBasic::3ea56a7ab82afc264d37f006a521947f, GCModeller\analysis\Metagenome\Metagenome\OTUTable\BIOM.vb"

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

    '   Total Lines: 243
    '    Code Lines: 179
    ' Comment Lines: 33
    '   Blank Lines: 31
    '     File Size: 8.79 KB


    ' Module BIOM
    ' 
    '     Function: (+2 Overloads) [Imports], BIOMTaxonomyString, denseMatrix, EXPORT, sparseMatrix
    '               TaxonomyString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.foundation.BIOM.v10
Imports SMRUCC.genomics.foundation.BIOM.v10.components
Imports SMRUCC.genomics.Metagenomics
Imports r = System.Text.RegularExpressions.Regex

''' <summary>
''' 生成BIOM数据模型
''' </summary>
Public Module BIOM

    ''' <summary>
    ''' 按照OTU的序列数量进行降序排序之后，所取出来的OTU的数量，默认只截取前100个OTU
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="takes">Reorder <see cref="Names.numOfSeqs"/> desc and then take top n otu for imports as biom data.</param>
    ''' <param name="cut">
    ''' <see cref="Names.numOfSeqs"/> should at least greater than this number cutoff value that will be imports as biom data.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function [Imports](source As IEnumerable(Of Names),
                              Optional takes% = 100,
                              Optional cut% = 50,
                              Optional denseMatrix As Boolean = True,
                              Optional comments$ = Nothing) As IntegerMatrix

        Dim array As Names() = LinqAPI.Exec(Of Names) _
 _
            () <= From x As Names
                  In source
                  Where x.numOfSeqs >= cut
                  Select x
                  Order By x.numOfSeqs Descending

        array = array.Take(takes).ToArray

        ' OTU list
        Dim rows As row() = LinqAPI.Exec(Of row) <=
 _
            From otu As Names
            In array
            Where Not otu.taxonomy.StringEmpty AndAlso otu.composition IsNot Nothing
            Select New row With {
                .id = otu.unique,
                .metadata = New meta With {
                    .taxonomy = otu.taxonomy.Split(";"c)
                }
            }

        ' Sample list
        Dim names As column() = LinqAPI.Exec(Of column) _
 _
            () <= From sid As String
                  In array _
                      .Where(Function(x)
                                 Return Not x.composition Is Nothing
                             End Function) _
                      .Select(Function(x) x.composition.Keys) _
                      .IteratesALL _
                      .Distinct
                  Select New column With {
                      .id = sid
                  }

        Dim nameIndex As Index(Of String) = names _
            .Select(Function(col) col.id) _
            .ToArray
        Dim data As Integer()()

        If denseMatrix Then
            data = array.denseMatrix(nameIndex).ToArray
        Else
            data = array.sparseMatrix(nameIndex).ToArray
        End If

        Return New IntegerMatrix With {
            .id = Guid.NewGuid.ToString,
            .format = "Biological Observation Matrix 1.0.0",
            .format_url = "http://biom-format.org",
            .type = namesOf.OTU_table,
            .generated_by = "GCModeller",
            .date = Now,
            .matrix_type = If(denseMatrix, matrix_type.dense, matrix_type.sparse),
            .matrix_element_type = "int",
            .shape = data.DimensionSizeOf.ToArray,
            .data = data,
            .rows = rows,
            .columns = names,
            .comment = comments Or $"Number of sequence cutoff={cut} and takes top {takes} OTU.".AsDefault
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="array"></param>
    ''' <param name="nameIndex">The column name index</param>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function sparseMatrix(array As gast.Names(), nameIndex As Index(Of String)) As IEnumerable(Of Integer())
        Dim ix, iy As Integer
        Dim composition%

        For Each x As SeqValue(Of Names) In array _
            .Where(Function(xx)
                       Return xx.composition IsNot Nothing
                   End Function) _
            .SeqIterator

            Dim n% = x.value.numOfSeqs

            For Each cpi In x.value.composition
                ix = x.i
                iy = nameIndex(cpi.Key)
                composition = CInt(n * cpi.Value / 100)

                Yield New Integer() {ix, iy, composition}
            Next
        Next
    End Function

    <Extension>
    Private Iterator Function denseMatrix(array As gast.Names(), nameIndex As Index(Of String)) As IEnumerable(Of Integer())
        Dim names$() = nameIndex.Objects

        For Each row As gast.Names In array
            Yield names _
                .Select(Function(name)
                            Return CInt(row.numOfSeqs * row.composition.TryGetValue(name, [default]:=0) / 100)
                        End Function) _
                .ToArray
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function BIOMTaxonomyString(taxonomy As Metagenomics.Taxonomy, Optional ranks As TaxonomyRanks = TaxonomyRanks.Strain) As String
        Return taxonomy.Select.Take(ranks - 100).ToArray.TaxonomyString
    End Function

    ReadOnly Unknown As Index(Of String) = {"", "Unassigned", "NA", NameOf(Unknown)}
    ReadOnly descRanks As SeqValue(Of String)() = NCBI.Taxonomy _
        .NcbiTaxonomyTree _
        .stdranks _
        .Objects _
        .Reverse _
        .SeqIterator _
        .ToArray

    <Extension>
    Public Function TaxonomyString(tax As Dictionary(Of String, String)) As String
        Dim l As New List(Of String)

        For Each rank As SeqValue(Of String) In descRanks
            If tax.ContainsKey(rank.value) AndAlso Not tax(rank.value) Like Unknown Then
                l += BIOMTaxonomy.BIOMPrefix(rank) & tax(rank.value)
            Else
                l += "NA"
            End If
        Next

        Dim s = r.Replace(l.JoinBy(";"), "(;NA)+$", "", RegexICMul)
        Return s
    End Function

    ''' <summary>
    ''' Imports sample matrix from biom file as OTU table data.
    ''' </summary>
    ''' <param name="biom"></param>
    ''' <returns></returns>
    <Extension>
    Public Function [Imports](biom As BIOMDataSet(Of Double)) As IEnumerable(Of OTUTable)
        Dim matrix As New Dictionary(Of String, OTUTable)

        For Each otu As NamedValue(Of [Property](Of Double)) In biom.PopulateRows
            If Not matrix.ContainsKey(otu.Name) Then
                matrix.Add(otu.Name, New OTUTable With {.ID = otu.Name})
            End If

            matrix(otu.Name).Append(otu, AddressOf Math.Max)
        Next

        Return matrix.Values
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="table">
    ''' 因为丰度数据可能是0到1之间的，也可能是原始的序列数量，也可能是经过归一化的，所以在这里会需要进行归一化操作，归一化为0到100之间的数
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function EXPORT(table As IEnumerable(Of OTUData(Of Double)), Optional denseMatrix As Boolean = True) As IntegerMatrix
        Dim matrix As OTUData(Of Double)() = table.ToArray
        Dim allSamples = matrix _
            .Select(Function(otu) otu.data.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray

        ' 在这里归一化为[0, 100]之间
        For Each sampleName As String In allSamples
            Dim counts As Vector = matrix _
                .Select(Function(otu)
                            Return otu.data.TryGetValue(sampleName, [default]:="0")
                        End Function) _
                .Select(AddressOf Val) _
                .AsVector

            counts = counts / counts.Max * 100

            For i As Integer = 0 To matrix.Length - 1
                matrix(i).data(sampleName) = CInt(counts(i))
            Next
        Next

        Dim array() = LinqAPI.Exec(Of Names) _
 _
            () <= From otu As OTUData(Of Double)
                  In table
                  Let taxonomy As String = otu.taxonomy _
                      .Split(";"c) _
                      .TaxonomyString
                  Select New Names With {
                      .numOfSeqs = 100,
                      .composition = otu.data,
                      .taxonomy = taxonomy,
                      .unique = otu.OTU
                  }

        Return array.Imports(array.Length + 10, 0, denseMatrix:=denseMatrix)
    End Function
End Module
