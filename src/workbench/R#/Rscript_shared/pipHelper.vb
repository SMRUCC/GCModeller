#Region "Microsoft.VisualBasic::97d91a19bfcd7de7ac1ddcb75afb9e22, R#\Rscript_shared\pipHelper.vb"

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

' Module pipHelper
' 
'     Function: getUniprotData
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

Module pipHelper

    Public Function getUniprotData(uniprot As Object, env As Environment) As [Variant](Of IEnumerable(Of entry), Message)
        If uniprot Is Nothing Then
            Return DirectCast(New entry() {}, IEnumerable(Of entry))
        End If

        If TypeOf uniprot Is entry() OrElse TypeOf uniprot Is IEnumerable(Of entry) Then
            Return New [Variant](Of IEnumerable(Of entry), Message)(DirectCast(uniprot, IEnumerable(Of entry)))
        ElseIf TypeOf uniprot Is pipeline AndAlso DirectCast(uniprot, pipeline).elementType Like GetType(entry) Then
            Return New [Variant](Of IEnumerable(Of entry), Message)(DirectCast(uniprot, pipeline).populates(Of entry)(env))
        ElseIf TypeOf uniprot Is vector AndAlso DirectCast(uniprot, vector).elementType Like GetType(entry) Then
            Return New [Variant](Of IEnumerable(Of entry), Message)(DirectCast(uniprot, vector).data.AsObjectEnumerator(Of entry))
        Else
            Return RInternal.debug.stop($"invalid data source input: {uniprot.GetType.FullName}!", env)
        End If
    End Function

    ''' <summary>
    ''' try to cast any data to a collection of fasta sequence data
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns>返回空值表示类型错误</returns>
    ''' <remarks>
    ''' supports data source type for cast to fasta sequence:
    ''' 
    ''' 1. <see cref="FastaSeq"/>, <see cref="FastaFile"/>
    ''' 2. a tuple list of <see cref="FastaSeq"/>
    ''' 3. cast a sequence string to <see cref="FastaSeq"/>
    ''' 4. <see cref="IFastaProvider"/> abstract data model, example as fastq sequence.
    ''' 5. cast dataframe(should contains data field: name/title, sequence) to <see cref="FastaSeq"/> collection
    ''' </remarks>
    Public Function GetFastaSeq(x As Object, env As Environment, Optional allowString As Boolean = True) As IEnumerable(Of FastaSeq)
        If x Is Nothing Then
            Return {}
        ElseIf TypeOf x Is vector Then
            x = DirectCast(x, vector).data
        ElseIf TypeOf x Is dataframe Then
            Return fastaFromDataframe(x)
        ElseIf TypeOf x Is vbObject Then
            x = DirectCast(x, vbObject).target
        ElseIf TypeOf x Is list Then
            x = DirectCast(x, list).data.ToArray
        End If

        Dim type As Type = x.GetType

        Select Case type
            Case GetType(FastaSeq)
                Return {DirectCast(x, FastaSeq)}
            Case GetType(FastaFile)
                Return DirectCast(x, FastaFile)
            Case GetType(FastaSeq())
                Return x
            Case Else
                Return GetFastaSeqs(x, type, env, allowString)
        End Select

        Return Nothing
    End Function

    Private Function GetFastaSeqs(x As Object, type As Type, env As Environment, allowString As Boolean) As IEnumerable(Of FastaSeq)
        If type.IsArray Then
            Dim el As Type = REnv.MeasureArrayElementType(x)

            If el Is GetType(FastaSeq) Then
                Return fastaFromCollection(x)
            ElseIf el Is GetType(String) AndAlso allowString Then
                Return fastaFromStrings(x)
            ElseIf el.ImplementInterface(Of IFastaProvider) Then
                Return From seq As Object
                       In DirectCast(x, Array)
                       Let fa As IFastaProvider = DirectCast(seq, IFastaProvider)
                       Select New FastaSeq(fa)
            End If
        ElseIf type Is GetType(pipeline) Then
            Dim pip As pipeline = DirectCast(x, pipeline)

            If pip.elementType Like GetType(FastaSeq) Then
                Return pip.populates(Of FastaSeq)(env)
            ElseIf pip.elementType Like GetType(String) AndAlso allowString Then
                Return fastaFromStrings(x)
            ElseIf pip.elementType.GetRawElementType.ImplementInterface(Of IFastaProvider) Then
                Return From fa As IFastaProvider
                       In pip.populates(Of IFastaProvider)(env)
                       Select New FastaSeq(fa)
            End If
        ElseIf type Is GetType(String) AndAlso allowString Then
            Return fastaFromStrings(x)
        End If

        Return Nothing
    End Function

    Private Iterator Function fastaFromDataframe(df As dataframe) As IEnumerable(Of FastaSeq)
        If Not (df.hasName("sequence") OrElse df.hasName("Sequence")) Then
            Call $"the input dataframe can not be cast to fasta sequence collection due to it contains no sequence data field: {df.colnames.GetJson}".warning
            Return
        End If

        Dim title As String() = CLRVector.asCharacter(If(df("name"), If(df("title"), df.rownames)))
        Dim seq As String() = CLRVector.asCharacter(If(df("sequence"), df("Sequence")))

        For i As Integer = 0 To seq.Length - 1
            Yield New FastaSeq With {
                .Headers = {title(i)},
                .SequenceData = seq(i)
            }
        Next
    End Function

    Private Iterator Function fastaFromCollection(a As Object) As IEnumerable(Of FastaSeq)
        Dim vec As Array

        If a.GetType.IsArray Then
            vec = DirectCast(a, Array)
        Else
            vec = CLRVector.asObject(a)
        End If

        For i As Integer = 0 To vec.Length - 1
            Yield DirectCast(vec.GetValue(i), FastaSeq)
        Next
    End Function

    Private Iterator Function fastaFromStrings(a As Object) As IEnumerable(Of FastaSeq)
        Dim strs As String() = CLRVector.asCharacter(a)
        Dim i As i32 = 1

        For Each str As String In strs
            Yield New FastaSeq With {
                .Headers = {$"seq_{++i}"},
                .SequenceData = str
            }
        Next
    End Function
End Module
