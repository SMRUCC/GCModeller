Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.ComponentModel.Loci.Abstract
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Language

Namespace Topologically

    ''' <summary>
    ''' Mirror 或者 Palindrome
    ''' </summary>
    Public Class PalindromeLoci : Inherits Contig
        Implements I_PolymerSequenceModel
        Implements ILoci

        ''' <summary>
        ''' 特殊的位点序列
        ''' </summary>
        ''' <returns></returns>
        Public Property Loci As String Implements I_PolymerSequenceModel.SequenceData
        ''' <summary>
        ''' <see cref="NucleotideLocation.Left"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Start As Integer Implements ILoci.Left
        ''' <summary>
        ''' 回文序列，在生成Mirror镜像位点数据的时候是使用这个和<see cref="Loci"/>组合产生的
        ''' </summary>
        ''' <returns></returns>
        Public Property Palindrome As String
        ''' <summary>
        ''' <see cref="NucleotideLocation.Right"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property PalEnd As Integer
        ''' <summary>
        ''' 和<see cref="loci"/>相对应的反向序列
        ''' </summary>
        ''' <returns></returns>
        Public Property MirrorSite As String

        Public ReadOnly Property Mirror As Integer
            Get
                Return Start + Len(Loci)
            End Get
        End Property

        Public ReadOnly Property Length As Integer
            Get
                Return Len(Loci)
            End Get
        End Property

        Public Shared Function SelectSite(sites As IEnumerable(Of PalindromeLoci)) As PalindromeLoci
            Dim LQuery As PalindromeLoci =
                LinqAPI.DefaultFirst(Of PalindromeLoci) <=
                    From site As PalindromeLoci
                    In sites
                    Select site
                    Order By site.Length Descending
            Return LQuery
        End Function

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(Start, PalEnd)
        End Function

        Public Overloads Shared Function GetMirror(seq As String) As String
            Return New String(seq.Reverse.ToArray)
        End Function

        Public Shared Function GetPalindrome(seq As String) As String
            seq = GetMirror(seq)
            seq = NucleicAcid.Complement(seq)
            Return seq
        End Function
    End Class
End Namespace