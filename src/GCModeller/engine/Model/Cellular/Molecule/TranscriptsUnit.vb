
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

Namespace Cellular.Molecule

    ''' <summary>
    ''' 一般为一个操纵子对象
    ''' </summary>
    Public Class TranscriptsUnit

        ''' <summary>
        ''' 复制子编号
        ''' </summary>
        ''' <remarks>
        ''' 因为所有的复制子都是一个整体，所以<see cref="Genotype.centralDogmas"/>之中不区分复制子
        ''' 在这里添加一个复制子的ID标签方便后续数据分析的时候的分组操作
        ''' </remarks>
        Public replicon As String
        ''' <summary>
        ''' the unique reference name of current transcripts unit object
        ''' </summary>
        Public name As String

        ''' <summary>
        ''' element list of <see cref="CentralDogma"/>
        ''' </summary>
        Public elements As String()

    End Class
End Namespace