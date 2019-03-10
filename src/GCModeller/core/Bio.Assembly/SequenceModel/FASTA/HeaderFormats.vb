Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.Uniprot

Namespace SequenceModel.FASTA

    ''' <summary>
    ''' Fasta序列在不同的数据库之中的标题的格式的帮助函数模块
    ''' </summary>
    Public Module HeaderFormats

        ''' <summary>
        ''' 在这里移除序列编号之中的版本号
        ''' </summary>
        ''' <param name="accession">``XXXXX.1``</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TrimAccessionVersion(accession As String) As String
            Return accession.Split("."c)(Scan0)
        End Function

#Region "UniProt"

        ''' <summary>
        ''' 格式参见<see cref="UniprotFasta"/>
        ''' </summary>
        ''' <param name="title"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetUniProtAccession(title As String) As String
            Return title.Split("|"c).ElementAtOrDefault(1)
        End Function

        Public Function TryGetUniProtAccession(title As String, ByRef accession As String) As Boolean
            If Not title.StringEmpty AndAlso title.IndexOf("|"c) > -1 Then
                accession = title.Split("|"c)(1)
                Return True
            Else
                Return False
            End If
        End Function
#End Region

    End Module
End Namespace