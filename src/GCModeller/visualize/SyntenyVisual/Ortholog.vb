Imports System.Drawing
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

''' <summary>
''' 直系同源的绘图数据模型
''' </summary>
Public Module OrthologAPI

    Delegate Function __getLine(pt1 As Point, pt2 As Point, color As Color) As Line

    ReadOnly __createLines As New Dictionary(Of LineStyles, __getLine) From {
        {LineStyles.Polyline, AddressOf GetPolyline},
        {LineStyles.Bézier, AddressOf GetBézier},
        {LineStyles.Straight, AddressOf GetLine}
    }

    ''' <summary>
    ''' Creates the drawing model from the bbh result.
    ''' </summary>
    ''' <param name="source">bbh ortholog analysis result</param>
    ''' <param name="query">The genomics context of the query</param>
    ''' <param name="hit">The genomics context of the hit</param>
    ''' <param name="colors">Color profiles, this can be family, COGS, pathways or others</param>
    ''' <param name="h1"></param>
    ''' <param name="h2"></param>
    ''' <param name="style"></param>
    ''' <param name="width">绘图区域的宽度</param>
    ''' <returns></returns>
    Public Function FromBBH(source As IEnumerable(Of BBHIndex),
                            query As PTT,
                            hit As PTT,
                            colors As Func(Of GeneBrief, GeneBrief, Color),
                            h1 As Integer,
                            h2 As Integer,
                            width As Integer,
                            margin As Integer,
                            Optional style As LineStyles = LineStyles.Polyline) As Line()

        Dim createLine As __getLine = __createLines(style)
        Dim l1 As Integer = query.Size
        Dim l2 As Integer = hit.Size
        Dim result As New List(Of Line)

        For Each x As BBHIndex In source
            Dim gq As GeneBrief = query(x.QueryName)
            Dim gh As GeneBrief = hit(x.HitName)
            Dim cl As Color = colors(gq, gh)

            If gq Is Nothing OrElse gh Is Nothing Then
                Call VBDebugger.Warning($"{x.QueryName} --> {x.HitName} unable found brief info!")
            Else
                Dim from As New Point(width * gq.ATG / l1 + margin, h1)
                Dim topt As New Point(width * gh.ATG / l2 + margin, h2)

                result += createLine(from, topt, cl)
            End If
        Next

        Return result
    End Function

    Private Function GetLine(pt1 As Point, pt2 As Point, color As Color) As Line
        Return New StraightLine(pt1, pt2, color)
    End Function

    Private Function GetPolyline(pt1 As Point, pt2 As Point, color As Color) As Line
        Return New Polyline(pt1, pt2, color)
    End Function

    Private Function GetBézier(pt1 As Point, pt2 As Point, color As Color) As Line
        Return New Bézier(pt1, pt2, color)
    End Function
End Module