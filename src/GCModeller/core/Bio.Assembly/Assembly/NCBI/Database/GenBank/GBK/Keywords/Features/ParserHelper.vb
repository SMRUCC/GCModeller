Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    Module ParserHelper

        <Extension> Public Function FeaturesListParser(data As IEnumerable(Of String)) As FEATURES
            Dim index&
            Dim tmp As String()
            Dim list As New List(Of Feature)
            Dim strData$() = data.__formatString()
            Dim source As Feature

            Do While index < strData.Length - 1
                tmp = readBlock(index, strData)
                index += tmp.Length
                list.Add(item:=tmp)
            Loop

            Try
                source = LinqAPI.DefaultFirst(Of Feature) <=
                    From site As Feature
                    In list.AsParallel
                    Where String.Equals("source", site.KeyName)
                    Select site

            Catch ex As Exception
                ex = New Exception(strData.JoinBy(vbCrLf))
                Call App.LogException(ex)
                source = __nullFeature()
            End Try

            Dim features As New FEATURES With {
                ._innerList = list,
                .source = source
            }
            Return features
        End Function

        <Extension>
        Private Sub __append(str As String, ByRef new_strData As List(Of String), ByRef sBuilder As StringBuilder)
            Call new_strData.Add(sBuilder.ToString)
            Call sBuilder.Clear()
            Call sBuilder.Append(str.Trim)
        End Sub

        Const NotEnoughData$ = """{0}"" is not enough data! Check your genbank file's format!"

        ''' <summary>
        ''' 去除数据中的断行
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Private Function __formatString(s As IEnumerable(Of String)) As String()
            Dim sb As New StringBuilder(4096)
            Dim out As New List(Of String)

            For Each line As String In s
                If String.IsNullOrEmpty(line) Then
                    Continue For  '  忽略掉空白行
                End If
                If line.Length < 21 Then
                    Throw New Exception(String.Format(NotEnoughData, line))
                End If
                If line(21) = "/"c OrElse line(6) <> " "c Then ' this means read a new qualifier or a new feature
                    Call __append(line, out, sb)
                Else
                    Call sb.Append(" " & line.Trim)
                End If
            Next

            Call out.Add(sb.ToString) '添加最后一行的数据

            Return out.Skip(1).ToArray
        End Function

        ''' <summary>
        ''' 读取从某一个行号开始的文本块
        ''' </summary>
        ''' <param name="start">The start index of the reading.(读取的起始位置)</param>
        ''' <param name="data">The text data source to read.(所读取的数据源)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function readBlock(start&, data As String()) As String()
            Dim index As Long = start + 1

            Do While data(index).Chars(Scan0) = "/"
                index += 1
                If index = data.Length Then
                    Exit Do
                End If
            Loop

            index -= 1

            Dim bufs(index - start) As String
            Array.ConstrainedCopy(data, start, bufs, Scan0, bufs.Length)
            Return bufs
        End Function

        Private Function __nullFeature() As Feature
            Dim nullLoci As New RegionSegment With {
                .Left = 0,
                .Right = 0
            }
            Return New Feature With {
                .KeyName = "source",
                .Location = New Location With {
                    .Complement = False,
                    .Locations = {nullLoci}
                }
            }
        End Function
    End Module
End Namespace