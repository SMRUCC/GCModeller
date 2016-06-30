Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace base

    ''' <summary>
    ''' Create a data frame from all combinations of the supplied vectors or factors. See the description of the return value for precise details of the way this is done.
    ''' 
    ''' A data frame containing one row for each combination of the supplied factors. The first factors vary fastest. The columns are labelled by the factors if these are supplied as named arguments or named components of a list. The row names are ‘automatic’.
    ''' Attribute "out.attrs" Is a list which gives the dimension And dimnames for use by predict methods.
    ''' </summary>
    <RFunc("expand.grid")> Public Class expandGrid : Inherits IRToken

        ''' <summary>
        ''' vectors, factors or a list containing these.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("...", ValueTypes.List, False, True)>
        Public Property x As RExpression()
        ''' <summary>
        ''' a logical indicating the "out.attrs" attribute (see below) should be computed and returned.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("KEEP.OUT.ATTRS")> Public Property KEEP_OUT_ATTRS As Boolean = True
        ''' <summary>
        ''' logical specifying if character vectors are converted to factors.
        ''' </summary>
        ''' <returns></returns>
        Public Property stringsAsFactors As Boolean = True
    End Class
End Namespace