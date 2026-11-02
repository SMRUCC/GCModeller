Imports System.Drawing
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Molecule.SMILES.Graph
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports ChemRenderMode = SMILESView.RenderMode
Imports Imaging = Microsoft.VisualBasic.Imaging
Imports Mol = SMRUCC.genomics.Molecule.SMILES.Graph
Imports Molecules = SMRUCC.genomics.Molecule.SMILES
Imports PdbStructure = SMRUCC.genomics.Data.RCSB.PDB.Structures
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports Styling = SMILESView.Styling
Imports Viewer = SMILESView.StructureViewer

''' <summary>
''' molecule structure toolkit for the R# scripting language: SMILES/InChI parsing, 
''' molecular graph query, 2D/3D coordinates calculation, PDB structure export and 
''' the 2D/3D molecule structure plot.
''' </summary>
''' <remarks>
''' the ``plot`` function overloading of the <see cref="Mol.MoleculeGraph"/> and 
''' <see cref="Mol.MoleculeGraph"/> array is attached in the <c>Main</c> function
''' of current package module.
''' </remarks>
<Package("molecular", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Public Module molecular_tools

    Sub Main()
        Call RInternal.generic.add("plot", GetType(Mol.MoleculeGraph), AddressOf plotMolecule)
        Call RInternal.generic.add("plot", GetType(Mol.MoleculeGraph()), AddressOf plotMolecules)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(Mol.MoleculeGraph), AddressOf toMoleculeFrame)
        Call RInternal.ConsolePrinter.AttachConsoleFormatter(Of Mol.MoleculeGraph)(AddressOf showMolecule)
    End Sub

#Region "解析"

    ''' <summary>
    ''' parse the given SMILES string as a molecule graph object
    ''' </summary>
    ''' <param name="smiles">the SMILES string text</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a <see cref="Mol.MoleculeGraph"/> object, or a <see cref="Message"/> 
    ''' error object if the given SMILES text is invalid.
    ''' </returns>
    ''' <example>
    ''' let phenol = parse_smiles("c1ccccc1O");
    ''' </example>
    <ExportAPI("parse_smiles")>
    <RApiReturn(GetType(Mol.MoleculeGraph))>
    Public Function parseSmiles(smiles As String, Optional env As Environment = Nothing) As Object
        If String.IsNullOrWhiteSpace(smiles) Then
            Return RInternal.debug.stop("the given smiles text is empty!", env)
        End If

        Try
            Return Molecules.MoleculeIO.ParseSmiles(smiles)
        Catch ex As Exception
            Return RInternal.debug.stop($"invalid smiles: {ex.Message}", env)
        End Try
    End Function

    ''' <summary>
    ''' parse the given InChI string as a molecule graph object
    ''' </summary>
    ''' <param name="inchi">the InChI string text</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a <see cref="Mol.MoleculeGraph"/> object, or a <see cref="Message"/> 
    ''' error object if the given InChI text is invalid.
    ''' </returns>
    ''' <example>
    ''' let phenol = parse_inchi("InChI=1S/C6H6O/c7-6-4-2-1-3-5-6/h1-5,7H");
    ''' </example>
    <ExportAPI("parse_inchi")>
    <RApiReturn(GetType(Mol.MoleculeGraph))>
    Public Function parseInChI(inchi As String, Optional env As Environment = Nothing) As Object
        If String.IsNullOrWhiteSpace(inchi) Then
            Return RInternal.debug.stop("the given inchi text is empty!", env)
        End If

        Try
            Return Molecules.MoleculeIO.ParseInChI(inchi)
        Catch ex As Exception
            Return RInternal.debug.stop($"invalid inchi: {ex.Message}", env)
        End Try
    End Function

    ''' <summary>
    ''' parse the given structure text as a molecule graph object, the text 
    ''' format(SMILES/InChI) is auto detected by default
    ''' </summary>
    ''' <param name="text">the SMILES or InChI string text</param>
    ''' <param name="format">
    ''' the structure text format: ``smiles`` or ``inchi``; a NULL value 
    ''' means auto detect the text format.
    ''' </param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a <see cref="Mol.MoleculeGraph"/> object, or a <see cref="Message"/> 
    ''' error object if the given text is invalid.
    ''' </returns>
    ''' <example>
    ''' let mol = parse_molecule("c1ccccc1O");
    ''' let mol = parse_molecule("InChI=1S/CH4/h1H4", format = "inchi");
    ''' </example>
    <ExportAPI("parse_molecule")>
    <RApiReturn(GetType(Mol.MoleculeGraph))>
    Public Function parseMolecule(text As String,
                                  Optional format As String = Nothing,
                                  Optional env As Environment = Nothing) As Object
        If String.IsNullOrWhiteSpace(text) Then
            Return RInternal.debug.stop("the given structure text is empty!", env)
        End If

        Try
            Return Molecules.MoleculeIO.Parse(text, parseFormat(format))
        Catch ex As Exception
            Return RInternal.debug.stop($"invalid molecule structure text: {ex.Message}", env)
        End Try
    End Function

    ''' <summary>
    ''' parse a vector of the structure text as a molecule graph collection
    ''' </summary>
    ''' <param name="text">the SMILES or InChI string vector</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a vector of the <see cref="Mol.MoleculeGraph"/> object which can be 
    ''' used by the ``plot`` function directly.
    ''' </returns>
    ''' <example>
    ''' plot(parse_molecules(c("CCO", "c1ccccc1O")), ncol = 2);
    ''' </example>
    <ExportAPI("parse_molecules")>
    <RApiReturn(GetType(Mol.MoleculeGraph))>
    Public Function parseMolecules(text As String(), Optional env As Environment = Nothing) As Object
        If text Is Nothing OrElse text.Length = 0 Then
            Return RInternal.debug.stop("the given structure text vector is empty!", env)
        End If

        Dim molecules As New List(Of Mol.MoleculeGraph)

        For Each line As String In text
            If String.IsNullOrWhiteSpace(line) Then
                Continue For
            End If

            Try
                Call molecules.Add(molecules.MoleculeIO.Parse(line))
            Catch ex As Exception
                Return RInternal.debug.stop($"invalid molecule structure text '{line}': {ex.Message}", env)
            End Try
        Next

        Return molecules.ToArray
    End Function

    ''' <summary>
    ''' read the structure text from a given text file(or a stream) and then 
    ''' parse it as a molecule graph object
    ''' </summary>
    ''' <param name="file">
    ''' a file path string or a <see cref="Stream"/> object which contains 
    ''' the SMILES or InChI text data.
    ''' </param>
    ''' <param name="format">
    ''' the structure text format: ``smiles`` or ``inchi``; a NULL value 
    ''' means auto detect the text format.
    ''' </param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a <see cref="Mol.MoleculeGraph"/> object, or a <see cref="Message"/> 
    ''' error object if the file could not be read or parsed.
    ''' </returns>
    ''' <example>
    ''' let mol = read.molecule("./phenol.smiles");
    ''' </example>
    <ExportAPI("read.molecule")>
    <RApiReturn(GetType(Mol.MoleculeGraph))>
    Public Function readMolecule(<RRawVectorArgument>
                                 file As Object,
                                 Optional format As String = Nothing,
                                 Optional env As Environment = Nothing) As Object

        Dim is_path As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env, is_filepath:=is_path)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Dim stream As Stream = s.TryCast(Of Stream)

        Try
            Dim text As String = New StreamReader(stream, Encoding.UTF8).ReadToEnd()

            Return parseMolecule(text, format, env)
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to read the molecule structure file: {ex.Message}", env)
        Finally
            If is_path Then
                Call stream.Dispose()
            End If
        End Try
    End Function

    ''' <summary>
    ''' write the given molecule graph as a SMILES string(Kekulé form)
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the SMILES string text</returns>
    ''' <example>
    ''' print(write_smiles(parse_smiles("c1ccccc1O")));
    ''' </example>
    <ExportAPI("write_smiles")>
    <RApiReturn(GetType(String))>
    Public Function writeSmiles(mol As Mol.MoleculeGraph, Optional env As Environment = Nothing) As Object
        If mol Is Nothing Then
            Return RInternal.debug.stop("the given molecule graph is nothing!", env)
        End If

        Return Molecules.MoleculeIO.WriteSmiles(mol)
    End Function

    ''' <summary>
    ''' get the structure text format of the given molecule graph
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <returns>the format name: ``smiles``, ``inchi`` or ``unknown``</returns>
    ''' <example>
    ''' print(molecule_format(parse_smiles("CCO")));   # [1] "smiles"
    ''' </example>
    <ExportAPI("molecule_format")>
    <RApiReturn(GetType(String))>
    Public Function getMoleculeFormat(mol As Mol.MoleculeGraph) As Object
        If mol Is Nothing Then
            Return "unknown"
        End If

        Select Case mol.Format
            Case MoleculeFormat.InChI : Return "inchi"
            Case MoleculeFormat.Smiles : Return "smiles"
            Case Else : Return "unknown"
        End Select
    End Function

#End Region

#Region "信息查询"

    ''' <summary>
    ''' get the molecular formula of the given molecule graph(Hill notation, 
    ''' implicit hydrogens are included)
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <returns>the molecular formula string, example as ``C6H6O``</returns>
    ''' <example>
    ''' print(molecule_formula(parse_smiles("c1ccccc1O")));   # [1] "C6H6O"
    ''' </example>
    <ExportAPI("molecule_formula")>
    <RApiReturn(GetType(String))>
    Public Function moleculeFormula(mol As Mol.MoleculeGraph) As Object
        If mol Is Nothing Then
            Return Nothing
        End If

        Return mol.MolecularFormula()
    End Function

    ''' <summary>
    ''' get the relative molecular weight of the given molecule graph
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <returns>the relative molecular weight value</returns>
    ''' <example>
    ''' print(molecule_weight(parse_smiles("c1ccccc1O")));
    ''' </example>
    <ExportAPI("molecule_weight")>
    <RApiReturn(GetType(Double))>
    Public Function moleculeWeight(mol As Mol.MoleculeGraph) As Object
        If mol Is Nothing Then
            Return Nothing
        End If

        Return mol.MolecularWeight()
    End Function

    ''' <summary>
    ''' get the atom table of the given molecule graph
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <returns>
    ''' a dataframe object with the data fields: index, element, charge, isotope, 
    ''' aromatic, hydrogen, degree, hybridization, x, y and z.
    ''' </returns>
    ''' <example>
    ''' print(molecule_atoms(parse_smiles("c1ccccc1O")));
    ''' </example>
    <ExportAPI("molecule_atoms")>
    <RApiReturn(GetType(dataframe))>
    Public Function moleculeAtoms(mol As Mol.MoleculeGraph) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If mol Is Nothing Then
            Return table
        End If

        Dim index As New List(Of Integer)
        Dim element As New List(Of String)
        Dim charge As New List(Of Integer)
        Dim isotope As New List(Of Integer)
        Dim aromatic As New List(Of Boolean)
        Dim hydrogen As New List(Of Integer)
        Dim degree As New List(Of Integer)
        Dim hybridization As New List(Of String)
        Dim x As New List(Of Double)
        Dim y As New List(Of Double)
        Dim z As New List(Of Double)

        For i As Integer = 0 To mol.AtomCount - 1
            Dim atom As Mol.Atom = mol.Atoms(i)

            Call index.Add(atom.Index)
            Call element.Add(atom.Element)
            Call charge.Add(atom.Charge)
            Call isotope.Add(atom.Isotope)
            Call aromatic.Add(atom.IsAromatic)
            Call hydrogen.Add(mol.TotalH(i))
            Call degree.Add(mol.Degree(i))
            Call hybridization.Add(mol.Hybridization(i).ToString)
            Call x.Add(atom.X)
            Call y.Add(atom.Y)
            Call z.Add(atom.Z)
        Next

        Call table.columns.Add("index", index.ToArray)
        Call table.columns.Add("element", element.ToArray)
        Call table.columns.Add("charge", charge.ToArray)
        Call table.columns.Add("isotope", isotope.ToArray)
        Call table.columns.Add("aromatic", aromatic.ToArray)
        Call table.columns.Add("hydrogen", hydrogen.ToArray)
        Call table.columns.Add("degree", degree.ToArray)
        Call table.columns.Add("hybridization", hybridization.ToArray)
        Call table.columns.Add("x", x.ToArray)
        Call table.columns.Add("y", y.ToArray)
        Call table.columns.Add("z", z.ToArray)

        Return table
    End Function

    ''' <summary>
    ''' get the chemical bond table of the given molecule graph
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <returns>
    ''' a dataframe object with the data fields: a, b, order, aromatic, stereo 
    ''' and ring. the atom index value in the ``a`` and ``b`` field is 0 based.
    ''' </returns>
    ''' <example>
    ''' print(molecule_bonds(parse_smiles("c1ccccc1O")));
    ''' </example>
    <ExportAPI("molecule_bonds")>
    <RApiReturn(GetType(dataframe))>
    Public Function moleculeBonds(mol As Mol.MoleculeGraph) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If mol Is Nothing Then
            Return table
        End If

        Dim a As New List(Of Integer)
        Dim b As New List(Of Integer)
        Dim order As New List(Of Double)
        Dim aromatic As New List(Of Boolean)
        Dim stereo As New List(Of String)
        Dim ring As New List(Of Boolean)

        For Each bond As Mol.Bond In mol.Bonds
            Call a.Add(bond.a)
            Call b.Add(bond.b)
            Call order.Add(bond.order)
            Call aromatic.Add(bond.IsAromatic)
            Call stereo.Add(bond.Stereo.ToString)
            Call ring.Add(bond.IsRing)
        Next

        Call table.columns.Add("a", a.ToArray)
        Call table.columns.Add("b", b.ToArray)
        Call table.columns.Add("order", order.ToArray)
        Call table.columns.Add("aromatic", aromatic.ToArray)
        Call table.columns.Add("stereo", stereo.ToArray)
        Call table.columns.Add("ring", ring.ToArray)

        Return table
    End Function

    ''' <summary>
    ''' get the minimum ring basis(SSSR) of the given molecule graph
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <returns>
    ''' a list object which contains the atom index vector of each ring, the 
    ''' atom index value is 0 based.
    ''' </returns>
    ''' <example>
    ''' print(molecule_rings(parse_smiles("c1ccccc1O")));
    ''' </example>
    <ExportAPI("molecule_rings")>
    Public Function moleculeRings(mol As Mol.MoleculeGraph) As Object
        Dim rings As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        If mol Is Nothing Then
            Return rings
        End If

        Dim data As List(Of Mol.Ring) = mol.Rings()

        For i As Integer = 0 To data.Count - 1
            rings.slots($"ring{i + 1}") = data(i).Atoms
        Next

        Return rings
    End Function

    ''' <summary>
    ''' get the connected components of the given molecule graph, this function 
    ''' is useful for splitting the multiple component molecule data, example 
    ''' as the salt form ``A.B``.
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <returns>a list object which contains the atom index vector of each component</returns>
    ''' <example>
    ''' print(molecule_components(parse_smiles("CCO.CC")));
    ''' </example>
    <ExportAPI("molecule_components")>
    Public Function moleculeComponents(mol As Mol.MoleculeGraph) As Object
        Dim parts As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        If mol Is Nothing Then
            Return parts
        End If

        Dim data As List(Of List(Of Integer)) = mol.Components()

        For i As Integer = 0 To data.Count - 1
            parts.slots($"component{i + 1}") = data(i).ToArray
        Next

        Return parts
    End Function

    ''' <summary>
    ''' get the non-fatal warning messages that generated during the molecule 
    ''' graph parsing and the coordinates rebuilding
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <returns>a character vector of the warning messages</returns>
    ''' <example>
    ''' print(molecule_warnings(parse_smiles("CCO")));
    ''' </example>
    <ExportAPI("molecule_warnings")>
    <RApiReturn(GetType(String))>
    Public Function moleculeWarnings(mol As Mol.MoleculeGraph) As Object
        If mol Is Nothing Then
            Return Nothing
        End If

        Return mol.Warnings.ToArray
    End Function

    ''' <summary>
    ''' get the basic structure summary of the given molecule graph
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <returns>
    ''' a dataframe object with a single data row and the data fields: formula, 
    ''' weight, atoms, bonds, rings, components and warnings.
    ''' </returns>
    ''' <example>
    ''' print(molecule_summary(parse_smiles("c1ccccc1O")));
    ''' </example>
    <ExportAPI("molecule_summary")>
    <RApiReturn(GetType(dataframe))>
    Public Function moleculeSummary(mol As Mol.MoleculeGraph) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = {"molecule"}
        }

        If mol Is Nothing Then
            Return table
        End If

        Call table.columns.Add("formula", {mol.MolecularFormula()})
        Call table.columns.Add("weight", {mol.MolecularWeight()})
        Call table.columns.Add("atoms", {mol.AtomCount})
        Call table.columns.Add("bonds", {mol.BondCount})
        Call table.columns.Add("rings", {mol.Rings().Count})
        Call table.columns.Add("components", {mol.Components().Count})
        Call table.columns.Add("warnings", {mol.Warnings.Count})

        Return table
    End Function

#End Region

#Region "坐标计算"

    ''' <summary>
    ''' rebuild the 2D plane coordinates of the given molecule graph, the 
    ''' coordinate value is written into the ``x`` and ``y`` data field of 
    ''' the atom object directly
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <param name="bond_length">the normalized bond length value, default is 1.0</param>
    ''' <param name="iterations">the force directed refinement iteration rounds, default is 300</param>
    ''' <param name="ideal_angle">the ideal bond angle in degree, default is 120</param>
    ''' <param name="min_separation">
    ''' the minimum allowed distance between the non-bonded atoms in the bond 
    ''' length unit, default is 0.85
    ''' </param>
    ''' <param name="seed">the random seed value for the deterministic layout result</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the given molecule graph object with the 2D coordinates updated</returns>
    ''' <example>
    ''' let mol = layout2d(parse_smiles("c1ccccc1O"));
    ''' </example>
    <ExportAPI("layout2d")>
    <RApiReturn(GetType(Mol.MoleculeGraph))>
    Public Function layout2D(mol As Mol.MoleculeGraph,
                             Optional bond_length As Double = 1.0,
                             Optional iterations As Integer = 300,
                             Optional ideal_angle As Double = 120,
                             Optional min_separation As Double = 0.85,
                             Optional seed As Integer = 12345,
                             Optional env As Environment = Nothing) As Object
        If mol Is Nothing Then
            Return RInternal.debug.stop("the given molecule graph is nothing!", env)
        End If

        Call mol.Layout2D(New Mol.Layout2DOptions With {
            .BondLength = bond_length,
            .Iterations = iterations,
            .IdealAngle = ideal_angle,
            .MinSeparation = min_separation,
            .RandomSeed = seed
        })

        Return mol
    End Function

    ''' <summary>
    ''' rebuild the 3D stereo coordinates of the given molecule graph, the 
    ''' coordinate value is written into the ``x``, ``y`` and ``z`` data field 
    ''' of the atom object directly
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <param name="iterations">the force field relaxation iteration rounds, default is 600</param>
    ''' <param name="add_hydrogens">
    ''' expand the implicit hydrogens as the explicit hydrogen atoms before 
    ''' the coordinates rebuilding? default is TRUE
    ''' </param>
    ''' <param name="respect_stereo">
    ''' correct the stereo configuration by the chirality marker? default is TRUE
    ''' </param>
    ''' <param name="seed">the random seed value for the deterministic embedding result</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the given molecule graph object with the 3D coordinates updated</returns>
    ''' <example>
    ''' let mol = embed3d(parse_smiles("c1ccccc1O"));
    ''' </example>
    <ExportAPI("embed3d")>
    <RApiReturn(GetType(Mol.MoleculeGraph))>
    Public Function embed3D(mol As Mol.MoleculeGraph,
                            Optional iterations As Integer = 600,
                            Optional add_hydrogens As Boolean = True,
                            Optional respect_stereo As Boolean = True,
                            Optional seed As Integer = 20240101,
                            Optional env As Environment = Nothing) As Object
        If mol Is Nothing Then
            Return RInternal.debug.stop("the given molecule graph is nothing!", env)
        End If

        Call mol.Embed3D(New Mol.Embed3DOptions With {
            .Iterations = iterations,
            .AddHydrogens = add_hydrogens,
            .RespectStereo = respect_stereo,
            .RandomSeed = seed
        })

        Return mol
    End Function

    ''' <summary>
    ''' expand all of the implicit hydrogens of the given molecule graph as the 
    ''' explicit hydrogen atoms
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the given molecule graph object with the explicit hydrogen atoms</returns>
    ''' <example>
    ''' let mol = add_hydrogens(parse_smiles("CCO"));
    ''' </example>
    <ExportAPI("add_hydrogens")>
    <RApiReturn(GetType(Mol.MoleculeGraph))>
    Public Function addHydrogens(mol As Mol.MoleculeGraph, Optional env As Environment = Nothing) As Object
        If mol Is Nothing Then
            Return RInternal.debug.stop("the given molecule graph is nothing!", env)
        End If

        Call mol.AddHydrogens()

        Return mol
    End Function

    ''' <summary>
    ''' remove all of the hydrogen atoms from the given molecule graph and keep 
    ''' the heavy atom skeleton only
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the given molecule graph object without the hydrogen atoms</returns>
    ''' <example>
    ''' let mol = remove_hydrogens(parse_smiles("CCO"));
    ''' </example>
    <ExportAPI("remove_hydrogens")>
    <RApiReturn(GetType(Mol.MoleculeGraph))>
    Public Function removeHydrogens(mol As Mol.MoleculeGraph, Optional env As Environment = Nothing) As Object
        If mol Is Nothing Then
            Return RInternal.debug.stop("the given molecule graph is nothing!", env)
        End If

        Call mol.RemoveHydrogens()

        Return mol
    End Function

    ''' <summary>
    ''' get the 3D coordinates table of the atoms in the given molecule graph
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <returns>
    ''' a dataframe object with the data fields: index, element, x, y and z
    ''' </returns>
    ''' <example>
    ''' print(molecule_coordinates(embed3d(parse_smiles("c1ccccc1O"))));
    ''' </example>
    <ExportAPI("molecule_coordinates")>
    <RApiReturn(GetType(dataframe))>
    Public Function moleculeCoordinates(mol As Mol.MoleculeGraph) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If mol Is Nothing Then
            Return table
        End If

        Dim index As New List(Of Integer)
        Dim element As New List(Of String)
        Dim x As New List(Of Double)
        Dim y As New List(Of Double)
        Dim z As New List(Of Double)

        For i As Integer = 0 To mol.AtomCount - 1
            Dim atom As Mol.Atom = mol.Atoms(i)

            Call index.Add(atom.Index)
            Call element.Add(atom.Element)
            Call x.Add(atom.X)
            Call y.Add(atom.Y)
            Call z.Add(atom.Z)
        Next

        Call table.columns.Add("index", index.ToArray)
        Call table.columns.Add("element", element.ToArray)
        Call table.columns.Add("x", x.ToArray)
        Call table.columns.Add("y", y.ToArray)
        Call table.columns.Add("z", z.ToArray)

        Return table
    End Function

#End Region

#Region "PDB导出"

    ''' <summary>
    ''' convert the given molecule graph as the PDB molecule structure object
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <param name="res_name">the residue name, default is ``UNL``</param>
    ''' <param name="chain">the chain identifier, default is ``A``</param>
    ''' <param name="res_seq">the residue sequence number, default is 1</param>
    ''' <param name="hetatm">
    ''' write the atom data as the ``HETATM`` record? default is TRUE, a FALSE 
    ''' value means the ``ATOM`` record will be used.
    ''' </param>
    ''' <param name="include_h">keep the hydrogen atoms in the exported structure? default is TRUE</param>
    ''' <param name="ensure_3d">
    ''' auto run the 3D coordinates embedding when the molecule graph has no 3D 
    ''' coordinates? default is TRUE
    ''' </param>
    ''' <param name="title">the structure title, a NULL value means use the molecular formula</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>a PDB molecule structure object</returns>
    ''' <example>
    ''' let pdb = as.pdb(parse_smiles("c1ccccc1O"));
    ''' </example>
    <ExportAPI("as.pdb")>
    <RApiReturn(GetType(PdbStructure.Molecule(Of PdbStructure.Atom)))>
    Public Function asPdb(mol As Mol.MoleculeGraph,
                          Optional res_name As String = "UNL",
                          Optional chain As String = "A",
                          Optional res_seq As Integer = 1,
                          Optional hetatm As Boolean = True,
                          Optional include_h As Boolean = True,
                          Optional ensure_3d As Boolean = True,
                          Optional title As String = Nothing,
                          Optional env As Environment = Nothing) As Object
        If mol Is Nothing Then
            Return RInternal.debug.stop("the given molecule graph is nothing!", env)
        End If

        Return Molecules.PDBExport.MoleculeToPdb.Convert(mol, pdbOptions(res_name, chain, res_seq, hetatm, include_h, ensure_3d, title))
    End Function

    ''' <summary>
    ''' convert the given molecule graph as the PDB structure file and then save 
    ''' it to the given file path
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <param name="file">the output PDB file path, the directory will be created automatically</param>
    ''' <param name="res_name">the residue name, default is ``UNL``</param>
    ''' <param name="chain">the chain identifier, default is ``A``</param>
    ''' <param name="res_seq">the residue sequence number, default is 1</param>
    ''' <param name="hetatm">write the atom data as the ``HETATM`` record? default is TRUE</param>
    ''' <param name="include_h">keep the hydrogen atoms in the exported structure? default is TRUE</param>
    ''' <param name="ensure_3d">auto run the 3D coordinates embedding if the coordinates is missing? default is TRUE</param>
    ''' <param name="title">the structure title, a NULL value means use the molecular formula</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the full file path of the generated PDB file</returns>
    ''' <example>
    ''' write.pdb(parse_smiles("c1ccccc1O"), "./phenol.pdb");
    ''' </example>
    <ExportAPI("write.pdb")>
    <RApiReturn(GetType(String))>
    Public Function writePdb(mol As Mol.MoleculeGraph,
                             file As String,
                             Optional res_name As String = "UNL",
                             Optional chain As String = "A",
                             Optional res_seq As Integer = 1,
                             Optional hetatm As Boolean = True,
                             Optional include_h As Boolean = True,
                             Optional ensure_3d As Boolean = True,
                             Optional title As String = Nothing,
                             Optional env As Environment = Nothing) As Object
        If mol Is Nothing Then
            Return RInternal.debug.stop("the given molecule graph is nothing!", env)
        End If
        If String.IsNullOrWhiteSpace(file) Then
            Return RInternal.debug.stop("the output pdb file path can not be empty!", env)
        End If

        Try
            Return Molecules.PDBExport.MoleculeToPdb.SaveAsPdb(
                mol,
                file,
                pdbOptions(res_name, chain, res_seq, hetatm, include_h, ensure_3d, title)
            )
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to write the pdb file: {ex.Message}", env)
        End Try
    End Function

#End Region

#Region "绘图"

    ''' <summary>
    ''' create a molecule structure plot theme object which can be reused by the 
    ''' ``plot``, ``save_png`` and ``save_svg`` function via the ``theme`` argument
    ''' </summary>
    ''' <param name="bg">the canvas background color, default is ``white``</param>
    ''' <param name="bond_color">the chemical bond color, default is ``#222222``</param>
    ''' <param name="bond_width">the chemical bond line width in pixel, default is 2</param>
    ''' <param name="bond_separation">the parallel line spacing of the double/triple bond in pixel, default is 4</param>
    ''' <param name="font">the atom label font name, default is ``Arial``</param>
    ''' <param name="atom_font_size">the atom label font size in pixel, default is 16</param>
    ''' <param name="title_font_size">the title font size in pixel, default is 15</param>
    ''' <param name="padding">the canvas padding in pixel, default is 28</param>
    ''' <param name="show_title">draw the structure text title on the top of the image? default is TRUE</param>
    ''' <param name="title_color">the title text color, default is ``#808080``</param>
    ''' <param name="hide_carbon">hide the carbon atom label? default is TRUE</param>
    ''' <param name="ball_scale">the atom ball radius scale of the ball and stick model, default is 0.5</param>
    ''' <param name="spacefill_scale">the atom ball radius scale of the space filling model, default is 1.0</param>
    ''' <param name="bond_radius">the bond cylinder radius in the 3D model, default is 0.13</param>
    ''' <param name="auto_orient">auto orient the molecule principal plane to the screen in the 3D plot? default is TRUE</param>
    ''' <param name="angle_x">the 3D camera euler angle X in degree, default is 12</param>
    ''' <param name="angle_y">the 3D camera euler angle Y in degree, default is -8</param>
    ''' <param name="angle_z">the 3D camera euler angle Z in degree, default is 0</param>
    ''' <param name="fov">the 3D camera field of view, default is 320</param>
    ''' <param name="sphere_stacks">the 3D sphere latitude segments, default is 10</param>
    ''' <param name="sphere_slices">the 3D sphere longitude segments, default is 16</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>a molecule structure plot theme object</returns>
    ''' <example>
    ''' plot(parse_smiles("c1ccccc1O"), theme = chem_theme(bg = "black", bond_color = "white"));
    ''' </example>
    <ExportAPI("chem_theme")>
    <RApiReturn(GetType(Styling.ChemTheme))>
    Public Function chemTheme(Optional bg As Object = "white",
                              Optional bond_color As Object = "#222222",
                              Optional bond_width As Double = 2,
                              Optional bond_separation As Double = 4,
                              Optional font As String = "Arial",
                              Optional atom_font_size As Double = 16,
                              Optional title_font_size As Double = 15,
                              Optional padding As Integer = 28,
                              Optional show_title As Boolean = True,
                              Optional title_color As Object = "#808080",
                              Optional hide_carbon As Boolean = True,
                              Optional ball_scale As Double = 0.5,
                              Optional spacefill_scale As Double = 1.0,
                              Optional bond_radius As Double = 0.13,
                              Optional auto_orient As Boolean = True,
                              Optional angle_x As Double = 12,
                              Optional angle_y As Double = -8,
                              Optional angle_z As Double = 0,
                              Optional fov As Double = 320,
                              Optional sphere_stacks As Integer = 10,
                              Optional sphere_slices As Integer = 16,
                              Optional env As Environment = Nothing) As Object

        Dim theme As New Styling.ChemTheme

        theme.BackgroundColor = getColorHex(bg, theme.BackgroundColor)
        theme.BondColor = getColorHex(bond_color, theme.BondColor)
        theme.BondWidth = CSng(bond_width)
        theme.BondSeparation = CSng(bond_separation)
        theme.FontName = If(String.IsNullOrEmpty(font), theme.FontName, font)
        theme.AtomFontSize = CSng(atom_font_size)
        theme.TitleFontSize = CSng(title_font_size)
        theme.Padding = Math.Max(padding, 0)
        theme.ShowTitle = show_title
        theme.TitleColor = getColorHex(title_color, theme.TitleColor)
        theme.HideCarbon = hide_carbon
        theme.BallScale = ball_scale
        theme.SpaceFillScale = spacefill_scale
        theme.BondRadius = bond_radius
        theme.AutoOrient = auto_orient
        theme.CameraAngleX = CSng(angle_x)
        theme.CameraAngleY = CSng(angle_y)
        theme.CameraAngleZ = CSng(angle_z)
        theme.FieldOfView = CSng(fov)
        theme.SphereStacks = sphere_stacks
        theme.SphereSlices = sphere_slices

        Return theme
    End Function

    ''' <summary>
    ''' plot the molecule structure as a 2D skeleton formula image or a 3D 
    ''' ball-and-stick / space-filling model image
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <param name="args">
    ''' the plot arguments, supports the data fields: size, dpi, dim, mode, theme 
    ''' and the inline theme parameters: bg, bond_color, bond_width, font, 
    ''' atom_font_size, title_font_size, padding, show_title, title_color, 
    ''' hide_carbon, ball_scale, spacefill_scale, bond_radius, auto_orient, 
    ''' angle_x, angle_y, angle_z, fov, sphere_stacks, sphere_slices.
    ''' </param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a raster image object of the molecule structure plot; a <see cref="Message"/> 
    ''' error object will be returned if the given molecule graph is invalid.
    ''' </returns>
    ''' <example>
    ''' plot(parse_smiles("c1ccccc1O"));
    ''' plot(layout2d(parse_smiles("c1ccccc1O")), size = "1600,1200");
    ''' plot(embed3d(parse_smiles("c1ccccc1O")), dim = 3, mode = "ball_and_stick");
    ''' plot(parse_smiles("c1ccccc1O"), dim = 3, mode = "space_filling");
    ''' </example>
    Public Function plotMolecule(mol As Mol.MoleculeGraph, args As list, env As Environment) As Object
        If mol Is Nothing Then
            Return RInternal.debug.stop("the given molecule graph is nothing!", env)
        End If
        If mol.IsEmpty Then
            Return RInternal.debug.stop("the given molecule graph is empty!", env)
        End If

        Dim theme As Styling.ChemTheme = getTheme(args, env)
        Dim size As Size = getSize(args, env, New SizeF(1200, 1000))
        Dim dpi As Integer = getDpi(args, env)
        Dim mode As ChemRenderMode = getRenderMode(args, env)
        Dim dim3 As Boolean = is3D(args, env)

        Return renderMolecule(mol, size, dpi, theme, dim3, mode)
    End Function

    ''' <summary>
    ''' plot a collection of the molecule structures as a grid image
    ''' </summary>
    ''' <param name="molecules">the molecule graph object collection</param>
    ''' <param name="args">
    ''' the plot arguments, supports the data fields: size, dpi, dim, mode, ncol, 
    ''' theme and the inline theme parameters which is same as the single 
    ''' molecule plot.
    ''' </param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>
    ''' a raster image object of the molecule structure grid plot; a 
    ''' <see cref="Message"/> error object will be returned if the given 
    ''' molecule collection is empty.
    ''' </returns>
    ''' <example>
    ''' plot(parse_molecules(c("CCO", "c1ccccc1O", "CC(=O)O")), ncol = 3);
    ''' </example>
    Public Function plotMolecules(molecules As Mol.MoleculeGraph(), args As list, env As Environment) As Object
        Dim data As Mol.MoleculeGraph() = molecules _
            .Where(Function(m) m IsNot Nothing AndAlso Not m.IsEmpty) _
            .ToArray

        If data.Length = 0 Then
            Return RInternal.debug.stop("the given molecule graph collection is empty!", env)
        End If

        Dim size As Size = getSize(args, env, New SizeF(1800, 1500))
        Dim dpi As Integer = getDpi(args, env)
        Dim mode As ChemRenderMode = getRenderMode(args, env)
        Dim dim3 As Boolean = is3D(args, env)
        Dim theme As Styling.ChemTheme = getTheme(args, env, forCell:=True)
        Dim ncol As Integer = args.getValue("ncol", env, CInt(Math.Ceiling(Math.Sqrt(data.Length))))

        If ncol <= 0 Then ncol = CInt(Math.Ceiling(Math.Sqrt(data.Length)))
        If ncol > data.Length Then ncol = data.Length

        Dim nrow As Integer = CInt(Math.Ceiling(data.Length / ncol))
        Dim cellSize As New Size(
            Math.Max(CInt(size.Width / ncol), 1),
            Math.Max(CInt(size.Height / nrow), 1)
        )
        Dim canvasSize As New Size(cellSize.Width * ncol, cellSize.Height * nrow)
        Dim bg As Color = Styling.ElementColors.ParseHex(theme.BackgroundColor, Color.White)
        Dim img As New Bitmap(canvasSize.Width, canvasSize.Height)

        Using gfx As Graphics = Graphics.FromImage(img)
            Call gfx.Clear(bg)

            For i As Integer = 0 To data.Length - 1
                Dim col As Integer = i Mod ncol
                Dim row As Integer = i \ ncol
                Dim offset As New Point(col * cellSize.Width, row * cellSize.Height)

                Using cell As Bitmap = renderMolecule(data(i), cellSize, dpi, theme, dim3, mode)
                    If cell Is Nothing Then
                        Continue For
                    End If

                    Call gfx.DrawImageUnscaled(cell, offset)
                End Using
            Next
        End Using

        Return img
    End Function

    ''' <summary>
    ''' render the molecule structure as a PNG image file and then save it to 
    ''' the given file path
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <param name="file">the output png image file path, the directory will be created automatically</param>
    ''' <param name="dim">the plot dimension: 2 for the 2D skeleton formula, 3 for the 3D model</param>
    ''' <param name="mode">the 3D render mode: ``ball_and_stick`` or ``space_filling``</param>
    ''' <param name="size">the image size, example as ``1600,1200``</param>
    ''' <param name="theme">the plot theme object that created by the ``chem_theme`` function</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>TRUE if the image file is saved successfully</returns>
    ''' <example>
    ''' save_png(parse_smiles("c1ccccc1O"), "./phenol-2d.png");
    ''' save_png(embed3d(parse_smiles("c1ccccc1O")), "./phenol-3d.png", dim = 3);
    ''' </example>
    <ExportAPI("save_png")>
    <RApiReturn(GetType(Boolean))>
    Public Function savePng(mol As Mol.MoleculeGraph,
                            file As String,
                            Optional [dim] As Integer = 2,
                            Optional mode As String = "ball_and_stick",
                            <RRawVectorArgument>
                            Optional size As Object = "1200,1000",
                            Optional theme As Object = Nothing,
                            Optional env As Environment = Nothing) As Object
        Return saveImage(mol, file, [dim], mode, size, theme, svg:=False, env:=env)
    End Function

    ''' <summary>
    ''' render the molecule structure as a SVG vector image file and then save 
    ''' it to the given file path
    ''' </summary>
    ''' <param name="mol">the target molecule graph object</param>
    ''' <param name="file">the output svg image file path, the directory will be created automatically</param>
    ''' <param name="dim">the plot dimension: 2 for the 2D skeleton formula, 3 for the 3D model</param>
    ''' <param name="mode">the 3D render mode: ``ball_and_stick`` or ``space_filling``</param>
    ''' <param name="size">the image size, example as ``1600,1200``</param>
    ''' <param name="theme">the plot theme object that created by the ``chem_theme`` function</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>TRUE if the image file is saved successfully</returns>
    ''' <example>
    ''' save_svg(parse_smiles("c1ccccc1O"), "./phenol-2d.svg");
    ''' </example>
    <ExportAPI("save_svg")>
    <RApiReturn(GetType(Boolean))>
    Public Function saveSvg(mol As Mol.MoleculeGraph,
                            file As String,
                            Optional [dim] As Integer = 2,
                            Optional mode As String = "ball_and_stick",
                            <RRawVectorArgument>
                            Optional size As Object = "1200,1000",
                            Optional theme As Object = Nothing,
                            Optional env As Environment = Nothing) As Object
        Return saveImage(mol, file, [dim], mode, size, theme, svg:=True, env:=env)
    End Function

#End Region

#Region "内部帮助函数"

    Private Function saveImage(mol As Mol.MoleculeGraph,
                               file As String,
                               [dim] As Integer,
                               mode As String,
                               size As Object,
                               theme As Object,
                               svg As Boolean,
                               env As Environment) As Object

        If mol Is Nothing Then
            Return RInternal.debug.stop("the given molecule graph is nothing!", env)
        End If
        If String.IsNullOrWhiteSpace(file) Then
            Return RInternal.debug.stop("the output image file path can not be empty!", env)
        End If

        Dim style As Styling.ChemTheme = TryCast(theme, Styling.ChemTheme)

        If style Is Nothing Then
            style = New Styling.ChemTheme
        End If

        Dim canvasSize As Size = graphicsPipeline.getSize(size, env, "1200,1000").SizeParser
        Dim renderMode As ChemRenderMode = parseRenderMode(mode)
        Dim viewer As New Viewer(mol, style)

        Try
            If [dim] >= 3 Then
                If svg Then
                    Return viewer.SaveSvg3D(file, canvasSize, renderMode)
                Else
                    Return viewer.SavePng3D(file, canvasSize, renderMode)
                End If
            Else
                If svg Then
                    Return viewer.SaveSvg2D(file, canvasSize)
                Else
                    Return viewer.SavePng2D(file, canvasSize)
                End If
            End If
        Catch ex As Exception
            Return RInternal.debug.stop($"failed to save the molecule structure image: {ex.Message}", env)
        End Try
    End Function

    ''' <summary>
    ''' render the given molecule graph to a raster bitmap image
    ''' </summary>
    Private Function renderMolecule(graph As Mol.MoleculeGraph,
                                    size As Size,
                                    dpi As Integer,
                                    theme As Styling.ChemTheme,
                                    dim3 As Boolean,
                                    mode As ChemRenderMode) As Bitmap

        Dim bg As Color = Styling.ElementColors.ParseHex(theme.BackgroundColor, Color.White)

        Using canvas As IGraphics = DriverLoad.CreateDefaultRasterGraphics(size, fill_color:=bg, dpi:=dpi)
            If dim3 Then
                Call New SMILESView.Draw3D.MoleculeDraw3D(graph, theme).Draw(canvas, mode)
            Else
                Call New SMILESView.Draw2D.MoleculeDraw2D(graph, theme).Draw(canvas)
            End If

            Return copyOfCanvas(canvas)
        End Using
    End Function

    ''' <summary>
    ''' copy the canvas image data as a standalone <see cref="Bitmap"/> object, 
    ''' the returned image is safe to use after the canvas is disposed.
    ''' </summary>
    Private Function copyOfCanvas(canvas As IGraphics) As Bitmap
        Dim raster As GdiRasterGraphics = TryCast(canvas, GdiRasterGraphics)

        If raster Is Nothing Then
            Return Nothing
        End If

        Using buffer As New MemoryStream
            Call raster.ImageResource.Save(buffer, Imaging.ImageFormats.Png)

            buffer.Position = 0

            Using raw As Image = Image.FromStream(buffer)
                Return New Bitmap(raw)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' build the molecule structure plot theme from the ``plot`` function 
    ''' arguments, the shared default theme object is never modified here.
    ''' </summary>
    Private Function getTheme(args As list, env As Environment, Optional forCell As Boolean = False) As Styling.ChemTheme
        Dim custom As Styling.ChemTheme = TryCast(args.getValue(Of Object)("theme", env, Nothing), Styling.ChemTheme)
        Dim theme As Styling.ChemTheme = If(custom Is Nothing, New Styling.ChemTheme, copyTheme(custom))
        Dim value As Object = argValue(args, env, "bg", "background")

        If value IsNot Nothing Then
            theme.BackgroundColor = getColorHex(value, theme.BackgroundColor)
        End If

        value = argValue(args, env, "bond_color", "bond.color")

        If value IsNot Nothing Then
            theme.BondColor = getColorHex(value, theme.BondColor)
        End If

        value = argValue(args, env, "title_color", "title.color")

        If value IsNot Nothing Then
            theme.TitleColor = getColorHex(value, theme.TitleColor)
        End If

        theme.BondWidth = CSng(args.getValue("bond_width", env, CDbl(theme.BondWidth)))
        theme.BondSeparation = CSng(args.getValue("bond_separation", env, CDbl(theme.BondSeparation)))
        theme.AtomFontSize = CSng(args.getValue("atom_font_size", env, CDbl(theme.AtomFontSize)))
        theme.TitleFontSize = CSng(args.getValue("title_font_size", env, CDbl(theme.TitleFontSize)))
        theme.Padding = args.getValue("padding", env, theme.Padding)
        theme.ShowTitle = args.getValue("show_title", env, theme.ShowTitle)
        theme.HideCarbon = args.getValue("hide_carbon", env, theme.HideCarbon)
        theme.BallScale = args.getValue("ball_scale", env, theme.BallScale)
        theme.SpaceFillScale = args.getValue("spacefill_scale", env, theme.SpaceFillScale)
        theme.BondRadius = args.getValue("bond_radius", env, theme.BondRadius)
        theme.AutoOrient = args.getValue("auto_orient", env, theme.AutoOrient)
        theme.CameraAngleX = CSng(args.getValue("angle_x", env, CDbl(theme.CameraAngleX)))
        theme.CameraAngleY = CSng(args.getValue("angle_y", env, CDbl(theme.CameraAngleY)))
        theme.CameraAngleZ = CSng(args.getValue("angle_z", env, CDbl(theme.CameraAngleZ)))
        theme.FieldOfView = CSng(args.getValue("fov", env, CDbl(theme.FieldOfView)))
        theme.SphereStacks = args.getValue("sphere_stacks", env, theme.SphereStacks)
        theme.SphereSlices = args.getValue("sphere_slices", env, theme.SphereSlices)

        Dim font As String = args.getValue(Of String)("font", env, Nothing)

        If font.StringEmpty(, True) Then
            font = args.getValue(Of String)("font_name", env, Nothing)
        End If
        If Not font.StringEmpty(, True) Then
            theme.FontName = font
        End If

        If forCell Then
            ' the cell plot should not draw the structure title and should 
            ' use a smaller padding value for avoid the overlap between cells
            theme.ShowTitle = False
            theme.Padding = Math.Max(4, CInt(theme.Padding / 2))
        End If

        Return theme
    End Function

    ''' <summary>
    ''' make a independent copy of the given theme object
    ''' </summary>
    Private Function copyTheme(source As Styling.ChemTheme) As Styling.ChemTheme
        Return New Styling.ChemTheme With {
            .BackgroundColor = source.BackgroundColor,
            .BondColor = source.BondColor,
            .BondWidth = source.BondWidth,
            .BondSeparation = source.BondSeparation,
            .FontName = source.FontName,
            .AtomFontSize = source.AtomFontSize,
            .TitleFontSize = source.TitleFontSize,
            .Padding = source.Padding,
            .ShowTitle = source.ShowTitle,
            .TitleColor = source.TitleColor,
            .LabelHaloAlpha = source.LabelHaloAlpha,
            .HideCarbon = source.HideCarbon,
            .AutoOrient = source.AutoOrient,
            .BallScale = source.BallScale,
            .SpaceFillScale = source.SpaceFillScale,
            .BondRadius = source.BondRadius,
            .CameraAngleX = source.CameraAngleX,
            .CameraAngleY = source.CameraAngleY,
            .CameraAngleZ = source.CameraAngleZ,
            .AmbientStrength = source.AmbientStrength,
            .FieldOfView = source.FieldOfView,
            .SphereStacks = source.SphereStacks,
            .SphereSlices = source.SphereSlices,
            .CylinderSegments = source.CylinderSegments,
            .CylinderRails = source.CylinderRails
        }
    End Function

    Private Function getSize(args As list, env As Environment, [default] As SizeF) As Size
        With graphicsPipeline.getSize(args, env, [default])
            Return New Size(Math.Max(CInt(.Width), 1), Math.Max(CInt(.Height), 1))
        End With
    End Function

    Private Function getDpi(args As list, env As Environment) As Integer
        Dim dpi As Integer = graphicsPipeline.getDpi(args.slots, env, [default]:=100)

        If dpi <= 0 Then dpi = 100

        Return dpi
    End Function

    Private Function is3D(args As list, env As Environment) As Boolean
        Return args.getValue("dim", env, 2) >= 3
    End Function

    Private Function getRenderMode(args As list, env As Environment) As ChemRenderMode
        Return parseRenderMode(args.getValue(Of String)("mode", env, "ball_and_stick"))
    End Function

    Private Function parseRenderMode(mode As String) As ChemRenderMode
        If mode.StringEmpty(, True) Then
            Return ChemRenderMode.BallAndStick
        End If

        Select Case mode.Trim.ToLower
            Case "space_filling", "space-filling", "spacefill", "space_fill", "vdw", "cpk"
                Return ChemRenderMode.SpaceFilling
            Case Else
                Return ChemRenderMode.BallAndStick
        End Select
    End Function

    Private Function parseFormat(format As String) As Mol.MoleculeFormat
        If format.StringEmpty(, True) Then
            Return Mol.MoleculeFormat.Unknown
        End If

        Select Case format.Trim.ToLower
            Case "smiles", "smi" : Return Mol.MoleculeFormat.Smiles
            Case "inchi", "inchi=1" : Return Mol.MoleculeFormat.InChI
            Case Else : Return Mol.MoleculeFormat.Unknown
        End Select
    End Function

    Private Function argValue(args As list, env As Environment, ParamArray names As String()) As Object
        For Each name As String In names
            Dim value As Object = args.getValue(Of Object)(name, env, Nothing)

            If value IsNot Nothing Then
                Return value
            End If
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' convert the R# color value to the ``#RRGGBB`` hexadecimal string
    ''' </summary>
    Private Function getColorHex(value As Object, Optional fallback$ = "#FFFFFF") As String
        If value Is Nothing Then
            Return fallback
        End If

        With graphicsPipeline.GetRawColor(value, fallback)
            Return $"#{ .R:X2}{ .G:X2}{ .B:X2}"
        End With
    End Function

    Private Function pdbOptions(res_name As String,
                                chain As String,
                                res_seq As Integer,
                                hetatm As Boolean,
                                include_h As Boolean,
                                ensure_3d As Boolean,
                                title As String) As Molecules.PDBExport.PdbExportOptions

        Return New Molecules.PDBExport.PdbExportOptions With {
            .ResName = If(res_name.StringEmpty(, True), "UNL", res_name),
            .ChainID = If(chain.StringEmpty(, True), "A", chain),
            .ResSeq = If(res_seq <= 0, 1, res_seq),
            .UseHetatm = hetatm,
            .IncludeHydrogens = include_h,
            .Ensure3D = ensure_3d,
            .Title = title
        }
    End Function

    Private Function toMoleculeFrame(x As Object, args As list, env As Environment) As dataframe
        Dim mol As Mol.MoleculeGraph = TryCast(x, Mol.MoleculeGraph)

        If mol Is Nothing Then
            Return New dataframe With {
                .columns = New Dictionary(Of String, Array)
            }
        End If

        Return moleculeAtoms(mol)
    End Function

    Private Function showMolecule(mol As Object) As String
        Return Molecules.MoleculeIO.Describe(TryCast(mol, Mol.MoleculeGraph))
    End Function

#End Region

End Module
