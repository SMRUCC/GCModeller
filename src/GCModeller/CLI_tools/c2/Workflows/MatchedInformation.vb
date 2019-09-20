#Region "Microsoft.VisualBasic::09199c4ffe8a2dc5cd6c4eda9a2b215f, CLI_tools\c2\Workflows\MatchedInformation.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    ' Class MatchedInformation
    ' 
    '     Properties: [Objects]
    ' 
    '     Function: CogStatics, Generate, Get_TFBS, GetObject, Load
    ' 
    '     Sub: Generate2, (+4 Overloads) Match, MatchCOG
    '     Class [Object]
    ' 
    '         Properties: Id, Operons
    ' 
    '         Function: Get_Matched, GetMotifs, ToString
    '         Class Operon
    ' 
    '             Properties: Id, Motifs
    ' 
    '             Function: Get_Matched, ToString
    '             Class Motif
    ' 
    '                 Properties: Id, PValue, TFBSs
    ' 
    '                 Function: ToString
    '                 Class TFBS
    ' 
    '                     Properties: BiologicalProcess, Effector, EValue, Family, Id
    '                                 LocusId, Matched, PValue, Regulation, Regulog
    ' 
    '                     Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' Class Regulators
    ' 
    '     Properties: Regulators
    ' 
    '     Function: Load
    ' 
    '     Sub: (+2 Overloads) Match
    '     Class Regulator
    ' 
    '         Properties: AccessionId, BiologicalProcess, Description, Effector, Matched
    '                     Regulation, Regulog, TFBSs
    ' 
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions


Public Class MatchedInformation

    <Xml.Serialization.XmlElement> Public Property [Objects] As [Object]()

    Dim _InnerTFBSsList As [Object].Operon.Motif.TFBS()

    Public Class [Object]
        <Xml.Serialization.XmlElement> Public Property Operons As Operon()
        <Xml.Serialization.XmlAttribute> Public Property Id As String

        Public Overrides Function ToString() As String
            Return Id
        End Function

        Public Function GetMotifs(Id As String) As Operon.Motif()
            Dim List As List(Of Operon.Motif) = New List(Of Operon.Motif)
            For Each Operon In Operons
                Call List.AddRange(Operon.Motifs)
            Next
            Dim LQuery = (From Motif In List Where String.Equals(Id, Motif.Id) Select Motif).ToArray
            Return LQuery
        End Function

        Public Function Get_Matched() As String()
            Dim List As List(Of String) = New List(Of String)
            For Each Operon In Operons
                Call List.AddRange(Operon.Get_Matched)
            Next
            Dim LQuery = (From item In List Select item Distinct Order By item Ascending).ToArray
            Return LQuery
        End Function

        Public Class Operon
            <Xml.Serialization.XmlAttribute> Public Property Id As String
            <Xml.Serialization.XmlElement> Public Property Motifs As Motif()

            ''' <summary>
            ''' Get matched regulator
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function Get_Matched() As String()
                Dim List As List(Of String) = New List(Of String)
                For Each Motif In Motifs
                    For Each tbfs In Motif.TFBSs
                        If Not tbfs.Matched.IsNullOrEmpty Then
                            Call List.AddRange(tbfs.Matched)
                        End If
                    Next
                Next
                Dim LQuery = (From item In List Select item Distinct Order By item Ascending).ToArray
                Return LQuery
            End Function

            Public Overrides Function ToString() As String
                Return Id
            End Function

            Public Class Motif
                <Xml.Serialization.XmlAttribute> Public Property Id As String
                <Xml.Serialization.XmlAttribute> Public Property PValue As Double
                <Xml.Serialization.XmlElement> Public Property TFBSs As List(Of TFBS) = New List(Of TFBS)

                Public Class TFBS
                    <Xml.Serialization.XmlAttribute> Public Property Id As String
                    <Xml.Serialization.XmlAttribute> Public Property Matched As String()
                    <Xml.Serialization.XmlAttribute> Public Property PValue As Double
                    <Xml.Serialization.XmlAttribute> Public Property EValue As Double

                    <Xml.Serialization.XmlAttribute> Public Property LocusId As String
                    <Xml.Serialization.XmlAttribute> Public Property Family As String
                    <Xml.Serialization.XmlAttribute> Public Property Regulog As String

                    <Xml.Serialization.XmlAttribute> Public Property BiologicalProcess As String
                    <Xml.Serialization.XmlElement> Public Property Effector As String
                    <Xml.Serialization.XmlAttribute> Public Property Regulation As String

                    Public Overrides Function ToString() As String
                        If Not Matched.IsNullOrEmpty Then
                            Return String.Format("{0}... -> {1}", Matched.First, Id)
                        Else
                            Return Id
                        End If
                    End Function
                End Class

                Public Overrides Function ToString() As String
                    Return Id
                End Function
            End Class
        End Class
    End Class

    Public Function Generate() As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Dim sBuilder As Text.StringBuilder = New Text.StringBuilder(1024)
        Dim CreateLine = Function(strArray As String()) As String
                             If strArray.IsNullOrEmpty Then Return ""

                             Call sBuilder.Clear()
                             For Each item In strArray
                                 If Not String.IsNullOrEmpty(item) Then
                                     Call sBuilder.Append(item & ";")
                                 End If
                             Next
                             If sBuilder.Length = 0 Then Return ""
                             Call sBuilder.Remove(sBuilder.Length - 1, 1)

                             Return sBuilder.ToString
                         End Function

        Dim File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
        Call File.AppendLine(New String() {"Pathway", "Matched_regulators", "AssociatedOperon", "OperonGenes"})
        For Each [Object] In Objects
            For Each operon In [Object].Operons
                Dim row = New String() {[Object].Id, CreateLine(operon.Get_Matched), Regex.Match(operon.Id, "AssociatedOperon=[^]]+").Value.Replace("AssociatedOperon=", ""),
                                        Regex.Match(operon.Id, "OperonGenes=[^]]+").Value.Replace("OperonGenes=", "").Replace(" ", "")}
                Call File.AppendLine(row)
            Next
        Next

        Return File
    End Function

    Public Sub Generate2(Pathways As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Pathways, ExportSaved As String)
        Dim sBuilder As Text.StringBuilder = New Text.StringBuilder(1024)
        Dim CreateLine = Function(strArray As String()) As String
                             If strArray.IsNullOrEmpty Then Return ""

                             Call sBuilder.Clear()
                             For Each item In strArray
                                 If Not String.IsNullOrEmpty(item) Then
                                     Call sBuilder.Append(item & ";")
                                 End If
                             Next
                             If sBuilder.Length = 0 Then Return ""
                             Call sBuilder.Remove(sBuilder.Length - 1, 1)

                             Return sBuilder.ToString
                         End Function
        Dim File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
        Call File.AppendLine(New String() {"Matched_regulators", "Pathway", "Pathway_Types"})
        For Each [Object] In Objects
            Dim Id = [Object].Id
            If InStr(Id, "X_") Then
                Id = Mid(Id, 3)
            End If
            If InStr(Id, "_split_") Then
                Id = Mid(Id, 1, InStr(Id, "_split_") - 1)
            End If
            Dim Types = CreateLine(Pathways.Select(Id).Types.ToArray)

            For Each operon In [Object].Operons
                Dim Matched = operon.Get_Matched
                For Each Regulator In Matched
                    Dim row = New String() {Regulator, Id, Types}
                    Call File.AppendLine(row)
                Next
            Next
        Next
        Dim LQuery = (From row In File Select row.AsLine Distinct).ToArray
        Call IO.File.WriteAllLines(ExportSaved, LQuery)
    End Sub

    Public Function GetObject(Id As String) As [Object]
        Dim LQuery = (From [object] In Objects Where String.Equals(Id, [object].Id) Select [object]).ToArray
        If LQuery.IsNullOrEmpty Then
            Return Nothing
        Else
            Return LQuery.First
        End If
    End Function

    Public Function Get_TFBS(Id As String) As [Object].Operon.Motif.TFBS()
        Dim LQuery = (From tfbs In Me._InnerTFBSsList.AsParallel Where String.Equals(tfbs.Id, Id) Select tfbs).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="MEME_OUT">Dir</param>
    ''' <param name="MAST_OUT">Dir</param>
    ''' <param name="RegulatorSequence">Fsa</param>
    ''' <param name="BestHitCsv">Csv</param>
    ''' <param name="ExportDir">Dir</param>
    ''' <param name="TFBSInfo">Fsa</param>
    ''' <param name="virulenceList">Txt</param>
    ''' <remarks></remarks>
    Public Shared Sub Match(MEME_OUT As String, MAST_OUT As String, RegulatorSequence As String, BestHitCsv As String, ExportDir As String, TFBSInfo As String, virulenceList As String(),
                            RegpreciseTFF As String, MetaCycDir As String, COGProfile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,
                            Proteins As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile, Optional _pathway As Boolean = True)
        Dim regulators = c2.Regulators.Load(RegulatorSequence)
        Dim TFBS = LANS.SystemsBiology.SequenceModel.FASTA.FastaFile.Read(TFBSInfo)
        Call regulators.Match(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(BestHitCsv))
        Call regulators.Match(RegpreciseTFF.LoadXml(Of LANS.SystemsBiology.DatabaseServices.Regtransbase.WebServices.RegPreciseTFFamily))
        Dim MetaCyc = LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(MetaCycDir)

        Call FileIO.FileSystem.CreateDirectory(ExportDir & "/typesView/")
        Call FileIO.FileSystem.CreateDirectory(ExportDir & "/cogView/")
        Call FileIO.FileSystem.CreateDirectory(ExportDir & "/cog_function_statisticsView/")

        For Each file In FileIO.FileSystem.GetFiles(MEME_OUT, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
            Dim FileName As String = FileIO.FileSystem.GetFileInfo(file).Name

            Dim inf = MatchedInformation.Load(file)
            Call inf.Match(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(MAST_OUT & "/" & FileName))
            Call inf.Match(TFBS)
            Call inf.Match(regulators)

            Call inf.GetXml.SaveTo(ExportDir & "/" & FileName & "_mathced.xml")
            Dim regulation = inf.Generate
            Call regulation.First().InsertAt("virulence_regulator", 0)
            Call regulation.First().InsertAt("virulence_pathway", 0)

            For i As Integer = 1 To regulation.Count - 1
                Dim row = regulation(i)
                Dim LQuery = (From virgene In virulenceList Where Array.IndexOf(row(1).Split(CChar(";")), virgene) > -1 Select 1).ToArray 'virulence regulator
                If LQuery.IsNullOrEmpty Then
                    row.InsertAt("", 0)
                Else
                    row.InsertAt(LQuery.Count, 0)
                End If
                LQuery = (From virgene In virulenceList Where Array.IndexOf(row(4).Split(CChar(";")), virgene) > -1 Select 1).ToArray 'virulence pathway
                If LQuery.IsNullOrEmpty Then
                    row.InsertAt("", 0)
                Else
                    row.InsertAt(LQuery.Count, 0)
                End If
            Next

            Call regulation.Save(ExportDir & "/" & FileName & "_regulation.csv", False)
            If _pathway Then Call inf.Generate2(MetaCyc.GetPathways, ExportDir & "/typesView/" & FileName)

            Call MatchCOG(COGProfile, regulation, ExportDir & "/cogView/" & FileName)
            Dim Profile = CogStatics(ExportDir & "/cogView/" & FileName, LANS.SystemsBiology.Assembly.NCBI.COG.Function.Default, Proteins, virulenceList)
            Dim RegulatorIds = (From row In Profile.Skip(1) Select row.First Distinct Order By First Ascending).ToArray
            Call Proteins.Takes(Function(fsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) Array.IndexOf(RegulatorIds, fsa.Attributes.First.Split.First) > -1).Save(ExportDir & "/cog_function_statisticsView/" & FileName & ".fsa")
            Call Profile.Save(ExportDir & "/cog_function_statisticsView/" & FileName, False)
        Next

        Call regulators.GetXml.SaveTo(ExportDir & "/regprecise_regulators.xml")
    End Sub

    Public Shared Function CogStatics(Data As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, [Function] As LANS.SystemsBiology.Assembly.NCBI.COG.Function,
                                      Proteins As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile, virulence As String()) _
        As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File

        Dim File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
        Call File.AppendLine(New String() {"RegulatorId"})
        Call File.First.AddRange((From item In [Function].Categories Select item.Class).ToArray)
        Call File.First.Add("virulence_genes")

        Dim Regulators = (From row In Data Select row.First Distinct Order By First Ascending).ToArray
        For Each Regulator In Regulators
            Dim LQuery = (From row In Data Where String.Equals(Regulator, row.First()) Select row).ToArray
            Dim Line As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject = New DocumentFormat.Csv.DocumentStream.RowObject From {Regulator}
            Call Line.AddRange((From n In [Function].Statistics((From row In LQuery Select row(2)).ToArray) Select CStr(n)).ToArray)
            Call Line.Add((From gene In (From row In LQuery Select row(1)).ToArray Where Array.IndexOf(virulence, gene) > -1 Select 1).ToArray.Count)
            Call File.AppendLine(Line)
        Next

        Return File
    End Function

    Public Shared Sub MatchCOG(COGProfile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,
                               Data As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,
                               ExportSaved As String)

        Dim View As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
        For i As Integer = 1 To Data.Count - 1
            Dim row = Data(i)
            If String.IsNullOrEmpty(Trim(row(3))) Then
                Continue For
            End If
            Dim MatchedRegulators = row(3).Split(CChar(";"))
            Dim OperonGenes = row(5).Split(CChar(";"))
            For Each item In MatchedRegulators
                For Each Gene In OperonGenes
                    Dim COGFind = COGProfile.FindAtColumn(Gene, 0)
                    If Not COGFind.IsNullOrEmpty Then
                        Dim COG = COGFind.First
                        Call View.AppendLine(New String() {item, Gene, COG(3), COG(4), COG(5)})
                    End If
                Next
            Next
        Next

        Dim LQuery = (From row In View Select row.AsLine Distinct).ToArray
        Call IO.File.WriteAllLines(ExportSaved, LQuery)
    End Sub

    Public Sub Match(Data As Regulators)
        For i As Integer = 0 To Me._InnerTFBSsList.Count - 1
            Dim TBFS = _InnerTFBSsList(i)
            Dim LQuery = (From regulator In Data.Regulators Where String.Equals(regulator.Regulog, TBFS.Regulog) AndAlso Array.IndexOf(regulator.TFBSs, TBFS.LocusId) > -1 Select regulator).ToArray
            If Not LQuery.IsNullOrEmpty Then
                TBFS.Matched = (From regulator In LQuery Select regulator.Matched).ToArray
                TBFS.BiologicalProcess = LQuery.First.BiologicalProcess
                TBFS.Effector = LQuery.First.Effector
                TBFS.Regulation = LQuery.First.Regulation
            End If
        Next
    End Sub

    Public Shared Function Load(MEME_OUT As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As MatchedInformation
        Dim Objects As String() = (From row In MEME_OUT.Skip(1) Select row.First Distinct Order By First Ascending).ToArray
        Dim MatchedInformation As MatchedInformation = New MatchedInformation With {.Objects = New [Object](Objects.Count - 1) {}}
        For i As Integer = 0 To Objects.Count - 1
            Dim [Object] As [Object] = New [Object] With {.Id = Objects(i)}
            Dim LQuery = (From row In MEME_OUT Where String.Equals(row.First, [Object].Id) Select row).ToArray
            Dim Operons = (From row In LQuery Select row(1) Distinct).ToArray
            [Object].Operons = New [Object].Operon(Operons.Count - 1) {}
            For j As Integer = 0 To Operons.Count - 1
                Dim Operon As [Object].Operon = New [Object].Operon With {.Id = Operons(j)}
                Dim Motifs = (From row In LQuery Where String.Equals(row(1), Operon.Id) Select New [Object].Operon.Motif With {.Id = row(2), .PValue = Val(row(3))}).ToArray
                Operon.Motifs = Motifs
                [Object].Operons(j) = Operon
            Next

            MatchedInformation.Objects(i) = [Object]
        Next

        Return MatchedInformation
    End Function

    Public Sub Match(Data As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
        For Each row In Data.Skip(1)
            If String.IsNullOrEmpty(row(1).Trim) Then
                Continue For
            End If

            Dim [Object] = Me.GetObject(row(0))
            Dim Motifs = [Object].GetMotifs(row(1))
            Dim TFBS As [Object].Operon.Motif.TFBS = New [Object].Operon.Motif.TFBS With {.Id = row(3), .EValue = Val(row(4)), .PValue = Val(row(2))}

            For i As Integer = 0 To Motifs.Count - 1
                Call Motifs(i).TFBSs.Add(TFBS)
            Next
        Next

        Dim List As List(Of [Object].Operon.Motif.TFBS) = New List(Of [Object].Operon.Motif.TFBS)
        For Each obj In Objects
            For Each operon In obj.Operons
                For Each motif In operon.Motifs
                    Call List.AddRange(motif.TFBSs)
                Next
            Next
        Next
        Me._InnerTFBSsList = List.ToArray
    End Sub

    Public Sub Match(Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
        Dim LQuery = (From fsa In Data Let Title As String = fsa.Title
                      Let Id As String = Title.Split()(1)
                      Let LocusId As String = Regex.Match(Title, "gene=[^]]+").Value.Replace("gene=", "")
                      Let Family As String = Regex.Match(Title, "family=[^]]+").Value.Replace("family=", "")
                      Let Regulog As String = Regex.Match(Title, "regulog=[^]]+").Value.Replace("regulog=", "")
                      Select New [Object].Operon.Motif.TFBS With {.Id = Id, .LocusId = LocusId, .Family = Family, .Regulog = Regulog}).ToArray
        For Each tfbs In LQuery
            Dim List = Me.Get_TFBS(tfbs.Id)
            For i As Integer = 0 To List.Count - 1
                Dim item = List(i)
                item.Family = tfbs.Family
                item.Regulog = tfbs.Regulog
                item.LocusId = tfbs.LocusId
            Next
        Next
    End Sub
End Class

Public Class Regulators

    <Xml.Serialization.XmlElement> Public Property Regulators As Regulator()

    Public Class Regulator
        <Xml.Serialization.XmlAttribute> Public Property AccessionId As String
        <Xml.Serialization.XmlElement> Public Property Description As String
        <Xml.Serialization.XmlAttribute> Public Property Regulog As String
        <Xml.Serialization.XmlElement> Public Property TFBSs As String()
        <Xml.Serialization.XmlAttribute> Public Property Matched As String
        <Xml.Serialization.XmlAttribute> Public Property BiologicalProcess As String
        <Xml.Serialization.XmlElement> Public Property Effector As String
        <Xml.Serialization.XmlAttribute> Public Property Regulation As String

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Matched) Then
                Return AccessionId
            Else
                Return String.Format("{0} -> {1}", AccessionId, Matched)
            End If
        End Function
    End Class

    Public Sub Match(Data As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
        For i As Integer = 0 To Regulators.Count - 1
            Dim Regulator = Regulators(i)
            Dim LQuery = (From row In Data Where String.Equals(Regulator.AccessionId, row(0)) Select row).ToArray
            If Not LQuery.IsNullOrEmpty Then
                Regulator.Matched = LQuery.First()(1)
            End If
        Next
    End Sub

    Public Shared Function Load(Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile) As Regulators
        Dim LQuery = (From fsa In Data Let title = fsa.Title
                      Let Id = title.Split()(1)
                      Let Description = Mid(title, Len(Id) + 4)
                      Let regulog = Regex.Match(Description, "Regulog=[^]]+").Value.Replace("Regulog=", "")
                      Let TFBSs = Regex.Match(Description, "tfbs=[^]]+").Value.Replace("tfbs=", "").Split(CChar(";"))
                      Select New Regulator With {.AccessionId = Id, .Description = Description, .Regulog = regulog, .TFBSs = TFBSs}).ToArray
        Return New Regulators With {.Regulators = LQuery}
    End Function

    Public Sub Match(Data As LANS.SystemsBiology.DatabaseServices.Regtransbase.WebServices.RegPreciseTFFamily)
        For Each family In Data.Family
            For Each regulog In family.Regulogs.Logs
                Dim LQuery = (From regulator In Regulators Where String.Equals(regulator.Regulog, regulog.Regulog.Key) Select regulator).ToArray
                For i As Integer = 0 To LQuery.Count - 1
                    Dim obj = LQuery(i)
                    obj.BiologicalProcess = regulog.TFBSs.BiologicalProcess
                    obj.Effector = regulog.TFBSs.Effector
                    obj.Regulation = regulog.TFBSs.RegulationMode
                Next
            Next
        Next
    End Sub
End Class
