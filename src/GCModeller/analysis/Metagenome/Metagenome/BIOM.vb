#Region "Microsoft.VisualBasic::64c1bc79a76ee2bbcd641bbad82b4b65, analysis\Metagenome\Metagenome\BIOM.vb"

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

' Module BIOM
' 
'     Function: [Imports], BIOMTaxonomyString, EXPORT, (+2 Overloads) TaxonomyString
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
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
    ''' <param name="takes%"></param>
    ''' <param name="cut%"></param>
    ''' <returns></returns>
    <Extension>
    Public Function [Imports](source As IEnumerable(Of Names), Optional takes% = 100, Optional cut% = 50) As IntegerMatrix
        Dim array As Names() = LinqAPI.Exec(Of Names) _
 _
        () <= From x As Names
              In source
              Where x.NumOfSeqs >= cut
              Select x
              Order By x.NumOfSeqs Descending

        array = array.Take(takes).ToArray

        Dim rows As row() = LinqAPI.Exec(Of row) <=
 _
            From x As Names
            In array
            Where Not x.taxonomy.StringEmpty AndAlso x.Composition IsNot Nothing
            Select New row With {
                .id = x.Unique,
                .metadata = New meta With {
                    .taxonomy = x.taxonomy.Split(";"c)
                }
            }

        Dim names As column() = LinqAPI.Exec(Of column) _
 _
            () <= From sid As String
                  In array _
                      .Where(Function(x) x.Composition IsNot Nothing) _
                      .Select(Function(x) x.Composition.Keys) _
                      .IteratesALL _
                      .Distinct
                  Select New column With {
                      .id = sid
                  }

        Dim data As New List(Of Integer())
        Dim nameIndex = names _
            .SeqIterator _
            .ToDictionary(Function(x) x.value.id,
                          Function(x) x.i)

        For Each x As SeqValue(Of Names) In array _
            .Where(Function(xx)
                       Return xx.Composition IsNot Nothing
                   End Function) _
            .SeqIterator

            Dim n% = x.value.NumOfSeqs

            For Each cpi In x.value.Composition
                data += {x.i, nameIndex(cpi.Key), CInt(n * Val(cpi.Value) / 100) + 1}
            Next
        Next

        Return New IntegerMatrix With {
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

    ''' <summary>
    ''' 为Taxonomy Lineage添加BIOM前缀
    ''' </summary>
    ''' <param name="tax$"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function TaxonomyString(tax$()) As String
        Return tax _
            .TakeWhile(Function(s) Not s.TaxonomyRankEmpty) _
            .SeqIterator _
            .Select(Function(s)
                        Return BIOMTaxonomy.BIOMPrefix(s.i) & s.value
                    End Function) _
            .JoinBy(";")
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function BIOMTaxonomyString(taxonomy As Metagenomics.Taxonomy, Optional ranks As TaxonomyRanks = TaxonomyRanks.Strain) As String
        Return taxonomy.ToArray.Take(ranks - 100).ToArray.TaxonomyString
    End Function

    ReadOnly Unknown As Index(Of String) = {"", "Unassigned", "NA", NameOf(Unknown)}
    ReadOnly descRanks As SeqValue(Of String)() = NCBI.Taxonomy _
        .NcbiTaxonomyTree _
        .stdranks _
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

    <Extension>
    Public Function EXPORT(table As IEnumerable(Of OTUData), Optional alreadyBIOMTax As Boolean = False) As IntegerMatrix
        Dim getTax As Func(Of String, String)

        If alreadyBIOMTax Then
            getTax = Function(s) s
        Else
            getTax = Function(tax)
                         Return tax.Split(";"c).TaxonomyString
                     End Function
        End If

        Dim array() = LinqAPI.Exec(Of Names) _
 _
            () <= From x As OTUData
                  In table
                  Let comp = x.Data.ToDictionary(
                      Function(xx) xx.Key,
                      Function(xx) CStr(Val(xx.Value) * 100)
                  )
                  Select New Names With {
                      .NumOfSeqs = 100,
                      .Composition = comp,
                      .taxonomy = getTax(x.Taxonomy),
                      .Unique = x.OTU
                  }

        Return array.Imports(array.Length + 10, 0)
    End Function
End Module
