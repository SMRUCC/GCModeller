Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports csv = Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Text

Namespace IO

    ''' <summary>
    ''' The CSV report writer of the topology search: it turns the search
    ''' results into the product tables of the pipeline.
    ''' (拓扑搜索的 CSV 报告写入器：把搜索结果转换为流水线的产物表格)
    ''' </summary>
    ''' <remarks>
    ''' The products follow the structure of the paper:
    ''' <list type="number">
    ''' <item> ``topologies.csv`` the robustness statistics of every searched topology; </item>
    ''' <item> ``functional_circuits.csv`` the sensitivity and precision of every functional circuit, the data of the Figure 1D and 4A scatter plots; </item>
    ''' <item> ``clusters.csv`` the Hamming cluster assignment of the functional topologies; </item>
    ''' <item> ``venn_summary.csv`` the motif intersection counts of the Figure 4B Venn diagram; </item>
    ''' <item> ``design_table.csv`` the Q value progression of the motif augmentations of the Figure 6 design table. </item>
    ''' </list>
    ''' </remarks>
    Public Module CircuitsCsvWriter

        ''' <summary>
        ''' Write the robustness statistics of every searched topology.
        ''' (写入全部被搜索拓扑的鲁棒性统计)
        ''' </summary>
        ''' <param name="path">the output file path</param>
        ''' <param name="results">the topology results of the search</param>
        ''' <param name="qThreshold">the over representation threshold that the flag column is judged on</param>
        Public Function WriteTopologies(path As String,
                                        results As IEnumerable(Of Assessment.TopologyResult),
                                        qThreshold As Integer) As Boolean
            If results Is Nothing Then
                Throw New ArgumentNullException(NameOf(results))
            End If

            Dim table As New csv.File
            Dim head As New csv.RowObject

            Call head.Add("topology_id")
            Call head.Add("signs")
            Call head.Add("link_count")
            Call head.Add("simulated")
            Call head.Add("ill_behaved")
            Call head.Add("q")
            Call head.Add("mean_s")
            Call head.Add("max_s")
            Call head.Add("mean_p")
            Call head.Add("mean_o1")
            Call head.Add("has_nfblb")
            Call head.Add("has_ifflp")
            Call head.Add("over_represented")
            Call head.Add("cluster_label")

            table += head

            For Each result As Assessment.TopologyResult In results.OrderBy(Function(r) r.TopologyId)
                Dim row As New csv.RowObject

                Call row.Add(result.TopologyId.ToString(CultureInfo.InvariantCulture))
                Call row.Add(result.Signs)
                Call row.Add(result.LinkCount.ToString(CultureInfo.InvariantCulture))
                Call row.Add(result.Simulated.ToString(CultureInfo.InvariantCulture))
                Call row.Add(result.IllBehaved.ToString(CultureInfo.InvariantCulture))
                Call row.Add(result.Functional.ToString(CultureInfo.InvariantCulture))
                Call row.Add(Assessment.FunctionalAssessor.Format(result.MeanSensitivity))
                Call row.Add(Assessment.FunctionalAssessor.Format(result.MaxSensitivity))
                Call row.Add(Assessment.FunctionalAssessor.Format(result.MeanPrecision))
                Call row.Add(Assessment.FunctionalAssessor.Format(result.MeanO1))
                Call row.Add(If(result.HasNFBLB, "True", "False"))
                Call row.Add(If(result.HasIFFLP, "True", "False"))
                Call row.Add(If(result.IsOverRepresented(qThreshold), "True", "False"))
                Call row.Add(result.ClusterLabel.ToString(CultureInfo.InvariantCulture))

                table += row
            Next

            Return table.Save(path, Encodings.UTF8)
        End Function

        ''' <summary>
        ''' Write the sensitivity and precision records of the functional
        ''' circuits, the raw data of the sensitivity precision scatter plots.
        ''' (写入功能电路的灵敏度与精度记录，即 S-P 散点图的原始数据)
        ''' </summary>
        ''' <param name="path">the output file path</param>
        ''' <param name="records">the circuit records of the search</param>
        Public Function WriteFunctionalCircuits(path As String,
                                                records As IEnumerable(Of Assessment.CircuitRecord)) As Boolean
            If records Is Nothing Then
                Throw New ArgumentNullException(NameOf(records))
            End If

            Dim table As New csv.File
            Dim head As New csv.RowObject

            Call head.Add("topology_id")
            Call head.Add("sample")
            Call head.Add("O1")
            Call head.Add("Opeak")
            Call head.Add("O2")
            Call head.Add("S")
            Call head.Add("P")
            Call head.Add("output_drift")
            Call head.Add("status")

            table += head

            For Each record As Assessment.CircuitRecord In records _
                .Where(Function(r) r.IsFunctional AndAlso Not r.IsIllBehaved) _
                .OrderBy(Function(r) r.TopologyId) _
                .ThenBy(Function(r) r.SampleIndex)

                Dim row As New csv.RowObject

                Call row.Add(record.TopologyId.ToString(CultureInfo.InvariantCulture))
                Call row.Add(record.SampleIndex.ToString(CultureInfo.InvariantCulture))
                Call row.Add(Assessment.FunctionalAssessor.Format(record.O1))
                Call row.Add(Assessment.FunctionalAssessor.Format(record.Opeak))
                Call row.Add(Assessment.FunctionalAssessor.Format(record.O2))
                Call row.Add(Assessment.FunctionalAssessor.Format(record.Sensitivity))
                Call row.Add(Assessment.FunctionalAssessor.Format(record.Precision))
                Call row.Add(Assessment.FunctionalAssessor.Format(record.OutputDrift))
                Call row.Add(record.Status.ToString(CultureInfo.InvariantCulture))

                table += row
            Next

            Return table.Save(path, Encodings.UTF8)
        End Function

        ''' <summary>
        ''' Write the Hamming cluster assignment of the topologies.
        ''' (写入拓扑的 Hamming 聚类归属)
        ''' </summary>
        ''' <param name="path">the output file path</param>
        ''' <param name="results">the topology results of the search, only the clustered ones are written</param>
        Public Function WriteClusters(path As String,
                                      results As IEnumerable(Of Assessment.TopologyResult)) As Boolean
            If results Is Nothing Then
                Throw New ArgumentNullException(NameOf(results))
            End If

            Dim table As New csv.File
            Dim head As New csv.RowObject

            Call head.Add("cluster_label")
            Call head.Add("topology_id")
            Call head.Add("signs")
            Call head.Add("link_count")
            Call head.Add("q")
            Call head.Add("has_nfblb")
            Call head.Add("has_ifflp")

            table += head

            For Each result As Assessment.TopologyResult In results _
                .Where(Function(r) r.ClusterLabel >= 0) _
                .OrderBy(Function(r) r.ClusterLabel) _
                .ThenByDescending(Function(r) r.Functional) _
                .ThenBy(Function(r) r.TopologyId)

                Dim row As New csv.RowObject

                Call row.Add(result.ClusterLabel.ToString(CultureInfo.InvariantCulture))
                Call row.Add(result.TopologyId.ToString(CultureInfo.InvariantCulture))
                Call row.Add(result.Signs)
                Call row.Add(result.LinkCount.ToString(CultureInfo.InvariantCulture))
                Call row.Add(result.Functional.ToString(CultureInfo.InvariantCulture))
                Call row.Add(If(result.HasNFBLB, "True", "False"))
                Call row.Add(If(result.HasIFFLP, "True", "False"))

                table += row
            Next

            Return table.Save(path, Encodings.UTF8)
        End Function

        ''' <summary>
        ''' Write the Venn statistics of the motif intersections, which reproduces
        ''' the Figure 4B Venn diagram of the paper.
        ''' (写入基序交集的 Venn 统计，复现论文 Figure 4B 的 Venn 图)
        ''' </summary>
        ''' <param name="path">the output file path</param>
        ''' <param name="scopes">
        ''' the named scopes of the statistics, for example the over represented
        ''' functional topologies and all of the functional topologies
        ''' </param>
        Public Function WriteVenn(path As String,
                                  scopes As IEnumerable(Of (scope As String, counts As (nfblbOnly As Integer, ifflpOnly As Integer, both As Integer, neither As Integer, total As Integer)))) As Boolean
            If scopes Is Nothing Then
                Throw New ArgumentNullException(NameOf(scopes))
            End If

            Dim table As New csv.File
            Dim head As New csv.RowObject

            Call head.Add("scope")
            Call head.Add("total")
            Call head.Add("nfblb_only")
            Call head.Add("ifflp_only")
            Call head.Add("both")
            Call head.Add("neither")

            table += head

            For Each scope In scopes
                Dim row As New csv.RowObject

                Call row.Add(scope.scope)
                Call row.Add(scope.counts.total.ToString(CultureInfo.InvariantCulture))
                Call row.Add(scope.counts.nfblbOnly.ToString(CultureInfo.InvariantCulture))
                Call row.Add(scope.counts.ifflpOnly.ToString(CultureInfo.InvariantCulture))
                Call row.Add(scope.counts.both.ToString(CultureInfo.InvariantCulture))
                Call row.Add(scope.counts.neither.ToString(CultureInfo.InvariantCulture))

                table += row
            Next

            Return table.Save(path, Encodings.UTF8)
        End Function

        ''' <summary>
        ''' Write the design table: the Q value progression of the minimal
        ''' adaptation networks and their motif augmentations.
        ''' (写入设计表：最小适应网络及其基序增强的 Q 值 progression)
        ''' </summary>
        ''' <param name="path">the output file path</param>
        ''' <param name="rows">the design table rows of the experiment</param>
        Public Function WriteDesignTable(path As String,
                                         rows As IEnumerable(Of Analysis.RobustnessAnalyzer.DesignTableRow)) As Boolean
            If rows Is Nothing Then
                Throw New ArgumentNullException(NameOf(rows))
            End If

            Dim table As New csv.File
            Dim head As New csv.RowObject

            Call head.Add("name")
            Call head.Add("signs")
            Call head.Add("link_count")
            Call head.Add("q")
            Call head.Add("simulated")
            Call head.Add("fraction")

            table += head

            For Each row In rows
                Dim out As New csv.RowObject

                Call out.Add(row.Name)
                Call out.Add(row.Signs)
                Call out.Add(row.LinkCount.ToString(CultureInfo.InvariantCulture))
                Call out.Add(row.Q.ToString(CultureInfo.InvariantCulture))
                Call out.Add(row.Simulated.ToString(CultureInfo.InvariantCulture))
                Call out.Add(row.Fraction.ToString("G6", CultureInfo.InvariantCulture))

                table += out
            Next

            Return table.Save(path, Encodings.UTF8)
        End Function

    End Module

End Namespace
