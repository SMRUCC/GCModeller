Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Linq

Namespace CatalogProfiling

    <HideModuleName>
    Module Extensions

        ''' <summary>
        ''' 根据用户所输入的名称字符串生成<see cref="ValueScaleColorProfile"/>或者<see cref="CategoryColorProfile"/>颜色管理器
        ''' </summary>
        ''' <param name="profile"></param>
        ''' <param name="colorSchema">
        ''' 如果需要生成<see cref="ValueScaleColorProfile"/>颜色管理器，字符串的格式应该是``scale(color_term)``
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function GetColors(profile As Dictionary(Of String, NamedValue(Of Double)()), colorSchema$, Optional logarithm# = 2) As ColorProfile
            Dim colors As ColorProfile

            If colorSchema.IsPattern("scale\(.+\)") Then
                colorSchema = colorSchema.GetStackValue("(", ")")
                colors = profile.Values _
                    .IteratesALL _
                    .DoCall(Function(data)
                                Return New ValueScaleColorProfile(data, colorSchema, 30, logarithm:=logarithm)
                            End Function)
            Else
                Dim category As New Dictionary(Of String, String)

                For Each profileGroup In profile
                    For Each term As NamedValue(Of Double) In profileGroup.Value
                        category(term.Name) = profileGroup.Key
                    Next
                Next

                colors = New CategoryColorProfile(category, colorSchema)
            End If

            Return colors
        End Function
    End Module
End Namespace