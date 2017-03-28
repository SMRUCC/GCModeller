#Region "Microsoft.VisualBasic::4558da1a6be69de5a8721d928e446234, ..\core\Bio.Assembly\Assembly\NCBI\Taxonomy\TaxidMaps.vb"

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
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text

Namespace Assembly.NCBI.Taxonomy

    Public Module TaxidMaps

        Public Delegate Function Mapping(id$) As Integer

        <Extension> Public Function MapByAcc(acc2taxid$) As Mapping
            Dim taxids As BucketDictionary(Of String, Integer) =
                ReadFile(acc2taxid) _
                .CreateBuckets(Function(x) x.Name,
                               Function(x) x.Value)
            Return Function(acc$) If(taxids.ContainsKey(acc), taxids(acc), -1)
        End Function

        Public Function MapByGI(gi2taxid$) As Mapping
            Dim taxids As BucketDictionary(Of Integer, Integer) = Taxonomy.AcquireAuto(gi2taxid)

            Return Function(sgi$)
                       Dim gi% = CInt(Val(sgi$))
                       Return If(taxids.ContainsKey(gi), taxids(gi), -1)
                   End Function
        End Function

        ''' <summary>
        ''' 对默认的nt库的fasta标题进行解析操作
        ''' </summary>
        ''' <param name="mapping"></param>
        ''' <param name="is_gi2taxid"></param>
        ''' <returns></returns>
        Public Function Reference2Taxid(mapping As Mapping, is_gi2taxid As Boolean) As Mapping
            Dim parser As Func(Of String, String) = (GetParser(is_gi2taxid))

            Return Function(ref)
                       Dim xid$ = parser(ref)

                       If String.IsNullOrEmpty(xid) Then
                           Call ref.PrintException
                           Return -1
                       Else
                           Return mapping(xid)
                       End If
                   End Function
        End Function

        Public Function GetParser(is_gi2taxid As Boolean) As Func(Of String, String)
            If is_gi2taxid Then
                Return Function(ref$)
                           Dim gis$ = Regex.Match(ref, "gi\|\d+").Value
                           Dim gi$ = gis.Split("|"c).LastOrDefault

                           Return gi
                       End Function
            Else
                Return AddressOf GetAccessionId
            End If
        End Function

#Region "NT header parser"

        ''' <summary>
        ''' 从标准的nt fasta标题之中解析出``accessionId``.
        ''' </summary>
        ''' <param name="header$"></param>
        ''' <returns></returns>
        Public Function GetAccessionId(header$) As String
            Return header _
                .Split _
                .First _
                .Split("."c) _
                .First
        End Function
#End Region
    End Module
End Namespace
