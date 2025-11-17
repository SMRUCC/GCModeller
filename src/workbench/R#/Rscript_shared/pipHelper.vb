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

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
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
    ''' 返回空值表示类型错误
    ''' </summary>
    ''' <param name="a"></param>
    ''' <returns></returns>
    Public Function GetFastaSeq(a As Object, env As Environment) As IEnumerable(Of FastaSeq)
        If a Is Nothing Then
            Return {}
        ElseIf TypeOf a Is vector Then
            a = DirectCast(a, vector).data
        ElseIf TypeOf a Is dataframe Then
            Return fastaFromDataframe(a)
        ElseIf TypeOf a Is vbObject Then
            a = DirectCast(a, vbObject).target
        End If

        Dim type As Type = a.GetType

        Select Case type
            Case GetType(FastaSeq)
                Return {DirectCast(a, FastaSeq)}
            Case GetType(FastaFile)
                Return DirectCast(a, FastaFile)
            Case GetType(FastaSeq())
                Return a
            Case GetType(list)
                Return fastaFromCollection(DirectCast(a, list).data)
            Case Else
                If type.IsArray Then
                    If REnv.MeasureArrayElementType(a) Is GetType(FastaSeq) Then
                        Return fastaFromCollection(a)
                    ElseIf REnv.MeasureArrayElementType(a) Is GetType(String) Then
                        Return fastaFromStrings(a)
                    End If
                ElseIf type Is GetType(pipeline) Then
                    Dim pip As pipeline = DirectCast(a, pipeline)

                    If pip.elementType Like GetType(FastaSeq) Then
                        Return pip.populates(Of FastaSeq)(env)
                    ElseIf pip.elementType Like GetType(String) Then
                        Return fastaFromStrings(a)
                    Else
                        Return Nothing
                    End If
                ElseIf type Is GetType(String) Then
                    Return fastaFromStrings(a)
                Else
                    Return Nothing
                End If
        End Select

        Return Nothing
    End Function

    Private Iterator Function fastaFromDataframe(df As dataframe) As IEnumerable(Of FastaSeq)
        If Not (df.hasName("name") OrElse df.hasName("title")) Then
            Return
        ElseIf Not df.hasName("sequence") Then
            Return
        End If

        Dim title As String() = CLRVector.asCharacter(If(df("name"), df("title")))
        Dim seq As String() = CLRVector.asCharacter(df("sequence"))

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
            vec = REnv.asVector(Of Object)(a)
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
