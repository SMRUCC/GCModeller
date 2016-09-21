Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 核酸序列的一致性的计算结果
''' </summary>
Public Class IdentityResult

    Public Property SeqId As String

    <Meta>
    Public Property Identities As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function SimpleTag(fa As FastaToken) As String
        Return fa.Title.Split.First
    End Function

    Public Shared Iterator Function SigmaMatrix(source As FastaFile, Optional round As Integer = -1, Optional simple As Boolean = True) As IEnumerable(Of IdentityResult)
        Dim nts As NucleicAcid() =
            source.ToArray(Function(x) New NucleicAcid(x), Parallel:=True)
        Dim getTag As Func(Of NucleicAcid, String)

        If simple Then
            getTag = Function(x) x.UserTag.Split.First
        Else
            getTag = Function(x) x.UserTag
        End If

        Dim getValue As Func(Of Double, Double)

        If round <= 0 Then
            getValue = Function(r) r
        Else
            getValue = Function(r) Math.Round(r, round)
        End If

        For Each nt As NucleicAcid In nts
            Dim result As List(Of NamedValue(Of Double)) =
                LinqAPI.MakeList(Of NamedValue(Of Double)) <=
                    From x As NucleicAcid
                    In nts.AsParallel
                    Where Not x Is nt
                    Let sigma As Double = DifferenceMeasurement.Sigma(nt, x)
                    Select New NamedValue(Of Double) With {
                        .Name = getTag(x),
                        .x = getValue(sigma * 1000)
                    }
            result += New NamedValue(Of Double) With {
                .Name = getTag(nt),
                .x = 0R
            }

            Call nt.UserTag.__DEBUG_ECHO

            Yield New IdentityResult With {
                .Identities = result.ToDictionary(Function(x) x.Name, Function(x) x.x),
                .SeqId = nt.UserTag
            }
        Next
    End Function
End Class