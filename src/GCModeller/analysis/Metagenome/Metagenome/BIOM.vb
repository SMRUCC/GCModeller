#Region "Microsoft.VisualBasic::7d3a4263ea2ac02a1ab6538964e8e5c4, ..\GCModeller\analysis\Metagenome\Metagenome\BIOM.vb"

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
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.foundation.BIOM.v10
Imports SMRUCC.genomics.Metagenomics

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
                .IteratesALL _
                .Distinct
            Select New column With {
                .id = sid
            }
        Dim data As New List(Of Integer())
        Dim nameIndex = names.SeqIterator.ToDictionary(
            Function(x) x.value.id,
            Function(x) x.i)

        For Each x As SeqValue(Of Names) In array.Where(Function(xx) xx.Composition IsNot Nothing).SeqIterator
            Dim n As Integer = x.value.NumOfSeqs

            For Each cpi In x.value.Composition
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
    Public Function EXPORT(table As IEnumerable(Of OTUData), Optional alreadyBIOMTax As Boolean = False) As Json
        Dim getTax As Func(Of String, String)

        If alreadyBIOMTax Then
            getTax = Function(s) s
        Else
            getTax = Function(tax) tax _
                .Split(";"c) _
                .SeqIterator _
                .ToArray(Function(s) BIOMPrefix(s.i) & s.value) _
                .JoinBy(";")
        End If

        Dim array As Names() = LinqAPI.Exec(Of Names) <=
            From x As OTUData
            In table
            Select New Names With {
                .NumOfSeqs = 100,
                .Composition = x.Data.ToDictionary(
                    Function(xx) xx.Key,
                    Function(xx) CStr(Val(xx.Value) * 100)),
                .taxonomy = getTax(x.Taxonomy),
                .Unique = x.OTU
            }
        Return array.Imports(array.Length + 10, 0)
    End Function
End Module
