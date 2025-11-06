#Region "Microsoft.VisualBasic::e8c65bd096f294a860e78a85ecffa073, core\Bio.Assembly\SequenceModel\FASTA\Reflection\FastaTools.vb"

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

    '   Total Lines: 275
    '    Code Lines: 187 (68.00%)
    ' Comment Lines: 53 (19.27%)
    '    - Xml Docs: 90.57%
    ' 
    '   Blank Lines: 35 (12.73%)
    '     File Size: 11.96 KB


    '     Module FastaExportMethods
    ' 
    '         Function: __seqCorrupted, Complement, (+3 Overloads) Export, FastaCorrupted, FastaTrimCorrupt
    '                   (+3 Overloads) Merge, Reverse
    '         Class SchemaCache
    ' 
    '             Properties: attributes, TitleFormat
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: GetTitleFormat
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace SequenceModel.FASTA.Reflection

    ''' <summary>
    ''' 對象模塊將數據庫中的一條記錄轉換為一條FASTA序列對象
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Package("Fasta.Tools")>
    Public Module FastaExportMethods

        ''' <summary>
        ''' 将某一个FASTA序列集合中的序列进行互补操作，对于蛋白质序列，则返回空值
        ''' </summary>
        ''' <param name="nt">这里应该输入的是核酸序列,因为只有核酸序列才可以进行互补,蛋白序列则不行</param>
        ''' <returns>返回互补链序列集合</returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Complement"), Extension>
        Public Function Complement(nt As FastaFile) As FastaFile
            Dim LQuery = From fa As SeqValue(Of FastaSeq)
                         In nt.SeqIterator.AsParallel
                         Let cp = FastaSeq.Complement(fa)
                         Where Not cp Is Nothing
                         Order By fa.i Ascending
                         Select cp

            Return New FastaFile(LQuery)
        End Function

        ''' <summary>
        ''' 将序列首尾反转
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Reverse")>
        <Extension>
        Public Function Reverse(fasta As FastaFile) As FastaFile
            Dim LQuery = From fa As SeqValue(Of FastaSeq)
                         In fasta.SeqIterator.AsParallel
                         Let rev As FastaSeq = fa.value.Reverse
                         Order By fa.i Ascending
                         Select rev

            Return New FastaFile(LQuery)
        End Function

        ''' <summary>
        ''' Merge the fasta sequence file from a file list.
        ''' </summary>
        ''' <param name="list"></param>
        ''' <param name="Trim"></param>
        ''' <param name="rawTitle"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Merge(list As IEnumerable(Of String), Trim As Boolean, rawTitle As Boolean) As FastaFile
            Dim mergeFa As FastaSeq() =
                LinqAPI.Exec(Of FastaSeq) <= From file As String
                                               In list.AsParallel
                                             Select FastaFile.Read(file)

            If Trim Then
                Dim setValue =
                    New SetValue(Of FastaSeq)().GetSet(NameOf(FastaSeq.Headers))

                mergeFa = LinqAPI.Exec(Of FastaSeq) <=
                    From fa As FastaSeq
                    In mergeFa.AsParallel
                    Let attrs As String() = New String() {fa.Headers.First.Split.First}
                    Select setValue(fa, attrs)

                mergeFa = LinqAPI.Exec(Of FastaSeq) <=
                    From fa As FastaSeq
                    In mergeFa.AsParallel
                    Select fa.FastaTrimCorrupt
            Else
                If Not rawTitle Then
                    For Each fa As FastaSeq In mergeFa
                        fa.Headers = {fa.Headers.First.Split.First}
                    Next
                End If
            End If

            Dim source As IEnumerable(Of FastaSeq) =
                From fa As FastaSeq
                In mergeFa.AsParallel
                Where Not String.IsNullOrEmpty(fa.SequenceData)
                Select fa

            Call Console.Write(".")

            Return New FastaFile(mergeFa)
        End Function

        ''' <summary>
        ''' Only merge fasta files in the top level directory.(这个函数不会返回空值)
        ''' </summary>
        ''' <param name="inDIR"></param>
        ''' <param name="trim"></param>
        ''' <returns></returns>
        Public Function Merge(inDIR As String, trim As Boolean, rawTitle As Boolean) As FastaFile
            Dim files As IEnumerable(Of String) = ls - l - wildcards("*.fa", "*.fsa", "*.fas", "*.fasta") <= inDIR
            Return files.Merge(trim, rawTitle)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inDIR"></param>
        ''' <param name="ext"></param>
        ''' <param name="trim"></param>
        ''' <param name="rawTitle">是否保留有原来的标题</param>
        ''' <returns></returns>
        Public Function Merge(inDIR As String, ext As String, trim As Boolean, rawTitle As Boolean) As FastaFile
            Dim files As IEnumerable(Of String) = ls - l - wildcards(ext) <= inDIR
            Return files.Merge(trim, rawTitle)
        End Function

        ''' <summary>
        ''' invalid chars for check in sequence
        ''' </summary>
        Const HTML_CHARS As String = "</>!\[].+:()0123456789"

        ''' <summary>
        ''' 有些从KEGG上面下载下来的数据会因为解析的问题出现错误，在这里判断是否有这种错误
        ''' </summary>
        ''' <param name="fa"></param>
        ''' <returns></returns>
        <ExportAPI("Fasta.Corrupted?")>
        <Extension> Public Function FastaCorrupted(fa As FastaSeq) As Boolean
            Return __seqCorrupted(fa.SequenceData)
        End Function

        Private Function __seqCorrupted(seq As String) As Boolean
            Dim LQuery As Integer = (From x As Char
                                     In seq
                                     Where HTML_CHARS.IndexOf(x) > -1
                                     Select 1000).FirstOrDefault
            Return LQuery > 0
        End Function

        Const LOCUS_ID As String = "[a-z]+_?\d+"
        Const KEGG_LOCUS As String = "[a-z]{3,5}[:]" & LOCUS_ID

        ''' <summary>
        ''' 这个函数是为了修正早期的KEGG序列数据下载工具的一些html解析错误，第一个字符肯定是M
        ''' </summary>
        ''' <param name="fa"></param>
        ''' <returns></returns>
        <ExportAPI("Fasta.Removes.Corruption")>
        <Extension>
        Public Function FastaTrimCorrupt(fa As FastaSeq) As FastaSeq
            Dim seq As String = fa.SequenceData
            Dim isCorrupted As Boolean
            Dim n As Integer

            Do While __seqCorrupted(seq)
                Dim i As Integer = InStr(seq, "M", CompareMethod.Text)
REDO:           seq = Mid(seq, i)
                isCorrupted = True

                If i = 0 OrElse n > fa.Length Then
                    Call $"ERROR__{fa.SequenceData}  =>  {seq}".debug
                    ' seq = ""
                    Exit Do
                Else
                    If i = 1 Then
                        i = 2
                        GoTo REDO
                    End If

                    n += 1
                End If
            Loop

            Dim locus As String = Regex.Match(fa.SequenceData, KEGG_LOCUS, RegexOptions.IgnoreCase).Value
            If String.IsNullOrEmpty(locus) Then
                locus = Regex.Match(fa.SequenceData, LOCUS_ID, RegexOptions.IgnoreCase).Value
            End If
            If String.IsNullOrEmpty(locus) Then
                locus = fa.Title
            End If

            If isCorrupted Then
                Call $"{fa.ToString} was corrupted, automatically corrected as {locus}!".debug
            End If

            Return New FastaSeq With {
                .Headers = {locus},
                .SequenceData = seq
            }
        End Function


        Public Function Export(Of T As ISequenceProvider)(source As IEnumerable(Of T)) As FastaFile
            Dim SchemaCache As SchemaCache = New SchemaCache(GetType(T))
            Dim LQuery = (From objItem As T
                          In source
                          Let fsa As FastaSeq = Export(objItem, SchemaCache)
                          Where Not fsa Is Nothing
                          Select fsa).ToArray
            Return LQuery
        End Function

        Public Function Export(Of TFsaObject As ISequenceProvider)(fa As TFsaObject) As FastaSeq
            If String.IsNullOrEmpty(fa.GetSequenceData) Then
                Return Nothing
            End If

            Dim SchemaCache As SchemaCache = New SchemaCache(GetType(TFsaObject))
            Dim fsa As FastaSeq = Export(fa, SchemaCache)
            Return fsa
        End Function

        Private Function Export(objItem As ISequenceProvider, SchemaCache As SchemaCache) As FastaSeq
            If String.IsNullOrEmpty(SchemaCache.TitleFormat) Then
                Dim stringItems = (From pairItem As KeyValuePair(Of FastaAttributeItem, System.Reflection.PropertyInfo)
                                   In SchemaCache.attributes
                                   Let value As String = pairItem.Value.GetValue(objItem).ToString
                                   Select If(String.IsNullOrEmpty(pairItem.Key.Precursor), New String() {value}, New String() {pairItem.Key.Precursor, value})).ToArray
                Dim Fsa As FastaSeq = New FastaSeq With {
                    .SequenceData = objItem.GetSequenceData,
                    .Headers = stringItems.ToVector
                }
                Return Fsa
            Else
                Dim stringItems = (From pairItem As KeyValuePair(Of FastaAttributeItem, System.Reflection.PropertyInfo)
                                   In SchemaCache.attributes
                                   Let value As String = pairItem.Value.GetValue(objItem).ToString
                                   Select If(String.IsNullOrEmpty(pairItem.Key.Format), value, String.Format(pairItem.Key.Format, value))).ToArray
                Dim Title As String = String.Format(SchemaCache.TitleFormat, stringItems)
                Dim Fsa As FastaSeq = New FastaSeq With {
                    .SequenceData = objItem.GetSequenceData,
                    .Headers = Title.Split(CChar("|"))
                }
                Return Fsa
            End If
        End Function

        Friend Class SchemaCache

            Public Property TitleFormat As String
            Public Property attributes As KeyValuePair(Of FastaAttributeItem, System.Reflection.PropertyInfo)()

            Sub New(TypeInfo As Type)
                TitleFormat = GetTitleFormat(TypeInfo)
                attributes = (From propertyInfo As System.Reflection.PropertyInfo
                              In TypeInfo.GetProperties()
                              Let custAttr As Object() = propertyInfo.GetCustomAttributes(FastaAttributeItem.FsaAttributeItem, True)
                              Where Not custAttr.IsNullOrEmpty
                              Let pairItem = New KeyValuePair(Of FastaAttributeItem, System.Reflection.PropertyInfo)(DirectCast(custAttr.First(), FastaAttributeItem), propertyInfo)
                              Select pairItem
                              Order By pairItem.Key.Index Ascending).ToArray
            End Sub

            Public Shared Function GetTitleFormat(TypeInfo As System.Type) As String
                Dim attrs As Object() = TypeInfo.GetCustomAttributes(FastaObject.FsaTitle, True)
                If attrs.IsNullOrEmpty Then
                    Return ""
                Else
                    Dim TitleFormat As String = DirectCast(attrs.First, FastaObject).Format
                    Return TitleFormat
                End If
            End Function
        End Class
    End Module
End Namespace
