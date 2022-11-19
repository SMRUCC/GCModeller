#Region "Microsoft.VisualBasic::92f637919f4e52716a643143722333f3, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\Expender.vb"

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

    '   Total Lines: 91
    '    Code Lines: 0
    ' Comment Lines: 80
    '   Blank Lines: 11
    '     File Size: 4.66 KB


    ' 
    ' /********************************************************************************/

#End Region

'Imports Microsoft.VisualBasic.Terminal.stdio  

'Namespace Builder

'    ''' <summary>
'    ''' 将通用蛋白质底物进行展开
'    ''' </summary>
'    ''' <remarks></remarks>
'    Public Class ExpendGeneralProtein : Inherits IBuilder

'        Sub New(MetaCyc As MetaCyc.File.FileSystem.DatabaseLoadder, Model As SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Model)
'            Call MyBase.New(MetaCyc, Model)
'        End Sub

'        Public Overrides Function Invoke() As Model
'            Dim originalCounts = Model.ProteinAssemblies.Count

'            For i As Integer = 0 To originalCounts - 1
'                Dim sourceRxn = Model.ProteinAssemblies(i)
'                Dim hwnd As Integer = ContainsGeneralSubstrate(sourceRxn)   '判断这个反应对象是否具有通用蛋白质底物对象
'                If hwnd > -1 Then
'                    Call Printf("GENERAL_RULE:: %s", sourceRxn.ToString)
'                    For Each protein In Model.Proteins
'                        Dim expendedRxn = Copy(sourceRxn)
'                        expendedRxn.Metabolites(hwnd).species = protein.UniqueId
'                        expendedRxn.UniqueID = String.Concat(New String() {protein.UniqueId, "-", expendedRxn.UniqueID})
'                        Call Model.ProteinAssemblies.Add(expendedRxn)
'                    Next
'                    '销毁源目标对象
'                    i -= 1
'                    Call Model.ProteinAssemblies.Remove(sourceRxn)
'                End If
'            Next

'            MyBase.Model.ProteinAssemblies = Model.ProteinAssemblies
'            Return MyBase.Model
'        End Function

'        ''' <summary>
'        ''' 按值复制对象
'        ''' </summary>
'        ''' <param name="rxn"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Shared Function Copy(rxn As SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) As SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction
'            Dim CopyObject As SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction = New Elements.Reaction
'            CopyObject.BaseType = rxn.BaseType
'            ' CopyObject.EnzymaticRxn = rxn.EnzymaticRxn
'            CopyObject.Enzymes = rxn.Enzymes
'            CopyObject.Keq_1 = rxn.Keq_1
'            CopyObject.Keq_2 = rxn.Keq_2
'            CopyObject.LOWER_BOUND = rxn.LOWER_BOUND
'            CopyObject.Name = rxn.Name
'            CopyObject.ObjectiveCoefficient = rxn.ObjectiveCoefficient
'            CopyObject.Regulators = rxn.Regulators
'            CopyObject.Reversible = rxn.Reversible
'            CopyObject.UniqueID = rxn.UniqueID
'            CopyObject.UPPER_BOUND = New SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = 10} ' rxn.UPPER_BOUND

'            Dim CopySpecieRef As System.Func(Of List(Of SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction.speciesReference), List(Of SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction.speciesReference)) =
'                Function(source As List(Of SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction.speciesReference)) _
'                    (From ref As SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction.speciesReference
'                     In source
'                     Select ref.CopyData()).AsList

'            CopyObject.Products = CopySpecieRef(rxn.Products)
'            CopyObject.Reactants = CopySpecieRef(rxn.Reactants)

'            Return CopyObject
'        End Function

'        ''' <summary>
'        ''' 仅判断通用的蛋白质底物的存在
'        ''' </summary>
'        ''' <param name="rxn"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Shared Function ContainsGeneralSubstrate(rxn As Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) As Integer
'            Dim Collection = rxn.Metabolites
'            Dim LQuery = (From hwnd As Integer In Collection.Sequence
'                          Let [sub] As SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction.speciesReference = Collection(hwnd)
'                          Where String.Equals([sub].species, "General-Protein-Substrates")
'                          Select hwnd).ToArray   '
'            If LQuery.IsNullOrEmpty Then
'                Return -1
'            Else
'                Return LQuery.First
'            End If
'        End Function
'    End Class
'End Namespace
