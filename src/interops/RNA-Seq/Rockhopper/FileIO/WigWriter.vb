' /********************************************************************************/
'
'  Rockhopper —— 基因组浏览器 WIG 文件输出
'
'  复刻自原始 Rockhopper CLI_API.java 中的
'    outputUTRsForBrowser / outputRNAsForBrowser / outputDifferentiallyExpressedGenesForBrowser。
'  仅将 PrintWriter 替换为 StreamWriter；WIG 的 fixedStep 轨道格式保持不变。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.IO

Namespace FileIO

    ''' <summary>
    ''' 生成可供基因组浏览器（IGV 等）加载的 WIG 轨道文件。
    ''' </summary>
    Public Module WigWriter

        ''' <summary>
        ''' 输出预测 UTR（5'/3' UTR）的 WIG 轨道：正链记 +1，负链记 -1。
        ''' </summary>
        Public Sub WriteUTRs(outputFile As String, genomeName As String, genes As List(Of Core.Gene), size As Integer)
            Dim geneCoordinates As Integer() = New Integer(size - 1) {}
            For i As Integer = 0 To genes.Count - 1
                Dim g As Core.Gene = genes(i)

                If g.ORF AndAlso g.StartT > 0 AndAlso g.Strand = "+"c Then
                    For j As Integer = g.StartT To g.Start - 1
                        If j >= 0 AndAlso j < geneCoordinates.Length Then geneCoordinates(j) = 1
                    Next
                End If
                If g.ORF AndAlso g.StartT > 0 AndAlso g.Strand = "-"c Then
                    For j As Integer = g.Start + 1 To g.StartT
                        If j >= 0 AndAlso j < geneCoordinates.Length Then geneCoordinates(j) = -1
                    Next
                End If
                If g.ORF AndAlso g.StopT > 0 AndAlso g.Strand = "+"c Then
                    For j As Integer = g.[Stop] + 1 To g.StopT
                        If j >= 0 AndAlso j < geneCoordinates.Length Then geneCoordinates(j) = 1
                    Next
                End If
                If g.ORF AndAlso g.StopT > 0 AndAlso g.Strand = "-"c Then
                    For j As Integer = g.StopT To g.[Stop] - 1
                        If j >= 0 AndAlso j < geneCoordinates.Length Then geneCoordinates(j) = -1
                    Next
                End If
            Next

            Call writeTrack(outputFile, "UTRs", genomeName, geneCoordinates, "color=255,0,255 altColor=255,0,255 graphType=bar viewLimits=-1:1")
        End Sub

        ''' <summary>
        ''' 输出预测新 RNA（ncRNA / sRNA）的 WIG 轨道。
        ''' </summary>
        Public Sub WriteRNAs(outputFile As String, genomeName As String, genes As List(Of Core.Gene), size As Integer)
            Dim geneCoordinates As Integer() = New Integer(size - 1) {}
            For i As Integer = 0 To genes.Count - 1
                Dim g As Core.Gene = genes(i)

                If Not g.ORF AndAlso g.Name.Equals("predicted RNA") Then
                    Dim value As Integer = 1
                    If g.Strand = "-"c Then value = -1
                    For j As Integer = g.First To g.Last
                        If j >= 0 AndAlso j < geneCoordinates.Length Then geneCoordinates(j) = value
                    Next
                End If
            Next

            Call writeTrack(outputFile, "Novel RNAs", genomeName, geneCoordinates, "color=0,255,0 altColor=0,255,0 graphType=bar viewLimits=-1:1")
        End Sub

        ''' <summary>
        ''' 输出差异表达基因的 WIG 轨道（数值为 -log10(q-value)，负链取负）。
        ''' </summary>
        Public Sub WriteDifferentiallyExpressedGenes(outputFile As String, genomeName As String, genes As List(Of Core.Gene), size As Integer)
            Dim geneCoordinates As Integer() = New Integer(size - 1) {}
            For i As Integer = 0 To genes.Count - 1
                Dim g As Core.Gene = genes(i)
                Dim qValue As Double = g.MinQvalue
                Dim value As Integer = 0
                If qValue = 0.0 Then
                    ' Special case. We cannot take log of zero.
                    value = 300
                Else
                    value = CInt(System.Math.Truncate(-System.Math.Log10(qValue)))
                End If
                If g.Strand = "-"c Then value = -value
                For j As Integer = g.First To g.Last
                    If j >= 0 AndAlso j < geneCoordinates.Length Then geneCoordinates(j) = value
                Next
            Next

            Call writeTrack(outputFile, "Differentially expressed genes", genomeName, geneCoordinates, "color=0,255,0 altColor=0,255,0 graphType=bar viewLimits=-10:10")
        End Sub

        ''' <summary>
        ''' 写出 fixedStep WIG 轨道。
        ''' </summary>
        Private Sub writeTrack(outputFile As String, trackName As String, genomeName As String, coordinates As Integer(), options As String)
            Try
                Using writer As New StreamWriter(outputFile)
                    writer.WriteLine($"track name=""{trackName}"" {options}")
                    writer.WriteLine($"fixedStep chrom={genomeName} start=1 step=1")
                    For j As Integer = 1 To coordinates.Length - 1
                        writer.WriteLine(coordinates(j))
                    Next
                End Using
            Catch e As FileNotFoundException
                Call Core.Logging.Output($"{vbLf}Error - could not open file {outputFile}{vbLf}{vbLf}")
            End Try
        End Sub

    End Module

End Namespace
