Imports System.Text

Namespace Assembly.MetaCyc.File.TabularDataFiles

    ''' <summary>
    ''' (enzymes.col) For each enzymatic reaction in the PGDB, the file lists the 
    ''' reaction equation, up to 4 pathways that contain the reaction, up to 4 
    ''' cofactors for the enzyme, up to 4 activators, up to 4 inhibitors, and the 
    ''' subunit structure of the enzyme.
    ''' </summary>
    ''' <remarks>
    ''' Columns (multiple columns are indicated in parentheses):
    ''' 
    '''   UNIQUE-ID
    '''   NAME
    '''   REACTION-EQUATION
    '''   PATHWAYS (4)
    '''   COFACTORS (4)
    '''   ACTIVATORS (4)
    '''   INHIBITORS (4)
    '''   SUBUNIT-COMPOSITION
    ''' 
    ''' </remarks>
    Public Class Enzymes

        Public Property Objects As [Object]()
        Public Property DbProperty As [Property]

        Public Shared Widening Operator CType(spath As String) As Enzymes
            Dim DataFile As MetaCyc.File.TabularFile = spath
            Dim NewObj As Generic.IEnumerable(Of [Object]) =
                From Line As RecordLine
                In DataFile.Objects
                Select [Object].GetData(Line)

            Return New Enzymes With {
                .DbProperty = DataFile.DbProperty,
                .Objects = NewObj.ToArray
            }
        End Operator
    End Class
End Namespace
