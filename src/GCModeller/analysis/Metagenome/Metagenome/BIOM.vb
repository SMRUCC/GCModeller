#Region "Microsoft.VisualBasic::217ec91663d7f8b2a8e7c23bf41c5e93, ..\GCModeller\analysis\Metagenome\Metagenome\BIOM.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Assembly.NCBI.TaxonNode
Imports SMRUCC.genomics.foundation.BIOM.v10

Public Module BIOM

    <Extension>
    Public Function [Imports](source As IEnumerable(Of Names), Optional takes As Integer = 100, Optional cut As Integer = 50) As Json
        Dim array As Names() = LinqAPI.Exec(Of Names) <=
            From x As Names
            In source
            Where x.NumOfSeqs >= cut
            Select x
            Order By x.NumOfSeqs Descending

        array = array.Take(takes).ToArray

        Dim rows As row() = LinqAPI.Exec(Of row) <=
            From x As Names
            In array
            Where Not x.taxonomy.IsBlank AndAlso x.Composition IsNot Nothing
            Select New row With {
                .id = x.Unique,
                .metadata = New meta With {
                    .taxonomy = x.taxonomy.Split(";"c)
                }
            }
        Dim names As column() = LinqAPI.Exec(Of column) <=
            From sid As String
            In array _
                .Where(Function(x) x.Composition IsNot Nothing) _
                .Select(Function(x) x.Composition.Keys) _
                .MatrixAsIterator _
                .Distinct
            Select New column With {
                .id = sid
            }
        Dim data As New List(Of Integer())
        Dim nameIndex = names.SeqIterator.ToDictionary(
            Function(x) x.obj.id,
            Function(x) x.i)

        For Each x As SeqValue(Of Names) In array.Where(Function(xx) xx.Composition IsNot Nothing).SeqIterator
            Dim n As Integer = x.obj.NumOfSeqs

            For Each cpi In x.obj.Composition
                data += {x.i, nameIndex(cpi.Key), CInt(n * Val(cpi.Value) / 100) + 1}
            Next
        Next

        Return New Json With {
            .id = Guid.NewGuid.ToString,
            .format = "Biological Observation Matrix 1.0.0",
            .format_url = "http://biom-format.org",
            .type = "OTU table",
            .generated_by = "GCModeller",
            .date = Now,
            .matrix_type = "sparse",
            .matrix_element_type = "int",
            .shape = {array.Length, 4},
            .data = data,
            .rows = rows,
            .columns = names
        }
    End Function

    <Extension>
    Public Function EXPORT(table As IEnumerable(Of RelativeSample)) As Json
        Dim array As Names() = LinqAPI.Exec(Of Names) <=
            From x As RelativeSample
            In table
            Select New Names With {
                .NumOfSeqs = 100,
                .Composition = x.Samples.ToDictionary(Function(xx) xx.Key, Function(xx) CStr(xx.Value * 100)),
                .taxonomy = x.Taxonomy.Split(";"c).SeqIterator.ToArray(Function(s) BIOMPrefix(s.i) & s.obj).JoinBy(";"),
                .Unique = x.OTU
            }
        Return array.Imports(array.Length + 10, 0)
    End Function

    Public Class RelativeSample
        <Column("#OTU_num")> Public Property OTU As String
        Public Property Taxonomy As String
        <Meta(GetType(Double))>
        Public Property Samples As Dictionary(Of String, Double)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Module
