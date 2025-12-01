#Region "Microsoft.VisualBasic::0e0753ce2f7a38baf08f081d7f972102, core\Bio.Assembly\Assembly\NCBI\Taxonomy\TaxidMaps.vb"

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
    '    Code Lines: 56 (60.87%)
    ' Comment Lines: 22 (23.91%)
    '    - Xml Docs: 95.45%
    ' 
    '   Blank Lines: 14 (15.22%)
    '     File Size: 3.52 KB


    '     Module TaxidMaps
    ' 
    ' 
    '         Delegate Function
    ' 
    '             Function: GetAccessionId, GetParser, MapByAcc, MapByGI, Reference2Taxid
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting

Namespace Assembly.NCBI.Taxonomy

    Public Module TaxidMaps

        ''' <summary>
        ''' 将所给定的编号映射为taxid的一个操作
        ''' </summary>
        ''' <param name="id">序列编号，例如gi编号或者accession编号</param>
        ''' <returns>taxid编号</returns>
        Public Delegate Function Mapping(id$) As UInteger

        <Extension>
        Public Function MapByAcc(acc2taxid$) As Mapping
            Dim taxids As BucketDictionary(Of String, UInteger) = Accession2Taxid.ReadFile(acc2taxid) _
                .CreateBuckets(Function(x) x.accession,
                               Function(x)
                                   Return x.taxid
                               End Function)

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
        ''' <param name="mapping">将序列的id编号映射为taxid的操作函数</param>
        ''' <param name="is_gi2taxid">是否是将gi编号映射为taxid编号？反之false的话是使用accession编号映射为taxid</param>
        ''' <returns></returns>
        Public Function Reference2Taxid(mapping As Mapping, is_gi2taxid As Boolean) As Mapping
            Dim parser As TextGrepMethod = GetParser(is_gi2taxid)

            Return Function(ref) As Integer
                       Dim xid$ = parser(ref)

                       If String.IsNullOrEmpty(xid) Then
                           Call ref.PrintException
                           Return -1
                       Else
                           Return mapping(xid)
                       End If
                   End Function
        End Function

        ''' <summary>
        ''' 根据参数返回gi编号或者accession编号的解析函数表达式
        ''' </summary>
        ''' <param name="is_gi2taxid"></param>
        ''' <returns></returns>
        Public Function GetParser(is_gi2taxid As Boolean) As TextGrepMethod
            If is_gi2taxid Then
                Return Function(ref$) As String
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
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
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
