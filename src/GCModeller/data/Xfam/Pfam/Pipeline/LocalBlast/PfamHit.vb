Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Data.Xfam.Pfam.Pipeline.Database
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Pipeline.LocalBlast

    ''' <summary>
    ''' A hit model of pfam domain annotation on query protein sequence.
    ''' </summary>
    Public Class PfamHit : Inherits BestHit

        ''' <summary>
        ''' The start position of this domain object on target sequence
        ''' </summary>
        ''' <returns></returns>
        Public Property start As Integer
        ''' <summary>
        ''' The end position of this domain object on target sequence.
        ''' </summary>
        ''' <returns></returns>
        Public Property ends As Integer

        ''' <summary>
        ''' 经过<see cref="BlastOutputParser"/>的解析输出，不论是哪个方向的比对结果，输出的结果<see cref="HitName"/>就是pfam的定义数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Pfam As PfamEntryHeader
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return PfamEntryHeader.ParseHeaderTitle(HitName)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"Dim {QueryName} As [{HitName}] = {identities} (positive:{positive}, evalue={evalue})"
        End Function
    End Class
End Namespace