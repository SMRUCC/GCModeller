Imports System.Drawing
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Public Class ReportRender

    Public Function CreateMap(compound As Compound, location As PointF) As MapShape

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gene">
    ''' <see cref="htext.ko00001"/>
    ''' </param>
    ''' <param name="location"></param>
    ''' <returns></returns>
    Public Function CreateMap(gene As BriteHText, location As PointF) As MapShape

    End Function
End Class

Public Class MapShape

    Public Property shape As String
    Public Property location As Double()
    ''' <summary>
    ''' kegg id list
    ''' </summary>
    ''' <returns></returns>
    Public Property entities As String()
    Public Property title As String

End Class