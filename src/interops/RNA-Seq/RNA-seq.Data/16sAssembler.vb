Imports System.IO
Imports SMRUCC.genomics.SequenceModel.SAM

Public Module Assembler

    Public Function SequenceCoverage(sam$, workspace$)
        Dim reader As New SAMStream(sam)

        Using headWriter = $"{workspace}/head.part".OpenWriter
            For Each header As SAMHeader In reader.IteratesAllHeaders
                If header.TAGValue = SAMHeader.TAGS.SQ Then
                    Call headWriter.WriteLine(header.GenerateDocumentLine)
                End If
            Next
        End Using

        Dim refs As New Dictionary(Of String, StreamWriter)

        For Each read As AlignmentReads In reader _
            .IteratesAllReads _
            .Where(Function(r) Not r.IsUnmappedReads)

            Dim key$ = Mid(read.RNAME, 1, 3)

            ' 可能会处理10GB以上的文件，数据量会非常大
            ' 所以不能够将reads数据都读进入内存中
            ' 在这里将reads缓存到硬盘工作区上的临时文件中
            If Not refs.ContainsKey(key) Then
                refs(key) = $"{workspace}/{key.First}/{key.NormalizePathString}.sam".OpenWriter

                Call Console.WriteLine()
                Call $"Open {key}".__INFO_ECHO
            Else
                Console.Write("."c)
            End If

            refs(key).WriteLine(read.GenerateDocumentLine)
        Next

        For Each ref As StreamWriter In refs.Values
            Call ref.Flush()
            Call ref.Close()
            Call ref.Dispose()
        Next

        ' 下面开始进行装配为contig
        Call (ls - l - r - "*.sam" <= workspace) _
            .AsParallel _
            .Select(Function(path)
                        Dim readsGroup = New SAMStream(path).IteratesAllReads.GroupBy(Function(r) r.RNAME)

                        For Each refer In readsGroup
                            Dim ref$ = refer.Key
                            Dim reads = refer.Select(Function(r) r.SequenceData).AsList
                            Dim contig$ = reads.AsList.ShortestCommonSuperString

                            Using view As StreamWriter = $"{path.TrimSuffix}-{ref.NormalizePathString}.txt".OpenWriter
                                Call reads.TableView(contig, view)
                            End Using
                        Next

                        Return Nothing
                    End Function) _
            .ToArray
    End Function
End Module
