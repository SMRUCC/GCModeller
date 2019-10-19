Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

<HideModuleName>
Public Module Extensions

    ''' <summary>
    ''' 将构建出来的图对象转换为表格数据模型，以进行文件保存操作
    ''' </summary>
    ''' <param name="g"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function DAGasTabular(g As NetworkGraph) As NetworkTables
        Return g.Tabular({"relationship", "definition", "evidence", "is_obsolete"})
    End Function
End Module
