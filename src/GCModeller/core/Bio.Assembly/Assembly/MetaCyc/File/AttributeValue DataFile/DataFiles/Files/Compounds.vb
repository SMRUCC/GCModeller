#Region "Microsoft.VisualBasic::2a0945a657e4f74c2df4f9ef88d2f6cc, ..\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\Compounds.vb"

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

Imports SMRUCC.genomics.Assembly.MetaCyc.Schema
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' 该细胞系统中的所有的小分子化合物的集合，本集合取决于代谢网络的结构以及控制物质跨膜运输的蛋白质
    ''' </summary>
    ''' <remarks>
    ''' 对于Compounds表而言，由于其包含的对象仅为小分子的代谢物，故而大分子的蛋白质分子不会出现在此列表之中
    ''' </remarks>
    Public Class Compounds : Inherits DataFile(Of Slots.Compound)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ABBREV-NAME", "ANTICODON",
                    "ATOM-CHARGES", "CATALYZES", "CFG-ICON-COLOR", "CHEMICAL-FORMULA",
                    "CITATIONS", "CODONS", "COFACTORS-OF", "COFACTORS-OR-PROSTHETIC-GROUPS-OF",
                    "COMMENT", "COMMENT-INTERNAL", "COMPONENT-COEFFICIENTS",
                    "COMPONENT-OF", "COMPONENTS", "CONSENSUS-SEQUENCE", "CREDITS",
                    "DATA-SOURCE", "DBLINKS", "DNA-FOOTPRINT-SIZE", "DOCUMENTATION",
                    "ENZYME-NOT-USED-IN", "GENE", "GO-TERMS", "GROUP-COORDS-2D",
                    "GROUP-INTERNALS", "HAS-NO-STRUCTURE?", "HIDE-SLOT?",
                    "IN-MIXTURE", "INCHI", "INTERNALS-OF-GROUP", "ISOZYME-SEQUENCE-SIMILARITY",
                    "LEFT-END-POSITION", "LOCATIONS", "MEMBER-SORT-FN", "MODIFIED-FORM",
                    "MOLECULAR-WEIGHT", "MOLECULAR-WEIGHT-EXP", "MOLECULAR-WEIGHT-KD",
                    "MOLECULAR-WEIGHT-SEQ", "MONOISOTOPIC-MW", "N+1-NAME", "N-1-NAME",
                    "N-NAME", "NEIDHARDT-SPOT-NUMBER", "NON-STANDARD-INCHI", "PI",
                    "PKA1", "PKA2", "PKA3", "PROSTHETIC-GROUPS-OF", "RADICAL-ATOMS",
                    "REGULATED-BY", "REGULATES", "RIGHT-END-POSITION", "SMILES", "SPECIES",
                    "SPLICE-FORM-INTRONS", "STRUCTURE-GROUPS", "STRUCTURE-LINKS", "SUPERATOMS",
                    "SYMMETRY", "SYNONYMS", "SYSTEMATIC-NAME", "TAUTOMERS", "TEMPLATE-FILE",
                    "UNMODIFIED-FORM"
                }
            End Get
        End Property

        Public Function GetCompoundsAbstract() As ICompoundObject()
            Return Values.ToArray(Function(x) DirectCast(x, ICompoundObject))
        End Function

        ''' <summary>
        ''' Get an object instance in the compounds table using its common name or synonymous name.
        ''' </summary>
        ''' <param name="CommonName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObject(CommonName As String) As Slots.Compound
            Dim LQuery = (From compound As Slots.Compound In MyBase.Values.AsParallel
                          Where True = __where(compound, CommonName)
                          Select compound).FirstOrDefault
            Return LQuery
        End Function

        Private Function __where(compound As Slots.Compound, commonName As String) As Boolean
            If String.Equals(commonName, compound.CommonName, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
            If String.Equals(commonName, compound.AbbrevName, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
            Dim NameQuery = (From strName As String In compound.Synonyms
                             Where String.Equals(strName, commonName, StringComparison.OrdinalIgnoreCase)
                             Select 100).FirstOrDefault > 50
            Return NameQuery
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function

        ''' <summary>
        ''' Tested load method.
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function LoadCompoundsData(path As String) As Compounds
            Dim dat As New Compounds
            Reflection.FileStream.Read(Of Slots.Compound, Compounds)(path, dat)
            dat.Values = (From met As Slots.Compound In dat.Values Select met.Trim).AsList
            Return dat
        End Function
    End Class
End Namespace
