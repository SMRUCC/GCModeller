Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty

''' <summary>
''' ``GC%``异常点分析
''' </summary>
Public Module GCOutlier

    Public Function Outlier(mla As IEnumerable(Of FastaToken),
                            Optional winsize As Integer = 250,
                            Optional steps As Integer = 50,
                            Optional method As NtProperty = Nothing) As NamedValue(Of NucleotideLocation)
        Dim data = GCData(mla, winsize, steps, method)

    End Function
End Module
