Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.DOOR

    Partial Public Module DOOR_IO

        <Extension>
        Private Function __generate(DOOR As DOOR, sId As String, trim As Boolean) As String
            Dim Genes As OperonGene() = DOOR.[Select](operonID:=sId)

            If trim Then
                If Genes.Length < 2 Then
                    Return ""
                End If
            End If

            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim LocationBuilder As StringBuilder = New StringBuilder(128)

            For Each obj In Genes
                Dim strand As String = If(
                    obj.Location.Strand = Strands.Forward,
                    "+; ",
                    "-; ")

                Call sBuilder.Append(obj.Synonym & "; ")
                Call LocationBuilder.Append(strand)
            Next
            Call sBuilder.Remove(sBuilder.Length - 2, 2)
            Call LocationBuilder.Remove(LocationBuilder.Length - 2, 2)

            Dim strData As New StringBuilder(1024)
            Call strData.Append(String.Format("{0},{1},", sId, Genes.Length))
            Call strData.Append(LocationBuilder.ToString & ",")
            Call strData.Append(sBuilder.ToString)

            Return strData.ToString
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="DOOR"></param>
        ''' <param name="trim">是否移除仅含有一个基因的操纵子对象？默认不进行移除</param>
        ''' <returns></returns>
        <Extension>
        Public Function csv(DOOR As DOOR, Optional trim As Boolean = False) As String
            Dim operonIDs As String() = LinqAPI.Exec(Of String) <=
 _
                From gene As OperonGene
                In DOOR.Genes
                Select gene.OperonID
                Distinct
                Order By OperonID Ascending

            Dim LQuery = LinqAPI.MakeList(Of String) <=
 _
                From OperonID As String
                In operonIDs
                Let row As String = DOOR.__generate(OperonID, trim)
                Where Not String.IsNullOrEmpty(row)
                Select row

            Call LQuery.Insert(0, "OperonID,Counts,Direction,OperonGenes")

            Return LQuery.JoinBy(ASCII.LF)
        End Function
    End Module
End Namespace