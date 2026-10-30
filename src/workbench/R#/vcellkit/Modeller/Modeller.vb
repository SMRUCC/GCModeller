#Region "Microsoft.VisualBasic::c3fa0d9f96a782f218e630f908d227c8, engine\vcellkit\Modeller\Modeller.vb"

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


    ' Code Statistics:

    '   Total Lines: 184
    '    Code Lines: 115 (62.50%)
    ' Comment Lines: 49 (26.63%)
    '    - Xml Docs: 93.88%
    ' 
    '   Blank Lines: 20 (10.87%)
    '     File Size: 7.42 KB


    ' Module vcellModeller
    ' 
    '     Function: applyKinetics, CompileLambda, eval, evalArgumentValues, Kinetics
    '               LoadVirtualCell, readJSON, taxonomy_info, writeJSON, WriteZipAssembly
    ' 
    '     Sub: createKineticsDbCache
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data.SABIORK.docuRESTfulWeb
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.Compiler
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RExpr = SMRUCC.Rsharp.Interpreter.ExecuteEngine.Expression

''' <summary>
''' virtual cell network kinetics modeller
''' </summary>
<Package("modeller", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Public Module vcellModeller

    ' ((kcat * E) * S) / (Km + S)
    ' (Vmax * S) / (Km + S)

    ''' <summary>
    ''' apply the kinetics parameters from the sabio-rk database.
    ''' </summary>
    ''' <param name="vcell"></param>
    ''' <returns></returns>
    <ExportAPI("apply.kinetics")>
    Public Function applyKinetics(vcell As VirtualCell, Optional cache$ = "./.cache") As VirtualCell
        Return New Modeller(vcell, cache).Compile()
    End Function

    ''' <summary>
    ''' create data repository from the sabio-rk database
    ''' </summary>
    ''' <param name="export$"></param>
    <ExportAPI("cacheOf.enzyme_kinetics")>
    Public Sub createKineticsDbCache(Optional export$ = "./", Optional ko01000 As String = "ko01000")
        Call htext.GetInternalResource(ko01000) _
            .QueryByECNumbers(export) _
            .ToArray
    End Sub

    ''' <summary>
    ''' read the virtual cell model file
    ''' </summary>
    ''' <param name="path">
    ''' the model file extension could be:
    ''' 
    ''' xml - small virtual cell model in a xml file
    ''' zip - large virtual cell model file save as multiple components in a zip file
    ''' json - large virtual cell model file save as json stream file
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.vcell")>
    Public Function LoadVirtualCell(path As String) As VirtualCell
        Select Case path.ExtensionSuffix
            Case "zip" : Return ZipAssembly.CreateVirtualCellXml(path)
            Case "xml" : Return path.LoadXml(Of VirtualCell)(stripInvalidsCharacter:=True)
            Case "json" : Return vcellModeller.readJSON(path)

            Case Else
                Throw New InvalidProgramException($"Unknown file format with extension suffix name: {path.ExtensionSuffix}!")
        End Select
    End Function

    ''' <summary>
    ''' get taxonomy information of the virtual cell model, this information will be used for identify the model source, and also for the model annotation in the future.
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns></returns>
    <ExportAPI("taxonomy_info")>
    Public Function taxonomy_info(model As VirtualCell) As Taxonomy
        Return model.taxonomy
    End Function

    ''' <summary>
    ''' save the virtual cell model as a large json file
    ''' </summary>
    ''' <param name="vcell"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("write.json_model")>
    Public Function writeJSON(vcell As VirtualCell, file As String, Optional indent As Boolean = True) As Boolean
        Dim s As Stream = file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
        Dim json As JsonElement = vcell.CreateJSONElement
        Call json.WriteJSON(s, New JSONSerializerOptions With {.indent = indent})
        Call s.Flush()
        Call s.Dispose()
        Return True
    End Function

    <ExportAPI("read.json_model")>
    Public Function readJSON(file As String) As VirtualCell
        Dim s As Stream = file.OpenReadonly
        Dim reader As New JsonParser(New StreamReader(s), tqdm:=True)
        Dim json As JsonObject = DirectCast(reader.OpenJSON, JsonObject)
        Dim model As VirtualCell = json.CreateObject(Of VirtualCell)
        Return model
    End Function

    ''' <summary>
    ''' save the virtual cell model as zip archive file
    ''' </summary>
    ''' <param name="vcell"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("write.zip")>
    Public Function WriteZipAssembly(vcell As VirtualCell, file As String) As Boolean
        Return ZipAssembly.WriteZip(vcell, file)
    End Function

    ''' <summary>
    ''' create dynamics kinetics
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="parameters"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("kinetics")>
    Public Function Kinetics(expr As String,
                             <RListObjectArgument>
                             parameters As list,
                             Optional env As Environment = Nothing) As Kinetics

        Dim args = evalArgumentValues(parameters, env)

        Return New Kinetics With {
            .formula = ScriptEngine.ParseExpression(expr),
            .parameters = args.Keys.ToArray,
            .paramVals = .parameters _
                .Select(Function(r) args(r)) _
                .ToArray,
            .target = expr
        }
    End Function

    Private Function evalArgumentValues(parameters As list, env As Environment) As Dictionary(Of String, Object)
        Return parameters.slots _
            .ToDictionary(Function(d) d.Key,
                          Function(d)
                              Dim v As Object = d.Value

                              If TypeOf v Is InvokeParameter Then
                                  v = REnv.single(DirectCast(v, InvokeParameter).Evaluate(env))
                              ElseIf TypeOf v Is RExpr Then
                                  v = REnv.single(DirectCast(v, RExpr).Evaluate(env))
                              Else
                                  v = REnv.single(v)
                              End If

                              Return v
                          End Function)
    End Function

    <ExportAPI("kinetics_lambda")>
    Public Function CompileLambda(kinetics As Kinetics) As DynamicInvoke
        Return kinetics.CompileLambda(New Dictionary(Of String, CentralDogma))
    End Function

    <ExportAPI("eval_lambda")>
    Public Function eval(kinetics As DynamicInvoke,
                         <RListObjectArgument>
                         Optional args As list = Nothing,
                         Optional env As Environment = Nothing) As Double

        Dim argv = evalArgumentValues(args, env)
        Dim fetch As Func(Of String, Double) =
            Function(ref)
                Return CDbl(argv(ref))
            End Function

        Return kinetics(fetch)
    End Function

    Public Function operonsWithPromoter(model As Genome) As Integer
        Return model.GetAllOperon.Values.Where(Function(o) Not o.promoter Is Nothing).Count
    End Function

    ''' <summary>
    ''' 校验模型对象在科学上是否完整：区室、基因组、调控网络、代谢网络、证据以及分类
    ''' </summary>
    ''' <param name="model"></param>
    <ExportAPI("check_model")>
    Public Sub checkModel(model As VirtualCell)
        Dim genome = model.genome
        Dim metabolism = model.metabolismStructure

        ' ---- 物种信息 ----
        Call DebugAudit.Check(Not model.taxonomy Is Nothing, "taxonomy exists")
        Call DebugAudit.Check(Not model.taxonomy.scientificName.StringEmpty(, True), "the scientific name of the species exists")
        Call DebugAudit.Check(Not model.taxonomy.ncbi_taxid.ToString.StringEmpty(, True), "the ncbi taxonomy id exists")
        Call DebugAudit.Check(Equals(model.schemaVersion, "3.0"), "the model file uses the GCMarkup 3.0 schema")

        ' ---- 区室 ----
        Call DebugAudit.Check(Not model.compartments.IsNullOrEmpty, "the compartment table exists")
        Call DebugAudit.Check(model.compartments.TryCount >= 3, "the model defines at least three compartments")
        Call DebugAudit.Check(model.compartments.Any(Function(c) c.kind = CompartmentKind.intracellular), "the cytosol compartment exists")
        Call DebugAudit.Check(model.compartments.Any(Function(c) c.kind = CompartmentKind.periplasm), "the periplasm compartment exists")

        ' ---- 基因组结构 ----
        Call DebugAudit.Check(Not genome Is Nothing, "the genome structure exists")
        Call DebugAudit.Check(Not genome.replicons.IsNullOrEmpty, "the genome contains at least one replicon")
        Call DebugAudit.Check(genome.genomeSize > 0, "the genome size has been calculated")
        Call DebugAudit.Check(genome.gcContent > 0.4 AndAlso genome.gcContent < 0.6, $"the gc content ({genome.gcContent}) is inside the normal range of E. coli")
        Call DebugAudit.Check(genome.GetAllGenes().TryCount > 1000, "the genome contains more than one thousand genes")
        Call DebugAudit.Check(genome.GetAllOperon.TryCount > 1000, "the genome contains more than one thousand transcription units")
        Call DebugAudit.Check(Not genome.proteins.IsNullOrEmpty, "the protein list is not empty")
        Call DebugAudit.Check(genome.proteins.SafeQuery.Count(Function(p) p.molecularWeight > 0) > 0, "the molecular weight of the proteins has been mapped")

        ' ---- 基因与蛋白质的注释 ----
        Dim genes = genome.GetAllGenes().ToArray
        Call DebugAudit.Check(genes.Count(Function(g) Not g.name.StringEmpty(, True)) > 0, "the gene symbol has been mapped")
        Call DebugAudit.Check(genes.Count(Function(g) Not g.dbLinks.IsNullOrEmpty) > 0, "the gene database cross references have been mapped")
        Call DebugAudit.Check(genome.proteins.SafeQuery.Count(Function(p) Not p.goTerms.IsNullOrEmpty) > 0, "the protein GO annotations have been mapped")
        Call DebugAudit.Check(Not genome.proteinFeatures.IsNullOrEmpty, "the protein features have been mapped")

        ' ---- 调控网络 ----
        Call DebugAudit.Check(Not genome.regulations.IsNullOrEmpty, "the transcription regulation network is not empty")
        Call DebugAudit.Check(genome.regulations.SafeQuery.Count(Function(r) Not r.operonId.StringEmpty(, True)) > 0, "the regulation relationship has been linked to the operon")
        Call DebugAudit.Check(Not genome.bindingSites.IsNullOrEmpty, "the dna binding sites have been mapped")
        Call DebugAudit.Check(Not genome.promoters.IsNullOrEmpty, "the promoters have been mapped")
        Call DebugAudit.Check(operonsWithPromoter(genome) > 0, "the promoter/terminator have been attached to the operons")

        ' ---- 代谢网络 ----
        Call DebugAudit.Check(Not metabolism Is Nothing, "the metabolism structure exists")
        Call DebugAudit.Check(Not metabolism.compounds.IsNullOrEmpty, "the compound list is not empty")
        Call DebugAudit.Check(Not metabolism.enzymes.IsNullOrEmpty, "the enzyme list is not empty")
        Call DebugAudit.Check(metabolism.reactions.size > 0, "the reaction list is not empty")
        Call DebugAudit.Check(metabolism.compounds.SafeQuery.Count(Function(c) Not c.formula.StringEmpty(, True)) > 0, "the chemical formula of the compounds has been mapped")
        Call DebugAudit.Check(metabolism.compounds.SafeQuery.Count(Function(c) c.mass > 0) > 0, "the molecular weight of the compounds has been mapped")
        Call DebugAudit.Check(metabolism.compounds.SafeQuery.Count(Function(c) Not c.smiles.StringEmpty(, True)) > 0, "the SMILES structure of the compounds has been mapped")
        Call DebugAudit.Check(metabolism.reactions.AsEnumerable.Count(Function(r) Not r.ec_number.IsNullOrEmpty) > 0, "the EC number of the reactions has been mapped")
        Call DebugAudit.Check(metabolism.reactions.AsEnumerable.Count(Function(r) Not r.gibbs = 0) > 0, "the gibbs free energy of the reactions has been mapped")
        Call DebugAudit.Check(metabolism.reactions.AsEnumerable.Count(Function(r) Not r.pathways.IsNullOrEmpty) > 0, "the reactions have been linked to the pathways")
        Call DebugAudit.Check(metabolism.reactions.AsEnumerable.Count(Function(r) Not r.enzymes.IsNullOrEmpty) > 0, "the reactions have been linked to the enzymes")
        Call DebugAudit.Check(metabolism.reactions.AsEnumerable.Count(Function(r) r.direction <> ReactionDirection.unknown) > 0, "the reaction direction has been mapped")
        Call DebugAudit.Check(Not metabolism.reactions.transportation.IsNullOrEmpty, "the transportation processes have been mapped")
        Call DebugAudit.Check(Not metabolism.maps.IsNullOrEmpty, "the pathway functional categories have been mapped")

        ' ---- 酶动力学 ----
        Call DebugAudit.Check(metabolism.enzymes.SafeQuery.Count(Function(e) Not e.catalysis.IsNullOrEmpty) > 0, "the enzyme kinetics functions have been mapped")
        Call DebugAudit.Check(metabolism.enzymes.SafeQuery.Count(Function(e) Not e.ECNumber.StringEmpty(, True)) > 0, "the EC number of the enzymes has been mapped")

        ' ---- 证据与分类 ----
        Call DebugAudit.Check(Not model.evidence.IsNullOrEmpty, "the publication evidence table is not empty")
        Call DebugAudit.Check(Not model.physiology Is Nothing, "the physiology parameters exist")
    End Sub
End Module
