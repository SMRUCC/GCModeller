Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SSystem.Script
Imports csv = Microsoft.VisualBasic.Data.Framework.IO

Namespace IO

    ''' <summary>
    ''' The output writer of the fitted gene regulatory network.
    ''' (拟合结果的产物写入器)
    ''' </summary>
    ''' <remarks>
    ''' 产物清单：
    ''' 1. alpha_beta.csv     每一个基因的速率常数与拟合误差
    ''' 2. matrix_G.csv       合成项的动力学阶矩阵
    ''' 3. matrix_H.csv       降解项的动力学阶矩阵
    ''' 4. matrix_net.csv     净调控弹性矩阵 G - H
    ''' 5. edges.csv          有向调控边表（source, interaction, target, weight）
    ''' 6. network.sif        Cytoscape 可以直接导入的 SIF 文本
    ''' 7. model.plas         可以直接被 PLAS 运行的 S-system 脚本
    ''' 8. model.xml          编译之后的模型对象
    ''' 9. fit_report.txt     拟合质量报告
    ''' </remarks>
    Public Module NetworkWriter

        ''' <summary>
        ''' 把拟合结果的全部产物保存到指定的文件夹
        ''' </summary>
        ''' <param name="network"></param>
        ''' <param name="dir"></param>
        ''' <returns></returns>
        Public Function SaveAll(network As GRN.SSystemNetwork, dir As String) As Boolean
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            If Not Directory.Exists(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Dim ok As Boolean = True

            ok = writeAlphaBeta(network, Path.Combine(dir, "alpha_beta.csv")) AndAlso ok
            ok = writeMatrix(network.Parameters.G, network.Parameters.GeneNames, Path.Combine(dir, "matrix_G.csv")) AndAlso ok
            ok = writeMatrix(network.Parameters.H, network.Parameters.GeneNames, Path.Combine(dir, "matrix_H.csv")) AndAlso ok
            ok = writeMatrix(network.Parameters.NetElasticity(), network.Parameters.GeneNames, Path.Combine(dir, "matrix_net.csv")) AndAlso ok
            ok = writeEdges(network.Edges, Path.Combine(dir, "edges.csv")) AndAlso ok
            ok = network.ExportCoefficientMatrix(Path.Combine(dir, "coefficient_matrix.csv")) AndAlso ok
            ok = network.ExportFormulaTable(Path.Combine(dir, "elastic_formulas.csv")) AndAlso ok

            Dim sif As String = GRN.NetworkTopology.ToSIF(network.Edges)

            If Not String.IsNullOrEmpty(sif) Then
                Call File.WriteAllText(Path.Combine(dir, "network.sif"), sif, New UTF8Encoding(False))
            End If

            ' 模型脚本与编译之后的模型对象
            Dim model = network.CreateModel()

            ok = model.WriteScript(Path.Combine(dir, "model.plas")) AndAlso ok
            ok = model.Save(Path.Combine(dir, "model.xml")) AndAlso ok

            Call File.WriteAllText(Path.Combine(dir, "fit_report.txt"), network.Report(), New UTF8Encoding(False))

            Return ok
        End Function

        Private Function writeAlphaBeta(network As GRN.SSystemNetwork, path As String) As Boolean
            Dim params = network.Parameters
            Dim table As New csv.File
            Dim head As New csv.RowObject

            Call head.Add("gene")
            Call head.Add("original")
            Call head.Add("alpha")
            Call head.Add("beta")
            Call head.Add("slope_rmse")
            Call head.Add("converged")

            table += head

            For i As Integer = 0 To params.nGenes - 1
                Dim row As New csv.RowObject

                Call row.Add(params.GeneNames(i))
                Call row.Add(params.OriginalNames(i))
                Call row.Add(format(params.alpha(i)))
                Call row.Add(format(params.beta(i)))
                Call row.Add(format(params.SlopeRMSE(i)))
                Call row.Add(params.Converged(i).ToString)

                table += row
            Next

            Return table.Save(path, Encodings.UTF8)
        End Function

        Private Function writeMatrix(matrix As Double()(), names As String(), path As String) As Boolean
            Dim table As New csv.File
            Dim head As New csv.RowObject

            Call head.Add("gene")

            For Each name As String In names
                Call head.Add(name)
            Next

            table += head

            For i As Integer = 0 To matrix.Length - 1
                Dim row As New csv.RowObject

                Call row.Add(names(i))

                For j As Integer = 0 To matrix(i).Length - 1
                    Call row.Add(format(matrix(i)(j)))
                Next

                table += row
            Next

            Return table.Save(path, Encodings.UTF8)
        End Function

        Private Function writeEdges(edges As GRN.RegulatoryEdge(), path As String) As Boolean
            Dim table As New csv.File
            Dim head As New csv.RowObject

            Call head.Add("source")
            Call head.Add("interaction")
            Call head.Add("target")
            Call head.Add("weight")
            Call head.Add("g")
            Call head.Add("h")

            table += head

            For Each edge As GRN.RegulatoryEdge In edges.SafeQuery
                Dim row As New csv.RowObject

                Call row.Add(edge.source)
                Call row.Add(edge.interaction)
                Call row.Add(edge.target)
                Call row.Add(format(edge.weight))
                Call row.Add(format(edge.g))
                Call row.Add(format(edge.h))

                table += row
            Next

            Return table.Save(path, Encodings.UTF8)
        End Function

        Private Function format(x As Double) As String
            Return x.ToString("G6", CultureInfo.InvariantCulture)
        End Function
    End Module
End Namespace
