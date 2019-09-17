#Region "Microsoft.VisualBasic::90e2b8898c074cd179c44b1f80d677cd, BASys\ecard\Ecard.vb"

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

    ' Class Ecard
    ' 
    '     Properties: Func, Keys, Name, Values
    ' 
    '     Function: GetNt, GetProt, Parser, ToString
    ' 
    ' Class EcardValue
    ' 
    '     Properties: [CDATE], [VARIANT], AUTHORS, BINOMIAL, CATEGORY
    '                 CLASSIFICATION, COMMENT, COMMON_NAME, EVIDENCE, GENUS
    '                 JOURNAL, MDATE, MEDLINE, NAME, NCBI_TAXID
    '                 PARAMETER, PUBMED, QUALITY, RP, Schema
    '                 SOURCE, SPECIES, SUBSPECIES, TITLE, TYPE
    '                 VALUE
    ' 
    '     Function: [New], __setValue, ToString
    ' 
    ' Structure Value
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class Ecard

    Public Property Name As NamedValue(Of String)
    Public Property Values As EcardValue()
    Public Property Keys As String()

    ''' <summary>
    ''' Gets the protein's aa sequence.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetProt() As FastaSeq
        For Each x In Values
            If x.NAME.TextEquals("translated sequence") Then
                Return New FastaSeq({Name.Name}, x.VALUE.scalar)
            End If
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' Gets the gene's nt sequence
    ''' </summary>
    ''' <returns></returns>
    Public Function GetNt() As FastaSeq
        For Each x In Values
            If x.NAME.TextEquals("Gene sequence") Then
                Return New FastaSeq({Name.Name}, x.VALUE.scalar)
            End If
        Next

        Return Nothing
    End Function

    Public ReadOnly Property Func As String
        Get
            For Each x In Values
                If x.NAME.TextEquals("Specific Function") Then
                    Return x.VALUE.scalar
                End If
            Next

            Return Nothing
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Name.GetJson
    End Function

    Public Shared Function Parser(path As String) As Ecard
        Dim tag As NamedValue(Of String) = Nothing
        Dim tokens = ecardParser.ParseFile(path, tag).ToArray
        Dim keys As String() = tokens.Select(
            Function(x) x.Keys).IteratesALL.Distinct.ToArray
        Dim schema = EcardValue.Schema
        Dim values As EcardValue() = tokens.Select(Function(x) EcardValue.[New](x, schema)).ToArray

        Return New Ecard With {
            .Name = tag,
            .Values = values,
            .Keys = keys
        }
    End Function
End Class

Public Class EcardValue

    Public Property NAME As String
    Public Property TYPE As String
    Public Property [CDATE] As String
    Public Property MDATE As String
    Public Property QUALITY As String
    Public Property EVIDENCE As String
    Public Property SOURCE As String
    Public Property PARAMETER As String
    Public Property VALUE As Value
    Public Property CATEGORY As String()
    Public Property AUTHORS As String
    Public Property TITLE As String
    Public Property JOURNAL As String
    Public Property MEDLINE As String
    Public Property PUBMED As String
    Public Property RP As String
    Public Property COMMENT As String
    Public Property CLASSIFICATION As String
    Public Property SPECIES As String
    Public Property GENUS As String
    Public Property COMMON_NAME As String
    Public Property BINOMIAL As String
    Public Property [VARIANT] As String
    Public Property SUBSPECIES As String
    Public Property NCBI_TAXID As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared ReadOnly Property Schema As Dictionary(Of String, PropertyInfo)
        Get
            Return New Dictionary(Of String, PropertyInfo)(__schema)
        End Get
    End Property

    Shared ReadOnly __schema As Dictionary(Of String, PropertyInfo) =
        GetType(EcardValue).Schema(PropertyAccess.ReadWrite) _
        .ToDictionary(
        Function(x) x.Key.Replace("_", " "),
        Function(x) x.Value)

    Private Shared Function __setValue(x As PropertyInfo) As Action(Of EcardValue, String)
        Return Sub(o, value) Call x.SetValue(o, value)
    End Function

    Friend Shared Function [New](token As Dictionary(Of String, String()), Schema As Dictionary(Of String, PropertyInfo)) As EcardValue
        Dim value As New EcardValue

        For Each x In token
            If x.Key = NameOf(EcardValue.VALUE) Then
                value.VALUE = New Value(x.Value, value.TYPE)
            Else
                Dim pp As PropertyInfo = Schema(x.Key)

                If pp.PropertyType.Equals(GetType(String)) Then
                    If x.Value.Length = 1 Then
                        Call pp.SetValue(value, x.Value(Scan0))
                    Else
                        Throw New Exception(x.Key)
                    End If
                Else
                    Call pp.SetValue(value, x.Value)
                End If
            End If
        Next

        Return value
    End Function
End Class

''' <summary>
''' <see cref="EcardValue"/> -> <see cref="EcardValue.VALUE"/>
''' </summary>
Public Structure Value

    ''' <summary>
    ''' simplescalar, SimpleText
    ''' </summary>
    Public scalar As String
    ''' <summary>
    ''' simplehash
    ''' </summary>
    Public hash As Dictionary(Of String, String)
    ''' <summary>
    ''' SimpleArray
    ''' </summary>
    Public array As String()

    Sub New(values As String(), type As String)
        If type.TextEquals("simplehash") Then
            hash = values.Select(
                Function(s) s.GetTagValue(vbTab)).ToDictionary(
                Function(x) x.Name,
                Function(x) x.Value)
        ElseIf type.TextEquals("SimpleArray") Then
            array = values
        Else
            scalar = String.Join(vbLf, values)
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure
