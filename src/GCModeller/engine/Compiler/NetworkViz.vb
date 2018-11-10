Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

''' <summary>
''' Export network visualize model for cytoscape software.
''' </summary>
Public Module NetworkViz

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cell"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 因为<see cref="CellularModule"/>仅包含有和计算相关的数据，会缺失很多细节信息
    ''' 所以直接使用<see cref="CellularModule"/>作为网络模型的数据源不太合适
    ''' 在这里使用Xml模型来作为统一的虚拟细胞的网络数据模型的来源，而table模型则可以在
    ''' 转换为Xml格式对象之后再调用这个方法产生网络可视化的模型
    ''' </remarks>
    <Extension>
    Public Function CreateGraph(cell As VirtualCell) As NetworkTables
        Dim geneNodes = cell.Genome.genes.Select(Function(gene) New Node With {.ID = gene.locus_tag, .NodeType = "gene"}).AsList

    End Function
End Module
