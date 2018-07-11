Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application

Public Class SyntenyRegion

    Public Property query As DoubleRange
    Public Property subject As DoubleRange

    Public Iterator Function Translate(genomeSize As (query&, subject&), plotRegion As RectangleF) As IEnumerable(Of PointF)
        Dim qY = plotRegion.Top
        Dim sY = plotRegion.Top
        Dim posX As Vector = {query.Min, query.Max, subject.Min, subject.Max}
        Dim gSize As Vector = {
            genomeSize.query, genomeSize.query,
            genomeSize.subject, genomeSize.subject
        }

        posX = posX / gSize * plotRegion.Width

        Dim i As int = Scan0

        Yield New PointF(posX(++i), qY)
        Yield New PointF(posX(++i), qY)
        Yield New PointF(posX(++i), sY)
        Yield New PointF(posX(++i), sY)
    End Function

    Public Shared Iterator Function PopulateRegions(maps As IEnumerable(Of BlastnMapping), Optional schema$ = "Set2:c8") As IEnumerable(Of SyntenyRegion)

    End Function
End Class