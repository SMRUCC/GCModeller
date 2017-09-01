#Region "Microsoft.VisualBasic::ae215fb6336753f92ef26b736dc45acb, ..\GCModeller\data\GO_gene-ontology\GeneOntology\DAG\Graph.vb"

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

Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.foundation.OBO_Foundry

Namespace DAG

    Public Class Graph

        ReadOnly __DAG As Dictionary(Of TermNode)
        ReadOnly _file$

        Public ReadOnly Property header As header

        Sub New(path$)
            Dim obo As GO_OBO = GO_OBO.LoadDocument(path$)
            __DAG = obo.Terms.BuildTree
            _file = path$
        End Sub

        Public Overrides Function ToString() As String
            Return _file.ToFileURL
        End Function

        ''' <summary>
        ''' These terms describe a component of a cell that is part of a larger object, such as an anatomical structure 
        ''' (e.g. rough endoplasmic reticulum or nucleus) or a gene product group (e.g. ribosome, proteasome or a protein dimer).
        ''' </summary>
        Const cellular_component$ = NameOf(cellular_component)
        ''' <summary>
        ''' A biological process term describes a series of events accomplished by one or more organized assemblies of molecular functions. 
        ''' Examples of broad biological process terms are "cellular physiological process" or "signal transduction". Examples of more 
        ''' specific terms are "pyrimidine metabolic process" or "alpha-glucoside transport". The general rule to assist in distinguishing 
        ''' between a biological process and a molecular function is that a process must have more than one distinct steps.
        ''' A biological process Is Not equivalent To a pathway. At present, the GO does Not Try To represent the dynamics Or dependencies 
        ''' that would be required To fully describe a pathway.
        ''' </summary>
        Const biological_process$ = NameOf(biological_process)
        ''' <summary>
        ''' Molecular function terms describes activities that occur at the molecular level, such as "catalytic activity" or "binding activity". 
        ''' GO molecular function terms represent activities rather than the entities (molecules or complexes) that perform the actions, 
        ''' and do not specify where, when, or in what context the action takes place. Molecular functions generally correspond to activities 
        ''' that can be performed by individual gene products, but some activities are performed by assembled complexes of gene products. 
        ''' Examples of broad functional terms are "catalytic activity" and "transporter activity"; examples of narrower functional terms are 
        ''' "adenylate cyclase activity" or "Toll receptor binding".
        ''' It Is easy To confuse a gene product name With its molecular Function; For that reason GO molecular functions are often appended 
        ''' With the word "activity".
        ''' </summary>
        Const molecular_function$ = NameOf(molecular_function)

        'Public Iterator Function Family(id$, [namespace] As Ontologies) As IEnumerable(Of Term())
        '    Dim parent As New Value(Of Term)
        '    Dim ns$ = [namespace].Description

        '    For Each pid As is_a In __DAG(id$).is_a.SafeQuery
        '        If (parent = __DAG(pid.uid$)).namespace = ns Then
        '            Yield parent + visits(pid.uid, ns$)
        '        End If
        '    Next
        'End Function

        'Private Function visits(id$, namespace$) As NamedValue(Of Term)()

        'End Function

        Public Function Infer(a$, b$) As Relationship

        End Function
    End Class
End Namespace
