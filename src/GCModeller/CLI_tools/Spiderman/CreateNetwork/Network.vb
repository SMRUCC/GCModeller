Imports System.Xml.Serialization
Imports Microsoft.VisualBasic

Public Class Network

    Public Class Edge
        <XmlAttribute> Public Property FromNode As String
        <XmlAttribute> Public Property ToNode As String
        <XmlElement> Public Property Description As String
        ''' <summary>
        ''' Path Search direction between <see cref="Edge.FromNode"></see> and <see cref="Edge.ToNode"></see> 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Direction As Directions

        Public Enum Directions
            DirectlyTo
            Bidirectional
        End Enum

        Public Overrides Function ToString() As String
            Dim strDirection As String = If(Direction = Directions.DirectlyTo, "-->", "<==>")
            Return String.Format("{0} {1} {2}; ({3})", FromNode.ToString, strDirection, ToNode.ToString, Description)
        End Function
    End Class

    Public Property Edges As Edge()

    Public Function SearchPath(FromNode As String, ToNode As String) As Edge()
        Dim DirectToLQuery = (From edge In Edges Where String.Equals(edge.FromNode, FromNode) Select edge Order By edge.Direction Ascending).ToArray
        Dim BidirectionalLQuery = (From edge In Edges Where String.Equals(edge.ToNode, FromNode) AndAlso edge.Direction = Edge.Directions.Bidirectional Select edge).ToArray

        If DirectToLQuery.IsNullOrEmpty AndAlso BidirectionalLQuery.IsNullOrEmpty Then
            Return New Edge() {}
        Else
            Dim Pathway As List(Of Edge) = New List(Of Edge)

            For Each Edge In DirectToLQuery
                If String.Equals(Edge.ToNode, ToNode) Then
                    Return New Edge() {Edge}
                Else
                    Call Pathway.AddRange(SearchPath(Edge.ToNode, ToNode))
                End If
            Next
            For Each Edge In BidirectionalLQuery
                If String.Equals(Edge.FromNode, ToNode) Then
                    Return New Edge() {Edge}
                Else
                    Call Pathway.AddRange(SearchPath(Edge.FromNode, ToNode))
                End If
            Next

            Return Pathway.ToArray
        End If
    End Function

    Public Sub Save(SavedFile As String)
        Call Me.GetXml.SaveTo(SavedFile)
    End Sub

    Public Shared Function LoadModel(GCML As String) As Network
        Dim Model = GCML.LoadXml(Of SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel)()
        Return LoadModel(Model)
    End Function

    Public Shared Function LoadModel(GCML As SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel) As Network
        Dim EdgeList As List(Of Network.Edge) = New List(Of Edge)
        Call EdgeList.AddRange(MetabolismNetwork.CreateObject(GCML.Metabolism.MetabolismNetwork.ToArray))

        Return New Network With {.Edges = EdgeList.ToArray}
    End Function

    Public Shared Function LoadCSVTabularModel(ModelFile As String) As Network
        Dim Model = New SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.DataModel.CellSystem(ModelFile, New Logging.LogFile(Settings.LogDIR & "/Spiderman.log"))
        Return LoadModel(Model.LoadAction)
    End Function
End Class
