Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class Ecard : Inherits ClassObject

    Public Property Name As NamedValue(Of String)
    Public Property Values As EcardValue()
    Public Property Keys As String()

    ''' <summary>
    ''' Gets the protein's aa sequence.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetProt() As FastaToken
        For Each x In Values
            If x.NAME.TextEquals("translated sequence") Then
                Return New FastaToken({Name.Name}, x.VALUE.scalar)
            End If
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' Gets the gene's nt sequence
    ''' </summary>
    ''' <returns></returns>
    Public Function GetNt() As FastaToken
        For Each x In Values
            If x.NAME.TextEquals("Gene sequence") Then
                Return New FastaToken({Name.Name}, x.VALUE.scalar)
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
            Function(x) x.Keys).MatrixAsIterator.Distinct.ToArray
        Dim schema = EcardValue.Schema
        Dim values As EcardValue() = tokens.ToArray(
            Function(x) EcardValue.[New](x, schema))

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
                Function(x) x.x)
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