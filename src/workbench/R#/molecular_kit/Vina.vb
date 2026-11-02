Imports System.IO
Imports System.Text.Json
Imports System.Text.Json.Serialization

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Vectorization

Imports Mol = SMRUCC.genomics.Molecule.SMILES.Graph
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports Smiles = SMRUCC.genomics.Molecule.SMILES
Imports Structures = SMRUCC.genomics.Data.RCSB.PDB.Structures
Imports VinaCore = SMRUCC.genomics.Molecule.VinaDock.Core
Imports VinaModel = SMRUCC.genomics.Molecule.VinaDock.Model

''' <summary>
''' molecular docking toolkit for the R# scripting language: the receptor/ligand 
''' preparation, the AutoDock Vina scoring based pose search, the MM-GBSA / 
''' Nwat-MMGBSA re-scoring and the docking result output.
''' </summary>
''' <remarks>
''' the docking algorithms are implemented in the ``MiniDock`` library project, 
''' current package module only provides the R# script interop layer: arguments 
''' parsing, pipeline orchestration, result tables and error wrapping.
''' </remarks>
<Package("Vina", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module Vina

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(VinaCore.VinaMolecule), AddressOf moleculeFrame)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(VinaModel.LigandResult), AddressOf posesFrame)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(VinaCore.MmGbsaResult), AddressOf mmgbsaFrame)

        Call RInternal.ConsolePrinter.AttachConsoleFormatter(Of VinaCore.VinaMolecule)(AddressOf showMolecule)
        Call RInternal.ConsolePrinter.AttachConsoleFormatter(Of VinaModel.LigandResult)(AddressOf showDockResult)
        Call RInternal.ConsolePrinter.AttachConsoleFormatter(Of VinaCore.MmGbsaResult)(AddressOf showMmGbsa)
    End Sub

#Region "结构准备"

    ''' <summary>
    ''' read the receptor protein structure from a given PDB file, and then 
    ''' prepare it for the docking: the Vina atom type and the PEOE partial 
    ''' charge are assigned automatically.
    ''' </summary>
    ''' <param name="file">the file path of the receptor PDB structure file</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a <see cref="VinaCore.VinaMolecule"/> object, or a <see cref="Message"/> 
    ''' error object if the given file is not exists or the structure is invalid.
    ''' </returns>
    ''' <example>
    ''' let receptor = read.receptor("./1abc.pdb");
    ''' </example>
    <ExportAPI("read.receptor")>
    <RApiReturn(GetType(VinaCore.VinaMolecule))>
    Public Function readReceptor(file As String, Optional env As Environment = Nothing) As Object
        Dim err As Message = checkFile(file, env)

        If err IsNot Nothing Then
            Return err
        End If

        Try
            Return prepareReceptor(file)
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to read the receptor structure: {ex.Message}", env)
        End Try
    End Function

    ''' <summary>
    ''' read the ligand structure from a given file and then prepare it for the 
    ''' docking: the ``.sdf``/``.mol`` file is treated as a small molecule ligand, 
    ''' other file(``.pdb``) is treated as a protein ligand for the 
    ''' protein-protein docking mode.
    ''' </summary>
    ''' <param name="file">the file path of the ligand structure file</param>
    ''' <param name="target_charge">
    ''' the net charge value of the ligand molecule, this value is only used for 
    ''' the PEOE partial charge correction of the small molecule ligand.
    ''' </param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a <see cref="VinaCore.VinaMolecule"/> object, or a <see cref="Message"/> 
    ''' error object if the given file is not exists or the structure is invalid.
    ''' </returns>
    ''' <example>
    ''' let ligand = read.ligand("./caffeine.sdf");
    ''' let protein = read.ligand("./peptide.pdb");
    ''' </example>
    <ExportAPI("read.ligand")>
    <RApiReturn(GetType(VinaCore.VinaMolecule))>
    Public Function readLigand(file As String,
                               Optional target_charge As Double = 0,
                               Optional env As Environment = Nothing) As Object
        Dim err As Message = checkFile(file, env)

        If err IsNot Nothing Then
            Return err
        End If

        Try
            Return prepareLigand(file, target_charge)
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to read the ligand structure: {ex.Message}", env)
        End Try
    End Function

    ''' <summary>
    ''' create the docking ligand molecule from a given SMILES/InChI structure 
    ''' text, the 3D coordinates are generated by the ``embed3d`` function of the 
    ''' ``molecular`` package automatically.
    ''' </summary>
    ''' <param name="smiles">
    ''' the SMILES or InChI structure text, the text format is auto detected.
    ''' </param>
    ''' <param name="add_hydrogens">
    ''' expand the implicit hydrogens as the explicit hydrogen atoms before the 
    ''' 3D coordinates embedding? default is FALSE: the docking scoring works on 
    ''' the heavy atom model, so the explicit hydrogen atoms are not required.
    ''' </param>
    ''' <param name="target_charge">the net charge value of the ligand molecule for the PEOE charge correction</param>
    ''' <param name="seed">the random seed value for the deterministic 3D embedding result</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a <see cref="VinaCore.VinaMolecule"/> object with the 3D coordinates, 
    ''' or a <see cref="Message"/> error object if the given structure text is invalid.
    ''' </returns>
    ''' <example>
    ''' let ligand = ligand_from_smiles("Cn1cnc2c1c(=O)n(C)c(=O)n2C");
    ''' </example>
    <ExportAPI("ligand_from_smiles")>
    <RApiReturn(GetType(VinaCore.VinaMolecule))>
    Public Function ligandFromSmiles(smiles As String,
                                     Optional add_hydrogens As Boolean = False,
                                     Optional target_charge As Double = 0,
                                     Optional seed As Integer = 20240101,
                                     Optional env As Environment = Nothing) As Object
        If String.IsNullOrWhiteSpace(smiles) Then
            Return RInternal.debug.stop("the given structure text is empty!", env)
        End If

        Try
            Return moleculeFromSmiles(smiles, add_hydrogens, target_charge, seed)
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to create the ligand molecule: {ex.Message}", env)
        End Try
    End Function

#End Region

#Region "对接"

    ''' <summary>
    ''' create the docking search options object which can be reused by the 
    ''' ``vina_dock`` and ``write_dock_report`` function via the ``options`` argument
    ''' </summary>
    ''' <param name="exhaustiveness">the independent search runs number, default is 8</param>
    ''' <param name="steps_per_run">the monte-carlo steps number of each search run, default is 30</param>
    ''' <param name="num_modes">the number of the output poses, default is 9</param>
    ''' <param name="min_rmsd">the RMSD threshold in Å for the pose duplicates removing, default is 1.5</param>
    ''' <param name="box_center">
    ''' the numeric vector of the search box center coordinates, example as 
    ''' ``c(x, y, z)``; a NULL value means the geometric center of the receptor.
    ''' </param>
    ''' <param name="box_half">the half size of the search box in Å, default is 12</param>
    ''' <param name="seed">the random seed value, 0 means random</param>
    ''' <param name="mmgbsa">run the MM-GBSA re-scoring on the best poses? default is FALSE</param>
    ''' <param name="nwat">the interface water number that kept in the Nwat-MMGBSA re-scoring, default is 0</param>
    ''' <param name="mmgbsa_top">the top N poses that run the MM-GBSA re-scoring, default is 3</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>a docking search options object</returns>
    ''' <example>
    ''' let opts = dock_options(exhaustiveness = 16, box_center = c(12.3, 4.5, 6.7), box_half = 10);
    ''' let result = vina_dock(receptor, ligand, options = opts);
    ''' </example>
    <ExportAPI("dock_options")>
    <RApiReturn(GetType(VinaCore.DockOptions))>
    Public Function dockOptions(Optional exhaustiveness As Integer = 8,
                                Optional steps_per_run As Integer = 30,
                                Optional num_modes As Integer = 9,
                                Optional min_rmsd As Double = 1.5,
                                <RRawVectorArgument>
                                Optional box_center As Object = Nothing,
                                Optional box_half As Double = 12.0,
                                Optional seed As Integer = 0,
                                Optional mmgbsa As Boolean = False,
                                Optional nwat As Integer = 0,
                                Optional mmgbsa_top As Integer = 3,
                                Optional env As Environment = Nothing) As Object

        Return createOptions(exhaustiveness, steps_per_run, num_modes, min_rmsd,
                             box_center, box_half, seed, mmgbsa, nwat, mmgbsa_top)
    End Function

    ''' <summary>
    ''' run the AutoDock Vina scoring based molecular docking between the given 
    ''' receptor and ligand molecules
    ''' </summary>
    ''' <param name="receptor">
    ''' the receptor molecule object that created by the ``read.receptor`` function.
    ''' </param>
    ''' <param name="ligand">
    ''' the ligand molecule object that created by the ``read.ligand`` or 
    ''' ``ligand_from_smiles`` function.
    ''' </param>
    ''' <param name="options">
    ''' the docking search options object that created by the ``dock_options`` 
    ''' function; when this argument is not NULL, all of the inline options 
    ''' arguments below will be ignored.
    ''' </param>
    ''' <param name="box_center">the search box center, a numeric vector of length 3</param>
    ''' <param name="box_half">the half size of the search box in Å, default is 12</param>
    ''' <param name="exhaustiveness">the independent search runs number, default is 8</param>
    ''' <param name="steps_per_run">the monte-carlo steps number of each search run, default is 30</param>
    ''' <param name="num_modes">the number of the output poses, default is 9</param>
    ''' <param name="min_rmsd">the RMSD threshold in Å for the pose duplicates removing, default is 1.5</param>
    ''' <param name="seed">the random seed value, 0 means random</param>
    ''' <param name="mmgbsa">run the MM-GBSA re-scoring on the best poses? default is FALSE</param>
    ''' <param name="nwat">the interface water number that kept in the Nwat-MMGBSA re-scoring, default is 0</param>
    ''' <param name="mmgbsa_top">the top N poses that run the MM-GBSA re-scoring, default is 3</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a <see cref="VinaModel.LigandResult"/> object which contains the docking 
    ''' poses; or a <see cref="Message"/> error object if the docking is failed.
    ''' </returns>
    ''' <example>
    ''' let result = vina_dock(receptor, ligand, box_center = c(10, 20, 30), mmgbsa = TRUE, nwat = 10);
    ''' print(as.data.frame(result));
    ''' </example>
    <ExportAPI("vina_dock")>
    <RApiReturn(GetType(VinaModel.LigandResult))>
    Public Function vinaDock(receptor As VinaCore.VinaMolecule,
                             ligand As VinaCore.VinaMolecule,
                             Optional options As Object = Nothing,
                             <RRawVectorArgument>
                             Optional box_center As Object = Nothing,
                             Optional box_half As Double = 12.0,
                             Optional exhaustiveness As Integer = 8,
                             Optional steps_per_run As Integer = 30,
                             Optional num_modes As Integer = 9,
                             Optional min_rmsd As Double = 1.5,
                             Optional seed As Integer = 0,
                             Optional mmgbsa As Boolean = False,
                             Optional nwat As Integer = 0,
                             Optional mmgbsa_top As Integer = 3,
                             Optional env As Environment = Nothing) As Object

        If receptor Is Nothing Then
            Return RInternal.debug.stop("the given receptor molecule is nothing!", env)
        End If
        If ligand Is Nothing Then
            Return RInternal.debug.stop("the given ligand molecule is nothing!", env)
        End If
        If receptor.Atoms.Count = 0 Then
            Return RInternal.debug.stop("the given receptor molecule has no atoms!", env)
        End If
        If ligand.Atoms.Count = 0 Then
            Return RInternal.debug.stop("the given ligand molecule has no atoms!", env)
        End If

        Dim opts As VinaCore.DockOptions = getOptions(options, box_center, box_half, exhaustiveness,
                                                      steps_per_run, num_modes, min_rmsd, seed,
                                                      mmgbsa, nwat, mmgbsa_top)

        Try
            Dim result As VinaModel.LigandResult = VinaCore.DockEngine.Dock(receptor, ligand, opts)

            If opts.Mmgbsa Then
                Call rescoreMmgbsa(receptor, ligand, result, opts)
            End If

            Return result
        Catch ex As Exception
            Return RInternal.debug.stop($"molecular docking is failed: {ex.Message}", env)
        End Try
    End Function

    ''' <summary>
    ''' run the molecular docking pipeline directly from the given receptor file 
    ''' and ligand file(input text) in a single call: the structures are read and 
    ''' prepared automatically before the docking
    ''' </summary>
    ''' <param name="receptor">the file path of the receptor PDB structure file</param>
    ''' <param name="ligand">
    ''' the file path of the ligand structure file(``.sdf``/``.mol``/``.pdb``); 
    ''' if the given text value is not exists as a file path, then it will be 
    ''' treated as the SMILES/InChI structure text of the small molecule ligand.
    ''' </param>
    ''' <param name="options">the docking search options object that created by the ``dock_options`` function</param>
    ''' <param name="box_center">the search box center, a numeric vector of length 3</param>
    ''' <param name="box_half">the half size of the search box in Å, default is 12</param>
    ''' <param name="exhaustiveness">the independent search runs number, default is 8</param>
    ''' <param name="steps_per_run">the monte-carlo steps number of each search run, default is 30</param>
    ''' <param name="num_modes">the number of the output poses, default is 9</param>
    ''' <param name="min_rmsd">the RMSD threshold in Å for the pose duplicates removing, default is 1.5</param>
    ''' <param name="seed">the random seed value, 0 means random</param>
    ''' <param name="mmgbsa">run the MM-GBSA re-scoring on the best poses? default is FALSE</param>
    ''' <param name="nwat">the interface water number that kept in the Nwat-MMGBSA re-scoring, default is 0</param>
    ''' <param name="mmgbsa_top">the top N poses that run the MM-GBSA re-scoring, default is 3</param>
    ''' <param name="target_charge">the net charge value of the small molecule ligand for the PEOE charge correction</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a <see cref="VinaModel.LigandResult"/> object which contains the docking 
    ''' poses; or a <see cref="Message"/> error object if any step of the pipeline 
    ''' is failed.
    ''' </returns>
    ''' <example>
    ''' let result = vina_dock_files("./1abc.pdb", "./ligand.sdf", box_half = 10);
    ''' let result = vina_dock_files("./1abc.pdb", "Cn1cnc2c1c(=O)n(C)c(=O)n2C", mmgbsa = TRUE);
    ''' </example>
    <ExportAPI("vina_dock_files")>
    <RApiReturn(GetType(VinaModel.LigandResult))>
    Public Function vinaDockFiles(receptor As String,
                                  ligand As String,
                                  Optional options As Object = Nothing,
                                  <RRawVectorArgument>
                                  Optional box_center As Object = Nothing,
                                  Optional box_half As Double = 12.0,
                                  Optional exhaustiveness As Integer = 8,
                                  Optional steps_per_run As Integer = 30,
                                  Optional num_modes As Integer = 9,
                                  Optional min_rmsd As Double = 1.5,
                                  Optional seed As Integer = 0,
                                  Optional mmgbsa As Boolean = False,
                                  Optional nwat As Integer = 0,
                                  Optional mmgbsa_top As Integer = 3,
                                  Optional target_charge As Double = 0,
                                  Optional env As Environment = Nothing) As Object

        Dim err As Message = checkFile(receptor, env)

        If err IsNot Nothing Then
            Return err
        End If
        If String.IsNullOrWhiteSpace(ligand) Then
            Return RInternal.debug.stop("the given ligand file path or structure text is empty!", env)
        End If

        Dim receptorMol As VinaCore.VinaMolecule
        Dim ligandMol As VinaCore.VinaMolecule

        Try
            receptorMol = prepareReceptor(receptor)

            If IO.File.Exists(ligand) Then
                ligandMol = prepareLigand(ligand, target_charge)
            Else
                ligandMol = moleculeFromSmiles(ligand, addHydrogens:=False, targetCharge:=target_charge, seed:=20240101)
            End If
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to prepare the docking structures: {ex.Message}", env)
        End Try

        Return vinaDock(receptorMol, ligandMol, options, box_center, box_half, exhaustiveness,
                        steps_per_run, num_modes, min_rmsd, seed, mmgbsa, nwat, mmgbsa_top, env)
    End Function

#End Region

#Region "结果输出"

    ''' <summary>
    ''' get the atom table of the given docking molecule object
    ''' </summary>
    ''' <param name="mol">the receptor or ligand molecule object</param>
    ''' <returns>
    ''' a dataframe object with the data fields: index, element, vina_type, charge, 
    ''' lj_eps, lj_rmin, x, y, z, chain, res_name, res_seq, atom_name, from_receptor 
    ''' and is_water.
    ''' </returns>
    ''' <example>
    ''' print(as.data.frame(read.receptor("./1abc.pdb")));
    ''' </example>
    <ExportAPI("vina_atoms")>
    <RApiReturn(GetType(dataframe))>
    Public Function dockingAtoms(mol As VinaCore.VinaMolecule) As dataframe
        Return moleculeFrame(mol)
    End Function

    ''' <summary>
    ''' get the atom coordinates table of the given docking pose
    ''' </summary>
    ''' <param name="result">the docking result object that created by the ``vina_dock`` function</param>
    ''' <param name="rank">the rank number of the target pose, the first pose is 1</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a dataframe object with the data fields: element, x, y, z, chain, res_name, 
    ''' res_seq and atom_name; or a <see cref="Message"/> error object if the given 
    ''' pose is not exists.
    ''' </returns>
    ''' <example>
    ''' print(pose_atoms(result, rank = 1));
    ''' </example>
    <ExportAPI("pose_atoms")>
    <RApiReturn(GetType(dataframe))>
    Public Function poseAtoms(result As VinaModel.LigandResult,
                              Optional rank As Integer = 1,
                              Optional env As Environment = Nothing) As Object
        Dim pose As VinaModel.Pose = getPose(result, rank, env)

        If pose Is Nothing Then
            Return RInternal.debug.stop($"the docking pose #{rank} is not exists!", env)
        End If

        Return poseFrame(pose)
    End Function

    ''' <summary>
    ''' write the atom coordinates of the given docking pose as a PDB structure file
    ''' </summary>
    ''' <param name="result">the docking result object that created by the ``vina_dock`` function</param>
    ''' <param name="file">the output PDB file path, the directory will be created automatically</param>
    ''' <param name="rank">the rank number of the target pose, the first pose is 1</param>
    ''' <param name="res_name">the residue name of the pose atoms, default is ``LIG``</param>
    ''' <param name="chain">the chain identifier of the pose atoms, default is ``L``</param>
    ''' <param name="title">the structure title, a NULL value means use the ligand id</param>
    ''' <param name="ligand">
    ''' the ligand molecule object that used for the docking, when this argument is 
    ''' not NULL, the ligand chemical bonds will be written as the ``CONECT`` record.
    ''' </param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the full file path of the generated PDB file, or a <see cref="Message"/> error object</returns>
    ''' <example>
    ''' write_pose_pdb(result, "./pose-1.pdb", rank = 1);
    ''' </example>
    <ExportAPI("write_pose_pdb")>
    <RApiReturn(GetType(String))>
    Public Function writePosePdb(result As VinaModel.LigandResult,
                                 file As String,
                                 Optional rank As Integer = 1,
                                 Optional res_name As String = "LIG",
                                 Optional chain As String = "L",
                                 Optional title As String = Nothing,
                                 Optional ligand As Object = Nothing,
                                 Optional env As Environment = Nothing) As Object
        If String.IsNullOrWhiteSpace(file) Then
            Return RInternal.debug.stop("the output pdb file path can not be empty!", env)
        End If

        Dim pose As VinaModel.Pose = getPose(result, rank, env)

        If pose Is Nothing Then
            Return RInternal.debug.stop($"the docking pose #{rank} is not exists!", env)
        End If
        If pose.Atoms Is Nothing OrElse pose.Atoms.Count = 0 Then
            Return RInternal.debug.stop($"the docking pose #{rank} has no atom coordinates!", env)
        End If

        Try
            Dim mol As Structures.Molecule(Of Structures.Atom) = poseToMolecule(pose, res_name, chain)
            Dim bonds As VinaCore.VinaMolecule = TryCast(ligand, VinaCore.VinaMolecule)

            If bonds IsNot Nothing Then
                For Each b As Structures.Bond In bonds.Bonds
                    Call mol.Bonds.Add(b)
                Next
            End If

            Dim path As String = Structures.PdbDocumentWriter.Save(
                mol,
                file,
                If(String.IsNullOrEmpty(title), If(String.IsNullOrEmpty(result.Id), "pose", result.Id), title),
                Nothing
            )

            Return path
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to write the pose pdb file: {ex.Message}", env)
        End Try
    End Function

    ''' <summary>
    ''' write the docking result as a JSON report file, the report format is 
    ''' compatible with the ``MiniDock`` commandline program output.
    ''' </summary>
    ''' <param name="result">the docking result object that created by the ``vina_dock`` function</param>
    ''' <param name="file">the output JSON file path, the directory will be created automatically</param>
    ''' <param name="receptor_atoms">the receptor atom counts for the report metadata, a negative value means unknown</param>
    ''' <param name="mode">the docking mode: ``ligand`` or ``protein-protein``</param>
    ''' <param name="options">the docking search options object that used in the docking</param>
    ''' <param name="pretty">write the indented JSON text? default is TRUE</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the full file path of the generated JSON file, or a <see cref="Message"/> error object</returns>
    ''' <example>
    ''' write_dock_report(result, "./dock_report.json", receptor_atoms = 2456, mode = "ligand");
    ''' </example>
    <ExportAPI("write_dock_report")>
    <RApiReturn(GetType(String))>
    Public Function writeDockReport(result As VinaModel.LigandResult,
                                    file As String,
                                    Optional receptor_atoms As Integer = -1,
                                    Optional mode As String = "ligand",
                                    Optional options As Object = Nothing,
                                    Optional pretty As Boolean = True,
                                    Optional env As Environment = Nothing) As Object
        If result Is Nothing Then
            Return RInternal.debug.stop("the given docking result is nothing!", env)
        End If
        If String.IsNullOrWhiteSpace(file) Then
            Return RInternal.debug.stop("the output json file path can not be empty!", env)
        End If

        Dim opts As VinaCore.DockOptions = TryCast(options, VinaCore.DockOptions)

        If opts Is Nothing Then
            opts = New VinaCore.DockOptions
        End If

        Try
            Dim report As VinaModel.DockReport = buildReport(
                result,
                opts,
                If(receptor_atoms < 0, 0, receptor_atoms),
                If(String.IsNullOrEmpty(mode), "ligand", mode)
            )
            Dim json As String = JsonSerializer.Serialize(report, jsonOptions(pretty))

            Call ensureDirectory(file)
            Call IO.File.WriteAllText(file, json)

            Return IO.Path.GetFullPath(file)
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to write the docking report: {ex.Message}", env)
        End Try
    End Function

#End Region

#Region "打分"

    ''' <summary>
    ''' evaluate the AutoDock Vina scoring function value of the given receptor 
    ''' and ligand complex, the ligand atoms are used as the fixed final pose 
    ''' coordinates directly
    ''' </summary>
    ''' <param name="receptor">the receptor molecule object</param>
    ''' <param name="ligand">
    ''' the ligand molecule object which contains the final pose coordinates.
    ''' </param>
    ''' <param name="rotatable">
    ''' add the rotatable bond penalty term(``0.0585 × N_rot``) into the score 
    ''' value? default is TRUE.
    ''' </param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the vina score value in kcal/mol, or a <see cref="Message"/> error object</returns>
    ''' <example>
    ''' let score = vina_score(receptor, ligand);
    ''' </example>
    <ExportAPI("vina_score")>
    <RApiReturn(GetType(Double))>
    Public Function vinaScore(receptor As VinaCore.VinaMolecule,
                              ligand As VinaCore.VinaMolecule,
                              Optional rotatable As Boolean = True,
                              Optional env As Environment = Nothing) As Object
        If receptor Is Nothing OrElse ligand Is Nothing Then
            Return RInternal.debug.stop("the given receptor or ligand molecule is nothing!", env)
        End If
        If receptor.Atoms.Count = 0 OrElse ligand.Atoms.Count = 0 Then
            Return RInternal.debug.stop("the given receptor or ligand molecule has no atoms!", env)
        End If

        Try
            Dim scorer As New VinaCore.VinaScorer(receptor.Atoms)
            Dim grads As Double() = New Double(5) {}
            Dim energy As Double = scorer.Evaluate(
                ligand.Atoms,
                New Double() {0, 0, 0},
                New Integer() {},
                New Integer() {},
                New Integer() {},
                New List(Of List(Of Integer)),
                grads
            )

            If rotatable Then
                Dim tree = VinaCore.MolBuilder.BuildTorsionTree(ligand)
                energy += VinaCore.VinaScorer.RotatableWeight * tree.Item2.Count
            End If

            Return energy
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to evaluate the vina score: {ex.Message}", env)
        End Try
    End Function

    ''' <summary>
    ''' evaluate the MM-GBSA binding free energy of the given receptor and ligand 
    ''' complex, the Nwat-MMGBSA mode is enabled when the ``nwat`` value is 
    ''' greater than zero
    ''' </summary>
    ''' <param name="receptor">
    ''' the receptor molecule object, the water atoms inside the receptor are 
    ''' treated as the interface water candidates.
    ''' </param>
    ''' <param name="ligand">the ligand molecule object which contains the final pose coordinates</param>
    ''' <param name="nwat">
    ''' the interface water number that kept in the Nwat-MMGBSA calculation, 
    ''' default is 0 which means no water is kept.
    ''' </param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a <see cref="VinaCore.MmGbsaResult"/> object which contains the binding free 
    ''' energy and its decomposition terms; or a <see cref="Message"/> error object.
    ''' </returns>
    ''' <example>
    ''' print(mmgbsa_score(receptor, ligand, nwat = 10));
    ''' </example>
    <ExportAPI("mmgbsa_score")>
    <RApiReturn(GetType(VinaCore.MmGbsaResult))>
    Public Function mmgbsaScore(receptor As VinaCore.VinaMolecule,
                                ligand As VinaCore.VinaMolecule,
                                Optional nwat As Integer = 0,
                                Optional env As Environment = Nothing) As Object
        If receptor Is Nothing OrElse ligand Is Nothing Then
            Return RInternal.debug.stop("the given receptor or ligand molecule is nothing!", env)
        End If
        If ligand.Atoms.Count = 0 Then
            Return RInternal.debug.stop("the given ligand molecule has no atoms!", env)
        End If

        Try
            Return VinaCore.DockEngine.MmGbsaRescore(receptor.Atoms, ligand.Atoms, Math.Max(nwat, 0))
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to evaluate the mm-gbsa energy: {ex.Message}", env)
        End Try
    End Function

    ''' <summary>
    ''' evaluate the MM-GBSA binding free energy for each ``MODEL`` frame of the 
    ''' given complex PDB structure file
    ''' </summary>
    ''' <param name="file">the file path of the complex PDB structure file</param>
    ''' <param name="ligand_chain">
    ''' the chain identifier of the ligand atoms, the atoms in this chain will be 
    ''' treated as the ligand side.
    ''' </param>
    ''' <param name="ligand_resname">
    ''' the residue name of the ligand atoms, the atoms with this residue name 
    ''' will be treated as the ligand side.
    ''' </param>
    ''' <param name="nwat">the interface water number that kept in the Nwat-MMGBSA calculation, default is 0</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a dataframe object with the data fields: model, delta_g, vdw, elec, 
    ''' gb_polar, sas_nonpolar, nwat_selected, receptor_atoms and ligand_atoms.
    ''' </returns>
    ''' <example>
    ''' print(mmgbsa_complex("./complex.pdb", ligand_chain = "L", nwat = 10));
    ''' </example>
    <ExportAPI("mmgbsa_complex")>
    <RApiReturn(GetType(dataframe))>
    Public Function mmgbsaComplex(file As String,
                                  Optional ligand_chain As String = Nothing,
                                  Optional ligand_resname As String = Nothing,
                                  Optional nwat As Integer = 0,
                                  Optional env As Environment = Nothing) As Object
        Dim err As Message = checkFile(file, env)

        If err IsNot Nothing Then
            Return err
        End If

        Dim model As New List(Of Integer)
        Dim deltaG As New List(Of Double)
        Dim vdw As New List(Of Double)
        Dim elec As New List(Of Double)
        Dim gbPolar As New List(Of Double)
        Dim sasNonpolar As New List(Of Double)
        Dim nwatSelected As New List(Of Integer)
        Dim receptorAtoms As New List(Of Integer)
        Dim ligandAtoms As New List(Of Integer)

        Try
            Dim frames As List(Of VinaCore.VinaMolecule) =
                Structures.StructureIO.ReadPdbFrames(Of VinaCore.VinaAtom, VinaCore.VinaMolecule)(file)
            Dim modelNo As Integer = 0

            For Each frame As VinaCore.VinaMolecule In frames
                modelNo += 1

                Dim rec As New List(Of VinaCore.VinaAtom)
                Dim lig As New List(Of VinaCore.VinaAtom)

                For Each a As VinaCore.VinaAtom In frame.Atoms
                    If a.IsWater Then
                        Continue For
                    End If

                    Dim isLigand As Boolean = False

                    If Not String.IsNullOrEmpty(ligand_chain) AndAlso a.ChainID.Trim = ligand_chain.Trim Then
                        isLigand = True
                    End If
                    If Not String.IsNullOrEmpty(ligand_resname) AndAlso
                        String.Equals(a.ResName.Trim, ligand_resname.Trim, StringComparison.OrdinalIgnoreCase) Then
                        isLigand = True
                    End If

                    If isLigand Then
                        Call lig.Add(a)
                    Else
                        Call rec.Add(a)
                    End If
                Next

                If lig.Count = 0 Then
                    Continue For
                End If

                Dim r As VinaCore.MmGbsaResult = VinaCore.DockEngine.MmGbsaRescore(rec, lig, Math.Max(nwat, 0))

                Call model.Add(modelNo)
                Call deltaG.Add(Math.Round(r.DeltaG, 3))
                Call vdw.Add(Math.Round(r.Vdw, 3))
                Call elec.Add(Math.Round(r.Elec, 3))
                Call gbPolar.Add(Math.Round(r.GbPolar, 3))
                Call sasNonpolar.Add(Math.Round(r.SasNonpolar, 3))
                Call nwatSelected.Add(r.NwatSelected)
                Call receptorAtoms.Add(rec.Count)
                Call ligandAtoms.Add(lig.Count)
            Next
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to evaluate the complex mm-gbsa energy: {ex.Message}", env)
        End Try

        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Call table.columns.Add("model", model.ToArray)
        Call table.columns.Add("delta_g", deltaG.ToArray)
        Call table.columns.Add("vdw", vdw.ToArray)
        Call table.columns.Add("elec", elec.ToArray)
        Call table.columns.Add("gb_polar", gbPolar.ToArray)
        Call table.columns.Add("sas_nonpolar", sasNonpolar.ToArray)
        Call table.columns.Add("nwat_selected", nwatSelected.ToArray)
        Call table.columns.Add("receptor_atoms", receptorAtoms.ToArray)
        Call table.columns.Add("ligand_atoms", ligandAtoms.ToArray)

        Return table
    End Function

    ''' <summary>
    ''' get the AutoDock Vina atom type parameter table which is used by the 
    ''' scoring function
    ''' </summary>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a dataframe object with the data fields: code, name, radius, hydrophobic, 
    ''' donor and acceptor.
    ''' </returns>
    ''' <example>
    ''' print(vina_atom_types());
    ''' </example>
    <ExportAPI("vina_atom_types")>
    <RApiReturn(GetType(dataframe))>
    Public Function vinaAtomTypes(Optional env As Environment = Nothing) As Object
        Dim code As New List(Of Integer)
        Dim name As New List(Of String)
        Dim radius As New List(Of Double)
        Dim hydrophobic As New List(Of Boolean)
        Dim donor As New List(Of Boolean)
        Dim acceptor As New List(Of Boolean)

        For t As Integer = 0 To VinaCore.VinaAtomTypes.TCount - 1
            Call code.Add(t)
            Call name.Add(VinaCore.VinaAtomTypes.TypeName(t))
            Call radius.Add(VinaCore.VinaAtomTypes.Radii(t))
            Call hydrophobic.Add(VinaCore.VinaAtomTypes.Hydrophobic(t))
            Call donor.Add(VinaCore.VinaAtomTypes.Donor(t))
            Call acceptor.Add(VinaCore.VinaAtomTypes.Acceptor(t))
        Next

        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Call table.columns.Add("code", code.ToArray)
        Call table.columns.Add("name", name.ToArray)
        Call table.columns.Add("radius", radius.ToArray)
        Call table.columns.Add("hydrophobic", hydrophobic.ToArray)
        Call table.columns.Add("donor", donor.ToArray)
        Call table.columns.Add("acceptor", acceptor.ToArray)

        Return table
    End Function

#End Region

#Region "内部帮助函数"

    ''' <summary>
    ''' read the receptor PDB structure and assign the vina atom type and the 
    ''' protein partial charge
    ''' </summary>
    Private Function prepareReceptor(path As String) As VinaCore.VinaMolecule
        Dim mol As VinaCore.VinaMolecule =
            Structures.StructureIO.ReadPdb(Of VinaCore.VinaAtom, VinaCore.VinaMolecule)(path)

        Call VinaCore.MolBuilder.AssignTypesPdb(mol)
        Call VinaCore.Charges.AssignProteinCharges(mol)

        For Each a As VinaCore.VinaAtom In mol.Atoms
            a.FromReceptor = True
        Next

        Return mol
    End Function

    ''' <summary>
    ''' read the ligand structure and assign the vina atom type and the partial charge
    ''' </summary>
    Private Function prepareLigand(path As String, targetCharge As Double) As VinaCore.VinaMolecule
        Dim ext As String = IO.Path.GetExtension(path).ToLowerInvariant
        Dim mol As VinaCore.VinaMolecule

        If ext = ".sdf" OrElse ext = ".mol" Then
            mol = VinaCore.SdfIO.ReadSdf(path)
            Call VinaCore.MolBuilder.AssignTypesSdf(mol)
            Call VinaCore.Charges.AssignPoeCharges(mol, targetCharge)
        Else
            mol = Structures.StructureIO.ReadPdb(Of VinaCore.VinaAtom, VinaCore.VinaMolecule)(path)
            Call VinaCore.MolBuilder.AssignTypesPdb(mol)
            Call VinaCore.Charges.AssignProteinCharges(mol)
        End If

        For Each a As VinaCore.VinaAtom In mol.Atoms
            a.FromReceptor = False
        Next

        Return mol
    End Function

    ''' <summary>
    ''' create the docking ligand molecule from the given structure text, the 3D 
    ''' coordinates are generated by the SMILES library
    ''' </summary>
    Private Function moleculeFromSmiles(text As String,
                                        addHydrogens As Boolean,
                                        targetCharge As Double,
                                        seed As Integer) As VinaCore.VinaMolecule

        Dim graph As Mol.MoleculeGraph = Smiles.MoleculeIO.Parse(text)

        Call graph.Embed3D(New Mol.Embed3DOptions With {
            .AddHydrogens = addHydrogens,
            .RespectStereo = True,
            .RandomSeed = seed
        })

        Dim mol As New VinaCore.VinaMolecule With {
            .Id = graph.MolecularFormula()
        }

        For i As Integer = 0 To graph.AtomCount - 1
            Dim src As Mol.Atom = graph.Atoms(i)

            Call mol.Atoms.Add(New VinaCore.VinaAtom With {
                .Element = src.Element,
                .X = src.X,
                .Y = src.Y,
                .Z = src.Z,
                .Charge = src.Charge,
                .AtomName = $"{src.Element}{i + 1}",
                .ResName = "LIG",
                .ChainID = "L",
                .ResSeq = 1,
                .AltLoc = "",
                .Occupancy = 1.0,
                .TempFactor = 0.0,
                .IsHet = True,
                .IsWater = False,
                .FromReceptor = False
            })
        Next

        For Each b As Mol.Bond In graph.Bonds
            Call mol.Bonds.Add(New Structures.Bond(b.a, b.b, b.order))
        Next

        Call VinaCore.MolBuilder.AssignTypesSdf(mol)
        Call VinaCore.Charges.AssignPoeCharges(mol, targetCharge)

        Return mol
    End Function

    Private Function createOptions(exhaustiveness As Integer,
                                   stepsPerRun As Integer,
                                   numModes As Integer,
                                   minRmsd As Double,
                                   boxCenter As Object,
                                   boxHalf As Double,
                                   seed As Integer,
                                   mmgbsa As Boolean,
                                   nwat As Integer,
                                   mmgbsaTop As Integer) As VinaCore.DockOptions

        Return New VinaCore.DockOptions With {
            .Exhaustiveness = If(exhaustiveness <= 0, 8, exhaustiveness),
            .StepsPerRun = If(stepsPerRun <= 0, 30, stepsPerRun),
            .NumModes = If(numModes <= 0, 9, numModes),
            .MinRmsd = If(minRmsd <= 0, 1.5, minRmsd),
            .BoxCenter = asBoxCenter(boxCenter),
            .BoxHalfSize = If(boxHalf <= 0, 12.0, boxHalf),
            .Seed = seed,
            .Mmgbsa = mmgbsa,
            .Nwat = Math.Max(nwat, 0),
            .MmgbsaTop = If(mmgbsaTop <= 0, 3, mmgbsaTop)
        }
    End Function

    ''' <summary>
    ''' resolve the docking options object from the ``options`` argument, or 
    ''' create a new options object from the inline argument values
    ''' </summary>
    Private Function getOptions(options As Object,
                                boxCenter As Object,
                                boxHalf As Double,
                                exhaustiveness As Integer,
                                stepsPerRun As Integer,
                                numModes As Integer,
                                minRmsd As Double,
                                seed As Integer,
                                mmgbsa As Boolean,
                                nwat As Integer,
                                mmgbsaTop As Integer) As VinaCore.DockOptions

        Dim opts As VinaCore.DockOptions = TryCast(options, VinaCore.DockOptions)

        If opts IsNot Nothing Then
            Return opts
        End If

        Return createOptions(exhaustiveness, stepsPerRun, numModes, minRmsd,
                             boxCenter, boxHalf, seed, mmgbsa, nwat, mmgbsaTop)
    End Function

    ''' <summary>
    ''' parse the search box center coordinates from a R# numeric vector
    ''' </summary>
    Private Function asBoxCenter(boxCenter As Object) As Double()
        If boxCenter Is Nothing Then
            Return Nothing
        End If

        Dim v As Double() = CLRVector.asNumeric(boxCenter)

        If v Is Nothing OrElse v.Length < 3 Then
            Return Nothing
        End If

        Return New Double() {v(0), v(1), v(2)}
    End Function

    ''' <summary>
    ''' run the MM-GBSA / Nwat-MMGBSA re-scoring on the best docking poses
    ''' </summary>
    Private Sub rescoreMmgbsa(receptor As VinaCore.VinaMolecule,
                              ligand As VinaCore.VinaMolecule,
                              result As VinaModel.LigandResult,
                              opts As VinaCore.DockOptions)

        If result Is Nothing OrElse result.Poses Is Nothing Then
            Return
        End If

        Dim top As Integer = Math.Min(Math.Max(opts.MmgbsaTop, 1), result.Poses.Count)

        For k As Integer = 0 To top - 1
            Dim pose As VinaModel.Pose = result.Poses(k)

            If pose.Atoms Is Nothing OrElse pose.Atoms.Count = 0 Then
                Continue For
            End If

            Dim poseAtoms As New List(Of VinaCore.VinaAtom)

            For Each pa As VinaModel.PoseAtom In pose.Atoms
                Call poseAtoms.Add(New VinaCore.VinaAtom With {
                    .X = pa.X,
                    .Y = pa.Y,
                    .Z = pa.Z,
                    .Element = pa.Element,
                    .ChainID = "L",
                    .ResName = If(String.IsNullOrEmpty(pa.ResName), "LIG", pa.ResName),
                    .ResSeq = 1,
                    .AtomName = pa.AtomName,
                    .FromReceptor = False
                })
            Next

            ' the pose atom index is the same as the ligand atom index
            For i As Integer = 0 To poseAtoms.Count - 1
                If i < ligand.Atoms.Count Then
                    poseAtoms(i).Charge = ligand.Atoms(i).Charge
                Else
                    poseAtoms(i).Charge = 0
                End If
            Next

            Dim r As VinaCore.MmGbsaResult = VinaCore.DockEngine.MmGbsaRescore(receptor.Atoms, poseAtoms, opts.Nwat)

            pose.Mmgbsa = New VinaModel.MmGbsaResultDto With {
                .DeltaG = Math.Round(r.DeltaG, 3),
                .Vdw = Math.Round(r.Vdw, 3),
                .Elec = Math.Round(r.Elec, 3),
                .GbPolar = Math.Round(r.GbPolar, 3),
                .SasNonpolar = Math.Round(r.SasNonpolar, 3),
                .Nwat = r.NwatSelected
            }
        Next
    End Sub

    Private Function getPose(result As VinaModel.LigandResult, rank As Integer, env As Environment) As VinaModel.Pose
        If result Is Nothing OrElse result.Poses Is Nothing Then
            Return Nothing
        End If

        For Each pose As VinaModel.Pose In result.Poses
            If pose.Rank = rank Then
                Return pose
            End If
        Next

        ' fallback to the array index when the rank value is not assigned
        Dim index As Integer = rank - 1

        If index >= 0 AndAlso index < result.Poses.Count Then
            Return result.Poses(index)
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' convert the pose atoms into a PDB molecule structure object
    ''' </summary>
    Private Function poseToMolecule(pose As VinaModel.Pose, resName As String, chain As String) As Structures.Molecule(Of Structures.Atom)
        Dim mol As New Structures.Molecule(Of Structures.Atom) With {
            .Id = $"pose_{pose.Rank}"
        }
        Dim name As String = If(String.IsNullOrEmpty(resName), "LIG", resName)
        Dim id As String = If(String.IsNullOrEmpty(chain), "L", chain)

        For i As Integer = 0 To pose.Atoms.Count - 1
            Dim pa As VinaModel.PoseAtom = pose.Atoms(i)

            Call mol.Atoms.Add(New Structures.Atom With {
                .Serial = i + 1,
                .AtomName = If(String.IsNullOrEmpty(pa.AtomName), $"{pa.Element}{i + 1}", pa.AtomName),
                .Element = pa.Element,
                .AltLoc = "",
                .ResName = name,
                .ChainID = id,
                .ResSeq = If(pa.ResSeq <= 0, 1, pa.ResSeq),
                .X = pa.X,
                .Y = pa.Y,
                .Z = pa.Z,
                .Occupancy = 1.0,
                .TempFactor = 0.0,
                .Charge = 0.0,
                .IsHet = True,
                .IsWater = False
            })
        Next

        Return mol
    End Function

    ''' <summary>
    ''' build the docking report DTO object
    ''' </summary>
    Private Function buildReport(result As VinaModel.LigandResult,
                                 opts As VinaCore.DockOptions,
                                 receptorAtoms As Integer,
                                 mode As String) As VinaModel.DockReport

        Return New VinaModel.DockReport With {
            .Program = "MiniDock",
            .Version = "1.0.0",
            .Mode = mode,
            .Parameters = New VinaModel.DockParameters With {
                .Exhaustiveness = opts.Exhaustiveness,
                .StepsPerRun = opts.StepsPerRun,
                .NumModes = opts.NumModes,
                .MinRmsd = opts.MinRmsd,
                .BoxCenter = opts.BoxCenter,
                .BoxHalfSize = opts.BoxHalfSize,
                .TemperatureMetropolis = Math.Round(293.15 * 0.001987, 4),
                .Weights = {-0.0356, -0.00516, 0.84, -0.0351, -0.587},
                .Mmgbsa = opts.Mmgbsa,
                .Nwat = opts.Nwat,
                .ReceptorAtoms = receptorAtoms,
                .Seed = opts.Seed
            },
            .Results = New List(Of VinaModel.LigandResult) From {result}
        }
    End Function

    Private Function jsonOptions(pretty As Boolean) As JsonSerializerOptions
        Return New JsonSerializerOptions With {
            .WriteIndented = pretty,
            .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }
    End Function

    Private Function checkFile(path As String, env As Environment) As Message
        If String.IsNullOrWhiteSpace(path) Then
            Return RInternal.debug.stop("the given structure file path is empty!", env)
        End If
        If Not IO.File.Exists(path) Then
            Return RInternal.debug.stop($"the given structure file does not exists: {path}", env)
        End If

        Return Nothing
    End Function

    Private Sub ensureDirectory(path As String)
        Dim folder As String = IO.Path.GetDirectoryName(IO.Path.GetFullPath(path))

        If Not String.IsNullOrEmpty(folder) AndAlso Not IO.Directory.Exists(folder) Then
            Call IO.Directory.CreateDirectory(folder)
        End If
    End Sub

#End Region

#Region "表格与控制台输出"

    Private Function moleculeFrame(x As Object, args As list, env As Environment) As dataframe
        Dim mol As VinaCore.VinaMolecule = TryCast(x, VinaCore.VinaMolecule)

        Return moleculeFrame(mol)
    End Function

    Private Function moleculeFrame(mol As VinaCore.VinaMolecule) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If mol Is Nothing Then
            Return table
        End If

        Dim index As New List(Of Integer)
        Dim element As New List(Of String)
        Dim vinaType As New List(Of String)
        Dim charge As New List(Of Double)
        Dim ljEps As New List(Of Double)
        Dim ljRmin As New List(Of Double)
        Dim x As New List(Of Double)
        Dim y As New List(Of Double)
        Dim z As New List(Of Double)
        Dim chain As New List(Of String)
        Dim resName As New List(Of String)
        Dim resSeq As New List(Of Integer)
        Dim atomName As New List(Of String)
        Dim fromReceptor As New List(Of Boolean)
        Dim isWater As New List(Of Boolean)

        For i As Integer = 0 To mol.Atoms.Count - 1
            Dim a As VinaCore.VinaAtom = mol.Atoms(i)

            Call index.Add(i)
            Call element.Add(a.Element)
            Call vinaType.Add(VinaCore.VinaAtomTypes.TypeName(a.VinaType))
            Call charge.Add(a.Charge)
            Call ljEps.Add(a.LjEps)
            Call ljRmin.Add(a.LjRmin)
            Call x.Add(a.X)
            Call y.Add(a.Y)
            Call z.Add(a.Z)
            Call chain.Add(a.ChainID)
            Call resName.Add(a.ResName)
            Call resSeq.Add(a.ResSeq)
            Call atomName.Add(a.AtomName)
            Call fromReceptor.Add(a.FromReceptor)
            Call isWater.Add(a.IsWater)
        Next

        Call table.columns.Add("index", index.ToArray)
        Call table.columns.Add("element", element.ToArray)
        Call table.columns.Add("vina_type", vinaType.ToArray)
        Call table.columns.Add("charge", charge.ToArray)
        Call table.columns.Add("lj_eps", ljEps.ToArray)
        Call table.columns.Add("lj_rmin", ljRmin.ToArray)
        Call table.columns.Add("x", x.ToArray)
        Call table.columns.Add("y", y.ToArray)
        Call table.columns.Add("z", z.ToArray)
        Call table.columns.Add("chain", chain.ToArray)
        Call table.columns.Add("res_name", resName.ToArray)
        Call table.columns.Add("res_seq", resSeq.ToArray)
        Call table.columns.Add("atom_name", atomName.ToArray)
        Call table.columns.Add("from_receptor", fromReceptor.ToArray)
        Call table.columns.Add("is_water", isWater.ToArray)

        Return table
    End Function

    Private Function posesFrame(x As Object, args As list, env As Environment) As dataframe
        Dim result As VinaModel.LigandResult = TryCast(x, VinaModel.LigandResult)

        Return posesFrame(result)
    End Function

    Private Function posesFrame(result As VinaModel.LigandResult) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If result Is Nothing OrElse result.Poses Is Nothing Then
            Return table
        End If

        Dim rank As New List(Of Integer)
        Dim vinaScore As New List(Of Double)
        Dim inter As New List(Of Double)
        Dim intra As New List(Of Double)
        Dim torsions As New List(Of Integer)
        Dim deltaG As New List(Of Double)
        Dim vdw As New List(Of Double)
        Dim elec As New List(Of Double)
        Dim gbPolar As New List(Of Double)
        Dim sasa As New List(Of Double)
        Dim nwat As New List(Of Integer)

        For Each pose As VinaModel.Pose In result.Poses
            Call rank.Add(pose.Rank)
            Call vinaScore.Add(pose.VinaScore)
            Call inter.Add(pose.Intermolecular)
            Call intra.Add(pose.Intramolecular)
            Call torsions.Add(pose.NumTorsions)

            If pose.Mmgbsa Is Nothing Then
                Call deltaG.Add(Double.NaN)
                Call vdw.Add(Double.NaN)
                Call elec.Add(Double.NaN)
                Call gbPolar.Add(Double.NaN)
                Call sasa.Add(Double.NaN)
                Call nwat.Add(0)
            Else
                Call deltaG.Add(pose.Mmgbsa.DeltaG)
                Call vdw.Add(pose.Mmgbsa.Vdw)
                Call elec.Add(pose.Mmgbsa.Elec)
                Call gbPolar.Add(pose.Mmgbsa.GbPolar)
                Call sasa.Add(pose.Mmgbsa.SasNonpolar)
                Call nwat.Add(pose.Mmgbsa.Nwat)
            End If
        Next

        Call table.columns.Add("rank", rank.ToArray)
        Call table.columns.Add("vina_score", vinaScore.ToArray)
        Call table.columns.Add("intermolecular", inter.ToArray)
        Call table.columns.Add("intramolecular", intra.ToArray)
        Call table.columns.Add("num_torsions", torsions.ToArray)
        Call table.columns.Add("mmgbsa_delta_g", deltaG.ToArray)
        Call table.columns.Add("mmgbsa_vdw", vdw.ToArray)
        Call table.columns.Add("mmgbsa_elec", elec.ToArray)
        Call table.columns.Add("mmgbsa_gb_polar", gbPolar.ToArray)
        Call table.columns.Add("mmgbsa_sas_nonpolar", sasa.ToArray)
        Call table.columns.Add("mmgbsa_nwat", nwat.ToArray)

        Return table
    End Function

    Private Function poseFrame(pose As VinaModel.Pose) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If pose Is Nothing OrElse pose.Atoms Is Nothing Then
            Return table
        End If

        Dim element As New List(Of String)
        Dim x As New List(Of Double)
        Dim y As New List(Of Double)
        Dim z As New List(Of Double)
        Dim chain As New List(Of String)
        Dim resName As New List(Of String)
        Dim resSeq As New List(Of Integer)
        Dim atomName As New List(Of String)

        For Each pa As VinaModel.PoseAtom In pose.Atoms
            Call element.Add(pa.Element)
            Call x.Add(pa.X)
            Call y.Add(pa.Y)
            Call z.Add(pa.Z)
            Call chain.Add(pa.Chain)
            Call resName.Add(pa.ResName)
            Call resSeq.Add(pa.ResSeq)
            Call atomName.Add(pa.AtomName)
        Next

        Call table.columns.Add("element", element.ToArray)
        Call table.columns.Add("x", x.ToArray)
        Call table.columns.Add("y", y.ToArray)
        Call table.columns.Add("z", z.ToArray)
        Call table.columns.Add("chain", chain.ToArray)
        Call table.columns.Add("res_name", resName.ToArray)
        Call table.columns.Add("res_seq", resSeq.ToArray)
        Call table.columns.Add("atom_name", atomName.ToArray)

        Return table
    End Function

    Private Function mmgbsaFrame(x As Object, args As list, env As Environment) As dataframe
        Dim result As VinaCore.MmGbsaResult = TryCast(x, VinaCore.MmGbsaResult)

        Return mmgbsaFrame(result)
    End Function

    Private Function mmgbsaFrame(result As VinaCore.MmGbsaResult) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = {"complex"}
        }

        If result Is Nothing Then
            Return table
        End If

        Call table.columns.Add("delta_g", {result.DeltaG})
        Call table.columns.Add("vdw", {result.Vdw})
        Call table.columns.Add("elec", {result.Elec})
        Call table.columns.Add("gb_polar", {result.GbPolar})
        Call table.columns.Add("sas_nonpolar", {result.SasNonpolar})
        Call table.columns.Add("nwat_requested", {result.NwatRequested})
        Call table.columns.Add("nwat_selected", {result.NwatSelected})

        Return table
    End Function

    Private Function showMolecule(mol As Object) As String
        Dim data As VinaCore.VinaMolecule = TryCast(mol, VinaCore.VinaMolecule)

        If data Is Nothing Then
            Return "<nothing>"
        End If

        Dim groups = data.Atoms _
            .GroupBy(Function(a) VinaCore.VinaAtomTypes.TypeName(a.VinaType)) _
            .OrderByDescending(Function(g) g.Count()) _
            .Select(Function(g) $"{g.Key}={g.Count()}")

        Dim netCharge As Double = data.Atoms.Sum(Function(a) a.Charge)
        Dim waters As Integer = data.Atoms.Where(Function(a) a.IsWater).Count()

        Return $"{If(String.IsNullOrEmpty(data.Id), "molecule", data.Id)} | atoms={data.Atoms.Count} " &
               $"bonds={data.Bonds.Count} waters={waters} " &
               $"charge={netCharge:F3} | " & String.Join(", ", groups)
    End Function

    Private Function showDockResult(result As Object) As String
        Dim data As VinaModel.LigandResult = TryCast(result, VinaModel.LigandResult)

        If data Is Nothing Then
            Return "<nothing>"
        End If

        Dim text As New Text.StringBuilder
        Dim poses As Integer = If(data.Poses Is Nothing, 0, data.Poses.Count)

        Call text.AppendLine($"{If(String.IsNullOrEmpty(data.Id), "ligand", data.Id)} | atoms={data.NumAtoms} " &
                             $"rotatable={data.NumRotatableBonds} poses={poses}")

        If poses > 0 Then
            Call text.AppendLine("rank  vina_score  intermol  rotors  mmgbsa_delta_g")

            For Each pose As VinaModel.Pose In data.Poses
                Dim dg As String = If(pose.Mmgbsa Is Nothing, "-", pose.Mmgbsa.DeltaG.ToString("F3"))

                Call text.AppendLine($"{pose.Rank}     {pose.VinaScore:F3}       {pose.Intermolecular:F3}     " &
                                     $"{pose.NumTorsions}       {dg}")
            Next
        End If

        Return text.ToString.Trim
    End Function

    Private Function showMmGbsa(result As Object) As String
        Dim data As VinaCore.MmGbsaResult = TryCast(result, VinaCore.MmGbsaResult)

        If data Is Nothing Then
            Return "<nothing>"
        End If

        Return $"MM-GBSA | delta_g={data.DeltaG:F3} vdw={data.Vdw:F3} elec={data.Elec:F3} " &
               $"gb_polar={data.GbPolar:F3} sasa={data.SasNonpolar:F3} " &
               $"nwat={data.NwatSelected}/{data.NwatRequested}"
    End Function

#End Region

End Module
