Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Driver

''' <summary>
''' X -> iBAQ表达量值
''' Y -> GO向量距离值
''' size -> logFC
''' color -> pvalue
''' </summary>
Public Module CloudPlot

    ''' <summary>
    ''' 绘制云图
    ''' </summary>
    ''' <param name="expression">原始数据</param>
    ''' <param name="annotations">蛋白注释结果</param>
    ''' <param name="DEPs">DEP计算结果文件</param>
    ''' <param name="schema$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Plot(expression As EntityObject(), annotations As UniprotAnnotations(), DEPs As EntityObject(), tag$, Optional schema$ = "Paired:c8") As GraphicsData
        Dim expressions =
            expression _
            .Select(Function(protein)
                        Return New NamedValue(Of Double) With {
                            .Name = protein.ID,
                            .Value = Val(protein(tag))
                        }
                    End Function) _
            .ToArray

    End Function
End Module
