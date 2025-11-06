#Region "Microsoft.VisualBasic::9a64802ba54f6f31b815eba9978e2f12, data\RCSB PDB\PDB\PDB.vb"

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

'   Total Lines: 292
'    Code Lines: 127 (43.49%)
' Comment Lines: 139 (47.60%)
'    - Xml Docs: 85.61%
' 
'   Blank Lines: 26 (8.90%)
'     File Size: 12.11 KB


' Class PDB
' 
'     Properties: ANISOU, AtomStructures, Author, CAVEAT, CISPEP
'                 Compound, Conect, crystal1, DbRef, Experiment
'                 Formula, Header, Helix, Het, HetName
'                 HETSYN, Journal, Keywords, Links, Master
'                 Matrix1, Matrix2, Matrix3, MaxSpace, MDLTYP
'                 MinSpace, MODRES, NUMMDL, Origin1, Origin2
'                 Origin3, Remark, Revisions, Scale1, Scale2
'                 Scale3, seqadv, Sequence, Sheet, SIGATM
'                 SIGUIJ, Site, Source, SourceText, SPLIT
'                 SPRSDE, SSBOND, Title
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: GenericEnumerator, (+2 Overloads) Load, Parse, ToString
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords

''' <summary>
''' The **RCSB PDB file format** is a standardized text-based format used to represent 3D structural
''' data of biological macromolecules, such as proteins, nucleic acids, and viruses. Managed by the
''' Research Collaboratory for Structural Bioinformatics (RCSB), it serves as the primary format for 
''' entries in the Protein Data Bank (PDB), a global repository for experimentally determined structures. 
''' Below is a detailed introduction:
''' 
''' ---
''' 
''' ### **Key Features**
''' 
''' 1. **Text-Based Structure**:  
'''    Plain text file (`.pdb` extension) with a **fixed-column format**, meaning data is organized 
'''    into specific columns for consistency. Each line begins with a **record type** (e.g., `ATOM`, 
'''    `HETATM`, `HEADER`) that defines the data it contains.
''' 
''' 2. **Core Components**:  
'''    - **Atomic Coordinates**: Stored in `ATOM` (standard residues) and `HETATM` (heteroatoms, e.g., water, ligands) records.  
'''    - **Metadata**: Includes details like the title (`TITLE`), experimental method (`EXPDTA`), authors (`AUTHOR`), and biological source (`SOURCE`).  
'''    - **Sequence Information**: Provided in `SEQRES` lines.  
'''    - **Secondary Structure**: Annotated in `HELIX`, `SHEET`, and `TURN` records.  
'''    - **Connectivity**: Bonds between atoms are listed in `CONECT` lines.  
'''    - **Crystallographic Data**: Unit cell parameters (`CRYST1`), symmetry operations, and resolution.
''' 
''' 3. **Example ATOM/HETATM Line**:  
'''    ```
'''    ATOM   2301  CA  SER A 301      26.417  24.105  34.560  1.00 30.97           C  
'''    HETATM 9101  O   HOH A 910      10.500  20.100  30.500  1.00 25.00           O  
'''    ```
'''    - **Columns 1-6**: Record type (e.g., `ATOM`).  
'''    - **Columns 7-11**: Atom serial number.  
'''    - **Columns 13-16**: Atom name (e.g., `CA` for alpha carbon).  
'''    - **Columns 17-20**: Residue name (e.g., `SER` for serine).  
'''    - **Column 22**: Chain identifier (e.g., `A`).  
'''    - **Columns 23-26**: Residue number.  
'''    - **Columns 31-54**: X, Y, Z coordinates.  
'''    - **Columns 55-60**: Occupancy and temperature factor (B-factor).  
'''    - **Columns 77-78**: Element symbol (e.g., `C`, `O`).
''' 
''' ---
''' 
''' ### **Common Record Types**
''' | Record   | Description                                                               |
''' |----------|---------------------------------------------------------------------------|
''' | `HEADER` | Molecular type, deposition date, and PDB ID (e.g., `1ABC`).               |
''' | `TITLE`  | Title of the structure.                                                   |
''' | `COMPND` | Molecular components in the entry (e.g., protein, ligand, ion).           |
''' | `SEQRES` | Amino acid/nucleotide sequence of the macromolecule.                      |
''' | `ATOM`   | 3D coordinates of standard residues (e.g., amino acids in a protein).     |
''' | `HETATM` | Coordinates of heteroatoms (non-standard residues: ligands, water, ions). |
''' | `HELIX`  | Details of α-helices.                                                     |
''' | `SHEET`  | Details of β-sheets.                                                      |
''' | `CONECT` | Bonds between atoms not covered by standard residue templates.            |
''' | `REMARK` | Annotations, experimental details, or warnings.                           |
''' 
''' ---
''' 
''' ### **Limitations**
''' - **Column Width Restrictions**: Legacy format limits data fields (e.g., residue numbers up to 9999, atom serial numbers up to 99,999).  
''' - **Sparse Connectivity Data**: Bonds are often inferred rather than explicitly listed.  
''' - **No Support for Large Structures**: Superseded by the **mmCIF/PDBx format** (more flexible, supports larger datasets).
''' 
''' ---
''' 
''' ### **Modernization: mmCIF/PDBx Format**
''' The PDB now prioritizes the **mmCIF format** (Macromolecular Crystallographic Information File), which 
''' uses a flexible, key-value-based structure without column limits. Legacy PDB files are automatically 
''' converted to mmCIF for archiving.
''' 
''' ---
''' 
''' ### **Tools for Viewing/Editing**
''' - **Visualization**: PyMOL, Chimera, VMD, RCSB PDB Viewer.  
''' - **Analysis**: BioPython, MDAnalysis.  
''' - **Database Access**: [RCSB PDB website](https://www.rcsb.org/) (search, download, and explore entries).
''' 
''' ---
''' 
''' ### **Example PDB File Snippet**
''' ```
''' HEADER    HYDROLASE                             15-JUL-98   1ABC              
''' TITLE     CRYSTAL STRUCTURE OF EXAMPLE ENZYME                                 
''' COMPND    MOL_ID: 1;                                                           
''' COMPND   2 MOLECULE: EXAMPLE ENZYME; CHAIN: A;                                 
''' SEQRES   1 A  321  SER GLY LEU ARG TYR ...                                      
''' ATOM      1  N   SER A   1      10.000  20.000  30.000  1.00 25.00           N  
''' ATOM      2  CA  SER A   1      11.000  21.000  31.000  1.00 26.00           C  
''' HETATM 1001  O   HOH A 1001     40.000  50.000  60.000  1.00 30.00           O  
''' HELIX    1  ALA A 10 THR A 20  1                                            
''' CONECT 1001 1002
''' ```
''' 
''' ---
''' 
''' ### **Use Cases**
''' - Studying protein-ligand interactions.  
''' - Analyzing enzyme active sites.  
''' - Visualizing mutations in diseases.  
''' - Teaching structural biology concepts.
''' 
''' For more details, visit the [RCSB PDB](https://www.rcsb.org/) and explore entries like [1ATP](https://www.rcsb.org/structure/1ATP).
''' </summary>
''' <remarks>
''' pdb file is the struct data about a protein complex, one pdb file may includes 
''' multiple protein and metabolite compound data.
''' </remarks>
Public Class PDB : Implements Enumeration(Of Atom)

    Public Const REGEX_HEAD As String = "[A-Z]+\s+(\d+)?\s"

    Public Property Header As Header
    Public Property Title As Title
    Public Property Compound As Compound
    Public Property Source As Source
    Public Property Keywords As Keywords.Keywords
    Public Property Experiment As ExperimentData
    Public Property Author As Author
    Public Property Journal As Journal
    Public Property Remark As Remark
    Public Property Sequence As Sequence
    Public Property Revisions As Revision
    Public Property DbRef As DbReference
    Public Property crystal1 As CRYST1
    Public Property Origin1 As ORIGX123
    Public Property Origin2 As ORIGX123
    Public Property Origin3 As ORIGX123
    Public Property Scale1 As SCALE123
    Public Property Scale2 As SCALE123
    Public Property Scale3 As SCALE123
    Public Property Matrix1 As MTRIX123
    Public Property Matrix2 As MTRIX123
    Public Property Matrix3 As MTRIX123

    Public Property SSBOND As SSBOND

    ''' <summary>
    ''' number of models inside current pdb file
    ''' </summary>
    ''' <returns></returns>
    Public Property NUMMDL As NUMMDL

    Public Property Het As Het
    Public Property HetName As HetName
    Public Property HETSYN As HETSYN
    Public Property Formula As Formula
    Public Property Site As Site
    Public Property Helix As Helix
    Public Property Sheet As Sheet
    Public Property Links As Link
    Public Property seqadv As SEQADV
    Public Property CISPEP As CISPEP
    Public Property Master As Master
    Public Property Conect As CONECT
    Public Property MODRES As MODRES
    Public Property SPRSDE As SPRSDE
    Public Property CAVEAT As CAVEAT
    Public Property MDLTYP As MDLTYP
    Public Property ANISOU As ANISOU
    Public Property SIGATM As SIGATM
    Public Property SIGUIJ As SIGUIJ
    Public Property SPLIT As SPLIT

    ''' <summary>
    ''' Populate out the multiple structure models inside current pdb data file
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property AtomStructures As IEnumerable(Of Atom)
        Get
            Return _atomStructuresData.Values
        End Get
    End Property

    Default Public ReadOnly Property Model(id As String) As Atom
        Get
            Return _atomStructuresData.TryGetValue(id)
        End Get
    End Property

    Default Public ReadOnly Property Model(i As Integer) As Atom
        Get
            Return _atomStructuresData.Values(i)
        End Get
    End Property

    ''' <summary>
    ''' There are multiple model inside a pdb file, start with ``MODEL`` and end with ``ENDMDL``.
    ''' </summary>
    Friend _atomStructuresData As New Dictionary(Of String, Atom)

    Public ReadOnly Property MaxSpace As Keywords.Point3D
        Get
            Dim all = AtomStructures.Select(Function(m) m.Atoms).IteratesALL.ToArray

            If all.Length = 0 Then
                Return Nothing
            End If

            Dim xmax = (From atom As AtomUnit In all Select atom.Location.X).Max
            Dim ymax = (From atom As AtomUnit In all Select atom.Location.Y).Max
            Dim zmax = (From atom As AtomUnit In all Select atom.Location.Z).Max

            Return New Point3D With {
                .X = xmax,
                .Y = ymax,
                .Z = zmax
            }
        End Get
    End Property

    Public ReadOnly Property MinSpace As Keywords.Point3D
        Get
            Dim all = AtomStructures.Select(Function(m) m.Atoms).IteratesALL.ToArray

            If all.Length = 0 Then
                Return Nothing
            End If

            Dim xmin = (From atom As AtomUnit In all Select atom.Location.X).Min
            Dim ymin = (From atom As AtomUnit In all Select atom.Location.Y).Min
            Dim zmin = (From atom As AtomUnit In all Select atom.Location.Z).Min

            Return New Point3D With {
                .X = xmin,
                .Y = ymin,
                .Z = zmin
            }
        End Get
    End Property

    ''' <summary>
    ''' the input data text of this pdb object
    ''' </summary>
    ''' <returns></returns>
    Public Property SourceText As String

    Protected Friend Sub New()
    End Sub

    Public Iterator Function ListLigands() As IEnumerable(Of NamedValue(Of Het.HETRecord))
        If Not HetName Is Nothing Then
            Dim nameIndex = HetName.Residues _
                .ToDictionary(Function(a) a.Name,
                              Function(a)
                                  Return a.Value
                              End Function)

            For Each ref As NamedValue(Of Het.HETRecord) In Het.HetAtoms
                Dim fullName As String = nameIndex.TryGetValue(ref.Name, [default]:=ref.Name)
                Dim het As Het.HETRecord = ref.Value
                Dim id As String = ref.Name

                Yield New NamedValue(Of Het.HETRecord)(id, het, fullName)
            Next
        End If
    End Function

    Public Overrides Function ToString() As String
        ' 20251011 header or title maybe missing for pdbqt output file
        Dim header_str As String = If(Header Is Nothing OrElse Header.EmptyContent, "", Header.ToString)
        Dim title_str As String = If(Title Is Nothing, "", Title.ToString)

        If header_str.StringEmpty AndAlso title_str.StringEmpty Then
            Return Nothing
        ElseIf header_str.StringEmpty Then
            Return title_str
        ElseIf title_str.StringEmpty Then
            Return header_str
        Else
            Return header_str & $" [{title_str}]"
        End If
    End Function

    ''' <summary>
    ''' 加载一个蛋白质的三维空间结构的数据文件
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Load(path As String) As PDB
        Return Parser _
            .Load(path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)) _
            .FirstOrDefault
    End Function

    ''' <summary>
    ''' Parse the given text content as pdb data
    ''' </summary>
    ''' <param name="text"></param>
    ''' <returns></returns>
    Public Shared Function Parse(text As String, Optional verbose As Boolean = False) As PDB
        Dim str As New MemoryStream(Encoding.UTF8.GetBytes(text))
        Dim pdb As PDB = Parser.Load(str, verbose).FirstOrDefault
        Return pdb
    End Function

    ''' <summary>
    ''' Load multiple pdb molecules from a given text stream data
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Load(s As Stream) As IEnumerable(Of PDB)
        Return Parser.Load(s)
    End Function

    Public Overloads Shared Widening Operator CType(path As String) As PDB
        Return PDB.Load(path)
    End Operator

    Public Iterator Function GenericEnumerator() As IEnumerator(Of Atom) Implements Enumeration(Of Atom).GenericEnumerator
        For Each model As Atom In _atomStructuresData.Values
            Yield model
        Next
    End Function
End Class
