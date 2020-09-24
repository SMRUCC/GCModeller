#Region "Microsoft.VisualBasic::b254466bd4c4504d3ce29382d304fc96, engine\IO\GCTabular\Compiler\RibosomeAssembly.vb"

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

    '     Class RibosomeAssembly
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: AssemblyLSU, AssemblySSU
    ' 
    '         Sub: __Internal____addFlag, AssembleRNAPolymerase, Invoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite

Namespace Compiler.Components

    Public Class RibosomeAssembly

        Dim Ptt As PTT
        Dim Rnt As PTT
        Dim ModelIO As FileStream.IO.XmlresxLoader

        Sub New(Ptt As String, Rnt As String, Model As FileStream.IO.XmlresxLoader)
            ModelIO = Model
            Me.Ptt = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(Ptt)
            Me.Rnt = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(Rnt)
            ModelIO.ExpressionKinetics = New List(Of FileStream.ExpressionKinetics)
        End Sub

        ''' <summary>
        ''' ab'b + sigma
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub AssembleRNAPolymerase()
            Dim Alpha = (From item In Ptt.GeneObjects Where String.Equals(item.Product, "RNA polymerase alpha subunit") Select item).First
            Dim Beta = (From item In Ptt.GeneObjects Where String.Equals(item.Product, "RNA polymerase beta subunit") Select item).First
            Dim Beta1 = (From item In Ptt.GeneObjects Where String.Equals(item.Product, "RNA polymerase beta' subunit") Select item).First
            Dim Sigma = (From item In Ptt.GeneObjects Where Regex.Match(item.Product, "RNA polymerase sigma-\d+", RegexOptions.IgnoreCase).Success Select item).ToArray

            Dim CoreEnzyme = New FileStream.ProteinAssembly With {.Lambda = 0.9, .ProteinComplexes = "RPE-CORE", .Upper_Bound = 100, .p_Dynamics_K = 1, .ProteinComponents = New String() {Alpha.Gene, Beta.Gene, Beta1.Gene}}
            Dim LQuery = (From item In Sigma Select New FileStream.ProteinAssembly With {
                             .Lambda = 0.5, .ProteinComplexes = String.Format("HOLOENZYME-{0}", Regex.Match(item.Product, "\d+").Value), .Upper_Bound = 100, .p_Dynamics_K = 1, .Comments = item.Gene,
                             .ProteinComponents = New String() {"RPE-CORE", item.Gene}}).ToArray
            ModelIO.RNAPolymerase = New List(Of FileStream.ProteinAssembly)
            Call ModelIO.RNAPolymerase.Add(CoreEnzyme)
            Call ModelIO.RNAPolymerase.AddRange(LQuery)

            Dim Metabolite As New FileStream.Metabolite With {
                .Identifier = CoreEnzyme.ProteinComplexes,
                .InitialAmount = 1000,
                .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes
            }

            Call ModelIO.MetabolitesModel.InsertOrUpdate(Metabolite) ',
            '     .DBLinks = MetaCyc.Schema.DBLinkManager.CreateObject({New KeyValuePair(Of String, String)("RNA-Polymerase", CoreEnzyme.ProteinComplexes)}).DBLinks})
            Call ModelIO.MetabolitesModel.AddRange((From item In LQuery
                                                    Select New FileStream.Metabolite With {
                                                        .Identifier = item.ProteinComplexes, .InitialAmount = 1000, .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes})) ',
            '   .DBLinks = MetaCyc.Schema.DBLinkManager.CreateObject({New KeyValuePair(Of String, String)("RNA-Polymerase", item.ProteinComplexes)}).DBLinks}).ToArray)

            Dim RNAPolymeraseList = (From Enzyme In LQuery Select Enzyme.ProteinComplexes Distinct).ToArray

            Call ModelIO.ExpressionKinetics.AddRange((From strId As String In RNAPolymeraseList Select New FileStream.ExpressionKinetics With {.ProteinComplex = strId, .Temperature = 28, .pH = 7, .Km = 0.5}))

            Dim sBuilder As StringBuilder = New StringBuilder(128)
            For Each itemId In RNAPolymeraseList
                Call sBuilder.Append(itemId & "; ")
            Next
            Call sBuilder.Remove(sBuilder.Length - 2, 2)
            ModelIO.SystemVariables.GetItem(SystemVariables.ID_RNA_POLYMERASE_PROTEIN).Value = sBuilder.ToString
        End Sub

        ''' <summary>
        ''' LSU + SSU -> Ribosome
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Invoke()
            Dim RibosomeAssembly = New FileStream.ProteinAssembly With {.Lambda = 0.7, .p_Dynamics_K = 1, .ProteinComplexes = "RIBOSOMAL-PROTEIN-COMPLEXE", .ProteinComponents = New String() {"SSU", "LSU"}, .Upper_Bound = 1000}
            ModelIO.RibosomeAssembly = New List(Of FileStream.ProteinAssembly)
            Call ModelIO.RibosomeAssembly.Add(RibosomeAssembly)
            Call ModelIO.RibosomeAssembly.AddRange(AssemblyLSU)
            Call ModelIO.RibosomeAssembly.AddRange(AssemblySSU)
            Call ModelIO.MetabolitesModel.InsertOrUpdate(New FileStream.Metabolite With {
                                                             .Identifier = RibosomeAssembly.ProteinComplexes,
                                                             .InitialAmount = 1000,
                                                             .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes}) ',
            '  .DBLinks = MetaCyc.Schema.DBLinkManager.CreateObject({New KeyValuePair(Of String, String)(key:="RibosomeAssembly",
            '     value:=RibosomeAssembly.ProteinComplexes)}).DBLinks})

            '创建tRNA基因与代谢物的产物连接
            Dim tRNAs = (From item In Rnt.GeneObjects Where InStr(item.Product, " tRNA") > 0 Select item).ToArray
            For Each tRNA As GeneBrief In tRNAs
                Dim tRNAGene = (From item In ModelIO.Transcripts Where String.Equals(item.Template, tRNA.Gene) Select item).ToArray
                If tRNAGene.IsNullOrEmpty Then
                    Call Console.WriteLine("Could not found the tRNA gene for ""{0}""", tRNA.Product)
                Else
                    Dim ProductId As String = tRNA.Product.ToUpper.Replace(" ", "-") & "S"
                    Dim tRNADataModel = tRNAGene.First
                    tRNADataModel.UniqueId = ProductId
                    tRNADataModel.TranscriptType = GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript.TranscriptTypes.tRNA

                    Call Console.WriteLine("Link() {0}({1}) <----> {2}", tRNA.Gene, tRNA.Product, ProductId)
                    Dim tRNAGeneTemplate = (From item In ModelIO.GenomeAnnotiation Where String.Equals(item.Identifier, tRNA.Gene) Select item).ToArray
                    If tRNAGeneTemplate.IsNullOrEmpty Then
                        Call Console.WriteLine("!!!! tRNA is empty {0}", tRNA.Gene)
                    End If
                    tRNAGeneTemplate.First.TranscriptProduct = ProductId
                End If
            Next

            ModelIO.SystemVariables.GetItem(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_RIBOSOME_ASSEMBLY_COMPLEXES).Value = RibosomeAssembly.ProteinComplexes
            ModelIO.SystemVariables.GetItem(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.PARA_TRANSCRIPTION).Value = 1
            ModelIO.SystemVariables.GetItem(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.PARA_TRANSLATION).Value = 1

            Call ModelIO.ExpressionKinetics.Add(New FileStream.ExpressionKinetics With {.Km = 0.2, .pH = 7, .Temperature = 28, .ProteinComplex = RibosomeAssembly.ProteinComplexes})
            Call AssembleRNAPolymerase()

            For Each RNAGene In Rnt.GeneObjects
                If String.IsNullOrEmpty(RNAGene.Product) Then
                    Continue For
                End If

                Dim RNAMetabolite = ModelIO.MetabolitesModel(RNAGene.Gene)

                If Not RNAMetabolite Is Nothing Then
                    Call ModelIO.MetabolitesModel.Remove(RNAMetabolite)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 16S rRNA + RP -> SSU
        ''' </summary>
        ''' <remarks></remarks>
        Public Function AssemblySSU() As FileStream.ProteinAssembly()
            Dim _16SrRNA = (From item In Rnt.GeneObjects Where String.Equals(item.Product, "16S ribosomal RNA") Select item.Gene).ToArray
            Dim RP = (From item In Ptt.GeneObjects Where InStr(item.Product, "30S ribosomal protein") = 1 Select item.Gene).ToArray
            Dim LQuery = (From strId As String
                          In _16SrRNA
                          Let get_Components = {RP, New String() {strId & "_16SrRNA"}}.ToVector
                          Select New FileStream.ProteinAssembly With {
                                     .Lambda = 0.8, .p_Dynamics_K = 1,
                                     .ProteinComplexes = "SSU",
                                     .Upper_Bound = 1000, .ProteinComponents = get_Components,
                                     .Comments = strId}).ToArray

            Call __Internal____addFlag(_16SrRNA, "_16SrRNA")
            Dim Metabolite = New FileStream.Metabolite With
                             {
                                 .Identifier = "SSU", .InitialAmount = 1000,
                                 .MetaboliteType = MetaboliteTypes.ProteinComplexes}
            Call ModelIO.MetabolitesModel.InsertOrUpdate(Metabolite) ',
            '.DBLinks = MetaCyc.Schema.DBLinkManager.CreateObject({New KeyValuePair(Of String, String)(key:="RibosomeAssembly",
            '                                                                                           value:="SSU")}).DBLinks})
            Return LQuery
        End Function

        Private Sub __Internal____addFlag(IdCollection As String(), Flag As String)
            For Each rRNAId As String In IdCollection
                Dim rRNALQuery = (From item In ModelIO.Transcripts Where String.Equals(item.Template, rRNAId) Select item).ToArray
                Dim rRNA = rRNALQuery.First
                rRNA.UniqueId = rRNAId & Flag
                rRNA.TranscriptType = GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript.TranscriptTypes.rRNA
            Next
        End Sub

        ''' <summary>
        ''' 5S rRNA + 23S rRNA + RP -> LSU
        ''' </summary>
        ''' <remarks></remarks>
        Public Function AssemblyLSU() As FileStream.ProteinAssembly()
            Dim _5SrRNA = (From item In Rnt.GeneObjects Where String.Equals(item.Product, "5S ribosomal RNA") Select item.Gene).ToArray
            Dim _23SrRNA = (From item In Rnt.GeneObjects Where String.Equals(item.Product, "23S ribosomal RNA") Select item.Gene).ToArray
            Dim RP = (From item In Ptt.GeneObjects Where InStr(item.Product, "50S ribosomal protein") = 1 Select item.Gene).ToArray
            Dim LSU = New List(Of FileStream.ProteinAssembly)

            For Each item In _5SrRNA
                Call LSU.AddRange((From Id As String In _23SrRNA
                                   Let get_Components = {RP, New String() {item & "_5SrRNA", Id & "_23SrRNA"}}.ToVector
                                   Select New FileStream.ProteinAssembly With
                                          {
                                              .Lambda = 0.8, .p_Dynamics_K = 1, .ProteinComplexes = "LSU",
                                              .Upper_Bound = 1000,
                                              .ProteinComponents = get_Components,
                                              .Comments = String.Format("{0}!{1}", item, Id)}).ToArray)
            Next

            Dim Metabolite = New FileStream.Metabolite With
                             {
                                 .Identifier = "LSU",
                                 .InitialAmount = 1000,
                                 .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes}

            Call __Internal____addFlag(_5SrRNA, "_5SrRNA")
            Call __Internal____addFlag(_23SrRNA, "_23SrRNA")
            Call ModelIO.MetabolitesModel.InsertOrUpdate(Metabolite) ',
            '.DBLinks = MetaCyc.Schema.DBLinkManager.CreateObject({New KeyValuePair(Of String, String)(key:="RibosomeAssembly",
            '                                                                                           value:="LSU")}).DBLinks})
            Return LSU.ToArray
        End Function
    End Class
End Namespace
