Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace NCBIBlastResult

    Public Class OrthologyProfile

        ''' <summary>
        ''' 功能分组名称标签信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Category As String

        ''' <summary>
        ''' + name: degree
        ''' + value: color
        ''' + description: n genes
        ''' </summary>
        ''' <returns></returns>
        Public Property HomologyDegrees As NamedValue(Of Color)()

        Public ReadOnly Property Total As Integer
            Get
                Return Aggregate level As NamedValue(Of Color)
                       In HomologyDegrees
                       Let n = Val(level.Description)
                       Into Sum(n)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Category} ({Total})"
        End Function

    End Class

    Public Module OrthologyProfiles

        <Extension>
        Public Function OrthologyProfiles(result As IEnumerable(Of BBHIndex), colors As RangeList(Of Double, NamedValue(Of Color))) As OrthologyProfile()

        End Function

        <Extension>
        Public Function Plot(profile As IEnumerable(Of OrthologyProfile),
                             Optional size$ = "3300,2700",
                             Optional margin$ = g.DefaultPadding,
                             Optional bg$ = "white") As GraphicsData

        End Function
    End Module
End Namespace