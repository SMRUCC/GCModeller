Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Serialization

Namespace ContextModel

    ''' <summary>
    ''' Context model of a specific genomics feature site.
    ''' </summary>
    Public Structure Context

        ''' <summary>
        ''' Current feature site
        ''' </summary>
        Public ReadOnly Feature As NucleotideLocation
        ''' <summary>
        ''' <see cref="Feature"/> its upstream region with a specific length
        ''' </summary>
        Public ReadOnly Upstream As NucleotideLocation
        ''' <summary>
        ''' <see cref="Feature"/> its downstream region with a specific length
        ''' </summary>
        Public ReadOnly Downstream As NucleotideLocation
        Public ReadOnly Antisense As NucleotideLocation

        ''' <summary>
        ''' The user custom tag data for this feature site.
        ''' </summary>
        Public ReadOnly Tag As String

        Sub New(g As IGeneBrief, dist As Integer)
            Call Me.New(g.Location, dist, g.ToString)
        End Sub

        Sub New(loci As NucleotideLocation, dist As Integer, Optional userTag As String = Nothing)
            Feature = loci
            Tag = NotEmpty(userTag, loci.ToString)

            If loci.Strand = Strands.Forward Then
                Upstream = New NucleotideLocation(loci.Left - dist, loci.Left, Strands.Forward)
                Downstream = New NucleotideLocation(loci.Right, loci.Right + dist, Strands.Forward)
                Antisense = New NucleotideLocation(loci.Left, loci.Right, Strands.Reverse)
            Else
                Upstream = New NucleotideLocation(loci.Right, loci.Right + dist, Strands.Reverse)
                Downstream = New NucleotideLocation(loci.Left - dist, loci.Left, Strands.Reverse)
                Antisense = New NucleotideLocation(loci.Left, loci.Right, Strands.Forward)
            End If
        End Sub

        Sub New(g As Contig, dist As Integer)
            Call Me.New(g.MappingLocation, dist, g.ToString)
        End Sub

        ''' <summary>
        ''' Get relationship between target <see cref="NucleotideLocation"/> with current feature site.
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="stranded">Get <see cref="SegmentRelationships"/> ignored of nucleotide <see cref="Strands"/>?</param>
        ''' <returns></returns>
        Public Function GetRelation(loci As NucleotideLocation, stranded As Boolean) As SegmentRelationships
            If stranded Then
                Return __relStranede(loci)
            Else
                Return __relUnstrand(loci)
            End If
        End Function

        ''' <summary>
        ''' Get <see cref="SegmentRelationships"/> ignored of nucleotide <see cref="Strands"/>.
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <returns></returns>
        Private Function __relUnstrand(loci As NucleotideLocation) As SegmentRelationships
            Dim rel As SegmentRelationships = __getRel(loci)

            If rel = SegmentRelationships.Blank Then
                If loci.Strand <> Feature.Strand Then
                    If Antisense.IsInside(loci) Then
                        Return SegmentRelationships.InnerAntiSense
                    End If
                End If
            End If

            Return rel
        End Function

        Private Function __getRel(loci As NucleotideLocation) As SegmentRelationships
            If Upstream.IsInside(loci) Then
                Return SegmentRelationships.UpStream
            ElseIf Downstream.IsInside(loci) Then
                Return SegmentRelationships.DownStream
            ElseIf Feature.Equals(loci, 1) Then
                Return SegmentRelationships.Equals
            ElseIf Feature.IsInside(loci) Then
                Return SegmentRelationships.Inside
            Else
                If Feature.IsInside(loci.Left) AndAlso Upstream.IsInside(loci.Right) Then
                    Return SegmentRelationships.UpStreamOverlap
                ElseIf Feature.IsInside(loci.Right) AndAlso Upstream.IsInside(loci.Left) Then
                    Return SegmentRelationships.UpStreamOverlap
                ElseIf Feature.IsInside(loci.Left) AndAlso Downstream.IsInside(loci.Right) Then
                    Return SegmentRelationships.DownStreamOverlap
                ElseIf Feature.IsInside(loci.Right) AndAlso Downstream.IsInside(loci.Left) Then
                    Return SegmentRelationships.DownStreamOverlap
                ElseIf loci.IsInside(Feature) Then
                    Return SegmentRelationships.Cover
                Else
                    Return SegmentRelationships.Blank
                End If
            End If
        End Function

        Private Function __relStranede(loci As NucleotideLocation) As SegmentRelationships
            If loci.Strand <> Feature.Strand Then  ' 不在同一条链之上
                If Antisense.IsInside(loci) Then
                    Return SegmentRelationships.InnerAntiSense
                Else
                    Return SegmentRelationships.Blank
                End If
            Else
                Return __getRel(loci)
            End If
        End Function

        ''' <summary>
        ''' Get tags data
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Tag
        End Function
    End Structure
End Namespace