Imports System.Globalization
Imports System.IO

Namespace Kmers.Kraken2

    Module KrakenParser

        ''' <summary>
        ''' 解析 Kraken2 的 --output 文件。
        ''' </summary>
        ''' <param name="filePath">--output 文件的完整路径。</param>
        ''' <returns>一个包含所有解析后记录的列表。</returns>
        Public Iterator Function ParseOutputFile(filePath As String) As IEnumerable(Of KrakenOutputRecord)
            Dim lines() As String = filePath.ReadAllLines

            For Each line As String In lines
                ' 跳过空行
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If

                Dim parts() As String = line.Split({vbTab}, StringSplitOptions.None)

                If parts.Length < 5 Then
                    Call $"invalid format of line data: {line}".warning
                    Continue For ' 确保有足够的列
                End If

                Dim record As New KrakenOutputRecord() With {
                     .StatusCode = parts(0).Trim(),
                     .ReadName = parts(1).Trim()
                }

                Integer.TryParse(parts(2).Trim(), record.TaxID)
                Integer.TryParse(parts(3).Trim(), record.ReadLength)

                ' 解析 LCA 映射详情 (第5列)
                Dim mappingParts() As String = parts(4).Trim().Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
                For Each mapping As String In mappingParts
                    Dim kv() As String = mapping.Split(":"c)
                    If kv.Length = 2 Then
                        Dim taxId As Long = 0
                        Dim count As Integer = 0
                        If Long.TryParse(kv(0), taxId) AndAlso Integer.TryParse(kv(1), count) Then
                            record.LcaMappings(taxId.ToString) = count
                        End If
                    End If
                Next

                Yield record
            Next
        End Function

        ''' <summary>
        ''' 解析 Kraken2 的 --report 文件。
        ''' </summary>
        ''' <param name="filePath">--report 文件的完整路径。</param>
        ''' <returns>一个包含所有解析后记录的列表。</returns>
        Public Iterator Function ParseReportFile(filePath As String) As IEnumerable(Of KrakenReportRecord)
            Dim lines() As String = File.ReadAllLines(filePath)

            For Each line As String In lines
                ' 跳过空行或注释行（如果有的话）
                If String.IsNullOrWhiteSpace(line) OrElse line.StartsWith("#") Then
                    Continue For
                End If

                Dim parts() As String = line.Split({vbTab}, StringSplitOptions.None)
                If parts.Length < 6 Then Continue For ' 确保有足够的列

                Dim record As New KrakenReportRecord()
                ' 使用 CultureInfo.InvariantCulture 来确保小数点总是被解析为 "."
                Double.TryParse(parts(0).Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, record.Percentage)
                Long.TryParse(parts(1).Trim(), record.ReadsAtRank)
                Long.TryParse(parts(2).Trim(), record.ReadsDirect)
                record.RankCode = parts(3).Trim()
                Call UInteger.TryParse(parts(4).Trim(), record.TaxID)
                record.ScientificName = parts(5).Trim()

                Yield record
            Next
        End Function
    End Module
End Namespace