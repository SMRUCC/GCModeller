Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' The pairwise delta-difference calculation result of the nucleotide sequences.
''' </summary>
Public Class IdentityResult : Implements INamedValue

    Public Property SeqId As String Implements INamedValue.Key
    Public Property Identities As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    ''' <summary>
    ''' 直接使用整条序列来进行计算
    ''' 
    ''' (compatible rename of the legacy ``SigmaMatrix``: 
    ''' ``Sigma`` follows the paper's ``delta*`` symbol)
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="round"></param>
    ''' <param name="simple"></param>
    ''' <returns></returns>
    Public Shared Iterator Function DeltaStarMatrix(source As FastaFile, Optional round% = -1, Optional simple As Boolean = True) As IEnumerable(Of IdentityResult)
        ' pre-build all of the signature caches once: every pairwise comparison 
        ' then only costs a 16-dim vector distance (the legacy implementation 
        ' re-counted the sequences for every pair)
        Dim nts As NucleicAcid() = source.Select(Function(seq) New NucleicAcid(seq)).ToArray
        Dim getTag As Func(Of NucleicAcid, String)

        If simple Then
            getTag = Function(x) x.tag.Split.First
        Else
            getTag = Function(x) x.tag
        End If

        Dim getValue As Func(Of Double, Double)

        If round <= 0 Then
            getValue = Function(r) r
        Else
            getValue = Function(r) Math.Round(r, round)
        End If

        For Each nt As NucleicAcid In nts
            Dim result = LinqAPI.MakeList(Of NamedValue(Of Double)) <=
                                                                      _
                From x As NucleicAcid
                In nts.AsParallel
                Where Not x Is nt  ' 由于是自己的全长序列与自己的全长序列进行比较，二者一致，故而距离为0，这里为了节省时间就不做计算了
                Let deltaStar As Double = DeltaStarDistance.DeltaStar(nt, x)
                Select New NamedValue(Of Double) With {
                    .Name = getTag(x),
                    .Value = getValue(deltaStar * 1000)
                }

            ' 自己与自己相互进行比较肯定是0距离的
            ' 直接添加
            result += New NamedValue(Of Double) With {
                .Name = getTag(nt),
                .Value = 0R
            }

            Call nt.tag.debug

            Yield New IdentityResult With {
                .Identities = result.ToDictionary(Function(x) x.Name, Function(x) x.Value),
                .SeqId = nt.tag
            }
        Next
    End Function

    ''' <summary>
    ''' 使用默认的``dnaA - gyrB``为外标尺进行计算
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="round%"></param>
    ''' <param name="simple"></param>
    ''' <returns></returns>
    Public Shared Iterator Function DeltaStarMatrix(source As IEnumerable(Of GBFF.File), Optional round% = -1, Optional simple As Boolean = True) As IEnumerable(Of IdentityResult)
        Dim data As GBFF.File() = source.ToArray
        ' pre-build all of the genome signature caches once
        Dim nts As NucleicAcid() = data.Select(Function(x) New NucleicAcid(x.Origin.ToFasta)).ToArray
        Dim getTag As Func(Of NucleicAcid, String)

        If simple Then
            getTag = Function(x) x.tag.Split.First
        Else
            getTag = Function(x) x.tag
        End If

        Dim getValue As Func(Of Double, Double)

        If round <= 0 Then
            getValue = Function(r) r
        Else
            getValue = Function(r) Math.Round(r, round)
        End If

        For i As Integer = 0 To data.Length - 1
            Dim genome As GBFF.File = data(i)
            Dim rule As New NucleicAcid(genome.dnaA_gyrB)
            Dim result = LinqAPI.MakeList(Of NamedValue(Of Double)) <=
                                                                      _
              From x As NucleicAcid
              In nts.AsParallel
              Let deltaStar As Double = DeltaStarDistance.DeltaStar(rule, x)
              Select New NamedValue(Of Double) With {
                  .Name = getTag(x),
                  .Value = getValue(deltaStar * 1000)
              }

            Call rule.tag.debug

            Yield New IdentityResult With {
                .Identities = result _
                    .ToDictionary(Function(x) x.Name,
                                  Function(x) x.Value),
                .SeqId = rule.tag
            }
        Next
    End Function
End Class
