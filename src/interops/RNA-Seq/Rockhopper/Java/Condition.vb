#Region "Microsoft.VisualBasic::3bd7c6295e42941130aef510c4ab9aad, RNA-Seq\Rockhopper\Java\Condition.vb"

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

    '     Class Condition
    ' 
    '         Properties: avgUpperQuartile, minDiffExpressionLevel, partner
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getReplicate, numReplicates, ToString
    ' 
    '         Sub: addReplicate, setMinDiffExpressionLevel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Text

'
' * Copyright 2013 Brian Tjaden
' *
' * This file is part of Rockhopper.
' *
' * Rockhopper is free software: you can redistribute it and/or modify
' * it under the terms of the GNU General Public License as published by
' * the Free Software Foundation, either version 3 of the License, or
' * any later version.
' *
' * Rockhopper is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' * GNU General Public License for more details.
' *
' * You should have received a copy of the GNU General Public License
' * (in the file gpl.txt) along with Rockhopper.  
' * If not, see <http://www.gnu.org/licenses/>.
' 

Namespace Java

    ''' <summary>
    ''' A Condition keeps track of the RNA-seq data in each
    ''' replicate experiment for a given condition.
    ''' </summary>
    Public Class Condition

        ''' <summary>
        '''******************************************
        ''' **********   INSTANCE VARIABLES   **********
        ''' </summary>

        Private replicates As List(Of Replicate)
        Private _partner As Integer
        ' If there are no replicates, index of most similar other condition.
        Private _minDiffExpressionLevel As Integer
        ' Used for differentially expressed p-value correction


        ''' <summary>
        '''***************************************
        ''' **********   CLASS VARIABLES   **********
        ''' </summary>

        ''' <summary>Avg upper quartile over all replicates in all conditions</summary>
        Private Shared _avgUpperQuartile As Long

        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        Public Sub New()
            Me.replicates = New List(Of Replicate)()
            Me._partner = -1
        End Sub


        ''' <summary>
        '''***********************************************
        ''' **********   PUBLIC INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Return the Replicate at the specified index.
        ''' </summary>
        Public Overridable Function getReplicate(i As Integer) As Replicate
            If (i >= 0) AndAlso (i < replicates.Count) Then
                Return replicates(i)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Adds a replicate to this Condition.
        ''' </summary>
        Public Overridable Sub addReplicate(r As Replicate)
            Me.replicates.Add(r)
        End Sub

        ''' <summary>
        ''' Return the number of Replicates performed for this Condition.
        ''' </summary>
        Public Overridable Function numReplicates() As Integer
            Return replicates.Count
        End Function

        ''' <summary>
        ''' Sets the partner of this Condition, i.e., the index of the other
        ''' condition that is most similar to this Condition.
        ''' </summary>
        Public Overridable Property partner() As Integer
            Get
                Return Me._partner
            End Get
            Set
                Me._partner = Value
            End Set
        End Property


        ''' <summary>
        ''' Estimates the minimum expression level, across all replicates,
        ''' necessary to compute a p-value for differential expression.
        ''' </summary>
        Public Overridable ReadOnly Property minDiffExpressionLevel() As Integer
            Get
                Return _minDiffExpressionLevel
            End Get
        End Property

        ''' <summary>
        ''' The background probability decreases as the number of reads increases.
        ''' We use this fact to estimate a minimum level of expression necessary
        ''' for us to be able to compute a p-value of differential expression.
        ''' For a given Condition, we ensure that all Replicates meet the
        ''' expression threshold. 
        ''' Currently, the expression threshold is set (based on anecdote and
        ''' experience) to 0.005.
        ''' </summary>
        Public Overridable Sub setMinDiffExpressionLevel()
            Dim THRESHOLD As Double = 0.005
            _minDiffExpressionLevel = 0
            While True
                Dim foundThreshold As Boolean = True
                For i As Integer = 0 To replicates.Count - 1
                    If replicates(i).getBackgroundProb(_minDiffExpressionLevel) >= THRESHOLD Then
                        foundThreshold = False
                    End If
                Next
                If foundThreshold Then
                    Return
                End If
                _minDiffExpressionLevel += 1
            End While
        End Sub

        ''' <summary>
        ''' Returns a String representation of a this object.
        ''' </summary>
        Public Overridable Overloads Function ToString() As String
            Dim sb As New StringBuilder()
            For i As Integer = 0 To replicates.Count - 1
                sb.Append(replicates(i).ToString() & vbLf)
            Next
            Return sb.ToString()
        End Function



        ''' <summary>
        '''********************************************
        ''' **********   PUBLIC CLASS METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Set the average upper quartile gene expression over
        ''' all replicates in all all conditions.
        ''' </summary>
        Public Shared Property avgUpperQuartile() As Long
            Get
                Return Condition._avgUpperQuartile
            End Get
            Set
                Condition._avgUpperQuartile = Value
            End Set
        End Property


        ''' <summary>
        '''************************************************
        ''' **********   PRIVATE INSTANCE METHODS   **********
        ''' </summary>
    End Class

End Namespace
