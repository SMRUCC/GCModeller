#Region "Microsoft.VisualBasic::12f792c189834d17430134acc304a13c, ..\GCModeller\engine\GCTabular\Compiler\SignalTransductionNetwork\SignalTransductionNetwork.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Logging
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite
Imports SMRUCC.genomics.Model.Network.STRING
Imports SMRUCC.genomics.Model.Network.STRING.Models.Pathway

Namespace Compiler.Components

    Public Class SignalTransductionNetwork

        Dim ModelIO As FileStream.IO.XmlresxLoader

        ''' <summary>
        ''' 蛋白质的标识号
        ''' </summary>
        ''' <remarks></remarks>
        Dim CheB As String()
        Dim CheR As String()
        Dim CheW As String()
        Dim MCPs As String()

        ''' <summary>
        ''' PF01339
        ''' </summary>
        ''' <remarks></remarks>
        Const PFAM_CHEB = "CheB"
        ''' <summary>
        ''' PF01739, PF03705
        ''' </summary>
        ''' <remarks></remarks>
        Const PFAM_CHER = "CheR"
        Const PFAM_CHEW = "CheW"

        Dim StringNetwork As SimpleCsv.Network
        Dim _Logging As LogFile
        ''' <summary>
        ''' 以KEGG编号为主键的代谢物字典
        ''' </summary>
        ''' <remarks></remarks>
        Dim KEGG_Compounds As Dictionary(Of String, FileStream.Metabolite)

        Sub New(ModelIo As FileStream.IO.XmlresxLoader, StringNetwork As SimpleCsv.Network, Logging As LogFile)
            Me.StringNetwork = StringNetwork
            Me.ModelIO = ModelIo
            Dim MisT2 As SMRUCC.genomics.Assembly.MiST2.MiST2 = ModelIo.MisT2
            Me.CheB = (From Protein In MisT2.MajorModules.First.Chemotaxis Where InStr(Protein.ImageUrl, PFAM_CHEB) > 0 Select Protein.Identifier Distinct).ToArray
            Me.CheR = (From Protein In MisT2.MajorModules.First.Chemotaxis Where InStr(Protein.ImageUrl, PFAM_CHER) > 0 Select Protein.Identifier Distinct).ToArray
            Me.CheW = (From Protein In MisT2.MajorModules.First.Chemotaxis Where InStr(Protein.ImageUrl, PFAM_CHEW) > 0 Select Protein.Identifier Distinct).ToArray
            Me.MCPs = (From Protein In MisT2.MajorModules.First.Chemotaxis Where String.Equals(Protein.Class, "MCP") Select Protein.Identifier Distinct).ToArray
            Me._Logging = Logging

            KEGG_Compounds = (From item In ModelIo.MetabolitesModel.AsParallel Where Not String.IsNullOrEmpty(item.Value.KEGGCompound) Select item.Value).ToArray.ToDictionary(Function(item) item.KEGGCompound)
        End Sub

        Public Sub Invoke(TranscriptRegulation As IEnumerable(Of FileStream.TranscriptUnit), Door As DOOR.DOOR, CrossTalks As IO.File)
            Call _Logging.WriteLine("Start to compile the signal transduction network...")

            Call _Logging.WriteLine("Compiling CheBMethylesterase reactions...")
            ModelIO.CheBMethylesterase = _compile_CheBMethylesterase()
            Call _Logging.WriteLine("Compiling CheRMethyltransferase reactions...")
            ModelIO.CheRMethyltransferase = _compile_CheRMethyltransferase()
            Call _Logging.WriteLine("Compiling CheBPhosphate reactions...")
            ModelIO.CheBPhosphate = _compile_CheBPhosphate()
            Call _Logging.WriteLine("Compiling ChemotaxisSensing profiles...")
            ModelIO.ChemotaxisSensing = _compile_ChemotaxisSensing()
            Call _Logging.WriteLine("Compiling HkAutoPhosphorus profiles...")
            ModelIO.HkAutoPhosphorus = _compile_HkAutoPhosphorus()
            ModelIO.CrossTalk = _compile_CrossTalks(Door, CrossTalks)
            ModelIO.OCSSensing = _compile_OCS_RULE()

            Call Add_autoGenerateProteinComplexSubstrate()

            Call _Logging.WriteLine("Signal transduction network compiling completed job done!")
        End Sub

        Private Sub Add_autoGenerateProteinComplexSubstrate()
            Dim SubstrateList As List(Of String) = New List(Of String)

            For Each Line As String() In (From item In ModelIO.CheBMethylesterase Select item.get_Metabolites).ToArray
                Call SubstrateList.AddRange(Line)
            Next
            For Each Line As String() In (From item In ModelIO.CheRMethyltransferase Select item.get_Metabolites).ToArray
                Call SubstrateList.AddRange(Line)
            Next
            For Each Line As String() In (From item In ModelIO.CheBPhosphate Select item.get_Metabolites).ToArray
                Call SubstrateList.AddRange(Line)
            Next
            For Each Line As String() In (From item In ModelIO.CrossTalk Select item.get_Metabolites).ToArray
                Call SubstrateList.AddRange(Line)
            Next
            For Each Line As String() In (From item In ModelIO.OCSSensing Select item.get_Metabolites).ToArray
                Call SubstrateList.AddRange(Line)
            Next
            SubstrateList = (From strValue As String In SubstrateList Select strValue Distinct Order By strValue Ascending).AsList

            For Each strMetaboliteId As String In SubstrateList

                If Not ModelIO.MetabolitesModel.ContainsKey(strMetaboliteId) Then
                    Dim DBLink As String = New MetaCyc.Schema.DBLinkManager.DBLink() With {
                        .DBName = "STrP.AUTOGenerated_ProteinComplex",
                        .AccessionId = strMetaboliteId
                    }.GetFormatValue
                    Dim Metabolite As FileStream.Metabolite = New FileStream.Metabolite With {
                        .Identifier = strMetaboliteId,
                        .InitialAmount = 1000,
                        .MetaboliteType = MetaboliteTypes.ProteinComplexes} ', .DBLinks = New String() {DBLink}}
                    Call ModelIO.MetabolitesModel.InsertOrUpdate(Metabolite)
                End If
            Next

            Call _Logging.WriteLine(String.Format("Added {0} auto generated metabolites into model file!", SubstrateList.Count))
        End Sub

        Private Function Internal_get_MappedID(MetaCycId As String) As String
            Dim LQuery = (From item In Me.ModelIO.EffectorMapping Where String.Equals(item.MetaCycId, MetaCycId) AndAlso Not String.IsNullOrEmpty(item.KEGGCompound) Select item).ToArray
            If LQuery.IsNullOrEmpty Then
                Return MetaCycId
            Else
                Dim Id As String = LQuery.First.KEGGCompound
                If KEGG_Compounds.ContainsKey(Id) Then
                    Return KEGG_Compounds(Id).Identifier
                Else
                    Return MetaCycId
                End If
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>效应物的编号在左端的第二个，在这里是将MetaCyc和KEGG的代谢物进行合并的，首先会查找出MetaCyc的编号，然后在Mapping之中查找，假若存在KEGGcompound，则使用UniqueId，否则只是用MetaCycID</remarks>
        Private Function _compile_OCS_RULE() As FileStream.MetabolismFlux()
            Dim OCS = (From Protein In ModelIO.STrPModel.Pathway
                       Where Protein.TF_MiST2Type = TFSignalTypes.OneComponentType
                       Select Protein).ToArray  '选取出OCS对象
            Dim ChunkList As List(Of FileStream.MetabolismFlux) = New List(Of FileStream.MetabolismFlux)

            For Each OCS_strId In OCS

                If Not OCS_strId.Effectors.IsNullOrEmpty Then
                    Dim Data = (From EffectorId As String In OCS_strId.Effectors Let E_ID As String = Internal_get_MappedID(EffectorId) Select ProteinComplexAssemble(OCS_strId.TF, E_ID, 1)).ToArray  '对该OCS对象的激活生成反应过程
                    Call ChunkList.AddRange(Data)

                    '下面需要将Motifs里面的每一个编号为OCSstrID的Regulator替换为蛋白质复合物
                    Dim LQuery = (From Regulation In ModelIO.Motifs Where Regulation.Regulators.IndexOf(OCS_strId.TF) > -1 Select Regulation).ToArray '选取motifs
                    Dim TF_ID As String = OCS_strId.TF

                    For Each item In Data    '替换motif之中的相应编号
                        For Each Motif In LQuery
                            Call Motif.Regulators.Remove(TF_ID)
                            Call Motif.Regulators.Add(item._Internal_compilerRight.First.Value)
                        Next
                    Next
                    '还需要根据motif的id和regulator的id在regulator之中查找相应的regulator，进行替换，以保持数据的一致性

                    Dim motifs As String() = (From item In LQuery Select item.Internal_GUID Distinct).ToArray
                    Dim Regulations = (From item As FileStream.Regulator
                                       In ModelIO.Regulators
                                       Where String.Equals(TF_ID, item.ProteinId) AndAlso Array.IndexOf(motifs, item.RegulatesMotif) > -1
                                       Select item).AsList

                    For Each regulator In Regulations
                        regulator.Effectors = (From item In Data Select item._Internal_compilerLeft(1).Value).ToArray
                    Next
                End If
            Next

            Return ChunkList.ToArray
        End Function

        Private Function _compile_CrossTalks(Door As DOOR.DOOR, CrossTalksProfile As IO.File) As FileStream.MetabolismFlux()

            Const PI = "PI"

            Dim Profile = CrossTalksAnalysis.Analysis(Me.ModelIO, CrossTalksProfile)
            Dim ChunkTemp As FileStream.MetabolismFlux() = (From item In Profile Select PhosphoTransfer_(item.Kinase, item.Regulator, item.Probability, PI)).ToArray

            Dim Index = (From item In ChunkTemp Select item.Identifier Distinct).ToArray
            ChunkTemp = (From Id As String In Index Select ChunkTemp.GetItem(Id)).ToArray

            Me.ModelIO.CrossTalksAnnotation = Profile

            Return ChunkTemp
        End Function

        Private Function _compile_HkAutoPhosphorus() As List(Of TCS.SensorInducers)
            Dim HK As List(Of String) = New List(Of String)

            For Each STrP In ModelIO.STrPModel.Pathway
                Call HK.AddRange((From item In STrP.TCSSystem Let Id As String = item.HK Select Id).ToArray)
            Next

            HK = (From strValue As String In HK Select strValue Distinct Order By strValue Ascending).AsList

            Dim TempChunk As New List(Of TCS.SensorInducers)
            For Each HKId As String In HK
                Dim LQuery = (From Mcp In Me.MCPs Let Confidence As Double = StringNetwork.GetConfidence(HKId, Mcp) + 0.05
                              Select String.Format("{0}={1}", Mcp, Confidence)).ToArray
                If LQuery.IsNullOrEmpty Then
                    Continue For
                End If

                Call TempChunk.Add(New TCS.SensorInducers With {.SensorId = HKId, .Inducers = LQuery})
            Next

            Return TempChunk
        End Function

        Private Function _compile_ChemotaxisSensing() As List(Of TCS.SensorInducers)
            Dim LQuery = (From Item As SMRUCC.genomics.Assembly.MiST2.Transducin
                                 In ModelIO.MisT2.MajorModules.First.Chemotaxis
                          Where String.Equals(Item.Class, "MCP")
                          Select New TCS.SensorInducers With {
                                     .SensorId = String.Format("[{0}][CH3]", Item.Identifier),
                                     .Inducers = New String() {}}).AsList
            Return LQuery
        End Function

        Private Function _compile_CheBPhosphate() As FileStream.MetabolismFlux()
            Dim TempChunk As List(Of FileStream.MetabolismFlux) = New List(Of FileStream.MetabolismFlux)

            For Each CheB As String In Me.CheB
                Dim CheBPI As String = String.Format("[{0}][PI]", CheB)

                Dim LQuery = (From HK In Me.ModelIO.MisT2.MajorModules.First.TwoComponent.HisK
                              Let Confidence As Double = StringNetwork.GetConfidence(CheB, HK.Identifier) + 0.05
                              Select New With {.HK = HK.Identifier, .Conf = Confidence}).AsList
                Call LQuery.AddRange((From HK In Me.ModelIO.MisT2.MajorModules.First.TwoComponent.HHK
                                      Let Confidence As Double = StringNetwork.GetConfidence(CheB, HK.Identifier) + 0.05
                                      Select New With {.HK = HK.Identifier, .Conf = Confidence}).AsList)
                Call LQuery.AddRange((From HK In Me.ModelIO.MisT2.MajorModules.First.TwoComponent.HRR
                                      Let Confidence As Double = StringNetwork.GetConfidence(CheB, HK.Identifier) + 0.05
                                      Select New With {.HK = HK.Identifier, .Conf = Confidence}).AsList)
                If LQuery.IsNullOrEmpty Then
                    Continue For
                End If

                Call TempChunk.AddRange((From HK In LQuery
                                         Let UniqueId As String = String.Format("{0}->{1}", HK.HK, CheB)
                                         Let DBLink As String() = New String() {New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "CheBMethylesterase", .AccessionId = UniqueId}.GetFormatValue}
                                         Let Equation As String = SMRUCC.genomics.ComponentModel.EquaionModel.EquationBuilder.ToString(
                                                  New KeyValuePair(Of Double, String)() {New KeyValuePair(Of Double, String)(1, CheB), New KeyValuePair(Of Double, String)(1, String.Format("[{0}][PI]", HK.HK))},
                                                  New KeyValuePair(Of Double, String)() {New KeyValuePair(Of Double, String)(1, HK.HK), New KeyValuePair(Of Double, String)(1, CheBPI)}, False)
                                         Select New FileStream.MetabolismFlux With {
                                             .UPPER_Bound = 100, .Identifier = UniqueId, .p_Dynamics_K_1 = HK.Conf, .Enzymes = New String() {},
                                             .Equation = Equation}).ToArray) ', .DBLinks = DBLink
            Next

            Return TempChunk.ToArray
        End Function

        Private Function _compile_CheRMethyltransferase() As FileStream.MetabolismFlux()
            Dim TempChunk As List(Of FileStream.MetabolismFlux) = New List(Of FileStream.MetabolismFlux)

            For Each Mcp In Me.MCPs
                Dim Mcp_Ch3 As String = String.Format("[{0}][CH3]", Mcp)

                Dim LQuery = (From CheR In Me.CheR Let Confidence As Double = StringNetwork.GetConfidence(CheR, Mcp) Where Confidence > 0.0R
                              Select New With {.CheR = CheR, .Conf = Confidence}).ToArray
                If LQuery.IsNullOrEmpty Then
                    Continue For
                End If

                Dim Equation As String = SMRUCC.genomics.ComponentModel.EquaionModel.EquationBuilder.ToString(
                                             New KeyValuePair(Of Double, String)() {New KeyValuePair(Of Double, String)(1, Mcp), New KeyValuePair(Of Double, String)(1, "S-ADENOSYLMETHIONINE")},
                                             New KeyValuePair(Of Double, String)() {New KeyValuePair(Of Double, String)(1, Mcp_Ch3), New KeyValuePair(Of Double, String)(1, "ADENOSYL-HOMO-CYS")}, False)

                Call TempChunk.AddRange((From CheBPI In LQuery
                                         Let UniqueId As String = String.Format("{0}->{1}", CheBPI.CheR, Mcp)
                                         Let DBLink As String() = New String() {New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "CheBMethylesterase", .AccessionId = UniqueId}.GetFormatValue}
                                         Select New FileStream.MetabolismFlux With {
                                             .UPPER_Bound = 100, .Identifier = UniqueId, .p_Dynamics_K_1 = CheBPI.Conf, .Enzymes = New String() {CheBPI.CheR},
                                             .Equation = Equation}).ToArray) ', .DBLinks = DBLink
            Next

            Return TempChunk.ToArray
        End Function

        Private Function _compile_CheBMethylesterase() As FileStream.MetabolismFlux()
            Dim TempChunk As List(Of FileStream.MetabolismFlux) = New List(Of FileStream.MetabolismFlux)

            For Each Mcp In Me.MCPs
                Dim Mcp_Ch3 As String = String.Format("[{0}][CH3]", Mcp)

                If Not ModelIO.MetabolitesModel.ContainsKey(Mcp_Ch3) Then
                    Dim Metabolite = New FileStream.Metabolite With
                                     {
                                         .Identifier = Mcp_Ch3,
                                         .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes,
                                         .InitialAmount = 1000}
                    Call ModelIO.MetabolitesModel.InsertOrUpdate(Metabolite) ',
                    ' .DBLinks = New String() {New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "MisT2", .AccessionId = Mcp_Ch3}.GetFormatValue}})
                End If

                Dim LQuery = (From CheB As String In Me.CheB
                              Let Confidence As Double = StringNetwork.GetConfidence(CheB, Mcp)
                              Where Confidence > 0.0R
                              Select CheB = String.Format("[{0}][PI]", CheB), Conf = Confidence).ToArray

                If LQuery.IsNullOrEmpty Then
                    Continue For
                End If

                Dim Equation As String = SMRUCC.genomics.ComponentModel.EquaionModel.EquationBuilder.ToString(
                                             New KeyValuePair(Of Double, String)() {New KeyValuePair(Of Double, String)(1, Mcp_Ch3), New KeyValuePair(Of Double, String)(1, "WATER")},
                                             New KeyValuePair(Of Double, String)() {New KeyValuePair(Of Double, String)(1, Mcp), New KeyValuePair(Of Double, String)(1, "METOH")}, False)

                Call TempChunk.AddRange((From CheBPI In LQuery
                                         Let UniqueId As String = String.Format("{0}->{1}", CheBPI.CheB, Mcp)
                                         Let DBLink As String() = New String() {New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "CheBMethylesterase", .AccessionId = UniqueId}.GetFormatValue}
                                         Select New FileStream.MetabolismFlux With {
                                             .UPPER_Bound = 100, .Identifier = UniqueId, .p_Dynamics_K_1 = CheBPI.Conf, .Enzymes = New String() {CheBPI.CheB},
                                             .Equation = Equation}).ToArray) ', .DBLinks = DBLink
            Next

            Return TempChunk.ToArray
        End Function
    End Class
End Namespace
