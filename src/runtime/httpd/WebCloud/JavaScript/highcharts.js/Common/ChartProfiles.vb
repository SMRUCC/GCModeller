Imports System.Runtime.CompilerServices
Imports SMRUCC.WebCloud.JavaScript.highcharts.viz3D

Friend Module ChartProfiles

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function profileBase(type As ChartTypes) As chart
        Return New chart With {.type = type.Description}
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function PieChart3D() As chart
        Return New chart With {
            .type = "pie",
            .options3d = New options3d With {
                .enabled = True,
                .alpha = 45,
                .beta = 0
            }
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function BarChart3D() As chart
        Return New chart With {
            .type = "column",
            .options3d = New options3d With {
                .enabled = True,
                .alpha = 10,
                .beta = 25,
                .depth = 70
            }
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function PolarChart() As chart
        Return New chart With {
            .polar = True
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function VariWide() As chart
        Return profileBase(ChartTypes.variwide)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function AreaSpline() As chart
        Return profileBase(ChartTypes.areaspline)
    End Function
End Module
