Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.GCOutlier
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class SeqDiff : Implements sIdEnumerable

    Public Property uid As String Implements sIdEnumerable.Identifier
    Public Property Tag As String
    Public Property Location As String
    Public Property Host As String
    Public Property [Date] As String
    Public Property data As Dictionary(Of String, String)

    Public Shared Function Parser(fa As FastaToken) As SeqDiff
        Dim attrs As String() = fa.Attributes

        Return New SeqDiff With {
            .uid = attrs(Scan0),
            .Date = attrs.Last,
            .Tag = attrs(1),
            .Host = If(attrs.Length = 5 OrElse attrs.Length = 4, attrs(2), attrs(3)),
            .Location = If(attrs.Length = 4, attrs(2), If(attrs.Length = 5, attrs(3), attrs(4))),
            .data = New Dictionary(Of String, String)
        }
    End Function

    Public Shared Sub GCOutlier(mla As IEnumerable(Of FastaToken), ByRef bufs As SeqDiff(), quantiles As Double(),
                                Optional winsize As Integer = 250,
                                Optional steps As Integer = 50,
                                Optional slideSize As Integer = 5)

        Dim gcSkewData = OutlierAnalysis(mla, quantiles, winsize, steps, slideSize, AddressOf GCSkew)
        Dim gcContentData = OutlierAnalysis(mla, quantiles, winsize, steps, slideSize, AddressOf GCContent)
        Dim dict As Dictionary(Of String, SeqDiff) = bufs.ToDictionary(Function(x) x.uid & x.Date)

        Call __addData(dict, gcSkewData, "GCSkew")
        Call __addData(dict, gcContentData, "GC%")
    End Sub

    Private Shared Sub __addData(ByRef dict As Dictionary(Of String, SeqDiff), data As IEnumerable(Of lociX), title As String)
        Dim g = From x As lociX
                In data
                Let attrs As String() = x.Title.Split("|"c)
                Let uid = attrs.First & attrs.Last
                Select x, uid
                Group x By uid Into Group

        For Each x In g
            Dim qg = From o As lociX
                     In x.Group
                     Select o
                     Group o By o.qLevel Into Group

            For Each ql In qg
                Dim gg = ql.Group.ToArray
                dict(x.uid).data.Add($"{title}(quantile={ql.qLevel}) avg", gg.Average(Function(o) o.value))
                dict(x.uid).data.Add($"{title}(quantile={ql.qLevel}) count", gg.Length)
            Next
        Next
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
