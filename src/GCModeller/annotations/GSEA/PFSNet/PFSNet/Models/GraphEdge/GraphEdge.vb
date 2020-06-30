Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language

Namespace DataStructure

    ''' <summary>
    ''' Gene to Gene Interaction.(基因与基因之间的互作关系)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GraphEdge

        <XmlAttribute>
        Public Property pathwayID As String

        ''' <summary>
        ''' The geneID of a gene node in current pathway
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("gene1")> Public Property g1 As String
        ''' <summary>
        ''' Another partner gene node its id in current pathway
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("gene2")> Public Property g2 As String

        ''' <summary>
        ''' Does <see cref="g1"/> is equals to <see cref="g2"/>?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property isSelfLoop As Boolean
            Get
                Return String.Equals(g1, g2)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Join(vbTab, pathwayID, g1, g2)
        End Function

        Public Shared Function LoadData(path As String) As GraphEdge()
            Dim LQuery As GraphEdge() =
                LinqAPI.Exec(Of GraphEdge) <= From line As String
                                              In IO.File.ReadAllLines(path)
                                              Let tokens As String() = Strings.Split(line, vbTab)
                                              Select New GraphEdge With {
                                                  .pathwayID = tokens(0),
                                                  .g1 = tokens(1),
                                                  .g2 = tokens(2)
                                              }
            Return LQuery
        End Function
    End Class
End Namespace