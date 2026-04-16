#Region "Microsoft.VisualBasic::cf992e1b914c7d88d7ba1ea953d77fda, data\SABIO-RK\ModelHelper.vb"

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

    '   Total Lines: 33
    '    Code Lines: 28 (84.85%)
    ' Comment Lines: 1 (3.03%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (12.12%)
    '     File Size: 1.53 KB


    ' Module ModelHelper
    ' 
    '     Function: CreateKineticsData
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.Data.SABIORK.TabularDump

Public Module ModelHelper

    Public Iterator Function CreateKineticsData(model As SbmlDocument) As IEnumerable(Of (rxnId As String, EnzymeCatalystKineticLaw))
        Dim indexer As New SBMLInternalIndexer(model)
        Dim mathList = model.mathML _
            .ToDictionary(Function(a) a.Name,
                          Function(a)
                              Return a.Value
                          End Function)

        For Each rxn As SBMLReaction In model.sbml.model.listOfReactions.AsEnumerable
            Dim mathId As String = "KL_" & rxn.kineticLawID
            Dim math As LambdaExpression = mathList.TryGetValue(mathId)

            If math Is Nothing OrElse math.lambda Is Nothing Then
                Continue For
                ' -1 situation: ci first maybe the kinetics id
            ElseIf (math.parameters.Length <> rxn.kineticLaw.math.apply.ci.Length) AndAlso
                (math.parameters.Length <> rxn.kineticLaw.math.apply.ci.Length - 1) Then
                Continue For
            ElseIf rxn.kineticLaw Is Nothing OrElse rxn.kineticLaw.listOfLocalParameters.IsNullOrEmpty Then
                Continue For
            Else
                Yield (rxn.id, EnzymeCatalystKineticLaw.Create(rxn, math, doc:=indexer))
            End If
        Next
    End Function
End Module

