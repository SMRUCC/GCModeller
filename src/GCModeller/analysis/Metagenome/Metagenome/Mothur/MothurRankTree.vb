Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Language

Public Class MothurRankTree : Inherits Tree(Of MothurData)

    Public Overrides ReadOnly Property QualifyName As String
        Get
            If Parent Is Nothing OrElse Parent.QualifyName = "Root" Then
                Return Data.taxon
            Else
                Return Parent.QualifyName & ";" & Data.taxon
            End If
        End Get
    End Property

    Public ReadOnly Property taxonmy As Metagenomics.Taxonomy
        Get
            Dim tokens As String() = QualifyName.StringSplit("\s*;\s*")
            Dim token As String = tokens.Last

            For i As Integer = tokens.Length - 2 To 0 Step -1
                If token <> tokens(i) Then
                    tokens = tokens.Take(i + 1).ToArray
                    Exit For
                End If
            Next

            Dim ranks = Metagenomics.TaxonomyParser(tokens.JoinBy(";"))
            Dim taxon As New Metagenomics.Taxonomy(ranks)

            Return taxon
        End Get
    End Property

    Public Function GetOTUTable() As OTUTable()
        Dim pull As New List(Of OTUTable)
        Call PullLeafNode(Me, pull)
        Return pull.ToArray
    End Function

    Private Shared Sub PullLeafNode(node As MothurRankTree, pull As List(Of OTUTable))
        If node.Childs.IsNullOrEmpty Then
            Dim taxon As Metagenomics.Taxonomy = node.taxonmy
            Dim sampleData As New Dictionary(Of String, Double)

            ' 20220507 sample id data will be missing from the summary
            ' result file if just contains only one sample data
            '
            ' use the [total] count as the sample data result
            ' at here!
            '
            If node.Data.samples.Count = 0 Then
                sampleData.Add("total", node.Data.total)
            Else
                For Each sampleId As String In node.Data.samples.Keys
                    Call sampleData.Add(sampleId, node.Data.samples(sampleId))
                Next
            End If

            Dim OTU As New OTUTable With {
                .ID = pull.Count + 1,
                .Properties = sampleData,
                .taxonomy = taxon
            }

            ' is leaf node
            Call pull.Add(OTU)
        Else
            For Each subtype As MothurRankTree In node.Childs.Values
                Call PullLeafNode(subtype, pull)
            Next
        End If
    End Sub

    Public Shared Function LoadTaxonomySummary(file As String) As MothurRankTree
        Dim rows As MothurData() = file.LoadTsv(Of MothurData).ToArray
        Dim i As i32 = Scan0
        Dim root As New MothurRankTree With {
            .Data = rows(Scan0),
            .ID = ++i,
            .label = .Data.rankID,
            .Childs = New Dictionary(Of String, Tree(Of MothurData))
        }
        Dim taxonNode As MothurRankTree = root
        Dim parent As New Dictionary(Of String, MothurRankTree)

        Call parent.Add(root.label, root)

        For Each row As MothurData In rows.Skip(1)
            taxonNode = New MothurRankTree With {
                .Childs = New Dictionary(Of String, Tree(Of MothurData)),
                .Data = row,
                .ID = ++i,
                .label = row.rankID
            }
            taxonNode.Parent = parent(row.parentID).Add(taxonNode)

            Call parent.Add(row.rankID, taxonNode)
        Next

        Return root
    End Function

End Class

Public Class MothurData

    Public Property taxlevel As Integer
    Public Property rankID As String
    Public Property taxon As String
    Public Property daughterlevels As Integer
    Public Property total As Integer
    Public Property samples As Dictionary(Of String, Integer)

    Public ReadOnly Property parentID As String
        Get
            Dim t As String() = rankID.Split("."c)
            Dim parent As String = t.Take(t.Length - 1).JoinBy(".")

            Return parent
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"[{rankID}] ({total}){taxon}"
    End Function

End Class