Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure

''' <summary>
''' Exports a PFSNet evaluation result as the legacy PFSNet result table.
''' </summary>
Public Module PFSNetResultTable

    ''' <summary>
    ''' Converts a PFSNet result into the legacy tabular rows:
    ''' ``subnetwork, phenotype, statistics, pvalue, nodes, edges, weight1, weight2, genes``.
    ''' </summary>
    ''' <param name="result">A PFSNet evaluation result.</param>
    ''' <returns>One tabular row per significant subnetwork of both phenotypes.</returns>
    <Extension>
    Public Function AsTabular(result As PFSNetResultOut) As SubNetworkTableRow()
        ThrowIfNothing(result)

        Dim class1 As String = If(String.IsNullOrWhiteSpace(result.class1), "A", result.class1)
        Dim class2 As String = If(String.IsNullOrWhiteSpace(result.class2), "B", result.class2)
        Dim rows As New System.Collections.Generic.List(Of SubNetworkTableRow)

        rows.AddRange(CreateRows(result.phenotype1, class1))
        rows.AddRange(CreateRows(result.phenotype2, class2))

        Return rows.ToArray
    End Function

    ''' <summary>
    ''' Writes a PFSNet result to a comma separated table using the legacy PFSNet column layout.
    ''' </summary>
    ''' <param name="result">A PFSNet evaluation result.</param>
    ''' <param name="file">The output csv file path.</param>
    ''' <returns>The full path of the written csv file.</returns>
    <Extension>
    Public Function SaveTabular(result As PFSNetResultOut, file As String) As String
        Dim rows As SubNetworkTableRow() = result.AsTabular()
        Dim directory As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(file))

        If Not String.IsNullOrEmpty(directory) Then
            System.IO.Directory.CreateDirectory(directory)
        End If

        Using writer As New System.IO.StreamWriter(file, append:=False, encoding:=New UTF8Encoding(False))
            writer.WriteLine("subnetwork,phenotype,statistics,pvalue,nodes,edges,weight1,weight2,genes")

            For Each row As SubNetworkTableRow In rows
                writer.WriteLine(row.ToCsvLine)
            Next
        End Using

        Return System.IO.Path.GetFullPath(file)
    End Function

    Private Function CreateRows(graphs As PFSNetGraph(), phenotype As String) As SubNetworkTableRow()
        Dim rows As New System.Collections.Generic.List(Of SubNetworkTableRow)

        If graphs Is Nothing Then
            Return rows.ToArray
        End If

        For Each graph As PFSNetGraph In graphs
            Dim nodes As PFSNetGraphNode() = If(graph.nodes, Array.Empty(Of PFSNetGraphNode)())
            Dim orderedNodes As PFSNetGraphNode() = nodes _
                .OrderBy(Function(node) node.name, StringComparer.Ordinal) _
                .ToArray
            Dim weight1 As Double = If(orderedNodes.Length = 0, 0, orderedNodes.Average(Function(node) node.weight))
            Dim weight2 As Double = If(orderedNodes.Length = 0, 0, orderedNodes.Average(Function(node) node.weight2))
            Dim edges As GraphEdge() = If(graph.edges, Array.Empty(Of GraphEdge)())

            rows.Add(New SubNetworkTableRow With {
                .subnetwork = graph.Id,
                .phenotype = phenotype,
                .statistics = graph.statistics,
                .pvalue = graph.pvalue,
                .nodes = orderedNodes.Length,
                .edges = edges.Length,
                .weight1 = weight1,
                .weight2 = weight2,
                .genes = String.Join("; ", orderedNodes.Select(Function(node) node.name))
            })
        Next

        Return rows.ToArray
    End Function

    Private Sub ThrowIfNothing(result As PFSNetResultOut)
        If result Is Nothing Then
            Throw New ArgumentNullException(NameOf(result))
        End If
    End Sub
End Module

''' <summary>
''' One row of the legacy PFSNet result table.
''' </summary>
Public Class SubNetworkTableRow

    ''' <summary>
    ''' The pathway or subnetwork identifier.
    ''' </summary>
    Public Property subnetwork As String
    ''' <summary>
    ''' The phenotype class label, for example ``A`` or ``B``.
    ''' </summary>
    Public Property phenotype As String
    ''' <summary>
    ''' The paired t statistic of this subnetwork.
    ''' </summary>
    Public Property statistics As Double
    ''' <summary>
    ''' The permutation p-value of this subnetwork.
    ''' </summary>
    Public Property pvalue As Double
    ''' <summary>
    ''' The gene node count of this subnetwork.
    ''' </summary>
    Public Property nodes As Integer
    ''' <summary>
    ''' The gene to gene interaction count of this subnetwork.
    ''' </summary>
    Public Property edges As Integer
    ''' <summary>
    ''' The average node weight in the first phenotype.
    ''' </summary>
    Public Property weight1 As Double
    ''' <summary>
    ''' The average node weight in the second phenotype.
    ''' </summary>
    Public Property weight2 As Double
    ''' <summary>
    ''' The gene members of this subnetwork, separated by ``"; "``.
    ''' </summary>
    Public Property genes As String

    ''' <summary>
    ''' Renders current row as a csv line using the legacy PFSNet formatting.
    ''' </summary>
    ''' <returns>A single csv data line.</returns>
    Public Function ToCsvLine() As String
        Dim culture As CultureInfo = CultureInfo.InvariantCulture
        Dim fields As String() = {
            CsvEscape(subnetwork),
            CsvEscape(phenotype),
            statistics.ToString("0.0000000", culture),
            " " & pvalue.ToString("0.0000000", culture),
            nodes.ToString(culture),
            edges.ToString(culture),
            " " & weight1.ToString("0.0000000", culture),
            " " & weight2.ToString("0.0000000", culture),
            CsvEscape(genes)
        }

        Return String.Join(",", fields)
    End Function

    Private Shared Function CsvEscape(value As String) As String
        value = If(value, "")

        If value.IndexOfAny({","c, """"c, ControlChars.Cr, ControlChars.Lf}) >= 0 Then
            Return ChrW(34) & value.Replace(ChrW(34), ChrW(34) & ChrW(34)) & ChrW(34)
        Else
            Return value
        End If
    End Function
End Class
