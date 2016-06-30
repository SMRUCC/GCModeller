Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.ComparativeGenomics.ModelAPI
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.NCBI.Extensions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Serialization

Namespace NCBIBlastResult

    ''' <summary>
    ''' Blast结果之中的hit对象的颜色映射
    ''' </summary>
    Public Module ColorSchema

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scores">需要从这里得到分数</param>
        ''' <returns></returns>
        Public Function IdentitiesBrush(scores As Func(Of Analysis.Hit, Double)) As ICOGsBrush
            Return AddressOf New __brushHelper With {
                .scores = scores,
                .colors = IdentitiesColors()
            }.GetBrush   ' 获取得到映射的颜色刷子的函数指针
        End Function

        Private Structure __brushHelper

            Public scores As Func(Of Analysis.Hit, Double)
            Public colors As RangeList(Of Double, TagValue(Of Color))

            Public Function GetBrush(gene As GeneBrief) As Brush
                Dim hit As New Analysis.Hit With {.HitName = gene.Synonym}
                Dim score As Double = scores(hit)
                Dim color As Color = colors.GetColor(score)
                Return New SolidBrush(color)
            End Function
        End Structure

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p">p的值在0-1之间</param>
        ''' <param name="Schema"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function GetColor(schema As RangeList(Of Double, TagValue(Of Color)), p As Double) As Color
            Return schema.GetBlastnIdentitiesColor(p * 100)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p">p的值在0-100之间</param>
        ''' <param name="Schema"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function GetBlastnIdentitiesColor(schema As RangeList(Of Double, TagValue(Of Color)), p As Double) As Color
            Dim success As Boolean = False
            Dim cl As TagValue(Of Color) = schema.SelectValue(p, [throw]:=False, success:=success)

#If DEBUG Then
            ' Call $"{p} --> {cl.GetJson}".__DEBUG_ECHO
#End If

            If Not success Then
                If p <= 0 Then
                    Return Color.Black
                Else
                    Return Color.Gray
                End If
            Else
                Return cl.Value
            End If
        End Function

        Public Function IdentitiesColors() As RangeList(Of Double, TagValue(Of Color))
            Return New RangeList(Of Double, TagValue(Of Color)) From {
                New RangeTagValue(Of Double, TagValue(Of Color))(0, 30, New TagValue(Of Color)("<= 30%", Color.Black)),
                New RangeTagValue(Of Double, TagValue(Of Color))(30, 55, New TagValue(Of Color)("30% - 55%", Color.Blue)),
                New RangeTagValue(Of Double, TagValue(Of Color))(55, 70, New TagValue(Of Color)("55% - 70%", Color.Green)),
                New RangeTagValue(Of Double, TagValue(Of Color))(70, 90, New TagValue(Of Color)("70% - 90%", Color.Purple)),
                New RangeTagValue(Of Double, TagValue(Of Color))(90, 100000, New TagValue(Of Color)(">= 90%", Color.Red))
            }
        End Function

        Public Function IdentitiesNonColors() As RangeList(Of Double, TagValue(Of Color))
            Return New RangeList(Of Double, TagValue(Of Color)) From {
                New RangeTagValue(Of Double, TagValue(Of Color))(0, 50, New TagValue(Of Color)("<= 50%", Color.LightGray)),
                New RangeTagValue(Of Double, TagValue(Of Color))(50, 75, New TagValue(Of Color)("50% - 75%", Color.DarkGray)),
                New RangeTagValue(Of Double, TagValue(Of Color))(75, 10000, New TagValue(Of Color)("> 75%", Color.Gray))
            }
        End Function

        Public Function BitScores() As RangeList(Of Double, TagValue(Of Color))
            Return New RangeList(Of Double, TagValue(Of Color)) From {
                New RangeTagValue(Of Double, TagValue(Of Color))(0, 40, New TagValue(Of Color)("< 40", Color.Black)),
                New RangeTagValue(Of Double, TagValue(Of Color))(40, 50, New TagValue(Of Color)("40 - 50", Color.Blue)),
                New RangeTagValue(Of Double, TagValue(Of Color))(50, 80, New TagValue(Of Color)("50 - 80", Color.Green)),
                New RangeTagValue(Of Double, TagValue(Of Color))(80, 200, New TagValue(Of Color)("80 - 200", Color.Purple)),
                New RangeTagValue(Of Double, TagValue(Of Color))(200, 10000, New TagValue(Of Color)(">= 200", Color.Red))
            }
        End Function
    End Module
End Namespace