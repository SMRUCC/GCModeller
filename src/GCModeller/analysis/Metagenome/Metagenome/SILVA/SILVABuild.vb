Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module SILVABuild

    ''' <summary>
    ''' 从SILVA序列库之中筛选出细菌或者古生菌的16s序列
    ''' </summary>
    ''' <param name="silva"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function SILVABacteria(silva As IEnumerable(Of FastaToken)) As IEnumerable(Of FastaToken)
        Dim title$
        Dim header As NamedValue(Of String)

        For Each seq As FastaToken In silva
            title = seq.Title
            header = title.GetTagValue(" ", trim:=True)

            If InStr(header.Value, "Bacteria;", CompareMethod.Text) > 0 Then
                Yield seq
            ElseIf InStr(header.Value, "Archaea;", CompareMethod.Text) > 0 Then
                Yield seq
            Else
                ' 不是细菌或者古生菌的16S序列
                ' 跳过
            End If
        Next
    End Function
End Module
