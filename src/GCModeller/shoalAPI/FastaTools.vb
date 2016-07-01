#Region "Microsoft.VisualBasic::596c19c3933b708ff0fa2a9803fb6497, ..\GCModeller\shoalAPI\FastaTools.vb"

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

Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment
Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.Extensions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.GCModeller.ShoalShell.Assembly.File.IO.Settings
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.Toolkits.RNA_Seq
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.BOW.DocumentFormat.Fastaq
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell

<[PackageNamespace]("Fasta.Tools", Publisher:="xie.guigang@gmail.com")>
Public Module FastaTools

    <ExportAPI("Export.Blastn.Mapping")>
    Public Function ExportMapping(blastn_mapping As String) As Boolean
        Dim Blastn = BlastPlus.Parser.TryParseUltraLarge(blastn_mapping)
        Dim Result = MapsAPI.Export(Blastn)
        Return Result.SaveTo(blastn_mapping & ".csv", False)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Dir"></param>
    ''' <param name="Parallel">当服务器的内存很小的时候不建议进行并行化，否则会占用掉很大的内存空间</param>
    ''' <returns></returns>
    <ExportAPI("Export.Blastn.Mapping.Batch")>
    Public Function ExportMappingBatch(Dir As String, Optional Parallel As Boolean = False) As Boolean
        Dim ScriptTemplate As String = $"Imports {GetType(FastaTools).NamespaceEntry.Namespace}" & vbCrLf &
                                        "Call Export.Blastn.Mapping %s"
        If Parallel Then
            Dim LQuery = (From path In Dir.LoadSourceEntryList({"*.txt"}).AsParallel
                          Let Script As String = ScriptTemplate.Replace("%s", path.Value)
                          Select FolkShoalThread(Script, path.Value)).ToArray
        Else
            Dim LQuery = (From path In Dir.LoadSourceEntryList({"*.txt"})
                          Let Script As String = ScriptTemplate.Replace("%s", path.Value)
                          Select FolkShoalThread(Script, path.Value)).ToArray
        End If

        Return True
    End Function

    <ExportAPI("Fq2Fasta")>
    Public Function Fq2Fasta(fq As String) As FASTA.FastaFile
        Dim Fastq = FastaqFile.Load(fq)
        Return Fastq.ToFasta
    End Function

    <ExportAPI("Fq2Fasta.Batch")>
    Public Function Fq2FastaBatch(<Parameter("Dir.Fq")> FqDir As String) As Boolean

        Dim SHOAL_SCRIPT As String = $"Imports {GetType(FastaTools).NamespaceEntry.Namespace}" & vbCrLf &
                                      "      fasta <- Fq2Fasta %s" & vbCrLf &
                                      "Call $fasta -> Write.Fasta path.saved %d Line.Break -1"

        Dim LQuery = (From pathEntry In FqDir.LoadSourceEntryList({"*.fq", "*.fastq", "*.fastaq"}).AsParallel
                      Let Saved = $"{FqDir}/{pathEntry.Key}.fasta".GetFullPath
                      Select Script = SHOAL_SCRIPT.Replace("%s", pathEntry.Value.CliPath).Replace("%d", Saved.CliPath), pathEntry).ToArray
        Call $"Data fastq loading job done....".__DEBUG_ECHO

        Dim ConvertAndSaved = (From fqScript
                                   In LQuery.AsParallel
                               Select FolkShoalThread(fqScript.Script, fqScript.pathEntry.Value)).ToArray
        Call __DEBUG_ECHO($"Saved data at {FqDir}")

        Return True
    End Function

    <ExportAPI("Read.Fasta")>
    Public Function LoadFasta(<Parameter("Fasta.Path")> Path As String) As FASTA.FastaFile
        Return FASTA.FastaFile.Read(Path)
    End Function

    <ExportAPI("Read.FastaToken")>
    Public Function LoadFastaToken(File As String) As FASTA.FastaToken
        Return FASTA.FastaToken.Load(File)
    End Function

    <ExportAPI("Length.Statics")>
    Public Function LengthStatics(Fasta As FASTA.FastaFile) As DocumentStream.File
        Dim LQuery = (From FastaToken In Fasta.AsParallel Select FastaToken.Length Group By Length Into Group).ToArray
        Dim Csv As New DocumentStream.File

        Call Csv.Add({"Length", "NumberOfmRNAs"})

        For Each dist In (From token In LQuery Select token Order By token.Length Ascending).ToArray

            Call Csv.Add({CStr(dist.Length), CStr(dist.Group.Count)})

        Next

        Return Csv
    End Function

    <ExportAPI("Write.Fasta")>
    Public Function WriteFasta(Token As FASTA.FastaToken,
                               <Parameter("Path.Saved")> SaveTo As String,
                               <Parameter("Line.Break",
                                          "The sequence residue width of the fasta token, if this parameter value is greater than 0, then the sequence will break into several lines by this length, value zero or negative value for no line break, each fasta sequence is in one line.")>
                               Optional LineBreak As Integer = 60) As Boolean
        Return Token.SaveTo(LineBreak:=LineBreak, Path:=SaveTo)
    End Function

    <ExportAPI("Write.Fasta")>
    Public Function WriteFasta(Fasta As FASTA.FastaFile,
                               <Parameter("Path.Saved")> SaveTo As String,
                               <Parameter("Line.Break",
                                          "The sequence residue width of the fasta token, if this parameter value is greater than 0, then the sequence will break into several lines by this length, value zero or negative value for no line break, each fasta sequence is in one line.")>
                               Optional LineBreak As Integer = 60) As Boolean
        Return Fasta.Save(LineBreak:=LineBreak, Path:=SaveTo, encoding:=Encodings.ASCII)
    End Function

    <ExportAPI("New.Reader")>
    Public Function ConstructSegmentReader(Sequence As FASTA.FastaToken) As NucleotideModels.SegmentReader
        Return New NucleotideModels.SegmentReader(Sequence)
    End Function

    <ExportAPI("Get.Segment")>
    Public Function GetSegment(Reader As NucleotideModels.SegmentReader,
                               Left As Integer,
                               Right As Integer,
                               Optional Complement As Boolean = False) As FASTA.FastaToken
        If Complement Then
            Return New SequenceModel.FASTA.FastaToken With {
                .SequenceData = Reader.ReadComplement(Left, Right - Left, True),
                .Attributes = {"Left=" & Left, "Right=" & Right, "Complement=" & Complement}
            }
        Else
            Return New SequenceModel.FASTA.FastaToken With {
                .SequenceData = Reader.GetSegmentSequence(Left, Right),
                .Attributes = {"Left=" & Left, "Right=" & Right, "Complement=" & Complement}
            }
        End If
    End Function

    <ExportAPI("New.Loci")>
    Public Function NewLoci(Left As Integer, Right As Integer, Optional Complement As Boolean = False) As NucleotideLocation
        Return New NucleotideLocation(Left, Right, Complement)
    End Function

    <ExportAPI("Get.Segment")>
    Public Function TryParseGetSegment(Reader As NucleotideModels.SegmentReader,
                                       Start As Integer,
                                       Length As Integer,
                                       Optional DownStream As Boolean = True) As FASTA.FastaToken
        Return New FASTA.FastaToken With {
            .SequenceData = Reader.TryParse(Start, Length, DownStream),
            .Attributes = {"Left=" & Start, "Length=" & Length, "DirectionDownStream=" & DownStream}
        }
    End Function

    <ExportAPI("Get.Segment")>
    Public Function TryParseGetSegment2(Reader As NucleotideModels.SegmentReader, loci As NucleotideLocation) As FASTA.FastaToken
        Return New FASTA.FastaToken With {
            .SequenceData = Reader.TryParse(loci).SequenceData,
            .Attributes = {loci.ToString}
        }
    End Function

    <ExportAPI("Genbank2ProFasta")>
    Public Function Genbank2ProFasta(gb As GBFF.File, Optional OnlyLocusTag As Boolean = False) As FASTA.FastaFile
        Return ExportProteins_Short(gb, OnlyLocusTag)
    End Function
End Module

