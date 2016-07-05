#Region "Microsoft.VisualBasic::84d28104d8d4e8356e8e064ffb1d5163, ..\GCModeller\CLI_tools\ProteinInteraction\_DEBUG_MAIN.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic
Imports System.Text
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions

Module _DEBUG_MAIN
    Private Sub ffff()
        Dim vsdip = Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load("E:\Desktop\xc8004_vs_dip_bestpair.csv")
        Dim id As String() = (From row In vsdip.Skip(1).AsParallel Let _id = row(0) Where Not String.IsNullOrEmpty(_id) Select _id Distinct Order By _id Ascending).ToArray


        Dim dipData = "E:\BLAST\db\dip\dip.csv".LoadCsv(Of DataPreparations.DipRecord)(False)
        Dim matchedfile = New List(Of Microsoft.VisualBasic.ComponentModel.Key_strArrayValuePair)

        For Each _id As String In id
            Dim rowCollection = (From row In vsdip.FindAtColumn(_id, 0) Let sss = row(1).Split.First.Trim Where Not String.IsNullOrEmpty(sss) Select sss Distinct).ToArray
            If Not rowCollection.IsNullOrEmpty Then
                Dim list = New List(Of String)
                For Each item In rowCollection
                    Dim LQuery = (From ddd In dipData.AsParallel Where String.Equals(ddd.IdInteractorA, item) Select ddd.IdInteractorB).ToArray
                    Call list.AddRange(LQuery)
                    LQuery = (From ddd In dipData.AsParallel Where String.Equals(ddd.IdInteractorB, item) Select ddd.IdInteractorA).ToArray
                    Call list.AddRange(LQuery)
                Next

                list = (From iiii In list Where Not String.IsNullOrEmpty(iiii.Trim) Select iiii Distinct Order By iiii).ToList

                Dim matchedList = New List(Of String)
                For Each fjfj In list
                    Dim collection = (From row In vsdip.FindAtColumn(fjfj, 1) Select row(0) Distinct).ToArray
                    Call matchedList.AddRange(collection)
                Next

                matchedList = (From fff In matchedList Select fff Distinct Order By fff Ascending).ToList

                Call matchedfile.Add(New Microsoft.VisualBasic.ComponentModel.Key_strArrayValuePair With {.Key = _id, .Value = matchedList.ToArray})
            Else
                Call matchedfile.Add(New Microsoft.VisualBasic.ComponentModel.Key_strArrayValuePair With {.Key = _id, .Value = New String() {}})
            End If
        Next

        Dim csvData As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File

        For Each item In matchedfile
            Dim row = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
            Call row.Add(item.Key)
            If Not item.Value.IsNullOrEmpty Then
                Dim sBuilder As StringBuilder = New StringBuilder(1024)

                For Each strData In item.Value
                    Call sBuilder.Append(String.Format("{0}; ", strData))
                Next
                Call sBuilder.Remove(sBuilder.Length - 2, 2)
                Call row.Add(sBuilder.ToString)
            End If

            Call csvData.Add(row)
        Next

        Call csvData.Save("x:\final.csv", False)

        End
    End Sub

    Sub Main()

        'Call ffff()
        'Dim svq = SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228.TryParse("E:\Desktop\dip_vs_8004.txt").ExportAllBestHist
        'Dim qvs = SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228.TryParse("E:\Desktop\8004_vs_dip.txt").ExportAllBestHist

        'Call svq.Save("x:\dddddd.csv")
        'Call qvs.Save("x:\fhfhfhfhfhfh.csv")

        'Call SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.GetDiReBh2(svq, qvs).Save("x:\ddd.csv")

        Dim nn As New DataPreparations("./data/xc8004pro", "./data/dip", "E:\BLAST\bin", "C:\Program Files\R\R-3.1.0\bin", "E:\Desktop\clustal-omega-1.2.0-win32\clustal-omega-1.2.0-win32\clustalo.exe", "e:\desktop\dip\temp")
        Call nn.InferInteraction("XC_1184").Save("x:\xc_1184.csv", False)

        ''     Dim List = SwissRegulon.GetSpeciesList

        'Dim nn = SwissRegulon.GetTCSList("Xanthomonas_campestris_8004")
        'Dim sk = nn(0)
        'Dim rr = nn(1)

        'For Each id In New String() {"Photobacterium_profundum_SS9", "Pseudomonas_aeruginosa_UCBPP-PA14"}
        '    Call Download(id)
        'Next

        'Call FileIO.FileSystem.CreateDirectory("G:\Desktop\论文初稿\xan8004_tcs_crosstalk\\sk")
        'Call FileIO.FileSystem.CreateDirectory("G:\Desktop\论文初稿\xan8004_tcs_crosstalk\\rr")

        'For Each s In sk
        '    Call SwissRegulon.GetScores(s, "Xanthomonas_campestris_8004", SwissRegulon.TCSComponentTypes.kinase).Save("G:\Desktop\论文初稿\xan8004_tcs_crosstalk\sk\" & s & ".csv")
        'Next

        'For Each r In rr
        '    Call SwissRegulon.GetScores(r, "Xanthomonas_campestris_8004", SwissRegulon.TCSComponentTypes.receiver).Save("G:\Desktop\论文初稿\xan8004_tcs_crosstalk\rr\" & r & ".csv")
        'Next

        'Call GenerateMatrix("G:\Desktop\论文初稿\xan8004_tcs_crosstalk\").Save("d:\xan8004_tcs_crosstalk.csv")

        '     Call New Matrix().Generate("h:\Desktop\论文初稿\xan8004_tcs_crosstalk\xan8004_tcs_crosstalk.csv", "Xanthomonas campestris pv. campestris str. 8004 Tow-Component System Cross-Talk").Save("x:\tcs.png", System.Drawing.Imaging.ImageFormat.Png)
    End Sub



    Public Sub Convert(dipFasta As SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
        Dim chunkBuffer As SMRUCC.genomics.SequenceModel.FASTA.FastaToken() =
            New SMRUCC.genomics.SequenceModel.FASTA.FastaToken(dipFasta.Count - 1) {}
        For i As Integer = 0 To chunkBuffer.Count - 1
            Dim fsaObj = New SMRUCC.genomics.SequenceModel.FASTA.FastaToken
            Dim oldFsa = dipFasta(i)

            fsaObj.SequenceData = oldFsa.SequenceData
            Dim attrs As List(Of String) = New List(Of String)
            If oldFsa.Attributes.Count = 1 Then
                attrs.Add(Mid(oldFsa.Attributes.First, 5))
            Else
                attrs.Add(String.Format("{0} {1}", Mid(oldFsa.Attributes.First, 5), oldFsa.Attributes(1)))
                attrs.AddRange(oldFsa.Attributes.Skip(2))
            End If

            fsaObj.Attributes = attrs.ToArray

            chunkBuffer(i) = fsaObj
        Next

        Call dipFasta.Clear()
        dipFasta.AddRange(chunkBuffer)

        Call dipFasta.Save()
    End Sub
End Module

