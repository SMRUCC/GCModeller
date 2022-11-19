#Region "Microsoft.VisualBasic::064a8aad37ad08ce8b80125319d5c16a, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Metabolism\Metabolism.vb"

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

    '   Total Lines: 62
    '    Code Lines: 29
    ' Comment Lines: 28
    '   Blank Lines: 5
    '     File Size: 2.56 KB


    '     Class Metabolism
    ' 
    '         Properties: Compartments, ConstraintMetaboliteMaps, MetabolismNetwork, Metabolites, Pathways
    ' 
    '         Function: AppendNewMetabolite
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels

Namespace GCML_Documents.XmlElements.Metabolism

    Public Class Metabolism

        ''' <summary>
        ''' 实现表达调控过程的底物约束所需要的代谢物
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConstraintMetaboliteMaps As ConstraintMetaboliteMap()
        Public Property Compartments As List(Of Compartment)
        ''' <summary>
        ''' The collection of all of the metabolites that required in this cell model.
        ''' (本模型中所需求的代谢物的集合)
        ''' </summary>
        ''' <remarks>
        ''' 所有的RNA，蛋白质，小分子化合物都会放置于这个集合之中
        ''' </remarks>
        Public Property Metabolites As List(Of Metabolite)
        ''' <summary>
        ''' The collection of all of the metabolite reaction in this cell model.
        ''' (本模型中的所有代谢反应的集合)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property MetabolismNetwork As List(Of Reaction)
        ''' <summary>
        ''' Metabolism pathways collection.(代谢途径的集合)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Pathways As Pathway()

        '''' <summary>
        '''' The collection of the enzyme that required in the metabolism reaction.
        '''' (代谢反应所需求的酶分子的集合)
        '''' </summary>
        '''' <remarks></remarks>
        'Public Property MetabolismEnzymes As EnzymeCatalystKineticLaw()

        Public Function AppendNewMetabolite(UniqueId As String, CompoundType As Metabolite.MetaboliteTypes) As Metabolite
            Dim LQuery = (From item In Me.Metabolites.AsParallel
                          Where String.Equals(item.Identifier, UniqueId)
                          Select item).ToArray
            If LQuery.IsNullOrEmpty Then
                Dim Metabolite = New Metabolite With {
                    .Identifier = UniqueId,
                    .InitialAmount = 10,
                    .CommonName = UniqueId,
                    .BoundaryCondition = False,
                    .MetaboliteType = CompoundType
                }
                Call Me.Metabolites.Add(Metabolite)
                Return Metabolite
            Else
                Return LQuery.First
            End If
        End Function
    End Class
End Namespace
